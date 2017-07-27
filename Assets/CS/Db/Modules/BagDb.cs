using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 背包相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 背包中的物品上限
		/// </summary>
		public int MaxItemNumOfBag = 20;
		/// <summary>
		/// 添加掉落物到背包
		/// </summary>
		/// <returns>The item to bag.</returns>
		/// <param name="drops">Drops.</param>
		public List<DropData> PushItemToBag(List<DropData> drops) {
			int bagNumLeft = GetItemNumLeftInBag();
			List<DropData> resultDrops = new List<DropData>();
			db = OpenDb();
			SqliteDataReader sqReader;
			DropData drop;
			int num;
			int maxNum;
			int addNum;
			for (int i = 0; i < drops.Count; i++) {
				drop = drops[i];
				if (drop.Item == null) {
					drop.MakeJsonToModel();
				}
                if (drop.Item.Type == ItemType.Task)
                {
                    //判断是否为任务物品，任务物品在使用过后就不再掉落
                    sqReader = db.ExecuteQuery("select * from UsedItemRecordsTable where ItemId = '" + drop.Item.Id + "' and BelongToRoleId = '" + currentRoleId + "'");
                    if (sqReader.HasRows)
                    {
                        continue;
                    }
                }
                //背包位子如果不足则除了任务物品之外，其他的物品都不能添加进背包
				if (bagNumLeft <= 0) {
					if (drop.Item.Type != ItemType.Task) {
						continue;
					}
				}
				//判断是否掉落
				if (!drop.IsTrigger()) {
					continue;
				}
				//查询背包里是否有物品以及物品的数量是否达到上限
				sqReader = db.ExecuteQuery("select * from BagTable where ItemId = '" + drop.Item.Id + "' and Num < MaxNum and BelongToRoleId = '" + currentRoleId + "'");
				if (!sqReader.HasRows) {
					//添加新的物品
					db.ExecuteQuery("insert into BagTable (ItemId, Type, Num, MaxNum, Lv, BelongToRoleId) values('" + drop.Item.Id + "', " + ((int)drop.Item.Type) + ", " + drop.Item.Num + ", " + drop.Item.MaxNum + ", " + drop.Item.Lv + ", '" + currentRoleId + "')");
				} 
				else {
					int itemNum = drop.Item.Num;
					//修改物品的数量
					while (sqReader.Read()) {
						num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
						maxNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxNum"));
						addNum = (maxNum - num) <= itemNum ? (maxNum - num) : itemNum;
						itemNum -= addNum;
						db.ExecuteQuery("update BagTable set Num = " + (num + addNum) + 
							" where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
						//如果掉落物的数量还有则下个循环继续处理添加物品
						if (itemNum > 0) {
							i--;
						}
					}
				}
				resultDrops.Add(drop);
				//任务物品不算入背包占位数
				if (drop.Item.Type != ItemType.Task) {
					bagNumLeft--;
				}
			}
			db.CloseSqlConnection();
			return resultDrops;
		}

		/// <summary>
		/// 查询背包中剩余的位子
		/// </summary>
		/// <returns>The item number in bag.</returns>
		public int GetItemNumLeftInBag() {
			int num = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select count(*) as num from BagTable where Type != " + (int)ItemType.Task + " and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				num = Mathf.Clamp(MaxItemNumOfBag - sqReader.GetInt32(sqReader.GetOrdinal("num")), 0, MaxItemNumOfBag);
			}
			db.CloseSqlConnection();
			return num;
		}

		/// <summary>
		/// 消耗物品
		/// </summary>
		/// <returns><c>true</c>, if item from bag was cost, <c>false</c> otherwise.</returns>
		/// <param name="itemId">Item identifier.</param>
		/// <param name="num">Number.</param>
		public bool CostItemFromBag(string itemId, int num) {
			bool _result = false;
			//计算数量是否满足要求
			if (GetItemNumByItemId(itemId) >= num) {
				db = OpenDb();
				//查询背包里的物品是否存在
				SqliteDataReader sqReader = db.ExecuteQuery("select * from BagTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "' order by Num");
				if (sqReader.HasRows) {
					//修改物品的数量
					int startNum = num;
					while (sqReader.Read() && startNum > 0) {
						int dataNum = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
						int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
						//需要处理相同物品不同数量的叠加消耗问题
						if (dataNum > startNum) {
							db.ExecuteQuery("update BagTable set Num = " + (dataNum - startNum) + 
								" where Id = " + id);
							startNum = 0;
						}
						else {
							db.ExecuteQuery("delete from BagTable where Id = " + id);
							startNum -= dataNum;
						}
					}
					_result = true;
				}
				db.CloseSqlConnection();
			}
			return _result;
		}

		/// <summary>
		/// 根据物品id查询物品的数量
		/// </summary>
		/// <returns>The item number by item identifier.</returns>
		/// <param name="itemId">Item identifier.</param>
		public int GetItemNumByItemId(string itemId) {
			int result = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select ItemId from BagTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "'");
			//先查询是否存在该物品
			if (sqReader.HasRows) {
				//计算数量是否满足要求
				sqReader = db.ExecuteQuery("select sum(Num) as AllNums from BagTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.HasRows && sqReader.Read()) {
					if (sqReader.GetOrdinal("AllNums") != null) {
						result = sqReader.GetInt32(sqReader.GetOrdinal("AllNums"));
					}
				}
			}
			db.CloseSqlConnection();
			return result;
		}

		/// <summary>
		/// 获取行囊物品数据
		/// </summary>
		public void GetBagPanelData() {
			ModifyResources();
			List<ItemData> items = new List<ItemData>();
			double silverNum = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ItemId, Num from BagTable where BelongToRoleId = '" + currentRoleId + "'");
			ItemData item;
			while(sqReader.Read()) {
				item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", sqReader.GetString(sqReader.GetOrdinal("ItemId")));
				item.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				item.Num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
				items.Add(item);
			}
			sqReader = db.ExecuteQuery("select ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			if (sqReader.Read()) {
				resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
				//查询目前的银子余额
				ResourceData resource = resources.Find(re => re.Type == ResourceType.Silver);
				if (resource != null) {
					silverNum = resource.Num;
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<ItemData>, double>(NotifyTypes.GetBagPanelDataEcho, items, silverNum);
		}

		/// <summary>
		/// 获取出售物品数据
		/// </summary>
		public void GetSellItemsPanelData() {
			List<ItemData> items = new List<ItemData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ItemId, Num from BagTable where BelongToRoleId = '" + currentRoleId + "'");
			ItemData item;
			while(sqReader.Read()) {
				item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", sqReader.GetString(sqReader.GetOrdinal("ItemId")));
				if (item.SellPrice >= 0) {
					item.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					item.Num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
					items.Add(item);
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<List<ItemData>>(NotifyTypes.GetSellItemsPanelDataEcho, items);
		}

		/// <summary>
		/// 出售背包中的物品
		/// </summary>
		/// <param name="ids">Identifiers.</param>
		public void SellItems(JArray ids) {
			List<ItemData> items = new List<ItemData>();
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ItemId, Num from BagTable where BelongToRoleId = '" + currentRoleId + "'");
			ItemData item;
			//查询出背包中所有的可出售物品
			while(sqReader.Read()) {
				item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", sqReader.GetString(sqReader.GetOrdinal("ItemId")));
				if (item.SellPrice >= 0) {
					item.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					item.Num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
					items.Add(item);
				}
			}
			//检查前端传递过来的选中的物品id是否合法
			int id;
			ItemData findItem;
			double addSilverNum = 0;
			double silverNum = 0;
			string queryStr = "";
			for (int i = 0; i < ids.Count; i++) {
				id = (int)ids[i];
				findItem = items.Find(find => find.PrimaryKeyId == id);
				if (findItem != null) {
					addSilverNum += findItem.SellPrice * findItem.Num;
                    if (i > 0)
                    {
                        queryStr += " or ";
                    }
                    queryStr += ("Id = " + id);
//					queryStr += (i > 0 ? " or " : "" + ("Id = " + id));
				}
			}
			//删除选中的物品 
			if (queryStr != "") {
				db.ExecuteQuery("delete from BagTable where " + queryStr);
			}
			//添加银子到资源
			sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
			List<ResourceData> resources = null;
			if (sqReader.Read()) {
				resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
				//查询目前的银子余额
				ResourceData resource = resources.Find(re => re.Type == ResourceType.Silver);
				if (resource != null) {
					resource.Num += addSilverNum;
					silverNum = resource.Num;
					//加钱
					db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
				}
			}
			db.CloseSqlConnection();
			Messenger.Broadcast<double>(NotifyTypes.SellItemsEcho, silverNum);
			Statics.CreatePopMsg(Vector3.zero, string.Format("{0} {1}", Statics.GetResourceName(ResourceType.Silver), "+" + addSilverNum.ToString()), Color.green, 30);
		}

		/// <summary>
		/// 丢弃物品
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void DiscardItem(int id) {
			bool discarded = false;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, ItemId from BagTable where Id = " + id);
			ItemData item = null;
			while(sqReader.Read()) {
				item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", sqReader.GetString(sqReader.GetOrdinal("ItemId")));
				//检测物品是否可以丢弃
				if (item.CanDiscard) {
					//丢弃物品
					db.ExecuteQuery("delete from BagTable where Id = " + id);
					discarded = true;
				}
			}
			db.CloseSqlConnection();
			if (discarded) {
				GetBagPanelData();
				Statics.CreatePopMsg(Vector3.zero, string.Format("<color=\"#1ABDE6\">{0}</color>已被丢弃", item.Name), Color.white, 30);
			}
		}

		/// <summary>
		/// 将背包里的辎重箱放入工坊资源中
		/// </summary>
		public void BringResourcesToWorkshop() {
			db = OpenDb();
			//查询背包
			SqliteDataReader sqReader = db.ExecuteQuery("select * from BagTable where BelongToRoleId = '" + currentRoleId + "' and Type >= " + (int)ItemType.Wheat + " and Type <= " + (int)ItemType.DarksteelIngot);
			if (sqReader.HasRows) {
				List<ResourceType> types = new List<ResourceType>();
				List<int> nums = new List<int>();
				ResourceType type;
				while(sqReader.Read()) {
					type = Statics.ChangeItemTypeToResourctType((ItemType)sqReader.GetInt32(sqReader.GetOrdinal("Type")));
					types.Add(type);
                    nums.Add(sqReader.GetInt32(sqReader.GetOrdinal("Num")) * sqReader.GetInt32(sqReader.GetOrdinal("Lv")));
				}
				if (types.Count > 0) {
					//查询出资源
					sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
					if (sqReader.Read()) {
						int resourceId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
						List<ResourceData> resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(sqReader.GetString(sqReader.GetOrdinal("ResourcesData")));
						ResourceData findResource;
						int num;
						string msg = "";
						for (int i = 0; i < types.Count; i++) {
							findResource = resources.Find(re => re.Type == types[i]);
							if (findResource != null) {
								num = nums[i];
								findResource.Num += num;
								msg += string.Format("\n{0}+{1}", Statics.GetResourceName(types[i]), num);
							}
							else {
								msg += string.Format("\n<color=\"#FF0000\">工坊产力不够无力储存{0}只好丢弃</color>", Statics.GetResourceName(types[i]));
							}
						}
						//清空背包里的辎重箱
						db.ExecuteQuery("delete from BagTable where BelongToRoleId = '" + currentRoleId + "' and Type >= " + (int)ItemType.Wheat + " and Type <= " + (int)ItemType.DarksteelIngot);
						//更新资源数据
						db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + JsonManager.GetInstance().SerializeObject(resources) + "' where Id = " + resourceId);
						if (msg != "") {
							msg = "这次游历江湖带回了" + msg;
							Statics.CreatePopMsg(Vector3.zero, msg, Color.green, 30);
						}
					}
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 使用物品
		/// </summary>
		/// <param name="Id">Identifier.</param>
		public void UseItem(int id) {
			db = OpenDb();
			string itemId = "";
			ItemType type = ItemType.None;
			int num = 0;
			SqliteDataReader sqReader = db.ExecuteQuery("select ItemId, Type, Num from BagTable where Id = " + id);
			if (sqReader.Read()) {
				itemId = sqReader.GetString(sqReader.GetOrdinal("ItemId"));
				type = (ItemType)sqReader.GetInt32(sqReader.GetOrdinal("Type"));
				num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
			}
			db.CloseSqlConnection();
			if (type != ItemType.None && num > 0) {
				ItemData item;
                switch (type)
                {
                    case ItemType.Food:
                        if (UserModel.CurrentUserData.PositionStatu == UserPositionStatusType.InArea)
                        {
                            Eat(id, num);
                        }
                        else
                        {
                            AlertCtrl.Show("野外闯荡江湖时才能吃干粮");
                        }
                        break;
                    case ItemType.Weapon:
                        item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", itemId);
                        if (AddNewWeapon(item.StringValue, ""))
                        {
                            WeaponData weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", item.StringValue);
                            Statics.CreatePopMsg(Vector3.zero, string.Format("<color=\"{0}\">{1}</color>+1", Statics.GetQualityColorString(weapon.Quality), weapon.Name), Color.white, 30);

                            //删除兵器盒
                            db = OpenDb();
                            db.ExecuteQuery("delete from BagTable where Id = " + id);
                            db.CloseSqlConnection();
                            //重新加载背包数据
                            GetBagPanelData();
                        }
                        else
                        {
                            AlertCtrl.Show("兵器匣已满，请先整理兵器匣");
                        }
                        break;
                    case ItemType.Book:
                        item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", itemId);
                        BookData book = JsonManager.GetInstance().GetMapping<BookData>("Books", item.StringValue);
                        if (AddNewBook(item.StringValue, ""))
                        {
                            Statics.CreatePopMsg(Vector3.zero, string.Format("<color=\"{0}\">{1}</color>+1", Statics.GetQualityColorString(book.Quality), book.Name), Color.white, 30);
					
                            //删除秘籍盒
                            db = OpenDb();
                            db.ExecuteQuery("delete from BagTable where Id = " + id);
                            db.CloseSqlConnection();
                            //重新加载背包数据
                            GetBagPanelData();
                        }
                        else
                        {
                            AlertCtrl.Show(string.Format("你已经习得<color=\"{0}\">{1}</color>, 无需再研读", Statics.GetQualityColorString(book.Quality), book.Name));
                        }
                        break;
                    default:
                        AlertCtrl.Show("该物品不可使用!");
                        break;
                }
			}
		}

		/// <summary>
		/// 吃干粮
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="num">Number.</param>
		public void Eat(int id, int num) {
			int eatNum = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, Data, AreaFoodNum from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				int userDataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				UserData user = JsonManager.GetInstance().DeserializeObject<UserData>(sqReader.GetString(sqReader.GetOrdinal("Data")));
				user.AreaFood.Num = sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum"));
				user.AreaFood.Num = user.AreaFood.Num > user.AreaFood.MaxNum ? user.AreaFood.MaxNum : user.AreaFood.Num;
				if (user.AreaFood.Num < user.AreaFood.MaxNum) {
					eatNum = user.AreaFood.MaxNum - user.AreaFood.Num;
					eatNum = eatNum <= num ? eatNum : num;
					user.AreaFood.Num += eatNum;
					num -= eatNum;
					if (num > 0) {
						//减掉吃掉的干粮辎重
						db.ExecuteQuery("update BagTable set Num = " + num + " where Id = " + id);
					}
					else {
						//删除干粮辎重
						db.ExecuteQuery("delete from BagTable where Id = " + id);
					}
					//更新当前干粮
					db.ExecuteQuery("Update UserDatasTable set Data = '" + JsonManager.GetInstance().SerializeObjectDealVector(user) + "', AreaFoodNum = " + user.AreaFood.Num + " where Id = " + userDataId);
					AreaMainPanelCtrl.MakeUpdateFoods(user.AreaFood.Num);
				}
				else {
					AlertCtrl.Show("目前体力充沛不需要进食!");
				}
			}
			db.CloseSqlConnection();
			if (eatNum > 0) {
				Statics.CreatePopMsg(Vector3.zero, string.Format("补充了{0}个干粮", eatNum), Color.green, 30);
				GetBagPanelData();

			}
		}

		/// <summary>
		/// 吃干粮
		/// </summary>
		/// <returns>The food.</returns>
		/// <param name="num">Number.</param>
		public int EatFood(int num) {
			int eatNum = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select Id, Data, AreaFoodNum from UserDatasTable where BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.Read()) {
				int userDataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
				UserData user = JsonManager.GetInstance().DeserializeObject<UserData>(sqReader.GetString(sqReader.GetOrdinal("Data")));
				user.AreaFood.Num = sqReader.GetInt32(sqReader.GetOrdinal("AreaFoodNum"));
				user.AreaFood.Num = user.AreaFood.Num > user.AreaFood.MaxNum ? user.AreaFood.MaxNum : user.AreaFood.Num;
				if (user.AreaFood.Num < user.AreaFood.MaxNum) {
					eatNum = user.AreaFood.MaxNum - user.AreaFood.Num;
					eatNum = eatNum <= num ? eatNum : num;
					user.AreaFood.Num += eatNum;
					//更新当前干粮
					db.ExecuteQuery("Update UserDatasTable set Data = '" + JsonManager.GetInstance().SerializeObjectDealVector(user) + "', AreaFoodNum = " + user.AreaFood.Num + " where Id = " + userDataId);
					AreaMainPanelCtrl.MakeUpdateFoods(user.AreaFood.Num);
				}
			}
			db.CloseSqlConnection();
			return eatNum;
		}

		/// <summary>
		/// 查询使用过的物品的数量
		/// </summary>
		/// <returns>The used item number by item identifier.</returns>
		/// <param name="itemId">Item identifier.</param>
		public int GetUsedItemNumByItemId(string itemId) {
			int result = 0;
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select ItemId from UsedItemRecordsTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "'");
			//先查询是否存在该物品
			if (sqReader.HasRows) {
				//计算数量是否满足要求
				sqReader = db.ExecuteQuery("select sum(Num) as AllNums from UsedItemRecordsTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "'");
				if (sqReader.HasRows && sqReader.Read()) {
					if (sqReader.GetOrdinal("AllNums") != null) {
						result = sqReader.GetInt32(sqReader.GetOrdinal("AllNums"));
					}
				}
			}
			db.CloseSqlConnection();
			return result;
		}

		/// <summary>
		/// 更新使用过的物品记录
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		/// <param name="num">Number.</param>
		public void UpdateUsedItemRecords(string itemId, int num) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select * from UsedItemRecordsTable where ItemId = '" + itemId + "' and BelongToRoleId = '" + currentRoleId + "'");
			if (sqReader.HasRows) {
				if (sqReader.Read()) {
					int id = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
					int numData = sqReader.GetInt32(sqReader.GetOrdinal("Num")) + num;
					db.ExecuteQuery("update UsedItemRecordsTable set Num = " + numData + " where Id = " + id);
				}
			}
			else {
				db.ExecuteQuery("insert into UsedItemRecordsTable (ItemId, Num, BelongToRoleId) values('" + itemId + "', 1, '" + currentRoleId + "');");
			}
			db.CloseSqlConnection();
		}

        /// <summary>
        /// 获取行囊物品数据
        /// </summary>
        public List<ItemData> GetItems(ItemType type) {
            List<ItemData> items = new List<ItemData>();
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select Id, ItemId, Num from BagTable where BelongToRoleId = '" + currentRoleId + "' and Type = " + (int)type + " order by Lv desc");
            ItemData item;
            while(sqReader.Read()) {
                item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", sqReader.GetString(sqReader.GetOrdinal("ItemId")));
                item.PrimaryKeyId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
                item.Num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
                items.Add(item);
            }
            db.CloseSqlConnection();
            return items;
        }
	}
}