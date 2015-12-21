using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;

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
			db.ExecuteQuery("create table if not exists RolesTable (RoleId text primary key not null, RoleData text not null, State integer not null, BelongToRoleId text not null, DateTime text not null);");
			#endregion

			db.CloseSqlConnection();

			AddNewRecord(currentRoleId, "-", "{}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData role = new RoleData();
			role.Id = currentRoleId;
			role.Name = "龙展";
			role.Desc = "主角光环";
			role.IconId = "100000";
			role.Occupation = OccupationType.GaiBang;
			AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), 1, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
