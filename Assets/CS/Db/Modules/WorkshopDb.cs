using System;
using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 工坊相关数据模块
	/// </summary>
	public partial class DbManager {
		int maxModifyResourceTimeout = 3600; //最大的间隔时间
		int modifyResourceTimeout = 30; //刷新资源间隔时间（单位：秒）

		/// <summary>
		/// 检测是否有新的生产单元
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void CheckNewWorkshopItems(string cityId) {
			db = OpenDb();
			List<ResourceRelationshipData> resourcesRelationshipsInCity = WorkshopModel.Relationships.FindAll(item => item.BelongToCityId == cityId);
			if (resourcesRelationshipsInCity.Count > 0) {
				List<ResourceData> resources;
				SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.Read()) {
					//更新
					resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
					for (int i = 0; i < resourcesRelationshipsInCity.Count; i++) {
						if (resources.FindIndex(item => item.Type == resourcesRelationshipsInCity[i].Type) < 0) {
							resources.Add(new ResourceData(resourcesRelationshipsInCity[i].Type, 0));
						}
					}
					db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				}
				else {
					//新增
					resources = new List<ResourceData>();
					for (int i = 0; i < resourcesRelationshipsInCity.Count; i++) {
						resources.Add(new ResourceData(resourcesRelationshipsInCity[i].Type, 0));
					}
					db.ExecuteQuery("insert into WorkshopResourceTable (ResourcesData, Ticks, WorkerNum, MaxWorkerNum, BelongToRoleId) values('" + JsonManager.GetInstance().SerializeObject(resources) + "', " + DateTime.Now.Ticks + ", 0, 0, '" + currentRoleId + "')");
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求工坊主界面数据(包括生产材料标签页数据)
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetWorkshopPanelData(string cityId) {
			JArray data = new JArray();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				string resourceDataStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				data.Add(resourceDataStr);
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("WorkerNum")));
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("MaxWorkerNum")));
				List<ResourceData> resultResources = returnAllReceiveResourcesOnece(resourceDataStr);
				data.Add(resultResources.Count > 0 ? JsonManager.GetInstance().SerializeObject(resultResources) : "[]");
				data.Add(modifyResourceTimeout);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<JArray>(NotifyTypes.GetWorkshopPanelDataEcho, data);
		}

		/// <summary>
		/// 计算n倍单位时间内一共产出的资源数
		/// </summary>
		/// <returns>The all receive resources onece.</returns>
		/// <param name="resourceDataStr">Resource data string.</param>
		/// <param name="n">N.</param>
		List<ResourceData> returnAllReceiveResourcesOnece(string resourcesDataStr, int n = 1) {
			return returnAllReceiveResourcesOnece(JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesDataStr), n);
		}

		/// <summary>
		/// 计算n倍单位时间内一共产出的资源数
		/// </summary>
		/// <returns>The all receive resources onece.</returns>
		/// <param name="resourcesData">Resources data.</param>
		/// <param name="n">N.</param>
		List<ResourceData> returnAllReceiveResourcesOnece(List<ResourceData> resourcesData, int n = 1) {
			List<ResourceData> resultResources = new List<ResourceData>();
			List<ResourceData> resources = resourcesData;
			ResourceData resource;
			ResourceRelationshipData relationship;
			ResourceData need;
			for (int i = 0; i < resources.Count; i++) {
				resource = resources[i];
				if (resource.WorkersNum > 0) {
					relationship = WorkshopModel.Relationships.Find(item => item.Type == resource.Type);
					if (relationship != null) {
						int index = resultResources.FindIndex(item => item.Type == relationship.Type);
						if (index < 0) {
							resultResources.Add(new ResourceData(relationship.Type, relationship.YieldNum * resource.WorkersNum * n));
						}
						else {
							resultResources[index].Num += (relationship.YieldNum * resource.WorkersNum * n);
						}

						for (int j = 0; j < relationship.Needs.Count; j++) {
							need = relationship.Needs[j];
							index = resultResources.FindIndex(item => item.Type == need.Type);
							if (index < 0) {
								resultResources.Add(new ResourceData(need.Type, -need.Num * resource.WorkersNum * n));	
							}
							else {
								resultResources[index].Num -= (need.Num * resource.WorkersNum * n);
							}
						}
					}
				}
			}
			return resultResources;
		}

		/// <summary>
		/// 增减资源的工作家丁数
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="addNum">Add number.</param>
		public void ChangeResourceWorkerNum(ResourceType type, int addNum) {
			if (addNum == 0) {
				return;
			}
			addNum = Mathf.Clamp(addNum, -1, 1);
			JArray data = new JArray();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				int workerNum = sqReader.GetInt32(sqReader.GetOrdinal("WorkerNum"));
				int maxWorkerNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxWorkerNum"));
				if (addNum > 0 && workerNum == 0) {
					db.CloseSqlConnection();
					return;
				}
				int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				string resourceDataStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
				List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourceDataStr);
				ResourceData findResource = resources.Find(item => item.Type == type);
				if (findResource != null) {
					if (addNum < 0 && findResource.WorkersNum == 0) {
						db.CloseSqlConnection();
						return;
					}
					findResource.WorkersNum += addNum;
					findResource.WorkersNum = findResource.WorkersNum < 0 ? 0 : findResource.WorkersNum;
					workerNum -= addNum;
					workerNum = workerNum < 0 ? 0 : (workerNum > maxWorkerNum ? maxWorkerNum : workerNum);
					List<ResourceData> resultResources = returnAllReceiveResourcesOnece(resources);
					data.Add((short)type);
					data.Add(findResource.WorkersNum);
					data.Add(workerNum);
					data.Add(maxWorkerNum);
					data.Add(resultResources.Count > 0 ? JsonManager.GetInstance().SerializeObject(resultResources) : "[]");
					//更新数据
					sqReader = db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "', WorkerNum = " + workerNum + " where Id = " + id);
				}
			}
			db.CloseSqlConnection();
			if (data.Count > 0) {
				Messenger.Broadcast<JArray>(NotifyTypes.ChangeResourceWorkerNumEcho, data);
			}
		}

		/// <summary>
		/// 刷新资源数据
		/// </summary>
		public void ModifyResources() {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				long ticks = sqReader.GetInt64(sqReader.GetOrdinal("Ticks"));
				DateTime oldDate = new DateTime(ticks);
				int n = (int)Mathf.Ceil(((float)(DateTime.Now - oldDate).TotalSeconds / modifyResourceTimeout));
				int maxN = maxModifyResourceTimeout / modifyResourceTimeout;
				if (n > 0) {
					n = n > maxN ? maxN : n;
					int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
					List<ResourceData> resultResources = returnAllReceiveResourcesOnece(resources);
				}
				Debug.LogWarning(n);
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求工坊兵器打造标签页数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetWorkshopWeaponBuildingTableData(string cityId) {
			
		}
	}
}