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
                if (result == 0)
                {
                    if (a.IsMindBook && !b.IsMindBook)
                    {
                        result = 1;
                    }
                    else if (!a.IsMindBook && b.IsMindBook)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
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
                string roleDataStr = sqReader.GetString(sqReader.GetOrdinal("RoleData"));
                roleDataStr = roleDataStr.IndexOf("{") == 0 ? roleDataStr : DESStatics.StringDecder(roleDataStr);
                RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(roleDataStr);
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
                        string bookId = sqReader.GetString(sqReader.GetOrdinal("BookId"));
                        BookData bookData = JsonManager.GetInstance().GetMapping<BookData>("Books", bookId);
                        if (bookData.LimitWeaponType == WeaponType.None || hostWeaponType == WeaponType.None || bookData.LimitWeaponType == hostWeaponType) {
                            resourceBookDataIds.Add(bookId);
                            db.ExecuteQuery("update BooksTable set SeatNo = " + addIndex + ", BeUsingByRoleId = '" + currentRoleId + "' where Id = " + id);
                            //更新角色的秘籍信息
                            role.ResourceBookDataIds = resourceBookDataIds;
                            db.ExecuteQuery("update RolesTable set RoleData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObjectDealVector(role)) + "' where RoleId = '" + roleId + "'");
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
                AlertCtrl.Show("最多只能同时携带三本书！", null);
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
                string roleDataStr = sqReader.GetString(sqReader.GetOrdinal("RoleData"));
                roleDataStr = roleDataStr.IndexOf("{") == 0 ? roleDataStr : DESStatics.StringDecder(roleDataStr);
                RoleData role = JsonManager.GetInstance().DeserializeObject<RoleData>(roleDataStr);
				sqReader = db.ExecuteQuery("select BookId from BooksTable where Id = " + id);
				if (sqReader.Read()) {
					string bookId = sqReader.GetString(sqReader.GetOrdinal("BookId"));
					db.ExecuteQuery("update BooksTable set SeatNo = 888, BeUsingByRoleId = '' where Id = " + id);
					int index = role.ResourceBookDataIds.FindIndex(item => item == bookId);
					if (index >= 0) {
						//更新角色的秘籍信息
						role.ResourceBookDataIds.RemoveAt(index);
                        db.ExecuteQuery("update RolesTable set RoleData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObjectDealVector(role)) + "' where RoleId = '" + roleId + "'");
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

        /// <summary>
        /// 查询是否获得特定的秘籍/心法
        /// </summary>
        /// <returns><c>true</c> if this instance has book the specified bookId; otherwise, <c>false</c>.</returns>
        /// <param name="bookId">Book identifier.</param>
        public bool HasBook(string bookId) {
            bool result = false;
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select Id from BooksTable where BookId = '" + bookId + "' and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.HasRows) {
                result = true;
            }
            db.CloseSqlConnection();
            return result;
        }

        /// <summary>
        /// 查询可以领悟的所有诀要
        /// </summary>
        /// <returns>The effective secrets.</returns>
        public List<SecretData> GetEffectiveSecrets() {
            List<SecretData> secrets = new List<SecretData>();
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select * from BookSecretsTable where BelongToRoleId = '" + currentRoleId + "' and BelongToBookId = ''");
            SecretData secret;
            while(sqReader.Read()) {
                secret = JsonManager.GetInstance().DeserializeObject<SecretData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretData"))));
                secret.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                secrets.Add(secret);
            }
            db.CloseSqlConnection();
            return secrets;
        }

        /// <summary>
        /// 将修为添加给秘籍
        /// </summary>
        /// <param name="exp">Exp.</param>
        /// <param name="books">Books.</param>
        public void InsertExpIntoBooks(long exp, List<BookData> books) {
            db = OpenDb();
            SqliteDataReader sqReader;
            BookData book;
            ExpData expData;
            long maxExp = Statics.GetBookMaxExp(QualityType.FlashRed);
            for (int i = 0, len = books.Count; i < len; i++)
            {
                book = books[i];
                if (!book.IsMindBook)
                {
                    sqReader = db.ExecuteQuery("select Id, ExpData from BookExpsTable where BookId = " + book.Id + " and BelongToRoleId = '" + currentRoleId + "'");
                    if (sqReader.Read())
                    {
                        expData = JsonManager.GetInstance().DeserializeObject<ExpData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("ExpData"))));
//                        expData.Cur = (long)Mathf.Clamp(expData.Cur + exp, 0, expData.Max);
                        expData.Cur = (long)Mathf.Clamp(expData.Cur + exp, 0, maxExp);
                        db.ExecuteQuery("update BookExpsTable set ExpData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(expData)) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
                    }
                    else
                    {
                        db.ExecuteQuery("insert into BookExpsTable (BookId, ExpData, SecretsData, BelongToRoleId) values('" + book.Id + "', '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(new ExpData(exp, Statics.GetBookMaxExp(book.Quality)))) + "', '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(new List<SecretData>())) + "', '" + currentRoleId + "')");
                    }
                }
            } 
            db.CloseSqlConnection();
        }

        /// <summary>
        /// 获取秘籍的修为和已领悟的诀要集合
        /// </summary>
        /// <returns>The book exp and secrets.</returns>
        public ExpAndSecretData GetBookExpAndSecrets(string bookId) {
            ExpAndSecretData data = new ExpAndSecretData();
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select ExpData, SecretsData from BookExpsTable where BookId = '" + bookId + "' and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.Read())
            {
                data.Exp = JsonManager.GetInstance().DeserializeObject<ExpData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("ExpData"))));
                data.Secrets = JsonManager.GetInstance().DeserializeObject<List<SecretData>>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretsData"))));
            }
            db.CloseSqlConnection();
            return data;
        }

        /// <summary>
        /// 根据角色拥有的秘籍Id集合查询出拥有的诀要总和
        /// </summary>
        /// <returns>The secrets belong books.</returns>
        /// <param name="bookIds">Book identifiers.</param>
        public List<SecretData> GetSecretsBelongBooks(List<string> bookIds) {
            List<SecretData> secrets = new List<SecretData>();
            db = OpenDb();
            SqliteDataReader sqReader;
            List<SecretData> secretsInDB;
            for (int i = 0, len = bookIds.Count; i < len; i++)
            {
                sqReader = db.ExecuteQuery("select SecretsData from BookExpsTable where BookId = '" + bookIds[i] + "' and BelongToRoleId = '" + currentRoleId + "'");
                if (sqReader.Read())
                {
                    secretsInDB = JsonManager.GetInstance().DeserializeObject<List<SecretData>>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretsData"))));
                    for (int j = 0, len2 = secretsInDB.Count; j < len2; j++)
                    {
                        secrets.Add(secretsInDB[j]);
                    }
                }
            }
            db.CloseSqlConnection();
            return secrets;
        }

        /// <summary>
        /// 领悟秘籍诀要
        /// </summary>
        /// <param name="secret">Secret.</param>
        public void StudySecret(BookData book, SecretData secret) {
            List<SecretData> secrets = null;
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select Id, ExpData, SecretsData from BookExpsTable where BookId = " + book.Id + " and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.Read())
            {
                ExpData exp = JsonManager.GetInstance().DeserializeObject<ExpData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("ExpData"))));
                secrets = JsonManager.GetInstance().DeserializeObject<List<SecretData>>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretsData"))));
                int bookLv = Statics.GetBookLV(exp.Cur);
                if (secrets.Count >= bookLv)
                {
                    AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>目前只能领悟最多{2}张诀要", Statics.GetQualityColorString(book.Quality), book.Name, bookLv));
                    db.CloseSqlConnection();
                    return;
                }
                else
                {
                    if (secrets.FindIndex(item => item.Type == secret.Type) < 0)
                    {
                        secrets.Add(secret);
                        db.ExecuteQuery("update BookExpsTable set SecretsData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(secrets)) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
                        db.ExecuteQuery("update BookSecretsTable set BelongToBookId = '" + book.Id + "' where Id = '" + secret.PrimaryKeyId + "'");
                    }
                    else
                    {
                        AlertCtrl.Show("该类型诀要不能重复领悟！");
                        db.CloseSqlConnection();
                        return;
                    }
                }
            }
            else
            {
                secrets = new List<SecretData>() { secret };
                //处理空数据初始化
                db.ExecuteQuery("insert into BookExpsTable (BookId, ExpData, SecretsData, BelongToRoleId) values('" + book.Id + "', '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(new ExpData(0, Statics.GetBookMaxExp(book.Quality)))) + "', '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(secrets)) + "', '" + currentRoleId + "')");
                db.ExecuteQuery("update BookSecretsTable set BelongToBookId = '" + book.Id + "' where Id = '" + secret.PrimaryKeyId + "'");
            }
            db.CloseSqlConnection();
            if (secrets != null)
            {
                Messenger.Broadcast<BookData, List<SecretData>>(NotifyTypes.DealSecretEcho, book, secrets);
                Statics.CreatePopMsg(Vector3.zero, string.Format("领悟<color=\"{0}\">{1}</color>后<color=\"{2}\">{3}</color>更为精进！!", Statics.GetQualityColorString(secret.Quality), secret.Name, Statics.GetQualityColorString(book.Quality), book.Name), Color.white, 30);
                SoundManager.GetInstance().PushSound("ui0010");
            }
        }

        /// <summary>
        /// 遗忘秘籍诀要
        /// </summary>
        /// <param name="book">Book.</param>
        /// <param name="secret">Secret.</param>
        public void ForgetSecret(BookData book, SecretData secret) {
            List<SecretData> secrets = null;
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select Id, ExpData, SecretsData from BookExpsTable where BookId = " + book.Id + " and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.Read())
            {
                secrets = JsonManager.GetInstance().DeserializeObject<List<SecretData>>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretsData"))));
                int findIndex = secrets.FindIndex(item => item.PrimaryKeyId == secret.PrimaryKeyId);
                if (findIndex >= 0)
                {
                    secrets.RemoveAt(findIndex);
                }
                db.ExecuteQuery("update BookExpsTable set SecretsData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(secrets)) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
                db.ExecuteQuery("update BookSecretsTable set BelongToBookId = '' where Id = '" + secret.PrimaryKeyId + "'");
            }
            db.CloseSqlConnection();
            if (secrets != null)
            {
                Messenger.Broadcast<BookData, List<SecretData>>(NotifyTypes.DealSecretEcho, book, secrets);
                SoundManager.GetInstance().PushSound("ui0008");
            }
        }

        /// <summary>
        /// 融合诀要
        /// </summary>
        /// <param name="secret">Secret.</param>
        public bool MixSecrets(SecretData secret) {
            bool result = false;
            if (secret.Quality >= QualityType.FlashRed)
            {
                AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>已经是顶级诀要无法继续融合", Statics.GetQualityColorString(secret.Quality), secret.Name));
                return result;
            }
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select count(*) as num from BookSecretsTable where T = " + ((short)secret.Type) + " and Q = " + ((short)secret.Quality) + " and BelongToRoleId = '" + currentRoleId + "'");
            if (sqReader.Read()) {
                if (sqReader.GetInt32(sqReader.GetOrdinal("num")) >= 2)
                {
                    sqReader = db.ExecuteQuery("select Id from BookSecretsTable where T = " + ((short)secret.Type) + " and Q = " + ((short)secret.Quality) + " and BelongToRoleId = '" + currentRoleId + "' and Id != " + secret.PrimaryKeyId + " order by Id limit 0, 1");
                    //删除素材诀要
                    while (sqReader.Read())
                    {
                        db.ExecuteQuery("delete from BookSecretsTable where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
                    }
                    //升级选中的诀要
                    sqReader = db.ExecuteQuery("select SecretData, Q from BookSecretsTable where Id = " + secret.PrimaryKeyId);
                    if (sqReader.Read())
                    {
                        SecretData secretData = JsonManager.GetInstance().DeserializeObject<SecretData>(DESStatics.StringDecder(sqReader.GetString(sqReader.GetOrdinal("SecretData"))));
                        short endQuality = (short)(((int)secretData.Quality) + 1);
                        secretData.Quality = (QualityType)(endQuality);
                        db.ExecuteQuery("update BookSecretsTable set SecretData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObjectDealVector(secretData)) + "', Q = " + ((short)secretData.Quality) + " where Id = " + secret.PrimaryKeyId);
                        Statics.CreatePopMsg(Vector3.zero, string.Format("融合<color=\"{0}\">{1}</color>后使你武功精进!", Statics.GetQualityColorString(secretData.Quality), secretData.Name), Color.white, 30);
                        SoundManager.GetInstance().PushSound("ui0010");
                        result = true;
                    }
                    else
                    {
                        AlertCtrl.Show("需要融合的诀要不存在");
                    }
                }
                else
                {
                    AlertCtrl.Show(string.Format("至少需要2张相同品质的<color=\"{0}\">{1}</color>才能融合成更高级的诀要", Statics.GetQualityColorString(secret.Quality), secret.Name));
                }
            }
            db.CloseSqlConnection();
            return result;
        }

	}
}