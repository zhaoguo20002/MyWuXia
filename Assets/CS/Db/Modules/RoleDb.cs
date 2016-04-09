using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 角色相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 主角数据
		/// </summary>
		public RoleData HostData;
		/// <summary>
		/// 添加新的角色数据
		/// </summary>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="roleData">Role data.</param>
		/// <param name="state">State.</param>
		/// <param name="seatNo">Seat no.</param>
		/// <param name="belongToRoleId">Belong to role identifier.</param>
		/// <param name="dateTime">Date time.</param>
		public bool AddNewRole(string roleId, string roleData, int state, int seatNo, string hometownCityId, string belongToRoleId, string dateTime) {
			bool result = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId from RolesTable where RoleId = '" + roleId + "' and BelongToRoleId = '" + belongToRoleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into RolesTable (RoleId, RoleData, State, SeatNo, HometownCityId, BelongToRoleId, InjuryType, Ticks, DateTime) values('" + roleId + "', '" + roleData + "', " + state + ", " + seatNo + ", '" + hometownCityId + "', '" + belongToRoleId + "', " + ((int)InjuryType.None) + ", " + DateTime.Now.Ticks + ", '" + dateTime + "');");
				result = true;
			}
			db.CloseSqlConnection();
			return result;
		}

		/// <summary>
		/// 请求队伍信息面板数据
		/// </summary>
		public void CallRoleInfoPanelData(bool isfighting) {
			db = OpenDb();
			//正序查询处于战斗队伍中的角色
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where BelongToRoleId = '" + currentRoleId + "' and State = " + (int)RoleStateType.InTeam + " order by SeatNo");
			JObject obj = new JObject();
			JArray data = new JArray();
			string roleId;
			while (sqReader.Read()) {
				roleId = sqReader.GetString(sqReader.GetOrdinal("RoleId"));
				data.Add(new JArray(
					roleId,
					sqReader.GetString(sqReader.GetOrdinal("RoleData")),
					sqReader.GetInt16(sqReader.GetOrdinal("State")),
					sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"))
				));
				//缓存主角数据
				if (roleId == currentRoleId) {
					HostData = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
					HostData.Injury = (InjuryType)sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
				}
			}
			obj["data"] = data;
			db.CloseSqlConnection();
			Messenger.Broadcast<JObject, bool>(NotifyTypes.CallRoleInfoPanelDataEcho, obj, isfighting);
		}

		/// <summary>
		/// 根据角色id查询角色数据
		/// </summary>
		/// <returns>The role data by role identifier.</returns>
		/// <param name="roleId">Role identifier.</param>
		public RoleData GetRoleDataByRoleId(string roleId) {
			RoleData data = null;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, RoleData, InjuryType from RolesTable where RoleId = '" + roleId + "' and State > 0 and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				data = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				data.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				data.Injury = (InjuryType)sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
			}
			db.CloseSqlConnection();
			return data;
		}

		/// <summary>
		/// 获取主角的角色数据
		/// </summary>
		/// <returns>The host role data.</returns>
		public RoleData GetHostRoleData() {
			return GetRoleDataByRoleId(currentRoleId);
		}

		/// <summary>
		/// 检测是否是否有新的侠客可以招募，有则加入到待选数据表中
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void CheckNewRoleIdsOfWinShop(string cityId) {
			JObject roleIdsOfWinShopDatas = JsonManager.GetInstance().GetJson("RoleIdsOfWinShopDatas");
			if (roleIdsOfWinShopDatas[cityId] != null) {
				db = OpenDb();
				JArray roleIdsData = (JArray)roleIdsOfWinShopDatas[cityId];
				string roleId;
				for (int i = 0; i < roleIdsData.Count; i++) {
					roleId = roleIdsData[i].ToString();
					SqliteDataReader sqReader = db.ExecuteQuery("select RoleData from RolesTable where RoleId = '" + roleId + "' and BelongToRoleId = '" + currentRoleId + "'");
					if (!sqReader.HasRows) {
						RoleData role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", roleId);
						db.ExecuteQuery("insert into RolesTable (RoleId, RoleData, State, SeatNo, HometownCityId, BelongToRoleId, InjuryType, Ticks, DateTime) values('" + roleId + "', '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "', 0, 888, '" + role.HometownCityId + "', '" + currentRoleId + "', " + ((int)InjuryType.None) + ", " + DateTime.Now.Ticks + ", '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
					}
				}
				db.CloseSqlConnection();
			}
			roleIdsOfWinShopDatas = null;
		}

		/// <summary>
		/// 请求酒馆中的侠客列表
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetRolesOfWinShopPanelData(string cityId) {
			db = OpenDb();
			List<RoleData> roles = new List<RoleData>();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where HometownCityId = '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "'");
			RoleData role;
			while (sqReader.Read()) {
				if (sqReader.GetString(sqReader.GetOrdinal("RoleId")) != sqReader.GetString(sqReader.GetOrdinal("BelongToRoleId"))) {
					role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
					role.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					role.State = (RoleStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
					role.Injury = (InjuryType)sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
					roles.Add(role);
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<RoleData>>(NotifyTypes.GetRolesOfWinShopPanelDataEcho, roles);
		}

		/// <summary>
		/// 结交侠客
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void InviteRole(int id) {
			bool invited = false;
			RoleData role = null;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, RoleId, State from RolesTable where Id = " + id);
			if (sqReader.Read()) {
				role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", sqReader.GetString(sqReader.GetOrdinal("RoleId")));
				RoleStateType state = (RoleStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				if (state == RoleStateType.NotRecruited) {
					sqReader = db.ExecuteQuery("select * from WeaponsTable where WeaponId = '" + role.ResourceWeaponDataId + "' and BeUsingByRoleId == '' and BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						//删掉兵器
						db.ExecuteQuery("delete from WeaponsTable where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
						//结交侠客
						db.ExecuteQuery("update RolesTable set State = " + ((int)RoleStateType.OutTeam) + ", SeatNo = 888 where Id = " + id);
						invited = true;
					}
					else {
						AlertCtrl.Show("你的兵器匣里并没有称手的兵器!", null);
					}
				}
				else {
					AlertCtrl.Show("你们已经结识!", null);
				}
			}
			db.CloseSqlConnection();
			if (invited && role != null) {
				Statics.CreatePopMsg(Vector3.zero, string.Format("你与<color=\"#FFFF00\">{0}</color>撮土为香，结成八拜之交!", role.Name), Color.white, 30);
				GetRolesOfWinShopPanelData(role.HometownCityId);
			}
		}

		/// <summary>
		/// 获取准备出发界面数据
		/// </summary>
		public void GetReadyToTravelPanelData() {
			ModifyResources();
			List<RoleData> roles = new List<RoleData>();
			ItemData food = null;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where State != " + ((int)RoleStateType.NotRecruited) + " and BelongToRoleId = '" + currentRoleId + "' order by State");
			RoleData role;
			while (sqReader.Read()) {
				role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				role.MakeJsonToModel();
				role.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				role.State = (RoleStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				role.Injury = (InjuryType)sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
				roles.Add(role);
			}
			sqReader = db.ExecuteQuery("select Data, AreaFoodNum from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				UserData user = JsonManager.GetInstance().DeserializeObject<UserData>(sqReader.GetString(sqReader.GetOrdinal("Data")));
				user.AreaFood.Num = sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum"));
				if (user.AreaFood.Num < user.AreaFood.MaxNum) {
					sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						//更新
						List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
						ResourceData resource = resources.Find(item => item.Type == ResourceType.Food);
						double cutNum = (double)(user.AreaFood.MaxNum - user.AreaFood.Num);
						cutNum = resource.Num >= cutNum ? cutNum : resource.Num;
						if (cutNum > 0) {
							resource.Num -= cutNum;
							user.AreaFood.Num += (int)cutNum;
						}
					}
				}
				food = user.AreaFood;
			}
			db.CloseSqlConnection();
			if (roles.Count > 0 && food != null) {
				Messenger.Broadcast<List<RoleData>, ItemData>(NotifyTypes.GetReadyToTravelPanelDataEcho, roles, food);
			}
		}

		/// <summary>
		/// 修改队伍的位子编号
		/// </summary>
		/// <param name="ids">Identifiers.</param>
		public void ChangeRolesSeatNo(JArray ids) {
			db = OpenDb();
			//将原来的角色先全部下阵
			db.ExecuteQuery("update RolesTable set State = " + ((int)RoleStateType.OutTeam) + " where State = " + ((int)RoleStateType.InTeam) + " and BelongToRoleId = '" + currentRoleId + "'");
			string id;
			for (int i = 0; i < ids.Count; i++) {
				id = ids[i].ToString();
				db.ExecuteQuery("update RolesTable set State = " + ((int)RoleStateType.InTeam) + ", SeatNo = " + i + " where Id = " + id);
			}
			//处理干粮
			SqliteDataReader sqReader = db.ExecuteQuery("select Data, AreaFoodNum from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				UserData user = JsonManager.GetInstance().DeserializeObject<UserData>(sqReader.GetString(sqReader.GetOrdinal("Data")));
				user.AreaFood.Num = sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum"));
				if (user.AreaFood.Num < user.AreaFood.MaxNum) {
					sqReader = db.ExecuteQuery("select * from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						//更新
						List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
						ResourceData resource = resources.Find(item => item.Type == ResourceType.Food);
						double cutNum = (double)(user.AreaFood.MaxNum - user.AreaFood.Num);
						cutNum = resource.Num >= cutNum ? cutNum : resource.Num;
						if (cutNum > 0) {
							resource.Num -= cutNum;
							user.AreaFood.Num += (int)cutNum;
							//扣除工坊中的干粮
							db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
							//增加随身携带的干粮
							db.ExecuteQuery("update UserDatasTable set AreaFoodNum = " + user.AreaFood.Num + " where BelongToRoleId = '" + currentRoleId + "'");
						}
					}
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast(NotifyTypes.ChangeRolesSeatNoEcho);
		}

		/// <summary>
		/// 请求医馆角色数据
		/// </summary>
		public void GetHospitalPanelData() {
			db = OpenDb();
			List<RoleData> roles = new List<RoleData>();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where State != " + ((int)RoleStateType.NotRecruited) + " and BelongToRoleId = '" + currentRoleId + "'");
			RoleData role;
			while (sqReader.Read()) {
				role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				role.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				role.State = (RoleStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				role.Injury = (InjuryType)sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
				roles.Add(role);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<RoleData>>(NotifyTypes.GetHospitalPanelDataEcho, roles);
		}

		/// <summary>
		/// 处理队伍中侠客的伤势
		/// </summary>
		public void MakeRolesInjury() {
			db = OpenDb();
			//健康的时候有概率变成之后的任何伤,受伤再死亡只会变成当前伤势之后的下一档伤
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, RoleId, InjuryType from RolesTable where State = " + ((int)RoleStateType.InTeam) + " and BelongToRoleId = '" + currentRoleId + "'");
			Dictionary<int, int> injuryMapping = new Dictionary<int, int>();
			int hostPrimaryKeyId = -1;
			while(sqReader.Read()) {
				injuryMapping.Add(sqReader.GetInt32(sqReader.GetOrdinal("Id")), sqReader.GetInt32(sqReader.GetOrdinal("InjuryType")));
				if (sqReader.GetString(sqReader.GetOrdinal("RoleId")) == currentRoleId) {
					hostPrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				}
			}
			int randomValue = 0;
			int injury = 0;
			foreach(int id in injuryMapping.Keys) {
				injury = injuryMapping[id];
				//绿伤会随机受伤
				if (injuryMapping[id] <= 0) {
					randomValue = UnityEngine.Random.Range(0, 10000);
					//有50%概率不受伤
					if (randomValue >= 5000 && randomValue < 9000) {
						injury = (int)InjuryType.White;
					}
					else if (randomValue >= 9000 && randomValue < 9500) {
						injury = (int)InjuryType.Yellow;
					}
					else if (randomValue >= 9500 && randomValue < 9750) {
						injury = (int)InjuryType.Purple;
					}
					else if (randomValue >= 9750 && randomValue < 9990) {
						injury = (int)InjuryType.Red;
					}
					else if (randomValue >= 9990) {
						injury = (int)InjuryType.Purple;
					}
				}
				else if (injuryMapping[id] < 5) {
					//已经受伤则往下个等级伤势演变,垂死状态不再受伤
					injury++;
				}
				db.ExecuteQuery("update RolesTable set InjuryType = " + injury + " where Id = " + id);
				if (hostPrimaryKeyId != id && injury == (int)InjuryType.Moribund) {
					//垂死的侠客要强制下阵，主角除外
					db.ExecuteQuery("update RolesTable set State = " + ((int)RoleStateType.OutTeam) + ", SeatNo = 888 where Id = " + id);
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 治疗侠客
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void CureRole(int id) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleData, InjuryType from RolesTable where Id = " + id);
			int injury;
			bool success = false;
			RoleData role = null;
			if (sqReader.Read()) {
				injury = sqReader.GetInt32(sqReader.GetOrdinal("InjuryType"));
				role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				if (injury > 0) {
					sqReader = db.ExecuteQuery("select Id, Num from BagTable where Type = " + ((int)ItemType.Vulnerary) + " and Lv >= " + injury + " and BelongToRoleId = '" + currentRoleId + "' order by Lv");
					int primaryKeyId = 0;
					int num = 0;
					if (sqReader.Read()) {
						primaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
						num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
						if (num > 1) {
							db.ExecuteQuery("update BagTable set Num = " + (num - 1) + " where Id = " + primaryKeyId);
						}
						else {
							db.ExecuteQuery("delete from BagTable where Id = " + primaryKeyId);
						}
						db.ExecuteQuery("update RolesTable set InjuryType = " + ((int)InjuryType.None) + " where Id = " + id);
						success = true;
					}
					else {
						AlertCtrl.Show(string.Format("行囊中并未发现有能够治愈{0}的伤药(需要{1}级伤药)", Statics.GetInjuryName((InjuryType)injury), injury), null);
					}
				}
			}
			db.CloseSqlConnection();

			if (success && role != null) {
				Statics.CreatePopMsg(Vector3.zero, string.Format("<color=\"#FFFF00\">{0}</color>{1}的伤势已经痊愈!", role.Name, Statics.GetGenderDesc(role.Gender)), Color.white, 30);
				GetHospitalPanelData();
				CallRoleInfoPanelData(false); //刷新队伍数据
			}
		}
	}
}
