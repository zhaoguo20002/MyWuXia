using System;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 生产材料关系描述数据
	/// </summary>
	public class ResourceRelationshipData {
		/// <summary>
		/// 资源类型
		/// </summary>
		public ResourceType Type;
		/// <summary>
		/// 开启城镇id
		/// </summary>
		public string BelongToCityId;
		/// <summary>
		/// 每次生产需要的原材料
		/// </summary>
		public List<ResourceData> Needs;
		/// <summary>
		/// 每次产量
		/// </summary>
		public int YieldNum;
		/// <summary>
		/// 价值
		/// </summary>
		public float Worth;
		/// <summary>
		/// 出售价
		/// </summary>
		public float SellWorth;
		/// <summary>
		/// 从商店购买价
		/// </summary>
		public float BuyWorth;

		public ResourceRelationshipData (ResourceType type, string belongToCityId, List<ResourceData> needs, int yieldNum, float worth, float sellWorth, float buyWorth) {
			Type = type;
			BelongToCityId = belongToCityId;
			Needs = needs;
			YieldNum = yieldNum;
			Worth = worth;
			SellWorth = sellWorth;
			BuyWorth = buyWorth;
		}
	}
}

