using UnityEngine;
using System.Collections;
using System.ComponentModel;

namespace Game {
	/// <summary>
	/// Gender type.
	/// </summary>
	public enum GenderType {
		/// <summary>
		/// 男
		/// </summary>
		[Description("男")]
		Male,
		/// <summary>
		/// 女
		/// </summary>
		[Description("女")]
		Female
	}

	/// <summary>
	/// 品质类型
	/// </summary>
	public enum QualityType {
		/// <summary>
		/// 白
		/// </summary>
		[Description("白")]
		White,
		/// <summary>
		/// 绿
		/// </summary>
		[Description("绿")]
		Green,
		/// <summary>
		/// 蓝
		/// </summary>
		[Description("蓝")]
		Blue,
		/// <summary>
		/// 紫
		/// </summary>
		[Description("紫")]
		Purple,
		/// <summary>
		/// 金
		/// </summary>
		[Description("金")]
		Gold,
		/// <summary>
		/// 橙
		/// </summary>
		[Description("橙")]
		Orange,
		/// <summary>
		/// 红
		/// </summary>
		[Description("红")]
		Red
	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public enum ItemType {
		/// <summary>
		/// 普通物品(卖出换钱)
		/// </summary>
		[Description("普通物品(卖出换钱)")]
		Normal,
		/// <summary>
		/// 任务物品
		/// </summary>
		[Description("任务物品")]
		Task,
		/// <summary>
		/// 生产材料(合成制作用的原料,家园里挂机产出)
		/// </summary>
		[Description("生产材料(合成制作用的原料,家园里挂机产出)")]
		Material,
		/// <summary>
		/// 伤药
		/// </summary>
		[Description("伤药")]
		Vulnerary,
		/// <summary>
		/// 消耗品
		/// </summary>
		[Description("消耗品")]
		Cost
	}

	/// <summary>
	/// 场景中事件枚举类型
	/// </summary>
	public enum SceneEventType {
		/// <summary>
		/// 进入城镇
		/// </summary>
		[Description("进入城镇")]
		EnterCity,
		/// <summary>
		/// 进入大地图
		/// </summary>
		[Description("进入大地图")]
		EnterArea,
		/// <summary>
		/// 触发战斗
		/// </summary>
		[Description("触发战斗")]
		Battle,
		/// <summary>
		/// 剧情对话
		/// </summary>
		[Description("剧情对话")]
		Dialog,
		/// <summary>
		/// 触发任务
		/// </summary>
		[Description("触发任务")]
		Task,
		/// <summary>
		/// 进入商店
		/// </summary>
		[Description("进入商店")]
		Store,
		/// <summary>
		/// 掉落物
		/// </summary>
		[Description("掉落物")]
		Gift,
		/// <summary>
		/// 采集点
		/// </summary>
		[Description("采集点")]
		Collection,
		/// <summary>
		/// 驿站[花钱回复行动体力,并且可以雇佣马车传送到其他城镇]
		/// </summary>
		[Description("驿站[花钱回复行动体力,并且可以雇佣马车传送到其他城镇]")]
		Inn,
		/// <summary>
		/// 酒馆[可能结交到伙伴]
		/// </summary>
		[Description("酒馆[可能结交到伙伴]")]
		WineShop
	}
}
