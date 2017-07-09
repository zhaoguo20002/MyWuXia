using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Game {
	/// <summary>
	/// 通用数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 佛洛依德算法实体类
		/// </summary>
		Floyd floyd;
		/// <summary>
		/// 城镇在临接矩阵中的索引与城镇Id的关联表
		/// </summary>
		static Dictionary<int, string> cityIndexToIdMapping;
		/// <summary>
		/// 城镇在临接矩阵中的索引与城镇名的关联表
		/// </summary>
		static Dictionary<int, string> cityIndexToNameMapping;
		/// <summary>
		/// 创建数据库
		/// </summary>
		public void CreateAllDbs() {
			db = OpenDb();

			#region 初始化存档表相关数据
			//存档数据表
			db.ExecuteQuery("create table if not exists RecordsTable (Id integer primary key autoincrement not null, RoleId text not null, Name text not null, Data text, DateTime text not null)");
			//用户基础数据表
			db.ExecuteQuery("create table if not exists UserDatasTable (Id integer primary key autoincrement not null, Data text, AreaFoodNum integer not null, TimeAngle single not null, TimeTicks long not null, BelongToRoleId text not null, DateTime text not null)");
			#endregion
		
			#region 初始化角色相关数据
			//当前获得的伙伴数据表
			db.ExecuteQuery("create table if not exists RolesTable (Id integer primary key autoincrement not null, RoleId text not null, RoleData text not null, State integer not null, SeatNo integer not null, HometownCityId text not null, BelongToRoleId text not null, InjuryType integer not null, Ticks long not null, DateTime text not null)");
			//背包数据表
			db.ExecuteQuery("create table if not exists BagTable (Id integer primary key autoincrement not null, ItemId text not null, Type integer not null, Num integer not null, MaxNum integer not null, Lv integer not null, BelongToRoleId text not null)");
			//玩家进入的区域大地图记录表
			db.ExecuteQuery("create table if not exists EnterAreaTable (Id integer primary key autoincrement not null, AreaName text not null, BelongToRoleId text not null)");
			//玩家进入的城镇记录表
			db.ExecuteQuery("create table if not exists EnterCityTable (Id integer primary key autoincrement not null, CityId text not null, BelongToRoleId text not null)");
			//兵器匣数据表
			db.ExecuteQuery("create table if not exists WeaponsTable (Id integer primary key autoincrement not null, WeaponId text not null, BeUsingByRoleId text not null, BelongToRoleId text not null)");
			//秘籍数据表
			db.ExecuteQuery("create table if not exists BooksTable (Id integer primary key not null, BookId text not null, State integer not null, SeatNo integer not null, BeUsingByRoleId text not null, BelongToCityId text not null, BelongToRoleId text not null)");
			#endregion

			#region 初始化任务表相关数据
			//当前可以操作的任务数据表(包括可以接取的任务,已完成的任务和接取条件不满足的任务)
			db.ExecuteQuery("create table if not exists TasksTable (Id integer primary key autoincrement not null, TaskId text not null, ProgressData text not null, CurrentDialogIndex integer not null, State integer not null, BelongToRoleId text not null)");
			//初始化动态事件表
			db.ExecuteQuery("create table if not exists EventsTable (Id integer primary key autoincrement not null, X integer not null, Y integer not null, Type integer not null, EventId text not null, SceneId text not null, Name text not null, BelongToRoleId text not null)");
			#endregion

			#region 初始化战斗记录相关数据
			//战斗获胜记录表
			db.ExecuteQuery("create table if not exists FightWinedRecordsTable (Id integer primary key autoincrement not null, FightId text not null, Star integer not null, Num integer not null, DateTime text not null, BelongToRoleId text not null)");
			//使用过的技能记录表
			db.ExecuteQuery("create table if not exists UsedTheSkillRecordsTable (Id integer primary key autoincrement not null, SkillId text not null, Num integer not null, DateTime text not null, BelongToRoleId text not null)");
			//各阶段兵器暴击次数记录表
			db.ExecuteQuery("create table if not exists WeaponPowerPlusSuccessedRecordsTable (Id integer primary key autoincrement not null, PlusIndex integer not null, Num integer not null, DateTime text not null, BelongToRoleId text not null)");
			//使用过的物品记录表
			db.ExecuteQuery("create table if not exists UsedItemRecordsTable (Id integer primary key autoincrement not null, ItemId text not null, Num integer not null, BelongToRoleId text not null)");
			#endregion

			#region 初始化工坊相关数据
			//工坊资源表
			db.ExecuteQuery("create table if not exists WorkshopResourceTable (Id integer primary key autoincrement not null, ResourcesData text not null, Ticks long not null, WorkerNum int not null, MaxWorkerNum int not null, BelongToRoleId text not null)");
			//工坊打造兵器表
			db.ExecuteQuery("create table if not exists WorkshopWeaponBuildingTable (Id integer primary key autoincrement not null, WeaponId text not null, State int not null, BelongToCityId text not null, BelongToRoleId text not null)");
			#endregion

			initTasks();

			db.CloseSqlConnection();

			//初始化佛洛依德算法
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/FloydDis");
			List<List<float>> dis = JsonManager.GetInstance().DeserializeObject<List<List<float>>>(asset.text);
			floyd = new Floyd(dis, dis.Count);
			asset = Resources.Load<TextAsset>("Data/Json/SceneIndexToIds");
			cityIndexToIdMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<int, string>>(asset.text);
			asset = Resources.Load<TextAsset>("Data/Json/SceneIndexToNames");
			cityIndexToNameMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<int, string>>(asset.text);
			asset = null;
			dis = null;

//			AddNewRecord(currentRoleId, "-", "{}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//
//			UserData userData = new UserData();
//			userData.AreaFood = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "1");
//			userData.AreaFood.Num = 0;
//			userData.AreaFood.MaxNum = 100;
//			userData.PositionStatu = UserPositionStatusType.InCity;
//			userData.CurrentAreaSceneName = "Area0";
//			userData.CurrentCitySceneId = "1";
//			userData.CurrentAreaX = 9;
//			userData.CurrentAreaY = 8;
//			AddNewUserData(JsonManager.GetInstance().SerializeObjectDealVector(userData), userData.AreaFood.Num, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//
//			RoleData role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", "1");
//			role.IsHost = true;
//			role.Id = currentRoleId;
//			role.Occupation = OccupationType.None;
//			role.ResourceBookDataIds.Clear();
//			if (AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), (int)RoleStateType.InTeam, 0, role.HometownCityId, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) {
//				AddNewWeapon(role.ResourceWeaponDataId, role.Id);
//				AddNewWeapon("2");
//				AddNewWeapon("22");
//				AddNewWeapon("11");
//				AddNewWeapon("3");
//			}

//			role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", "2");
//			AddNewRole(role.Id, JsonManager.GetInstance().SerializeObjectDealVector(role), (int)RoleStateType.InTeam, 1, role.HometownCityId, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//			role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", "3");
//			AddNewRole(role.Id, JsonManager.GetInstance().SerializeObjectDealVector(role), (int)RoleStateType.InTeam, 2, role.HometownCityId, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

		}

		/// <summary>
		/// 返回某个表的总记录条数
		/// </summary>
		/// <returns>The table row count.</returns>
		/// <param name="tableName">Table name.</param>
		public int GetTableRowCount(string tableName) {
			db = OpenDb();
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
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from RecordsTable where RoleId = '" + roleId + "'");
			if (!sqReader.HasRows) {
				//首次创建存档记录
				Debug.LogWarning("首次创建存档记录");
				db.ExecuteQuery("insert into RecordsTable (RoleId, Name, Data, DateTime) values('" + roleId + "', '" + name + "', '" + data + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 查询游戏存档数
		/// </summary>
		/// <returns>The record number.</returns>
		public int GetRecordNum() {
			int num = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select count(*) as num from RecordsTable");
			if (sqReader.Read()) {
				num = sqReader.GetInt32(sqReader.GetOrdinal("num"));
			}
			db.CloseSqlConnection();
			return num;
		}

		/// <summary>
		/// 查询游戏记录数据
		/// </summary>
		public void GetRecordListData() {
			List<JArray> data = new List<JArray>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RecordsTable order by Id");
			while (sqReader.Read()) {
				data.Add(
					new JArray(
						sqReader.GetInt32(sqReader.GetOrdinal("Id")),
						sqReader.GetString(sqReader.GetOrdinal("RoleId")),
						sqReader.GetString(sqReader.GetOrdinal("Name")),
						sqReader.GetString(sqReader.GetOrdinal("Data")),
						sqReader.GetString(sqReader.GetOrdinal("DateTime"))
					)
				);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<JArray>>(NotifyTypes.GetRecordListDataEcho, data);
		}

		/// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void DeleteRecord(int id) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId from RecordsTable where Id = " + id);
			string roleId = "";
			if (sqReader.Read()) {
				roleId = sqReader.GetString(sqReader.GetOrdinal("RoleId"));
			}
			if (roleId != "") {
				db.ExecuteQuery("delete from RecordsTable where Id = " + id);
				db.ExecuteQuery("delete from UserDatasTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from RolesTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from BagTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from EnterAreaTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from EnterCityTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from WeaponsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from BooksTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from TasksTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from EventsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from FightWinedRecordsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from UsedTheSkillRecordsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from WeaponPowerPlusSuccessedRecordsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from UsedItemRecordsTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from WorkshopResourceTable where BelongToRoleId = '" + roleId + "'");
				db.ExecuteQuery("delete from WorkshopWeaponBuildingTable where BelongToRoleId = '" + roleId + "'");
			}
			db.CloseSqlConnection();
			Messenger.Broadcast(NotifyTypes.ShowMainPanel);
			GetRecordListData();
		}

		/// <summary>
		/// 添加用户基础数据
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="num">Number.</param>
		/// <param name="belongToRoleId">Belong to role identifier.</param>
		/// <param name="dateTime">Date time.</param>
		public void AddNewUserData(string data, int num, string belongToRoleId, string dateTime) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from UserDatasTable where BelongToRoleId = '" + belongToRoleId + "'");
			if (!sqReader.HasRows) {
				//首次创建用户基础数据记录
				Debug.LogWarning("首次创建用户基础数据记录");
				db.ExecuteQuery("insert into UserDatasTable (Data, AreaFoodNum, TimeAngle, TimeTicks, BelongToRoleId, DateTime) values('" + data + "', " + num + ", 0, " + DateTime.Now.Ticks + ", '" + belongToRoleId + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求用户基础信息
		/// </summary>
		public void CallUserData() {
			db = OpenDb();
			//正序查询处于战斗队伍中的角色
			SqliteDataReader sqReader = db.ExecuteQuery("select * from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			JObject obj = new JObject();
			JArray data = new JArray();
			while (sqReader.Read()) {
				data.Add(sqReader.GetString(sqReader.GetOrdinal("BelongToRoleId")));
				data.Add(sqReader.GetString(sqReader.GetOrdinal("Data")));
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum")));
				data.Add(sqReader.GetFloat(sqReader.GetOrdinal("TimeAngle")));
				data.Add(sqReader.GetInt64(sqReader.GetOrdinal("TimeTicks")));
			}
			obj["data"] = data;
			db.CloseSqlConnection();
			Messenger.Broadcast<JObject>(NotifyTypes.CallUserDataEcho, obj);
		}

		/// <summary>
		/// 更新用户基础信息
		/// </summary>
		/// <param name="dataStr">Data string.</param>
		public void UpdateUserData(string dataStr) {
			db = OpenDb();
			db.ExecuteQuery("update UserDatasTable set Data = '" + dataStr + "' where BelongToRoleId = '" + currentRoleId + "'");
			db.CloseSqlConnection();
			//更新完后立马返回查询结果
			CallUserData();
		}

		/// <summary>
		/// 更新时辰时间戳
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <param name="ticks">Ticks.</param>
		public void UpdateTimeTicks(float angle, long ticks) {
			db = OpenDb();
			db.ExecuteQuery("update UserDatasTable set TimeAngle = " + angle + ", TimeTicks = " + ticks + " where BelongToRoleId = '" + currentRoleId + "'");
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 更新体力食物的数量
		/// </summary>
		/// <param name="ticks">Ticks.</param>
		public void UpdateAreaFoodNum(int num) {
			db = OpenDb();
			db.ExecuteQuery("update UserDatasTable set AreaFoodNum = " + num + " where BelongToRoleId = '" + currentRoleId + "'");
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 记录进入的区域大地图
		/// </summary>
		/// <param name="areaName">Area name.</param>
		public void CheckEnterArea(string areaName) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from EnterAreaTable where AreaName = '" + areaName + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into EnterAreaTable (AreaName, BelongToRoleId) values('" + areaName + "', '" + currentRoleId + "')");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 记录进入的城镇
		/// </summary>
		/// <param name="areaName">Area name.</param>
		public void CheckEnterCity(string cityId) {
			SceneData scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
			//战斗据点不处理任何增加数据
			if (scene.IsJustFightScene) {
				return;
			}
			//非战斗城镇才需要检测各项数据
			if (!scene.IsJustFightScene) {
				//检测是否有新的可以招募的侠客
				CheckNewRoleIdsOfWinShop(cityId);
				//初始化用于判定新增结识侠客的id列表
				CreateRoleIdOfWinShopNewFlagList();
				//检测工坊是否有新的生产单元
				CheckNewWorkshopItems(cityId);
				//检测是否有新的可以打造的兵器
				CheckNewWeaponIdsOfWorkshop(cityId);
				//初始化用于判定新增锻造兵器的id列表
				CreateWeaponIdOfWorkShopNewFlagList();
				//检测是否发现新的秘籍
				CheckNewBooksOfForbiddenArea(cityId);
				//初始化用于判定秘境中新增秘籍的id列表
				CreateBookIdOfCurrentForbiddenAreaNewFlagList(cityId);
				//将背包里的辎重箱资源存入工坊
				BringResourcesToWorkshop();
			}
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from EnterCityTable where CityId = '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into EnterCityTable (CityId, BelongToRoleId) values('" + cityId + "', '" + currentRoleId + "')");
				//根据开启的城镇数计算工坊的家丁数上限和干粮上限
				sqReader = db.ExecuteQuery("select count(*) as num from EnterCityTable where BelongToRoleId = '" + currentRoleId + "'");
				int num = 0;
				int maxWorkerNum = 0;
				int areaFoodMaxNum = 0;
				if (sqReader.Read()) {
					num = sqReader.GetInt32(sqReader.GetOrdinal("num"));
					//新到一个城镇会增加5个家丁
					maxWorkerNum = Mathf.Clamp(10 + num * 5, 15, 200); //上限200
					//新到一个城镇会增加10个干粮上限
					areaFoodMaxNum = Mathf.Clamp(20 + num * 10, 30, 300); //上限300 
				}
				if (maxWorkerNum > 0) {
					sqReader = db.ExecuteQuery("select Id, WorkerNum, MaxWorkerNum from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
						int oldWorkerNum = sqReader.GetInt32(sqReader.GetOrdinal("WorkerNum"));
						int oldMaxWorkerNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxWorkerNum"));
						if (oldMaxWorkerNum < maxWorkerNum) {
							//累加空闲家丁，更新家丁上限
							db.ExecuteQuery("update WorkshopResourceTable set WorkerNum = " + (oldWorkerNum + (maxWorkerNum - oldMaxWorkerNum)) + ", MaxWorkerNum = " + maxWorkerNum + " where Id = '" + id + "'");
						}
					}
				}
				if (areaFoodMaxNum > 0) {
					sqReader = db.ExecuteQuery("select Id, Data from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						UserData user = JsonManager.GetInstance().DeserializeObject<UserData>(sqReader.GetString(sqReader.GetOrdinal("Data")));
						user.AreaFood.MaxNum = areaFoodMaxNum;
						db.ExecuteQuery("Update UserDatasTable set Data = '" + JsonManager.GetInstance().SerializeObjectDealVector(user) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
						if (UserModel.CurrentUserData != null) {
							UserModel.CurrentUserData.AreaFood.MaxNum = user.AreaFood.MaxNum;
						}
					}
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 区域大地图上移动判定
		/// </summary>
		/// <param name="direction">Direction.</param>
		/// <param name="duringMove">If set to <c>true</c> during move.</param>
		public void MoveOnArea(string direction, bool duringMove) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, AreaFoodNum from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			int foodNum = 0;
			if (sqReader.Read()) {
				foodNum = sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum"));
				if (foodNum > 0) {
					foodNum--;
					db.ExecuteQuery("update UserDatasTable set AreaFoodNum = '" + foodNum + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<string, int, bool>(NotifyTypes.MoveOnAreaEcho, direction, foodNum, duringMove);
		}

		/// <summary>
		/// 查询城镇开启情况列表
		/// </summary>
		public void GetCitySceneMenuData(string cityId) {
			List<string> openedCityIds = new List<string>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select CityId from EnterCityTable where BelongToRoleId = '" + currentRoleId + "'");
			while(sqReader.Read()) {
				openedCityIds.Add(sqReader.GetString(sqReader.GetOrdinal("CityId")));
			}
			db.CloseSqlConnection();
			SceneData scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
			scene.MakeJsonToModel();
			CityScenePanelCtrl.Show(scene, openedCityIds);
		}

		/// <summary>
		/// 获取城镇中驿站的传送点数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetInnInCityData(string cityId) {
			List<FloydResult> results = new List<FloydResult>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select CityId from EnterCityTable where CityId != '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "'");
			SceneData currentScene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
			SceneData scene;
			FloydResult result;
			while(sqReader.Read()) {
				scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", sqReader.GetString(sqReader.GetOrdinal("CityId")));
				if (!scene.IsJustFightScene) {
					result = floyd.GetResult(currentScene.FloydIndex, scene.FloydIndex);
					if (result != null && result.Distance < 1000) {
						result.Id = scene.Id;
						result.Name = scene.Name;
						result.FromIndex = currentScene.FloydIndex;
						result.ToIndex = scene.FloydIndex;
						results.Add(result);
					}
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<FloydResult>>(NotifyTypes.GetInnInCityDataEcho, results);
		}

		/// <summary>
		/// 寻路索引转换成城镇id
		/// </summary>
		/// <returns>The city identifier by index.</returns>
		/// <param name="index">Index.</param>
		public string GetCityIdByIndex(int index) {
			return cityIndexToIdMapping.ContainsKey(index) ? cityIndexToIdMapping[index] : "";
		}

		/// <summary>
		/// 寻路索引转换成城镇名
		/// </summary>
		/// <returns>The city name by index.</returns>
		/// <param name="index">Index.</param>
		public string GetCityNameByIndex(int index) {
			return cityIndexToNameMapping.ContainsKey(index) ? cityIndexToNameMapping[index] : "";
		}

		/// <summary>
		/// 前往城镇
		/// </summary>
		/// <param name="fromIndex">From index.</param>
		/// <param name="toIndex">To index.</param>
		public void GoToCity(int fromIndex, int toIndex) {
			SceneData toScene = null;
			ModifyResources();
			db = OpenDb();
			string indexToId = JsonManager.GetInstance().GetMapping<string>("SceneIndexToIds", toIndex.ToString());
			SqliteDataReader sqReader = db.ExecuteQuery("select CityId from EnterCityTable where CityId == '" + indexToId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.HasRows) {
				FloydResult result = floyd.GetResult(fromIndex, toIndex);
				//查询银子是否足够支付路费
				sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
				List<ResourceData> resources = null;
				if (sqReader.Read()) {
					resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
					//查询目前的银子余额
					ResourceData resource = resources.Find(re => re.Type == ResourceType.Silver);
					if (resource != null) {
						if (resource.Num >= result.Distance) {
							resource.Num -= result.Distance;
							//扣钱
							db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
							toScene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", GetCityIdByIndex(toIndex));
							Debug.LogWarning(GetCityIdByIndex(toIndex) + "," + toScene.Id + "," + toScene.Name);
						}
						else {
							AlertCtrl.Show("银子不够支付路费！");
						}
					}
				}
			}
			else {
				AlertCtrl.Show("并没有开启前方传送点！");
			}
			db.CloseSqlConnection();
			if (toScene != null) {
				Messenger.Broadcast<SceneData>(NotifyTypes.GoToCityEcho, toScene);
			}
		}
	}
}
