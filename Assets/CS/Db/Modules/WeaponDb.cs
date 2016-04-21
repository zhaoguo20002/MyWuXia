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
		/// 最大能拥有的兵器数
		/// </summary>
		public int MaxWeaponNum = 20;
		/// <summary>
		/// 添加新武器
		/// </summary>
		/// <param name="weaponId">Weapon identifier.</param>
		/// <param name="beUsingByRoleId">Be using by role identifier.</param>
		public bool AddNewWeapon(string weaponId, string beUsingByRoleId = "") {
			bool result = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select count(*) as num from WeaponsTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				if (sqReader.GetInt32(sqReader.GetOrdinal("num")) < MaxWeaponNum) {
					db.ExecuteQuery("insert into WeaponsTable (WeaponId, BeUsingByRoleId, BelongToRoleId) values('" + weaponId + "', '" + beUsingByRoleId + "', '" + currentRoleId + "')");
					result = true;

				}
			}
			db.CloseSqlConnection();
			return result;
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
			if (sqReader.Read()) {
				//获取角色数据
				RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				sqReader = db.ExecuteQuery("select * from WeaponsTable where BeUsingByRoleId = '" + beUsingByRoleId + "' and BelongToRoleId ='" + currentRoleId + "'");
				while (sqReader.Read()) {
					//将兵器先卸下
					int dataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '' where Id = " + dataId);
				}
				sqReader = db.ExecuteQuery("select Id, WeaponId from WeaponsTable where Id = " + id);
				if (sqReader.Read()) {
					string weaponId = sqReader.GetString(sqReader.GetOrdinal("WeaponId"));
					WeaponData weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", weaponId);
					if (weapon.Occupation == OccupationType.None || weapon.Occupation == HostData.Occupation) {
						//装备新兵器
						db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '" + beUsingByRoleId + "' where Id = " + id);
						//更新角色的武器信息
						role.ResourceWeaponDataId = weaponId;
						db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "' where RoleId = '" + beUsingByRoleId + "'");
					}
					else {
						AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>只能{2}弟子才能使用!", Statics.GetQualityColorString(weapon.Quality), weapon.Name, Statics.GetOccupationName(weapon.Occupation)));
					}
				}
			}
			db.CloseSqlConnection();
			GetWeaponsListPanelData(); //刷新兵器匣列表
			CallRoleInfoPanelData(false); //刷新队伍数据
		}

		/// <summary>
		/// 卸下兵器
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void TakeOffWeapon(int id) {
			db = OpenDb();
			db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '' where Id = " + id);
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
//			if (hostWeapon != null) {
				weapons.Insert(0, hostWeapon);
//			}
			Messenger.Broadcast<List<WeaponData>, RoleData>(NotifyTypes.GetWeaponsListPanelDataEcho, weapons, HostData);
		}
	}
}