using UnityEngine;
using System.Collections;

namespace Game {
	public class DropData {
		/// <summary>
		/// 凋落物Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 掉落概率
		/// </summary>
		public float Rate;
		/// <summary>
		/// 掉落数量
		/// </summary>
		public int Num;
		/// <summary>
		/// 物品资源索引Id
		/// </summary>
		public string ResourceItemId;
		/// <summary>
		/// 凋落物对象
		/// </summary>
		public ItemData Item;

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			
		}
	}
}
