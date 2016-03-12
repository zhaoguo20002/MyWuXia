using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace Game {
	/// <summary>
	/// 角色相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 添加新的角色数据
		/// </summary>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="roleData">Role data.</param>
		/// <param name="state">State.</param>
		/// <param name="seatNo">Seat no.</param>
		/// <param name="belongToRoleId">Belong to role identifier.</param>
		/// <param name="dateTime">Date time.</param>
		public void AddNewRole(string roleId, string roleData, int state, int seatNo, string belongToRoleId, string dateTime) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId from RolesTable where RoleId = '" + roleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into RolesTable values('" + roleId + "', '" + roleData + "', " + state + ", " + seatNo + ", '" + belongToRoleId + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求队伍信息面板数据
		/// </summary>
		public void CallRoleInfoPanelData(bool isfighting) {
			db = OpenDb();
			//正序查询处于战斗队伍中的角色
			SqliteDataReader sqReader = db.ExecuteQuery("select * from RolesTable where BelongToRoleId = '" + currentRoleId + "' and State = 1 order by SeatNo");
			JObject obj = new JObject();
			JArray data = new JArray();
			while (sqReader.Read()) {
				data.Add(new JArray(
					sqReader.GetString(sqReader.GetOrdinal("RoleId")),
					sqReader.GetString(sqReader.GetOrdinal("RoleData")),
					sqReader.GetInt16(sqReader.GetOrdinal("State"))
				));
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
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleData from RolesTable where RoleId = '" + roleId + "' and BelongToRoleId = '" + currentRoleId + "'");
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
	}
}
