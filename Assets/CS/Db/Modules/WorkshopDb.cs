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
		
		long workshopResourceUpdateTicks = 0; //控制更新

		List<ResourceData> workshopResources = null;

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
				data.Add(resultResources.Count > 0 ? JsonManager.GetInstance().SerializeObjectDealVector(resultResources) : "[]");
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
		List<ResourceData> returnAllReceiveResourcesOnece(string resourceDataStr, int n = 1) {
			List<ResourceData> resultResources = new List<ResourceData>();
			List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourceDataStr);
			ResourceData resource;
			ResourceRelationshipData relationship;
			ResourceData need;
			for (int i = 0; i < resources.Count; i++) {
				resource = resources[i];
				if (resource.Num > 0) {
					relationship = WorkshopModel.Relationships.Find(item => item.Type == resource.Type);
					if (relationship != null) {
						int index = resultResources.FindIndex(item => item.Type == relationship.Type);
						if (index < 0) {
							resultResources.Add(new ResourceData(relationship.Type, relationship.YieldNum));
						}
						else {
							resultResources[index].Num += relationship.YieldNum;
						}

						for (int j = 0; j < relationship.Needs.Count; j++) {
							need = relationship.Needs[i];
							index = resultResources.FindIndex(item => item.Type == need.Type);
							if (index < 0) {
								resultResources.Add(new ResourceData(need.Type, -need.Num));	
							}
							else {
								resultResources[index].Num -= need.Num;
							}
						}
					}
				}
			}
			return resultResources;
		}

		/// <summary>
		/// 请求工坊兵器打造标签页数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetWorkshopWeaponBuildingTableData(string cityId) {
			
		}
	}
}