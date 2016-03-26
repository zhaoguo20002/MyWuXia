using UnityEngine;
using System.Collections;

namespace Game {
	public class ItemData {
		/// <summary>
		/// 物品Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 物品类型
		/// </summary>
		public ItemType Type;
		/// <summary>
		/// 物品图标
		/// </summary>
		public string IconId;
		/// <summary>
		/// 物品名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 物品描述
		/// </summary>
		public string Desc;
		/// <summary>
		/// 出售价格(-1为不可出售)
		/// </summary>
		public int SellPrice;
		/// <summary>
		/// 购买价格
		/// </summary>
		public int BuyPrice;
		/// <summary>
		/// 是否可以被丢弃
		/// </summary>
		public bool CanDiscard;
		/// <summary>
		/// 当前数量
		/// </summary>
		public int Num;
		/// <summary>
		/// 堆叠上限
		/// </summary>
		public int MaxNum;
		/// <summary>
		/// 转换类掉落物品需要变换成特定类型的id
		/// </summary>
		public string ChangeToId;
		/// <summary>
		/// 物品等级
		/// </summary>
		public int Lv;

		public ItemData() {
			IconId = "";
			Desc = "";
			SellPrice = 0;
			BuyPrice = 1;
			CanDiscard = true;
			Num = 1;
			MaxNum = 1;
			ChangeToId = "";
			Lv = 1;
		}
	}
}
