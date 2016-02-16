using UnityEngine;
using System.Collections;

namespace Game {
	public class DropData {
		/// <summary>
		/// 掉落概率[0-100]
		/// </summary>
		public float Rate;
		/// <summary>
		/// 掉落数量
		/// </summary>
		public int Num;
		/// <summary>
		/// 物品资源索引Id
		/// </summary>
		public string ResourceItemDataId;
		/// <summary>
		/// 凋落物对象
		/// </summary>
		public ItemData Item;

		public DropData() {
			Rate = 100;
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

		/// <summary>
		/// 判断是否掉落
		/// </summary>
		/// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
		public bool IsTrigger() {
			return UnityEngine.Random.Range(0f, 100f) <= Rate;
		}
	}
}
