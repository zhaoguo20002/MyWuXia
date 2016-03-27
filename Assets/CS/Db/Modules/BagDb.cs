﻿using UnityEngine;
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
			}
			db.CloseSqlConnection();
			return resultDrops;
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
	}
}