using UnityEngine;
using System.Collections;

namespace Game {
	/// <summary>
	/// Gender type.
	/// </summary>
	public enum GenderType {
		/// <summary>
		/// 男
		/// </summary>
		Male,
		/// <summary>
		/// 女
		/// </summary>
		Female
	}

	/// <summary>
	/// 品质类型
	/// </summary>
	public enum QualityType {
		/// <summary>
		/// 白
		/// </summary>
		White,
		/// <summary>
		/// 绿
		/// </summary>
		Green,
		/// <summary>
		/// 蓝
		/// </summary>
		Blue,
		/// <summary>
		/// 紫
		/// </summary>
		Purple,
		/// <summary>
		/// 金
		/// </summary>
		Gold,
		/// <summary>
		/// 橙
		/// </summary>
		Orange,
		/// <summary>
		/// 红
		/// </summary>
		Red
	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public enum ItemType {
		/// <summary>
		/// 普通物品
		/// </summary>
		Normal,
		/// <summary>
		/// 任务物品
		/// </summary>
		Task,
		/// <summary>
		/// 生产材料(合成制作用的原料,家园里挂机产出)
		/// </summary>
		Material,
		/// <summary>
		/// 伤药
		/// </summary>
		Vulnerary
	}
}
