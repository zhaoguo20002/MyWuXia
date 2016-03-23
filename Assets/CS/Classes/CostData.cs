using System;

namespace Game {
	/// <summary>
	/// 消耗品实体类(所有需要用物品进行合成的东西都统一用此类)
	/// </summary>
	public class CostData {
		/// <summary>
		/// id
		/// </summary>
		public string Id;
		/// <summary>
		/// 需要的物品数量
		/// </summary>
		public int Num;
		public CostData(string id, int num) {
			Id = id;
			Num = num;
		}
	}
}

