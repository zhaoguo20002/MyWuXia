using UnityEngine;
using System.Collections;

namespace Game {
	public class Gift {
		/// <summary>
		/// 当前物品数量
		/// </summary>
		public int Num;
		/// <summary>
		/// 物品索引Id
		/// </summary>
		public string ResourceItemDataId;
		/// <summary>
		/// 物品信息
		/// </summary>
		public ItemData Item;

		public Gift() {
			Num = 1;
			ResourceItemDataId = "";
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", ResourceItemDataId);
			Num = Num > Item.MaxNum ? Item.MaxNum : Num;
		}
	}
}
