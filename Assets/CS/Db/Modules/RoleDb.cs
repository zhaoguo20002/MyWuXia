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
				db.ExecuteQuery("insert into RolesTable values('" + roleId + "', '" + roleData + "', " + state + ", " + seatNo + ", '" + hometownCityId + "', '" + belongToRoleId + "', '" + dateTime + "');");
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
					sqReader.GetInt16(sqReader.GetOrdinal("State"))
				));
				//缓存主角数据
				if (roleId == currentRoleId) {
					HostData = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
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
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleData from RolesTable where RoleId = '" + roleId + "' and State > 0 and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				data = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
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
						db.ExecuteQuery("insert into RolesTable values('" + roleId + "', '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "', 0, -1, '" + role.HometownCityId + "', '" + currentRoleId + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
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
					role.State = (RoleStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
					roles.Add(role);
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<RoleData>>(NotifyTypes.GetRolesOfWinShopPanelDataEcho, roles);
		}
	}
}
