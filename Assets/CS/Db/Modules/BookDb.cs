﻿using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Game {
	/// <summary>
	/// 秘籍相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 请求特定城镇秘境中的秘籍数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetBooksOfForbiddenAreaPanelData(string cityId) {
			List<BookData> books = new List<BookData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from BooksTable where BelongToCityId = '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "' order by SeatNo");
			BookData book;
			while(sqReader.Read()) {
				book = JsonManager.GetInstance().GetMapping<BookData>("Books", sqReader.GetString(sqReader.GetOrdinal("BookId")));
				book.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				book.State = (BookStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				book.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
				books.Add(book);
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 检测是否是否发现新的秘籍，有则加入到待选数据表中
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void CheckNewBooksOfForbiddenArea(string cityId) {
			JObject booksOfForbiddenAreaData = JsonManager.GetInstance().GetJson("BooksOfForbiddenAreaData");
			if (booksOfForbiddenAreaData[cityId] != null) {
				db = OpenDb();
				JArray bookIdsData = (JArray)booksOfForbiddenAreaData[cityId];
				string bookId;
				for (int i = 0; i < bookIdsData.Count; i++) {
					bookId = bookIdsData[i].ToString();
					SqliteDataReader sqReader = db.ExecuteQuery("select Id from BooksTable where BookId = '" + bookId + "' and BelongToRoleId = '" + currentRoleId + "'");
					if (!sqReader.HasRows) {
						db.ExecuteQuery("insert into BooksTable (BookId, State, SeatNo, BeUsingByRoleId, BelongToCityId, BelongToRoleId) values('" + bookId + "', " + ((int)BookStateType.Unread) + ", -1, '', '" + cityId + "', '" + currentRoleId + "')");
					}
				}
				db.CloseSqlConnection();
			}
			booksOfForbiddenAreaData = null;
		}

		/// <summary>
		/// 请求书筐中的秘籍数据
		/// </summary>
		public void GetBooksListPanelData() {
			List<BookData> books = new List<BookData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from BooksTable where State = " + ((int)BookStateType.Read) + " and BelongToRoleId = '" + currentRoleId + "' order by SeatNo");
			BookData book;
			while(sqReader.Read()) {
				book = JsonManager.GetInstance().GetMapping<BookData>("Books", sqReader.GetString(sqReader.GetOrdinal("BookId")));
				book.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				book.State = (BookStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				book.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
				books.Add(book);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<BookData>>(NotifyTypes.GetBooksListPanelDataEcho, books);
		}

		/// <summary>
		/// 装备秘籍
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void UseBook(int id) {
			db = OpenDb();
			//查询角色信息
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId, RoleData from RolesTable where RoleId = '" + currentRoleId + "' and BelongToRoleId = '" + currentRoleId + "'");
			int addIndex = -1;
			if (sqReader.Read()) {
				string roleId = sqReader.GetString(sqReader.GetOrdinal("RoleId"));
				//获取角色数据
				RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				sqReader = db.ExecuteQuery("select BookId, SeatNo from BooksTable where SeatNo >= 0 and State = " + ((int)BookStateType.Read) + " and BelongToRoleId = '" + currentRoleId + "'");
				List<string> resourceBookDataIds = new List<string>();
				List<int> seatNos = new List<int>() { -1, -1, -1 };
				int seatNo;
				while(sqReader.Read()) {
					seatNo = sqReader.GetInt32(sqReader.GetOrdinal("SeatNo"));
					if (seatNos.Count > seatNo) {
						seatNos[seatNo] = seatNo;
						resourceBookDataIds.Add(sqReader.GetString(sqReader.GetOrdinal("BookId")));
					}
				}
				addIndex = seatNos.FindIndex(item => item == -1);
				if (addIndex >= 0) {
					sqReader = db.ExecuteQuery("select BookId from BooksTable where Id = " + id);
					if (sqReader.Read()) {
						resourceBookDataIds.Add(sqReader.GetString(sqReader.GetOrdinal("BookId")));
						db.ExecuteQuery("update BooksTable set SeatNo = " + addIndex + ", BeUsingByRoleId = '" + currentRoleId + "' where Id = " + id);
						//更新角色的秘籍信息
						role.ResourceBookDataIds = resourceBookDataIds;
						db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "' where RoleId = '" + roleId + "'");
					}
				}
			}
			db.CloseSqlConnection();
			if (addIndex >= 0) {
				GetBooksListPanelData();
				CallRoleInfoPanelData(false); //刷新队伍数据
			}
			else {
				AlertCtrl.Show("最多只能同时装备3本秘籍!", null);
			}
		}

		/// <summary>
		/// 卸下秘籍
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void UnuseBook(int id) {
			db = OpenDb();//查询角色信息
			SqliteDataReader sqReader = db.ExecuteQuery("select RoleId, RoleData from RolesTable where RoleId = '" + currentRoleId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				string roleId = sqReader.GetString(sqReader.GetOrdinal("RoleId"));
				//获取角色数据
				RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(sqReader.GetString(sqReader.GetOrdinal("RoleData")));
				sqReader = db.ExecuteQuery("select BookId from BooksTable where Id = " + id);
				if (sqReader.Read()) {
					string bookId = sqReader.GetString(sqReader.GetOrdinal("BookId"));
					db.ExecuteQuery("update BooksTable set SeatNo = 888, BeUsingByRoleId = '' where Id = " + id);
					int index = role.ResourceBookDataIds.FindIndex(item => item == bookId);
					if (index >= 0) {
						//更新角色的秘籍信息
						role.ResourceBookDataIds.RemoveAt(index);
						db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "' where RoleId = '" + roleId + "'");
					}
				}
			}
			db.CloseSqlConnection();
			GetBooksListPanelData();
			CallRoleInfoPanelData(false); //刷新队伍数据
		}
	}
}