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
			BookData book0 = new BookData();
			book0.Id = "book0";
			book0.Name = "降龙掌法";
			book0.IconId = "200000";
			role.Books = new List<BookData>(){
				book0
			};
			AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), 1, 0, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero0 = new RoleData();
			hero0.Id = "hero_100001";
			hero0.Name = "萧大侠";
			hero0.Desc = "南院大王";
			hero0.IconId = "100001";
			hero0.Occupation = OccupationType.GaiBang;
			hero0.AttackSpeed = 5;
			AddNewRole(hero0.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero0), 1, 1, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero1 = new RoleData();
			hero1.Id = "hero_100002";
			hero1.Name = "虚竹";
			hero1.Desc = "小和尚";
			hero1.IconId = "100002";
			hero1.Occupation = OccupationType.XiaoYao;
			AddNewRole(hero1.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero1), 1, 2, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			hero1.AttackSpeed = 5;
			RoleData hero2 = new RoleData();
			hero2.Id = "hero_100003";
			hero2.Name = "小师妹";
			hero2.Desc = "就是小师妹";
			hero2.IconId = "100003";
			hero2.Occupation = OccupationType.XiaoYao;
			hero2.Gender = GenderType.Female;
			hero2.AttackSpeed = 5;
			AddNewRole(hero2.Id, JsonManager.GetInstance().SerializeObjectDealVector(hero2), 1, 3, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero3 = new RoleData();
			hero3.Id = "hero_100004";
			hero3.Name = "替补师妹";
			hero3.Desc = "就是替补师妹";
			hero3.IconId = "100003";
			hero3.Occupation = OccupationType.XiaoYao;
			hero3.Gender = GenderType.Female;
			hero3.AttackSpeed = 5;
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
