using UnityEngine;
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
//            SqliteDataReader sqReader = db.ExecuteQuery("select * from BooksTable where BelongToCityId = '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "' order by SeatNo");
            SqliteDataReader sqReader = db.ExecuteQuery("select * from BooksTable where BelongToRoleId = '" + currentRoleId + "' order by SeatNo");
			BookData book;
			while(sqReader.Read()) {
				book = JsonManager.GetInstance().GetMapping<BookData>("Books", sqReader.GetString(sqReader.GetOrdinal("BookId")));
				book.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				book.State = (BookStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
				book.BeUsingByRoleId = sqReader.GetString(sqReader.GetOrdinal("BeUsingByRoleId"));
				books.Add(book);
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<BookData>, RoleData>(NotifyTypes.GetBooksOfForbiddenAreaPanelDataEcho, books, HostData);
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
						db.ExecuteQuery("insert into BooksTable (BookId, State, SeatNo, BeUsingByRoleId, BelongToCityId, BelongToRoleId) values('" + bookId + "', " + ((int)BookStateType.Unread) + ", 888, '', '" + cityId + "', '" + currentRoleId + "')");
					}
				}
				db.CloseSqlConnection();
			}
			booksOfForbiddenAreaData = null;
		}

		/// <summary>
		/// 初始化用于判定秘境新增秘籍的id列表
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void CreateBookIdOfCurrentForbiddenAreaNewFlagList(string cityId) {
			db = OpenDb();
			CitySceneModel.BookIdOfCurrentForbiddenAreaNewFlagList = new List<string>();
			SqliteDataReader sqReader = db.ExecuteQuery("select BookId from BooksTable where BelongToCityId = '" + cityId + "' and BelongToRoleId = '" + currentRoleId + "'");
			while (sqReader.Read()) {
				CitySceneModel.BookIdOfCurrentForbiddenAreaNewFlagList.Add(sqReader.GetString(sqReader.GetOrdinal("BookId")));
			}
			db.CloseSqlConnection();
		}

        int sortBooks(BookData a, BookData b) {
            int result = b.BeUsingByRoleId.CompareTo(a.BeUsingByRoleId);
            if (result == 0)
            {
                result = b.Quality.CompareTo(a.Quality);
            }
            return result;
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
            books.Sort(sortBooks);
			Messenger.Broadcast<List<BookData>>(NotifyTypes.GetBooksListPanelDataEcho, books);
		}

		/// <summary>
		/// 装备秘籍
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void UseBook(int id) {
            //查询主角当前的兵器类型
            WeaponType hostWeaponType = GetHostWeaponType();
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
				List<int> seatNos = new List<int>() { -1, -1 };
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
                        string bookId = sqReader.GetString(sqReader.GetOrdinal("BookId"));
                        BookData bookData = JsonManager.GetInstance().GetMapping<BookData>("Books", bookId);
                        if (bookData.LimitWeaponType == WeaponType.None || hostWeaponType == WeaponType.None || bookData.LimitWeaponType == hostWeaponType) {
                            resourceBookDataIds.Add(bookId);
                            db.ExecuteQuery("update BooksTable set SeatNo = " + addIndex + ", BeUsingByRoleId = '" + currentRoleId + "' where Id = " + id);
                            //更新角色的秘籍信息
                            role.ResourceBookDataIds = resourceBookDataIds;
                            db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObjectDealVector(role) + "' where RoleId = '" + roleId + "'");
                            SoundManager.GetInstance().PushSound("ui0011");
                        } else {
                            AlertCtrl.Show(string.Format("装备上[{0}]才能习练\n<color=\"{1}\">{2}</color>\n{3}", Statics.GetEnmuDesc<WeaponType>(bookData.LimitWeaponType), Statics.GetQualityColorString(bookData.Quality), bookData.Name, hostWeaponType != WeaponType.None ? ("你现在拿的是[" + Statics.GetEnmuDesc<WeaponType>(hostWeaponType) + "]") : "你现在手里没有任何兵器"), null);
                        }
					}
				}
			}
			db.CloseSqlConnection();
			if (addIndex >= 0) {
				GetBooksListPanelData();
				CallRoleInfoPanelData(false); //刷新队伍数据
			}
			else {
                AlertCtrl.Show("最多只能同时携带2本书！", null);
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

		/// <summary>
		/// 研读秘籍
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void ReadBook(int id) {
			bool read = false;
			BookData book = null;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select BookId from BooksTable where Id = " + id);
			if (sqReader.Read()) {
				book = JsonManager.GetInstance().GetMapping<BookData>("Books", sqReader.GetString(sqReader.GetOrdinal("BookId")));
				if (book.Occupation == OccupationType.None || book.Occupation == HostData.Occupation) {
					bool enough = true;
					string msg = "";
					CostData cost;
					ItemData item;
					//计算需要的物品是否足够
					for (int i = 0; i < book.Needs.Count; i++) {
						cost = book.Needs[i];
						item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", cost.Id);
						sqReader = db.ExecuteQuery("select Num from BagTable where ItemId = '" + cost.Id + "' and BelongToRoleId = '" + currentRoleId + "'");
						if (sqReader.Read()) {
							if (sqReader.GetInt32(sqReader.GetOrdinal("Num")) < cost.Num) {
								enough = false;
								msg = string.Format("行囊里的{0}不够", item.Name);
								break;
							}
						}
						else {
							enough = false;
							msg = string.Format("行囊里并不曾见过有{0}", item.Name);
							break;
						}
					}
					
					if (enough) {
						int num;
						//扣除物品
						for (int i = 0; i < book.Needs.Count; i++) {
							cost = book.Needs[i];
							sqReader = db.ExecuteQuery("select Id, Num from BagTable where ItemId = '" + cost.Id + "' and BelongToRoleId = '" + currentRoleId + "'");
							if (sqReader.Read()) {
								num = sqReader.GetInt32(sqReader.GetOrdinal("Num")) - cost.Num;
								num = num < 0 ? 0 : num;
								if (num > 0) {
									db.ExecuteQuery("update BagTable set Num = " + num + " where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
								}
								else {
									db.ExecuteQuery("delete from BagTable where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
								}
							}
						}
						//研读秘籍
						db.ExecuteQuery("update BooksTable set State = " + ((int)BookStateType.Read) + ", SeatNo = 888 where Id = " + id);
						read = true;
					}
					else {
						AlertCtrl.Show(msg, null);
					}
				}
				else {
					AlertCtrl.Show(string.Format("非{0}弟子不得研习<color=\"{1}\">{2}</color>!", Statics.GetOccupationName(book.Occupation), Statics.GetQualityColorString(book.Quality), book.Name));
				}
			}
			db.CloseSqlConnection();
			if (read && book != null) {
				Statics.CreatePopMsg(Vector3.zero, string.Format("研读<color=\"{0}\">{1}</color>后使你武功精进!", Statics.GetQualityColorString(book.Quality), book.Name), Color.white, 30);
				GetBooksOfForbiddenAreaPanelData(book.BelongToCityId);
                PlayerPrefs.SetString("AddedNewBookFlag", "true");
                Messenger.Broadcast(NotifyTypes.MakeRoleInfoPanelRedPointRefresh);
                SoundManager.GetInstance().PushSound("ui0010");
			}
		}

		/// <summary>
		/// 添加一本新秘籍
		/// </summary>
		/// <returns><c>true</c>, if new book was added, <c>false</c> otherwise.</returns>
		/// <param name="bookId">Book identifier.</param>
		/// <param name="cityId">City identifier.</param>
		public bool AddNewBook(string bookId, string cityId = "") {
			bool result = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id from BooksTable where BookId = '" + bookId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into BooksTable (BookId, State, SeatNo, BeUsingByRoleId, BelongToCityId, BelongToRoleId) values('" + bookId + "', " + ((int)BookStateType.Read) + ", 888, '', '" + cityId + "', '" + currentRoleId + "')");
				result = true;
			}
			db.CloseSqlConnection();
            PlayerPrefs.SetString("AddedNewBookFlag", "true");
            Messenger.Broadcast(NotifyTypes.MakeRoleInfoPanelRedPointRefresh);
			return result;
		}
	}
}