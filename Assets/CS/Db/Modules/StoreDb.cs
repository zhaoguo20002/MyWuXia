using System;
using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 杂货铺相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 获取杂货铺商品数据
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetStorePanelData(string cityId) {
            SceneData currentScene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
            GetStoreData(currentScene.ResourceStoreId);
		}

        /// <summary>
        /// 回去商店数据
        /// </summary>
        /// <param name="storeId">Store identifier.</param>
        public void GetStoreData(string storeId) {
            ModifyResources();
            StoreData store = JsonManager.GetInstance().GetMapping<StoreData>("Stores", storeId);
            store.MakeJsonToModel();
            List<ItemData> items = store.Items;
            double silverNum = 0;
            db = OpenDb();
            SqliteDataReader sqReader = db.ExecuteQuery("select ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
            List<ResourceData> resources = null;
            if (sqReader.Read()) {
                string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
                //查询目前的银子余额
                ResourceData resource = resources.Find(item => item.Type == ResourceType.Silver);
                if (resource != null) {
                    silverNum = resource.Num;
                }
            }
            db.CloseSqlConnection();
            Messenger.Broadcast<List<ItemData>, double>(NotifyTypes.GetStorePanelDataEcho, items, silverNum);
        }

		/// <summary>
		/// 购买物品
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		/// <param name="num">Number.</param>
		public void BuyItem(string itemId, int num = 1) {
			ModifyResources();
			ItemData item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", itemId);
			//单次购买的数量不能超出物品叠加上限
			int startNum = num <= item.MaxNum ? num : item.MaxNum; 
			int itemNum = startNum;
			double silverNum = 0;
			db = OpenDb();
			//查询背包是否已满
			SqliteDataReader sqReader = db.ExecuteQuery("select count(*) as num from BagTable where BelongToRoleId = '" + currentRoleId + "'");
			bool enoughBagSeat = false;
			bool enoughMoney = false;
			string msg = "";
			if (sqReader.Read()) {
				if (sqReader.GetInt32(sqReader.GetOrdinal("num")) < MaxItemNumOfBag) {
					enoughBagSeat = true;
				}
			}
			if (enoughBagSeat) {
				sqReader = db.ExecuteQuery("select Id, ResourcesData from WorkshopResourceTable where BelongToRoleId = '" + currentRoleId + "'");
				List<ResourceData> resources = null;
				if (sqReader.Read()) {
                    string resourcesStr = sqReader.GetString(sqReader.GetOrdinal("ResourcesData"));
                    resourcesStr = resourcesStr.IndexOf("[") == 0 ? resourcesStr : DESStatics.StringDecder(resourcesStr);
                    resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(resourcesStr);
					//查询目前的银子余额
					ResourceData resource = resources.Find(re => re.Type == ResourceType.Silver);
					if (resource != null) {
						if (resource.Num >= item.BuyPrice * itemNum) {
							enoughMoney = true;
							resource.Num -= (item.BuyPrice * itemNum);
							//扣钱
                            db.ExecuteQuery("update WorkshopResourceTable set ResourcesData = '" + DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(resources)) + "' where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
							silverNum = resource.Num;
						}
					}
				}

				//背包未满
				if (enoughMoney) { 
					//银子够买
					//查询背包里是否有物品以及物品的数量是否达到上限
					sqReader = db.ExecuteQuery("select * from BagTable where ItemId = '" + itemId + "' and Num < MaxNum and BelongToRoleId = '" + currentRoleId + "'");
					if (!sqReader.HasRows) {
						//添加新的物品
						db.ExecuteQuery("insert into BagTable (ItemId, Type, Num, MaxNum, Lv, BelongToRoleId) values('" + itemId + "', " + ((int)item.Type) + ", " + itemNum + ", " + item.MaxNum + ", " + item.Lv + ", '" + currentRoleId + "')");
					} 
					else {
						int itemDataNum;
						int itemDataMaxNum;
						int addNum;
						//修改以后物品的数量
						while (sqReader.Read() && itemNum > 0) {
							itemDataNum = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
							itemDataMaxNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxNum"));
							addNum = (itemDataMaxNum - itemDataNum) <= itemNum ? (itemDataMaxNum - itemDataNum) : itemNum;
							itemNum -= addNum;
							db.ExecuteQuery("update BagTable set Num = " + (itemDataNum + addNum) + 
								" where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
						}
						//对已有物品进行累加后还有剩余数量，则这个数量全部新增为新物品
						if (itemNum > 0) {
							db.ExecuteQuery("insert into BagTable (ItemId, Type, Num, MaxNum, Lv, BelongToRoleId) values('" + itemId + "', " + ((int)item.Type) + ", " + itemNum + ", " + item.MaxNum + ", " + item.Lv + ", '" + currentRoleId + "')");
						}
					}
					
				}
			}
			db.CloseSqlConnection();
			if (!enoughBagSeat) {
				AlertCtrl.Show("行囊已满,不能携带更多的物品!");
			}
			else if (!enoughMoney) {
				AlertCtrl.Show("银子不够!\n(<color=\"#00FF00\">工坊中可以赚取银子</color>)");
			}
			else {
				msg = string.Format("<color=\"#1ABDE6\">{0}</color>+{1}", item.Name, startNum);
				Messenger.Broadcast<string, double>(NotifyTypes.BuyItemEcho, msg, silverNum);
			}
		}
	}
}