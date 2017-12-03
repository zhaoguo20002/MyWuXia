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
            PlayerPrefs.SetString("AddedNewWeaponFlag", "true");
            Messenger.Broadcast(NotifyTypes.MakeRoleInfoPanelRedPointRefresh);
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
                string roleDataStr = sqReader.GetString(sqReader.GetOrdinal("RoleData"));
                roleDataStr = roleDataStr.IndexOf("{") == 0 ? roleDataStr : DESStatics.StringDecder(roleDataStr);
                RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(roleDataStr);
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
                    if (weapon.BelongToRoleId == "") {
                        if (weapon.Occupation == OccupationType.None || weapon.Occupation == HostData.Occupation) {
                            //装备新兵器
                            db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '" + beUsingByRoleId + "' where Id = " + id);
                            //更新角色的武器信息
                            role.ResourceWeaponDataId = weaponId;
                            //查询下新武器替换上后秘籍需不需要卸下
                            sqReader = db.ExecuteQuery("select Id, BookId from BooksTable where BeUsingByRoleId = '" + currentRoleId + "' and BelongToRoleId = '" + currentRoleId + "'");
                            BookData book;
                            string unuseBookMsg = "";
                            while(sqReader.Read()) {
                                book = JsonManager.GetInstance().GetMapping<BookData>("Books", sqReader.GetString(sqReader.GetOrdinal("BookId")));
                                if (book.LimitWeaponType != WeaponType.None && (book.LimitWeaponType != weapon.Type)) {
                                    db.ExecuteQuery("update BooksTable set SeatNo = 888, BeUsingByRoleId = '' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
                                    int index = role.ResourceBookDataIds.FindIndex(item => item == book.Id);
                                    if (index >= 0) {
                                        //更新角色的秘籍信息
                                        role.ResourceBookDataIds.RemoveAt(index);
                                    }
                                    unuseBookMsg += " " + book.Name;
                                }
                            }
                            //更新主角关联数据
                            db.ExecuteQuery("update RolesTable set RoleData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObjectDealVector(role)) + "' where RoleId = '" + beUsingByRoleId + "'");
                            if (unuseBookMsg != "") {
                                Statics.CreatePopMsg(Vector3.zero, string.Format("拿上<color=\"{0}\">{1}</color>后不可能再习练{2}", Statics.GetQualityColorString(weapon.Quality), weapon.Name, unuseBookMsg), Color.white, 30);
                            }
                            SoundManager.GetInstance().PushSound("ui0011");
                        } else {
                            AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>只有 {2} 才能使用!", Statics.GetQualityColorString(weapon.Quality), weapon.Name, Statics.GetOccupationDesc(weapon.Occupation)));
                        }
                    } else {
                        AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>只有 {2} 才能使用!", Statics.GetQualityColorString(weapon.Quality), weapon.Name, JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", weapon.BelongToRoleId).Name));
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
            SqliteDataReader sqReader = db.ExecuteQuery("select RoleId, RoleData from RolesTable where RoleId = '" + currentRoleId + "'");
            if (sqReader.Read())
            {
                //获取角色数据
                string roleDataStr = sqReader.GetString(sqReader.GetOrdinal("RoleData"));
                roleDataStr = roleDataStr.IndexOf("{") == 0 ? roleDataStr : DESStatics.StringDecder(roleDataStr);
                RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(roleDataStr);
                role.ResourceWeaponDataId = "";
                //更新主角关联数据
                db.ExecuteQuery("update RolesTable set RoleData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObjectDealVector(role)) + "' where RoleId = '" + currentRoleId + "'");
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
//			if (hostWeapon != null) {
				weapons.Insert(0, hostWeapon);
//			}
			Messenger.Broadcast<List<WeaponData>, RoleData>(NotifyTypes.GetWeaponsListPanelDataEcho, weapons, HostData);
		}

        /// <summary>
        /// 查询特定侠客当前装备的兵器id
        /// </summary>
        /// <returns>The role weapon identifier.</returns>
        /// <param name="roleId">Role identifier.</param>
        public string GetRoleWeaponId(string roleId) {
            string weaponId = "";
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select WeaponId from WeaponsTable where BeUsingByRoleId = '" + roleId + "' and BelongToRoleId ='" + currentRoleId + "'");
            if (sqReader.Read()) {
                weaponId = sqReader.GetString(sqReader.GetOrdinal("WeaponId"));
            }
            db.CloseSqlConnection();
            return weaponId;
        }

        /// <summary>
        /// 查询主角当前装备的兵器id
        /// </summary>
        /// <returns>The host weapon identifier.</returns>
        public string GetHostWeaponId() {
            return GetRoleWeaponId(currentRoleId);
        }

        /// <summary>
        /// 返回主角当前装备的兵器类型
        /// </summary>
        /// <returns>The host weapon type.</returns>
        public WeaponType GetHostWeaponType() {
            string weaponId = GetHostWeaponId();
            return JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", weaponId).Type;
        }

        /// <summary>
        /// 根据兵器Id查询兵器强化等级
        /// </summary>
        /// <returns>The weapon L.</returns>
        /// <param name="weaponId">Weapon identifier.</param>
        public WeaponLVData GetWeaponLV(string weaponId) {
            WeaponLVData WeaponLVData = null;
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select Data from WeaponLVsTable where WeaponId = '" + weaponId + "' and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.Read())
            {
                //获取角色数据
                WeaponLVData = JsonManager.GetInstance().DeserializeObject<WeaponLVData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("Data"))));
            }
            else
            {
                WeaponLVData = new WeaponLVData(0, 10);
            }
            db.CloseSqlConnection();
            return WeaponLVData;
        }

	}
}