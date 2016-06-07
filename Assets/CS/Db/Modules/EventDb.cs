using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Game {
	/// <summary>
	/// 动态事件相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 创建新的事件
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="eventId">Event identifier.</param>
		/// <param name="sceneId">Scene identifier.</param>
		/// <param name="name">Name.</param>
		public void CreateNewEvent(SceneEventType type, string eventId, string areaId, string name = "") {
			//计算区域大地图上的随机坐标点
			if (AreaModel.AreaMainScript == null) {
				return;
			}
			SizeData mapSize = JsonManager.GetInstance().GetMapping<SizeData>("AreaSizeDatas", areaId);
			int x = UnityEngine.Random.Range(1, mapSize.Width - 1);
			int y = UnityEngine.Random.Range(1, mapSize.Height - 1);
			string randomEventId;
			tk2dRuntime.TileMap.TileInfo groundTile;
			bool canNotAdd = true;
			//查找到合法的事件点坐标
			while(canNotAdd) {
				randomEventId = areaId + "_" + x.ToString() + "_" + y.ToString();
				groundTile = AreaModel.AreaMainScript.Map.GetTileInfoForTileId(AreaModel.AreaMainScript.Map.GetTile(x, y, 0));
				if (groundTile == null //非法坐标
					|| groundTile.stringVal == "obstacle" //为寻路障碍点
					|| AreaMain.StaticAreaEventsMapping.ContainsKey(randomEventId) //为静态事件
					|| AreaMain.ActiveAreaEventsMapping.ContainsKey(randomEventId)) //为动态事件
				{
					x = UnityEngine.Random.Range(1, mapSize.Width - 1);
					y = UnityEngine.Random.Range(1, mapSize.Height - 1);
				}
				else {
					canNotAdd = false;
				}
			}

			db = OpenDb();
			db.ExecuteQuery("insert into EventsTable (X, Y, Type, EventId, SceneId, Name, BelongToRoleId) values(" + x + ", " + y + ", " + ((int)type) + ", '" + eventId + "', '" + areaId + "', '" + name + "', '" + currentRoleId + "')");
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 战斗胜利后将战斗事件从区域大地图上移除
		/// </summary>
		/// <param name="fightId">Fight identifier.</param>
		public void RemoveFightEvent(string fightId) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from EventsTable where EventId = '" + fightId + "' and BelongToRoleId = '" + currentRoleId + "'");
			while(sqReader.Read()) {
				db.ExecuteQuery("delete from EventsTable where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 任务完成后将任务事件从区域大地图上移除
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		public void RemoveTaskEvent(string taskId) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from EventsTable where EventId = '" + taskId + "' and BelongToRoleId = '" + currentRoleId + "'");
			while(sqReader.Read()) {
				db.ExecuteQuery("delete from EventsTable where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 获取区域动态事件列表
		/// </summary>
		/// <param name="sceneId">Scene identifier.</param>
		public void GetActiveEventsInArea(string sceneId) {
			List<EventData> eventsData = new List<EventData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from EventsTable where SceneId = '" + sceneId + "' and BelongToRoleId = '" + currentRoleId + "'");
			EventData eventData;
			while(sqReader.Read()) {
				eventData = new EventData();
				eventData.X = sqReader.GetInt32(sqReader.GetOrdinal("X"));
				eventData.Y = sqReader.GetInt32(sqReader.GetOrdinal("Y"));
				eventData.Id = string.Format("{0}_{1}_{2}", sceneId, eventData.X, eventData.Y);
				eventData.EventId = sqReader.GetString(sqReader.GetOrdinal("EventId"));
				eventData.Name = sqReader.GetString(sqReader.GetOrdinal("Name"));
				eventData.SceneId = sceneId;
				eventData.Type = (SceneEventType)sqReader.GetInt32(sqReader.GetOrdinal("Type"));
				eventsData.Add(eventData);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<EventData>>(NotifyTypes.GetActiveEventsInAreaEcho, eventsData);
		}
	}
}