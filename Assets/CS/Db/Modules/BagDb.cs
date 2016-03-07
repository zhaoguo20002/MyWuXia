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
		/// 添加掉落物到背包
		/// </summary>
		/// <param name="drops">Drops.</param>
		public void PushItemToBag(List<DropData> drops) {
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
				//查询背包里是否有物品以及物品的数量是否达到上限
				sqReader = db.ExecuteQuery("select * from BagTable where ItemId = '" + drop.ResourceItemDataId + "' and Num < MaxNum and BelongToRoleId = '" + currentRoleId + "'");
				if (!sqReader.HasRows) {
					//添加新的物品
					db.ExecuteQuery("insert into BagTable (ItemId, Num, MaxNum, BelongToRoleId) values('" + drop.ResourceItemDataId + "', " + drop.Item.Num + ", " + drop.Item.MaxNum + ", '" + currentRoleId + "')");
				} 
				else {
					//修改物品的数量
					while (sqReader.Read()) {
						num = sqReader.GetInt32(sqReader.GetOrdinal("Num"));
						maxNum = sqReader.GetInt32(sqReader.GetOrdinal("MaxNum"));
						addNum = (maxNum - num) <= drop.Item.Num ? (maxNum - num) : drop.Item.Num;
						drop.Item.Num -= addNum;
						db.ExecuteQuery("update BagTable set Num = " + (num + addNum) + 
							" where Id = " + sqReader.GetInt32(sqReader.GetOrdinal("Id")));
						//如果掉落物的数量还有则下个循环继续处理添加物品
						if (drop.Item.Num > 0) {
							i--;
						}
					}
				}
			}
			db.CloseSqlConnection();
		}
	}
}