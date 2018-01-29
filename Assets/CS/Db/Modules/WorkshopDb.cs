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
		int maxModifyResourceTimeout = 28800; //离线后仍然产出的最大时间（单位：秒）
		int modifyResourceTimeout = 20; //刷新资源间隔时间（单位：秒）

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
                    string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                    resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                    resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
					for (int i = 0; i < resourcesRelationshipsInCity.Count; i++) {
						if (resources.FindIndex(item => item.Type == resourcesRelationshipsInCity[i].Type) < 0) {
							resources.Add(new ResourceData(resourcesRelationshipsInCity[i].Type, 0));
						}
					}
                    db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				}
				else {
					//新增
					resources = new List<ResourceData>();
					for (int i = 0; i < resourcesRelationshipsInCity.Count; i++) {
						resources.Add(new ResourceData(resourcesRelationshipsInCity[i].Type, 0));
					}
                    db.ExecuteQuery("insert into WorkshopResourceTable (ResourcesData, Ticks, WorkerNum, MaxWorkerNum, BelongToRoleId) values('" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "', " + DateTime.Now.Ticks + ", 0, 0, '" + currentRoleId + "')");
                }
				//记录当前所有的工坊资源类型列表
				CitySceneModel.ResourceTypeStrOfWorkShopNewFlagList = new List<string>();
				for (int i = resources.Count - 1; i >= 0; i--) {
					CitySceneModel.ResourceTypeStrOfWorkShopNewFlagList.Add(resources[i].Type.ToString());
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求工坊主界面数据(包括生产材料标签页数据)
		/// </summary>
		public void GetWorkshopPanelData() {
			JArray data = new JArray();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                string resourceDataStr = resourcesStr;
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				data.Add(resourceDataStr);
//				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("WorkerNum")));
//                data.Add(sqReader.GetInt32(sqReader.GetOrdinal("MaxWorkerNum")));
                data.Add(GetWorkerNum());
                data.Add(GetMaxWorkerNum());
				List<ResourceData> resultResources = returnAllReceiveResourcesOnece(resourceDataStr);
				data.Add(JsonManager.GetInstance().SerializeObject(resultResources));
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
			ResourceData find;
			bool canProduce;
			int _n;
			int _curN;
			for (int i = 0; i < resources.Count; i++) {
				resource = resources[i];
				if (resource.WorkersNum > 0) {
					relationship = WorkshopModel.Relationships.Find(item => item.Type == resource.Type);
					if (relationship != null) {
						_n = n;
						canProduce = true;
						//先检测生产资源需要的资源是否足够，如果不足够则不生产
						for (int j = 0; j < relationship.Needs.Count; j++) {
							need = relationship.Needs[j];
//							if (resources.FindIndex(item => item.Type == need.Type && item.Num >= (need.Num * resource.WorkersNum * n)) < 0) {
//								canProduce = false;
//								break;
//							}
							//如果所需的最大材料不足则计算下只能生产多少，按最少的生产批次生产
							find = resources.Find(item => item.Type == need.Type);
							_curN = (int)(find.Num / (need.Num * resource.WorkersNum));
							if (_curN > 0) {
								if (_curN < _n) {
									_n = _curN;
								}
							}
							else {
								canProduce = false;
								break;
							}
						}
						if (canProduce) {
							int index = resultResources.FindIndex(item => item.Type == relationship.Type);
							if (index < 0) {
								resultResources.Add(new ResourceData(relationship.Type, relationship.YieldNum * resource.WorkersNum * _n));
							}
							else {
								resultResources[index].Num += (relationship.YieldNum * resource.WorkersNum * _n);
							}
							
							for (int j = 0; j < relationship.Needs.Count; j++) {
								need = relationship.Needs[j];
								index = resultResources.FindIndex(item => item.Type == need.Type);
								if (index < 0) {
									resultResources.Add(new ResourceData(need.Type, -need.Num * resource.WorkersNum * _n));	
								}
								else {
									resultResources[index].Num -= (need.Num * resource.WorkersNum * _n);
								}
							}
						}
					}
				}
			}
			for (int i = resultResources.Count - 1; i >= 0; i--) {
				if (resultResources[i].Num == 0) {
					resultResources.RemoveAt(i);
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
			try {
				db = OpenDb();
				SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.Read()) {
//					int workerNum = sqReader.GetInt32(sqReader.GetOrdinal("WorkerNum"));
//                    int maxWorkerNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxWorkerNum"));
                    int workerNum = GetWorkerNum();
                    int maxWorkerNum = GetMaxWorkerNum();
					if (addNum > 0 && workerNum == 0) {
						db.CloseSqlConnection();
						return;
					}
					int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                    string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                    resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                    string resourceDataStr = resourcesStr;
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
						data.Add(JsonManager.GetInstance().SerializeObject(resultResources));
						//更新数据
                        db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "', WorkerNum = " + workerNum + " where Id = " + id);
                        SetWorkerNum(workerNum);
					}
				}
				db.CloseSqlConnection();
			}
			catch(Exception e) {
				db.CloseSqlConnection();
				Debug.LogWarning("ChangeResourceWorkerNum-------------------error:" + e.ToString() + " - " + e.StackTrace);
				return;
			}
			if (data.Count > 0) {
				Messenger.Broadcast<JArray>(NotifyTypes.ChangeResourceWorkerNumEcho, data);
			}
		}

		/// <summary>
		/// 刷新资源数据
		/// </summary>
		public void ModifyResources() {
			JArray data = new JArray(modifyResourceTimeout, "[]", "[]");
			try {
				db = OpenDb();
				SqliteDataReader sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.Read()) {
					long ticks = sqReader.GetInt64(sqReader.GetOrdinal("Ticks"));
					DateTime oldDate = new DateTime(ticks);
					float skipSeconds = (float)(DateTime.Now - oldDate).TotalSeconds;
					int n = (int)Mathf.Floor((skipSeconds / modifyResourceTimeout));
					//设置最大的托管时间间隔（1小时之外的资源无效）
					int maxN = maxModifyResourceTimeout / modifyResourceTimeout;
					//记录倒计时时间
					data[0] = skipSeconds < modifyResourceTimeout ? (int)(modifyResourceTimeout - skipSeconds) : modifyResourceTimeout;
					if (n > 0) {
						n = n > maxN ? maxN : n;
						int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                        string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                        resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                        List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
						List<ResourceData> resultResources = returnAllReceiveResourcesOnece(resources, n);
						if (resultResources.Count > 0) {
							ResourceData result;
							ResourceData find;
							for (int i = 0; i < resultResources.Count; i++) {
								result = resultResources[i];
								find = resources.Find(item => item.Type == result.Type);
								if (find != null) {
									//累加拥有的资源
									find.Num += result.Num;
									find.Num = find.Num < 0 ? 0 : find.Num;
								}
							}
                            db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "', Ticks = " + DateTime.Now.Ticks + " where Id = " + id);
						}
						else {
							//没有产出也需要更新时间戳
							db.ExecuteQuery("update WorkshopResourceTable set Ticks = " + DateTime.Now.Ticks + " where Id = " + id);
						}
						//添加收获的资源列表
						data[1] = JsonManager.GetInstance().SerializeObject(resultResources);
						//计算单位产出
						data[2] = JsonManager.GetInstance().SerializeObject(returnAllReceiveResourcesOnece(resources));
					}
				}
				db.CloseSqlConnection();
			}
			catch(Exception e) {
				db.CloseSqlConnection();
				Debug.LogWarning("ModifyResources-------------------error:" + e.ToString() + " - " + e.StackTrace);
				return;
			}
			Messenger.Broadcast<JArray>(NotifyTypes.ModifyResourcesEcho, data);
		}

		/// <summary>
		/// 检测是否是否有新的兵器可以打造，有则加入到待选数据表中
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void CheckNewWeaponIdsOfWorkshop(string cityId) {
			JObject weaponIdsOfWorkshopDatas = JsonManager.GetInstance().GetJson("WeaponIdsOfWorkshopData");
			if (weaponIdsOfWorkshopDatas[cityId] != null) {
				db = OpenDb();
				JArray weaponIdsData = (JArray)weaponIdsOfWorkshopDatas[cityId];
				string weaponid;
				SqliteDataReader sqReader;
                WeaponData weapon;
				for (int i = 0; i < weaponIdsData.Count; i++) {
					weaponid = weaponIdsData[i].ToString();
					if (weaponid != "1") { //步缠手是最基础的武器不能打造
                        //判断此件装备属否只属于主角，需要动态判定主角门派来控制是否能够在工坊里打造
                        weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", weaponid);
                        if (!weapon.JustBelongToHost || weapon.Occupation == HostData.Occupation) {
                            sqReader = db.ExecuteQuery("select Id from WorkshopWeaponBuildingTable where WeaponId = '" + weaponid + "' and BelongToRoleId = '" + currentRoleId + "'");
                            if (!sqReader.HasRows) {
                                db.ExecuteQuery("insert into WorkshopWeaponBuildingTable (WeaponId, State, BelongToCityId, BelongToRoleId) values('" + weaponid + "', 0, '" + cityId + "', '" + currentRoleId + "')");
                            }
                        }
					}
				}
				db.CloseSqlConnection();
			}
			weaponIdsOfWorkshopDatas = null;
		}

		/// <summary>
		/// 创建工坊中的锻造兵器id列表
		/// </summary>
		public void CreateWeaponIdOfWorkShopNewFlagList() {
			db = OpenDb();
			CitySceneModel.WeaponIdOfWorkShopNewFlagList = new List<string>();
			SqliteDataReader sqReader = db.ExecuteQuery("select WeaponId from WorkshopWeaponBuildingTable where BelongToRoleId = '" + currentRoleId + "'");
			while (sqReader.Read()) {
				CitySceneModel.WeaponIdOfWorkShopNewFlagList.Add(sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求工坊兵器打造标签页数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetWorkshopWeaponBuildingTableData() {
			JArray data = new JArray();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select WeaponId from WorkshopWeaponBuildingTable where BelongToRoleId = '" + currentRoleId + "'");
			while (sqReader.Read()) {
				data.Add(sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<JArray>(NotifyTypes.GetWorkshopWeaponBuildingTableDataEcho, data);
		}

		/// <summary>
		/// 打造兵器
		/// </summary>
		/// <param name="weaponId">Weapon identifier.</param>
		public void CreateNewWeaponOfWorkshop(string weaponId) {
			db = OpenDb();
			WeaponData weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", weaponId);
			List<ResourceData> needs = new List<ResourceData>();
			ResourceData need;
			ResourceData find;
			for (int i = 0; i < weapon.Needs.Count; i++) {
				need = weapon.Needs[i];
				find = needs.Find(item => item.Type == need.Type);
				if (find == null) {
					needs.Add(new ResourceData(need.Type, need.Num));
				}
				else {
					find.Num += need.Num;
				}
			}
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			int id = 0;
			if (sqReader.Read()) {
				id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
			}
			db.CloseSqlConnection();

			if (resources != null) {
				bool canAdd = true;
				string msg = "";
				for (int i = 0; i < needs.Count; i++) {
					need = needs[i];
					find = resources.Find(item => item.Type == need.Type);
					if (find != null && find.Num >= need.Num) {
						find.Num -= need.Num;
					}
					else {
						canAdd = false;
						msg = string.Format("{0}不足!", Statics.GetResourceName(need.Type));
						break;
					}
				}
				if (canAdd) {
					//检测添加武器
					if (AddNewWeapon(weapon.Id)) {
						db = OpenDb();
                        db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "' where Id = " + id);
						db.CloseSqlConnection();
						Statics.CreatePopMsg(Vector3.zero, string.Format("<color=\"{0}\">{1}</color>+1", Statics.GetQualityColorString(weapon.Quality), weapon.Name), Color.white, 30);
                        SoundManager.GetInstance().PushSound("ui0007");
                    }
					else {
						AlertCtrl.Show("兵器匣已满!", null);
					}
				}
				else {
					AlertCtrl.Show(msg, null);
				}
			}
		}

		/// <summary>
		/// 请求兵器分解标签页数据
		/// </summary>
		public void GetWorkshopWeaponBreakingTableData() {
			List<WeaponData> weapons = new List<WeaponData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponsTable where (BeUsingByRoleId = '" + currentRoleId + "' or BeUsingByRoleId = '') and BelongToRoleId ='" + currentRoleId + "'");
			WeaponData weapon;
			while (sqReader.Read()) {
				weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
                //闪金以下兵器才能溶解
                if (weapon.Quality < QualityType.FlashGold)
                {
                    weapon.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                    weapon.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
                    for (int i = 0; i < weapon.Needs.Count; i++) {
                        weapon.Needs[i].Num = Mathf.Floor((float)(weapon.Needs[i].Num * 0.5f)); //熔解兵器只返还50%的资源
                    }
                    weapons.Add(weapon);
                }
			}
			db.CloseSqlConnection();
			weapons.Sort((a, b) => b.Quality.CompareTo(a.Quality));
			Messenger.Broadcast<List<WeaponData>>(NotifyTypes.GetWorkshopWeaponBreakingTableDataEcho, weapons);
		}

		/// <summary>
		/// 熔解兵器
		/// </summary>
		/// <param name="primaryKeyId">Primary key identifier.</param>
		public void BreakWeapon(int primaryKeyId) {
			int resultId = 0;
			db = OpenDb();
			//查询资源
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			int id = 0;
			if (sqReader.Read()) {
				id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
			}
			if (resources != null) {
				sqReader = db.ExecuteQuery("select Id, WeaponId from WeaponsTable where Id = " + primaryKeyId);
				if (sqReader.Read()) {
					WeaponData weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
					ResourceData findResource;
					ResourceData resource;
					float addNum;
					for (int i = 0; i < weapon.Needs.Count; i++) {
						resource = weapon.Needs[i];
						findResource = resources.Find(item => item.Type == resource.Type);
						if (findResource != null) {
							addNum = Mathf.Floor((float)(resource.Num * 0.5f));
							//累加资源
							findResource.Num += addNum;
						}
					}
					//删除兵器
					db.ExecuteQuery("delete from WeaponsTable where Id = " + primaryKeyId);
					//更新资源
                    db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "' where Id = " + id);
					resultId = primaryKeyId;
				}
			}
			db.CloseSqlConnection();
			if (primaryKeyId > 0) {
				Messenger.Broadcast<int>(NotifyTypes.BreakWeaponEcho, primaryKeyId);
			}
		}

		/// <summary>
		/// 消耗资源
		/// </summary>
		/// <returns><c>true</c>, if resource was cost, <c>false</c> otherwise.</returns>
		/// <param name="type">Type.</param>
		/// <param name="num">Number.</param>
		public bool CostResource(ResourceType type, int num) {
			ModifyResources();
			bool result = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			int id = 0;
			if (sqReader.Read()) {
				id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
			}

			if (resources != null) {
				ResourceData find = resources.Find(item => item.Type == type);
				if (find != null && find.Num >= num) {
					find.Num -= num;
                    db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "' where Id = " + id);
					result = true;
				}
			}
			db.CloseSqlConnection();
			return result;
		}

		/// <summary>
		/// 查询特定资源的数量
		/// </summary>
		/// <returns>The resource number.</returns>
		/// <param name="type">Type.</param>
		public double GetResourceNum(ResourceType type) {
			ModifyResources();
			double num = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			int id = 0;
			if (sqReader.Read()) {
				id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
			}
			if (resources != null) {
				ResourceData find = resources.Find(item => item.Type == type);
				if (find != null) {
					num = find.Num;
				}
			}
			db.CloseSqlConnection();
			return num;
		}

        public int GetWorkerNum() {
            return Mathf.Clamp(PlayerPrefs.GetInt("WN_For_" + currentRoleId), 0, GetMaxWorkerNum());
        }

        public int GetMaxWorkerNum() {
            return Mathf.Clamp(PlayerPrefs.GetInt("MWN_For_" + currentRoleId), 0, 500 + GetPlusWorkerNum());
        }

        public int GetPlusWorkerNum() {
            return Mathf.Clamp(PlayerPrefs.GetInt("PWN_For_" + currentRoleId), 0, GetMaxPlusWorkerNum());
        }

        public void SetWorkerNum(int num) {
            PlayerPrefs.SetInt("WN_For_" + currentRoleId, Mathf.Clamp(num, 0, GetMaxWorkerNum()));
        }

        public void SetMaxWorkerNum(int num) {
            PlayerPrefs.SetInt("MWN_For_" + currentRoleId, Mathf.Clamp(num, 0, 500 + GetPlusWorkerNum()));
        }

        public void SetPlusWorkerNum(int num) {
            PlayerPrefs.SetInt("PWN_For_" + currentRoleId, Mathf.Clamp(num, 0, GetMaxPlusWorkerNum()));
        }

        public int GetMaxPlusWorkerNum() {
            return 200;
        }

        public void ClearWorkerNums(string roldId) {
            PlayerPrefs.SetInt("WN_For_" + roldId, 0);
            PlayerPrefs.SetInt("MWN_For_" + roldId, 0);
            PlayerPrefs.SetInt("PWN_For_" + roldId, 0);
        }
	}
}