using UnityEngine;
using System.Collections;
using System;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace Game {
	/// <summary>
	/// 数据库管理类
	/// </summary>
	public class DbManager {
		static DbManager _instance;
		public static DbManager Instance {
			get {
				if (_instance == null) {
					_instance = new DbManager("MyWuXiaDB.db");
				}
				return _instance;
			}
		}

		string dbConnectionString;
		DbAccess db;
		string currentRoleId;
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		public DbManager(string connectionString) {
			dbConnectionString = connectionString;
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("CurrentRoleId"))) {
				PlayerPrefs.SetString("CurrentRoleId", "role_0");
			}
			currentRoleId = PlayerPrefs.GetString("CurrentRoleId");
		}

		/// <summary>
		/// 创建数据库
		/// </summary>
		public void CreateAllDbs() {
			db = new DbAccess(dbConnectionString);

			#region 初始化存档表相关数据
			db.ExecuteQuery("create table if not exists RecordsTable (Id integer primary key autoincrement not null, RoleId text not null, Name text not null, Data text, DateTime text not null);");

			#endregion

			#region 初始化角色表相关数据
			db.ExecuteQuery("create table if not exists RolesTable (RoleId text primary key not null, RoleData text not null, State integer not null, BelongToRoleId text not null, DateTime text not null);");
			#endregion

			db.CloseSqlConnection();

			AddNewRecord(currentRoleId, "-", "{}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData role = new RoleData();
			role.Id = currentRoleId;
			role.Name = "龙展";
			role.Desc = "主角光环";
			role.IconId = "100000";
			role.Occupation = OccupationType.GaiBang;
			AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), 1, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
		}

		/// <summary>
		/// 返回某个表的总记录条数
		/// </summary>
		/// <returns>The table row count.</returns>
		/// <param name="tableName">Table name.</param>
		public int GetTableRowCount(string tableName) {
			db = new DbAccess(dbConnectionString);
			SqliteDataReader sqReader = db.ExecuteQuery("select * from " + tableName + ";");
			int count = 0;
			while (sqReader.Read()) {
				count++;
			}
			db.CloseSqlConnection();
			return count;
		}

		/// <summary>
		/// 添加存档记录数据
		/// </summary>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="name">Name.</param>
		/// <param name="data">Data.</param>
		/// <param name="dateTime">Date time.</param>
		public void AddNewRecord(string roleId, string name, string data, string dateTime) {
			db = new DbAccess(dbConnectionString);
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from RecordsTable where RoleId = '" + roleId + "'");
			if (!sqReader.HasRows) {
				//首次创建存档记录
				Debug.LogWarning("首次创建存档记录");
				db.ExecuteQuery("insert into RecordsTable (RoleId, Name, Data, DateTime) values('" + roleId + "', '" + name + "', '" + data + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 添加新的角色数据
		/// </summary>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="roleData">Role data.</param>
		/// <param name="state">State.</param>
		/// <param name="belongToRoleId">Belong to role identifier.</param>
		/// <param name="dateTime">Date time.</param>
		public void AddNewRole(string roleId, string roleData, int state, string belongToRoleId, string dateTime) {
			db = new DbAccess(dbConnectionString);
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId from RolesTable where RoleId = '" + roleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into RolesTable values('" + roleId + "', '" + roleData + "', " + state + ", '" + belongToRoleId + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求队伍信息面板数据
		/// </summary>
		public void CallRoleInfoPanelData() {
			db = new DbAccess(dbConnectionString);
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where BelongToRoleId = '" + currentRoleId + "'");
			JObject obj = new JObject();
			JArray data = new JArray();
			if (sqReader.Read()) {
				data.Add(new JArray(
					sqReader.GetString(sqReader.GetOrdinal("RoleId")),
					sqReader.GetString(sqReader.GetOrdinal("RoleData")),
					sqReader.GetInt16(sqReader.GetOrdinal("State"))
				));
			}
			obj["data"] = data;
			Messenger.Broadcast<JObject>(NotifyTypes.CallRoleInfoPanelDataEcho, obj);
			db.CloseSqlConnection();
		}
	}
}
