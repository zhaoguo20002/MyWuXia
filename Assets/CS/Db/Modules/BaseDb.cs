using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 通用数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 创建数据库
		/// </summary>
		public void CreateAllDbs() {
			db = OpenDb();

			#region 初始化存档表相关数据
			db.ExecuteQuery("create table if not exists RecordsTable (Id integer primary key autoincrement not null, RoleId text not null, Name text not null, Data text, DateTime text not null);");

			#endregion

			#region 初始化角色表相关数据
			db.ExecuteQuery("create table if not exists RolesTable (RoleId text primary key not null, RoleData text not null, State integer not null, SeatNo integer not null, BelongToRoleId text not null, DateTime text not null);");
			#endregion

			db.CloseSqlConnection();

			AddNewRecord(currentRoleId, "-", "{}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			RoleData role = new RoleData();
			role.Id = currentRoleId;
			role.Name = "龙展";
			role.Desc = "主角光环";
			role.IconId = "100000";
			role.Occupation = OccupationType.GaiBang;
			role.IsHost = true;
			role.AttackSpeed = 5;
			role.HP = 10000;
			role.MaxHP = role.HP;
			BookData book0 = new BookData();
			book0.Id = "book0";
			book0.Name = "降龙掌法";
			book0.IconId = "200000";
			SkillData skill0 = new SkillData();
			skill0.Type = SkillType.PhysicsAttack;
			skill0.Name = "亢龙有悔";
			SkillData skill1 = new SkillData();
			skill1.Type = SkillType.PhysicsAttack;
			skill1.Name = "飞龙在天";
			SkillData skill2 = new SkillData();
			skill2.Type = SkillType.PhysicsAttack;
			skill2.Name = "鸿渐于陆";
			SkillData skill3 = new SkillData();
			skill3.Type = SkillType.PhysicsAttack;
			skill3.Name = "利涉大川";
			SkillData skill4 = new SkillData();
			skill4.Type = SkillType.PhysicsAttack;
			skill4.Name = "神龙摆尾";
			book0.Skills.Add(skill0);
			book0.Skills.Add(skill1);
			book0.Skills.Add(skill2);
			book0.Skills.Add(skill3);
			book0.Skills.Add(skill4);
			role.Books = new List<BookData>(){
				book0
			};
			WeaponData weapon0 = new WeaponData();
			weapon0.Id = "weapon0";
			weapon0.Id = "降龙拳套";
			weapon0.Width = 100;
			weapon0.Rates = new float[] { 1, 0.8f, 0.3f, 0.1f };
			role.Weapon = weapon0;
			AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), 1, 0, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero0 = new RoleData();
			hero0.Id = "hero_100001";
			hero0.Name = "萧大侠";
			hero0.Desc = "南院大王";
			hero0.IconId = "100001";
			hero0.Occupation = OccupationType.GaiBang;
			hero0.AttackSpeed = 5;
			hero0.HP = 10000;
			hero0.MaxHP = hero0.HP;
			BookData book1 = new BookData();
			book1.Id = "book1";
			book1.Name = "打狗棒法";
			book1.IconId = "200000";
			hero0.Books = new List<BookData>(){
				book1
			};
			WeaponData weapon1 = new WeaponData();
			weapon1.Id = "weapon1";
			weapon1.Id = "打狗棒";
			weapon1.Width = 100;
			weapon1.Rates = new float[] { 1, 0.6f, 0.2f, 0.1f };
			hero0.Weapon = weapon1;
			AddNewRole(hero0.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero0), 1, 1, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero1 = new RoleData();
			hero1.Id = "hero_100002";
			hero1.Name = "虚竹";
			hero1.Desc = "小和尚";
			hero1.IconId = "100002";
			hero1.Occupation = OccupationType.XiaoYao;
			BookData book2 = new BookData();
			book2.Id = "book2";
			book2.Name = "小无相功";
			book2.IconId = "200000";
			hero1.Books = new List<BookData>(){
				book2
			};
			hero1.AttackSpeed = 5;
			hero1.HP = 10000;
			hero1.MaxHP = hero1.HP;
			WeaponData weapon2 = new WeaponData();
			weapon2.Id = "weapon2";
			weapon2.Id = "天山蚕丝拳套";
			weapon2.Width = 100;
			weapon2.Rates = new float[] { 1, 0.6f, 0.2f, 0.1f };
			hero1.Weapon = weapon2;
			AddNewRole(hero1.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero1), 1, 2, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero2 = new RoleData();
			hero2.Id = "hero_100003";
			hero2.Name = "小师妹";
			hero2.Desc = "就是小师妹";
			hero2.IconId = "100003";
			hero2.Occupation = OccupationType.XiaoYao;
			BookData book3 = new BookData();
			book3.Id = "book3";
			book3.Name = "八荒六合唯我独尊功";
			book3.IconId = "200000";
			hero2.Books = new List<BookData>(){
				book3
			};
			hero2.Gender = GenderType.Female;
			hero2.AttackSpeed = 5;
			hero2.HP = 10000;
			hero2.MaxHP = hero2.HP;
			WeaponData weapon3 = new WeaponData();
			weapon3.Id = "weapon3";
			weapon3.Id = "青釭剑";
			weapon3.Width = 100;
			weapon3.Rates = new float[] { 1, 0.6f, 0.2f, 0.1f };
			hero2.Weapon = weapon3;
			AddNewRole(hero2.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero2), 1, 3, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero3 = new RoleData();
			hero3.Id = "hero_100004";
			hero3.Name = "替补师妹";
			hero3.Desc = "就是替补师妹";
			hero3.IconId = "100003";
			hero3.Occupation = OccupationType.XiaoYao;
			BookData book4 = new BookData();
			book4.Id = "book4";
			book4.Name = "天山六阳掌";
			book4.IconId = "200000";
			hero3.Books = new List<BookData>(){
				book4
			};
			hero3.Gender = GenderType.Female;
			hero3.AttackSpeed = 5;
			hero3.HP = 10000;
			hero3.MaxHP = hero3.HP;
			WeaponData weapon4 = new WeaponData();
			weapon4.Id = "weapon4";
			weapon4.Id = "青釭剑";
			weapon4.Width = 100;
			weapon4.Rates = new float[] { 1, 0.6f, 0.2f, 0.1f };
			hero3.Weapon = weapon4;
			AddNewRole(hero3.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero3), -1, -1, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
	}
}
