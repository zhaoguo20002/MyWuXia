using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
			//存档数据表
			db.ExecuteQuery("create table if not exists RecordsTable (Id integer primary key autoincrement not null, RoleId text not null, Name text not null, Data text, DateTime text not null);");
			//用户基础数据表
			db.ExecuteQuery("create table if not exists UserDatasTable (Id integer primary key autoincrement not null, Data text, BelongToRoleId text not null, DateTime text not null)");

			#endregion
		
			#region 初始化角色表相关数据
			//当前获得的伙伴数据表
			db.ExecuteQuery("create table if not exists RolesTable (RoleId text primary key not null, RoleData text not null, State integer not null, SeatNo integer not null, BelongToRoleId text not null, DateTime text not null);");
			#endregion

			#region 初始化任务表相关数据
			//当前可以操作的任务数据表(包括可以接取的任务,已完成的任务和接取条件不满足的任务)
			db.ExecuteQuery("create table if not exists TasksTable (Id integer primary key autoincrement not null, TaskId text not null, CurrentDialogIndex integer not null, State integer not null, BelongToRoleId text not null)");
			#endregion

			db.CloseSqlConnection();

			AddNewTask("1");
			GetTaskListPanelData();

			AddNewRecord(currentRoleId, "-", "{}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			UserData userData = new UserData();
			userData.AreaFood = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "1");
			userData.AreaFood.Num = userData.AreaFood.MaxNum;
			userData.PositionStatu = UserPositionStatusType.InArea;
			userData.CurrentAreaSceneName = "Area0";
			userData.CurrentAreaX = 1;
			userData.CurrentAreaY = 1;
			AddNewUserData(JsonManager.GetInstance().SerializeObjectDealVector(userData), currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			RoleData role = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", "1");
			role.MakeJsonToModel();
			role.Id = currentRoleId;
			AddNewRole(currentRoleId, JsonManager.GetInstance().SerializeObjectDealVector(role), 1, 0, currentRoleId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			RoleData hero0 = new RoleData();
			hero0.Id = "hero_100001";
			hero0.Name = "萧大侠";
			hero0.Desc = "南院大王";
			hero0.IconId = "100001";
			hero0.Occupation = OccupationType.GaiBang;
			hero0.AttackSpeed = 5;
			hero0.HP = 10000;
			hero0.MaxHP = 10000;
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
			hero1.MaxHP = 10000;
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
			hero2.MaxHP = 10000;
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
			hero3.MaxHP = 10000;
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

		/// <summary>
		/// 添加用户基础数据
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="belongToRoleId">Belong to role identifier.</param>
		/// <param name="dateTime">Date time.</param>
		public void AddNewUserData(string data, string belongToRoleId, string dateTime) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from UserDatasTable where BelongToRoleId = '" + belongToRoleId + "'");
			if (!sqReader.HasRows) {
				//首次创建用户基础数据记录
				Debug.LogWarning("首次创建用户基础数据记录");
				db.ExecuteQuery("insert into UserDatasTable (Data, BelongToRoleId, DateTime) values('" + data + "', '" + belongToRoleId + "', '" + dateTime + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求用户基础信息
		/// </summary>
		public void CallUserData() {
			db = OpenDb();
			//正序查询处于战斗队伍中的角色
			SqliteDataReader sqReader = db.ExecuteQuery("select * from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			JObject obj = new JObject();
			JArray data = new JArray();
			while (sqReader.Read()) {
				data.Add(sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				data.Add(sqReader.GetString(sqReader.GetOrdinal("Data")));
			}
			obj["data"] = data;
			db.CloseSqlConnection();
			Messenger.Broadcast<JObject>(NotifyTypes.CallUserDataEcho, obj);
		}

		/// <summary>
		/// 更新用户基础信息
		/// </summary>
		/// <param name="dataStr">Data string.</param>
		public void UpdateUserData(string dataStr) {
			db = OpenDb();
			db.ExecuteQuery("update UserDatasTable set Data = '" + dataStr + "' where BelongToRoleId = '" + currentRoleId + "'");
			db.CloseSqlConnection();
			//更新完后立马返回查询结果
			CallUserData();
		}
	}
}
