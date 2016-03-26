using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Game {
	/// <summary>
	/// 战斗相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 记录战斗结果
		/// </summary>
		/// <param name="win">If set to <c>true</c> window.</param>
		/// <param name="fightId">Fight identifier.</param>
		/// <param name="star">Star.</param>
		public void SendFightResult(bool win, string fightId, int star) {
			List<DropData> drops = new List<DropData>();
			if (win) {
				db = OpenDb();
				SqliteDataReader sqReader = db.ExecuteQuery("select * from FightWinedRecordsTable where FightId = '" + fightId + "' and BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.HasRows) {
					if (sqReader.Read()) {
						int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
						int starData = sqReader.GetInt32(sqReader.GetOrdinal("Star"));
						starData = star > starData ? star : starData;
						int numData = sqReader.GetInt32(sqReader.GetOrdinal("Num")) + 1;
						db.ExecuteQuery("update FightWinedRecordsTable set Star = " + starData + ", Num = " + numData + " where Id = " + id);
					}
				}
				else {
					db.ExecuteQuery("insert into FightWinedRecordsTable (FightId, Star, Num, DateTime, BelongToRoleId) values('" + fightId + "', " + star + ", 1, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + currentRoleId + "');");
				}
				db.CloseSqlConnection();
				FightData fight = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
				if (fight.Drops.Count > 0) {
					drops = PushItemToBag(fight.Drops);
				}
			}
			else {
				//失败后需要计算队伍中侠客的伤势
				MakeRolesInjury();
			}
			Messenger.Broadcast<bool, List<DropData>>(NotifyTypes.SendFightResultEcho, win, drops);
		}

		/// <summary>
		/// 判断某场战斗是否获胜
		/// </summary>
		/// <returns><c>true</c> if this instance is fight wined the specified fightId; otherwise, <c>false</c>.</returns>
		/// <param name="fightId">Fight identifier.</param>
		public bool IsFightWined(string fightId) {
			bool result = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from FightWinedRecordsTable where FightId = '" + fightId + "' and BelongToRoleId = '" + currentRoleId + "'");
			result = sqReader.HasRows;
			db.CloseSqlConnection();
			return result;
		}

		/// <summary>
		/// 记录使用过的招式
		/// </summary>
		/// <param name="skillId">Skill identifier.</param>
		/// <param name="num">Number.</param>
		public void UpdateUsedTheSkillRecords(string skillId, int num) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from UsedTheSkillRecordsTable where SkillId = '" + skillId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.HasRows) {
				if (sqReader.Read()) {
					int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					int numData = sqReader.GetInt32(sqReader.GetOrdinal("Num")) + num;
					db.ExecuteQuery("update UsedTheSkillRecordsTable set Num = " + numData + " where Id = " + id);
				}
			}
			else {
				db.ExecuteQuery("insert into UsedTheSkillRecordsTable (SkillId, Num, DateTime, BelongToRoleId) values('" + skillId + "', 1, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + currentRoleId + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 查询使用的技能次数
		/// </summary>
		/// <returns>The used the skill times.</returns>
		/// <param name="skillId">Skill identifier.</param>
		public int GetUsedTheSkillTimes(string skillId) {
			int times = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Num from UsedTheSkillRecordsTable where SkillId = '" + skillId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				times = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
			}
			db.CloseSqlConnection();
			return times;
		}

		/// <summary>
		/// 记录武器暴击
		/// </summary>
		/// <param name="plusIndex">Plus index.</param>
		/// <param name="num">Number.</param>
		public void UpdateWeaponPowerPlusSuccessedRecords(int plusIndex, int num) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponPowerPlusSuccessedRecordsTable where PlusIndex = " + plusIndex + " and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.HasRows) {
				if (sqReader.Read()) {
					int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					int numData = sqReader.GetInt32(sqReader.GetOrdinal("Num")) + num;
					db.ExecuteQuery("update WeaponPowerPlusSuccessedRecordsTable set Num = " + numData + " where Id = " + id);
				}
			}
			else {
				db.ExecuteQuery("insert into WeaponPowerPlusSuccessedRecordsTable (PlusIndex, Num, DateTime, BelongToRoleId) values(" + plusIndex + ", 1, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + currentRoleId + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 查询武器暴击次数
		/// </summary>
		/// <param name="plusIndex">Plus index.</param>
		public int GetWeaponPowerPlusSuccessedTimes(int plusIndex) {
			int times = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponPowerPlusSuccessedRecordsTable where PlusIndex = " + plusIndex + " and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				times = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
			}
			db.CloseSqlConnection();
			return times;
		}
	}
}