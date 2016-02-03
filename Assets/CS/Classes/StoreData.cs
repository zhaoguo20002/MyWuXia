using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class StoreData {
		/// <summary>
		/// 商店主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 商店名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 商店出售的物品Id列表
		/// </summary>
		public List<string> ResourceItemDataIds;
		/// <summary>
		/// 商店出售的物品
		/// </summary>
		public List<ItemData> Items;

		public StoreData() {
			ResourceItemDataIds = new List<string>();
			Items = new List<ItemData>();
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Items.Clear();
			for (int i= 0; i< ResourceItemDataIds.Count; i++) {
				Items.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", ResourceItemDataIds[i]));
			}
		}
	}
}
