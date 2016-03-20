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
			//查询角色信息
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId, RoleData from RolesTable where RoleId = '" + beUsingByRoleId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.HasRows) {
				sqReader = db.ExecuteQuery("select * from WeaponsTable where BeUsingByRoleId = '" + beUsingByRoleId + "' and BelongToRoleId ='" + currentRoleId + "'");
				while (sqReader.Read()) {
					//将兵器先卸下
					int dataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '' where Id = " + dataId);
				}
				sqReader = db.ExecuteQuery("select Id, WeaponId from WeaponsTable where Id = " + id);
				if (sqReader.Read()) {
					string weaponId = sqReader.GetString(sqReader.GetOrdinal("WeaponId"));
					//装备新兵器
					db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '" + beUsingByRoleId + "' where Id = " + id);
					//更新角色的武器信息
					RoleData role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", beUsingByRoleId);
					role.ResourceWeaponDataId = weaponId;
					db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "' where RoleId = '" + beUsingByRoleId + "'");
				}
			}
			db.CloseSqlConnection();
			GetWeaponsListPanelData(); //刷新兵器匣列表
			CallRoleInfoPanelData(false); //刷新队伍数据
		}

		/// <summary>
		/// 获取兵器匣界面数据
		/// </summary>
		public void GetWeaponsListPanelData() {
			List<WeaponData> weapons = new List<WeaponData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponsTable where (BeUsingByRoleId = '" + currentRoleId + "' or BeUsingByRoleId = '') and BelongToRoleId ='" + currentRoleId + "'");
			WeaponData weapon;
			WeaponData hostWeapon = null;
			while (sqReader.Read()) {
				weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", sqReader.GetString(sqReader.GetOrdinal("WeaponId")));
				weapon.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				weapon.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
				if (weapon.BeUsingByRoleId != currentRoleId) {
					weapons.Add(weapon);
				}
				else {
					hostWeapon = weapon;
				}
			}
			db.CloseSqlConnection();
			weapons.Sort((a, b) => b.Quality.CompareTo(a.Quality));
			//主角的兵器需要排在第一个
			if (hostWeapon != null) {
				weapons.Insert(0, hostWeapon);
			}
			Messenger.Broadcast<List<WeaponData>>(NotifyTypes.GetWeaponsListPanelDataEcho, weapons);
		}
	}
}