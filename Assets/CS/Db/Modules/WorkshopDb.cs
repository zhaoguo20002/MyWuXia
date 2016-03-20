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
					db.ExecuteQuery("insert into WorkshopResourceTable (ResourcesData, Ticks, BelongToRoleId) values('" + JsonManager.GetInstance().SerializeObject(resources) + "', " + DateTime.Now.Ticks + ", '" + currentRoleId + "')");
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求工坊生产材料标签页数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetWorkshopResourceTableData(string cityId) {
			CheckNewWorkshopItems(cityId);
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				workshopResourceUpdateTicks = sqReader.GetInt64(sqReader.GetOrdinal("Ticks"));
				workshopResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
				Debug.LogWarning(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
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