using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Game {
	/// <summary>
	/// 武器相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 添加新武器
		/// </summary>
		/// <param name="weaponId">Weapon identifier.</param>
		/// <param name="beUsingByRoleId">Be using by role identifier.</param>
		public void AddNewWeapon(string weaponId, string beUsingByRoleId = "") {
			db = OpenDb();
			db.ExecuteQuery("insert into WeaponsTable (WeaponId, BeUsingByRoleId, BelongToRoleId) values('" + weaponId + "', '" + beUsingByRoleId + "', '" + currentRoleId + "')");
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 替换兵器(不允许侠客不拿兵器)
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="beUsingByRoleId">Be using by role identifier.</param>
		public void ReplaceWeapon(int id, string beUsingByRoleId) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponsTable where BeUsingByRoleId = '" + beUsingByRoleId + "' and BelongToRoleId ='" + currentRoleId + "'");
			while (sqReader.Read()) {
				//将兵器先卸下
				int dataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '' where Id = " + dataId);
			}
			sqReader = db.ExecuteQuery("select Id from WeaponsTable where Id = " + id);
			if (sqReader.HasRows) {
				//装备新兵器
				db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '" + beUsingByRoleId + "' where Id = " + id);
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 获取兵器匣界面数据
		/// </summary>
		public void GetWeaponsListPanelData() {
			List<WeaponData> weapons = new List<WeaponData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponsTable where (BeUsingByRoleId = '" + currentRoleId + "' or BeUsingByRoleId = '') and BelongToRoleId ='" + currentRoleId + "'");
			WeaponData weapon;
			while (sqReader.Read()) {
				weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
				weapon.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
				if (weapon.BeUsingByRoleId != currentRoleId) {
					weapons.Add(weapon);
				}
				else {
					//主角的兵器需要排在第一个
					weapons.Insert(0, weapon);
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<WeaponData>>(NotifyTypes.GetWeaponsListPanelDataEcho, weapons);
		}
	}
}