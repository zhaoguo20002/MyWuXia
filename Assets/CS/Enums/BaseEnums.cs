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
		Red,
        /// <summary>
        /// 闪金
        /// </summary>
        [Description("闪金")]
        FlashGold,
        /// <summary>
        /// 闪橙
        /// </summary>
        [Description("闪橙")]
        FlashOrange,
        /// <summary>
        /// 闪红
        /// </summary>
        [Description("闪红")]
        FlashRed
	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public enum ItemType {
		/// <summary>
		/// 无
		/// </summary>
		[Description("无")]
		None = -1,
		/// <summary>
		/// 普通物品(卖出换钱)
		/// </summary>
		[Description("普通物品")]
		Normal = 0,
		/// <summary>
		/// 任务物品
		/// </summary>
		[Description("任务物品")]
		Task = 1,
		/// <summary>
		/// 生产材料(合成制作用的原料,家园里挂机产出的资源合成后获得的半成品)
		/// </summary>
		[Description("生产材料")]
		Material = 2,
		/// <summary>
		/// 伤药
		/// </summary>
		[Description("伤药")]
		Vulnerary = 3,
		/// <summary>
		/// 酒
		/// </summary>
		[Description("酒")]
		Wine = 4,
		/// <summary>
		/// 菜
		/// </summary>
		[Description("菜")]
		Dinner = 5,
		/// <summary>
		/// 掉落伙伴(使用后获得)
		/// </summary>
		[Description("掉落伙伴")]
		Role = 6,
		/// <summary>
		/// 掉落兵器(使用后获得)
		/// </summary>
		[Description("兵器盒")]
		Weapon = 7,
		/// <summary>
		/// 掉落秘籍(使用后获得)
		/// </summary>
		[Description("秘籍盒")]
		Book = 8,
		/// <summary>
		/// 消耗品(比如一定数量的秘籍残卷可以找到特定npc去兑换成秘籍)
		/// </summary>
		[Description("消耗品")]
        Cost = 9,
        /// <summary>
        /// 工坊产出资源干粮
        /// </summary>
        [Description("干粮辎重箱")]
        Food = 10,
		/// <summary>
		/// 工坊产出资源小麦
		/// </summary>
		[Description("小麦辎重箱")]
		Wheat = 11,
		/// <summary>
		/// 工坊产出资源石料
		/// </summary>
		[Description("石料辎重箱")]
		Stone = 12,
		/// <summary>
		/// 工坊产出资源木材
		/// </summary>
		[Description("木材辎重箱")]
		Wood = 13,
		/// <summary>
		/// 工坊产出资源铁
		/// </summary>
		[Description("铁辎重箱")]
		Iron = 14,
		/// <summary>
		/// 工坊产出资源银
		/// </summary>
		[Description("银辎重箱")]
		SilverOre = 15,
		/// <summary>
		/// 工坊产出资源钢
		/// </summary>
		[Description("钢辎重箱")]
		Steel = 16,
		/// <summary>
		/// 工坊产出资源银子
		/// </summary>
		[Description("银子辎重箱")]
		Silver = 17,
		/// <summary>
		/// 工坊产出资源钨
		/// </summary>
		[Description("钨辎重箱")]
		Tungsten = 18,
		/// <summary>
		/// 工坊产出资源玉
		/// </summary>
		[Description("玉辎重箱")]
		Jade = 19,
		/// <summary>
		/// 工坊产出资源赤铁
		/// </summary>
		[Description("赤铁辎重箱")]
		RedSteel = 20,
		/// <summary>
		/// 工坊产出资源百炼钢
		/// </summary>
		[Description("百炼钢辎重箱")]
		RefinedSteel = 21,
		/// <summary>
		/// 工坊产出资源钨钢
		/// </summary>
		[Description("钨钢辎重箱")]
		TungstenSteel = 22,
		/// <summary>
		/// 工坊产出资源乌金木
		/// </summary>
		[Description("乌金木辎重箱")]
		Zingana = 23,
		/// <summary>
		/// 工坊产出资源玄铁锭
		/// </summary>
		[Description("玄铁锭辎重箱")]
		DarksteelIngot = 24,
        /// <summary>
        /// 回血药物
        /// </summary>
        [Description("回血药物")]
        Drug = 50,
        /// <summary>
        /// 秘籍修为
        /// </summary>
        [Description("秘籍修为")]
        Secret = 51,
        /// <summary>
        /// 神秘诀要锦囊
        /// </summary>
        [Description("神秘诀要锦囊")]
        RandomSecre = 52,
        /// <summary>
        /// 诀要-增减益气血上限
        /// </summary>
        [Description("诀要-增减益气血上限")]
        SecretIncreaseMaxHP = 101,
        /// <summary>
        /// 诀要-增减益气血上限比例
        /// </summary>
        [Description("诀要-增减益气血上限比例")]
        SecreIncreaseMaxHPRate = 102,
        /// <summary>
        /// 诀要-增减益外功点数
        /// </summary>
        [Description("诀要-增减益外功点数")]
        SecreIncreasePhysicsAttack = 103,
        /// <summary>
        /// 诀要-增减益外功比例
        /// </summary>
        [Description("诀要-增减益外功比例")]
        SecreIncreasePhysicsAttackRate = 104,
        /// <summary>
        /// 诀要-增减益外防点数
        /// </summary>
        [Description("诀要-增减益外防点数")]
        SecreIncreasePhysicsDefense = 105,
        /// <summary>
        /// 诀要-增减益外防比例
        /// </summary>
        [Description("诀要-增减益外防比例")]
        SecreIncreasePhysicsDefenseRate = 106,
        /// <summary>
        /// 诀要-增减益内功点数
        /// </summary>
        [Description("诀要-增减益内功点数")]
        SecreIncreaseMagicAttack = 107,
        /// <summary>
        /// 诀要-增减益内功比例
        /// </summary>
        [Description("诀要-增减益内功比例")]
        SecreIncreaseMagicAttackRate = 108,
        /// <summary>
        /// 诀要-增减益内防点数
        /// </summary>
        [Description("诀要-增减益内防点数")]
        SecreIncreaseMagicDefense = 109,
        /// <summary>
        /// 诀要-增减益内防比例
        /// </summary>
        [Description("诀要-增减益内防比例")]
        SecreIncreaseMagicDefenseRate = 110,
        /// <summary>
        /// 诀要-增减益固定伤害值
        /// </summary>
        [Description("诀要-增减益固定伤害值")]
        SecreIncreaseFixedDamage = 111,
        /// <summary>
        /// 诀要-增减益伤害比例
        /// </summary>
        [Description("诀要-增减益伤害比例")]
        SecreIncreaseDamageRate = 112,
        /// <summary>
        /// 诀要-增减益减伤比例
        /// </summary>
        [Description("诀要-增减益减伤比例")]
        SecreIncreaseHurtCutRate = 113,
        /// <summary>
        /// 诀要-增减益轻功点数
        /// </summary>
        [Description("诀要-增减益轻功点数")]
        SecreIncreaseDodge = 114,
        /// <summary>
        /// 诀要-中毒抵抗
        /// </summary>
        [Description("诀要-中毒抵抗")]
        SecreDrugResistance = 115,
        /// <summary>
        /// 诀要-缴械抵抗
        /// </summary>
        [Description("诀要-缴械抵抗")]
        SecreDisarmResistance = 116,
        /// <summary>
        /// 诀要-眩晕抵抗
        /// </summary>
        [Description("诀要-眩晕抵抗")]
        SecreVertigoResistance = 117,
        /// <summary>
        /// 诀要-定身抵抗
        /// </summary>
        [Description("诀要-定身抵抗")]
        SecreCanNotMoveResistance = 118,
        /// <summary>
        /// 诀要-迟缓抵抗
        /// </summary>
        [Description("诀要-迟缓抵抗")]
        SecreSlowResistance = 119,
        /// <summary>
        /// 诀要-混乱抵抗
        /// </summary>
        [Description("诀要-混乱抵抗")]
        SecreChaosResistance = 120,
        /// <summary>
        /// 诀要-惊慌抵抗
        /// </summary>
        [Description("诀要-惊慌抵抗")]
        SecreAlarmedResistance = 121,
        /// <summary>
        /// 诀要-减少所有侠客的武功招式CD时间
        /// </summary>
        [Description("诀要-减少所有侠客的武功招式CD时间")]
        SecreCutCD = 122,
        /// <summary>
        /// 诀要-抵御n次阵亡效果
        /// </summary>
        [Description("诀要-抵御n次阵亡效果")]
        SecreImmortal = 123,
        /// <summary>
        /// 诀要-n%概率秒杀敌方(对Boss无效)
        /// </summary>
        [Description("诀要-n%概率秒杀敌方(对Boss无效)")]
        SecreKilled = 124,
        /// <summary>
        /// 诀要-增加掉落概率n%
        /// </summary>
        [Description("诀要-增加掉落概率n%")]
        SecreMakeAFortune = 125,
        /// <summary>
        /// 诀要-增加气血恢复点数
        /// </summary>
        [Description("诀要-增加气血恢复点数")]
        SecrePlusIncreaseHP = 126
	}

	/// <summary>
	/// 场景中事件枚举类型
	/// </summary>
	public enum SceneEventType {
		/// <summary>
		/// 进入城镇
		/// </summary>
		[Description("进入城镇")]
		EnterCity = 0,
		/// <summary>
		/// 进入大地图
		/// </summary>
		[Description("进入大地图")]
		EnterArea = 1,
		/// <summary>
		/// 触发战斗
		/// </summary>
		[Description("触发战斗")]
		Battle = 2,
		/// <summary>
		/// 剧情对话
		/// </summary>
		[Description("剧情对话")]
		Dialog = 3,
		/// <summary>
		/// 触发任务
		/// </summary>
		[Description("触发任务")]
		Task = 4,
		/// <summary>
		/// 进入商店
		/// </summary>
		[Description("进入商店")]
		Store = 5,
		/// <summary>
		/// 掉落物
		/// </summary>
		[Description("掉落物")]
		Gift = 6,
		/// <summary>
		/// 采集点
		/// </summary>
		[Description("采集点")]
		Collection = 7,
		/// <summary>
		/// 驿站[花钱回复行动体力,并且可以雇佣马车传送到其他城镇]
		/// </summary>
		[Description("驿站[花钱回复行动体力,并且可以雇佣马车传送到其他城镇]")]
		Inn = 8,
		/// <summary>
		/// 酒馆[可能结交到伙伴]
		/// </summary>
		[Description("酒馆[可能结交到伙伴]")]
		WineShop = 9,
		/// <summary>
		/// 出生点事件[衔接大地图传送事件]
		/// </summary>
		[Description("出生点事件[衔接大地图传送事件]")]
		BirthPoint = 10,
		/// <summary>
		/// 禁用事件
		/// </summary>
		[Description("禁用事件[只针对静态事件]")]
		DisableEvent = 11,
		/// <summary>
		/// 开启干粮箱子
		/// </summary>
		/// </summary>
		[Description("开启干粮箱子")]
		EatFood = 12,
        /// <summary>
        /// 通天塔战斗
        /// </summary>
        [Description("通天塔战斗")]
        Pagoda = 13,
        /// <summary>
        /// 通天塔守卫禁用事件
        /// </summary>
        [Description("通天塔守卫禁用事件")]
        DisablePagoda = 14
	}

	/// <summary>
	/// 区域大地图事件开启条件类型
	/// </summary>
	public enum SceneEventOpenType {
		/// <summary>
		/// 无开启条件
		/// </summary>
		[Description("无开启条件")]
		None,
		/// <summary>
		/// 特定战斗胜利
		/// </summary>
		[Description("特定战斗胜利")]
		FightWined,
		/// <summary>
		/// 需要特定道具
		/// </summary>
		[Description("需要特定道具")]
		NeedItem,
		/// <summary>
		/// 完成特定任务
		/// </summary>
		[Description("完成特定任务")]
		NeedTasks
	}

	/// <summary>
	/// 阅历类型枚举
	/// </summary>
	public enum ExperienceType {
		/// <summary>
		/// 是否开启传送点
		/// </summary>
		[Description("是否开启传送点")]
		OpenedTransferPoint,
		/// <summary>
		/// 累计使用招式次数
		/// </summary>
		[Description("累计使用招式次数")]
		UsedSkillNums,
		/// <summary>
		/// 累计使用招式成功暴击次数
		/// </summary>
		[Description("累计使用招式成功暴击次数")]
		SuccessedWeaponPowerPlusNums,
		/// <summary>
		/// 曾经拥有过的金钱数
		/// </summary>
		[Description("曾经拥有过的金钱数")]
		TopMoney,
		/// <summary>
		/// 获得过的东西
		/// </summary>
		[Description("获得过的东西")]
		GotMadeItem,
		/// <summary>
		/// 招募到的伙伴
		/// </summary>
		[Description("招募到的伙伴")]
		RecruitedPartner,
		/// <summary>
		/// 道德点数区间
		/// </summary>
		[Description("道德点数区间")]
		MoralRange,
		/// <summary>
		/// 获得过的秘籍
		/// </summary>
		[Description("获得过的秘籍")]
		GotBookId,
		/// <summary>
		/// 某大区域大地图的探索度是否为100%
		/// </summary>
		[Description("某大区域大地图的探索度是否为100%")]
		TravelOverAreaName,
		/// <summary>
		/// 完成过的任务
		/// </summary>
		[Description("完成过的任务")]
		CompletedTaskId
	}

	/// <summary>
	/// 任务类型枚举
	/// </summary>
	public enum TaskType {
		/// <summary>
		/// 无接取限制
		/// </summary>
		[Description("无接取限制")]
		None,
		/// <summary>
		/// 根据时辰来判定是否可接取任务
		/// 索引顺序:["午时", "未时", "申时", "酉时", "戌时", "亥时", "子时", "丑时", "寅时", "卯时", "辰时", "巳时"]
		/// </summary>
		[Description("根据时辰来判定是否可接取任务")]
		TheHour,
		/// <summary>
		/// 根据主角性别来判定是否可接取任务
		/// </summary>
		[Description("根据主角性别来判定是否可接取任务")]
		Gender,
		/// <summary>
		/// 根据道德点数区间来判定是否可接取任务
		/// </summary>
		[Description("根据道德点数区间来判定是否可接取任务")]
		MoralRange,
		/// <summary>
		///  根据是否拥有特定道具来判定是否可接取任务
		/// </summary>
		[Description("根据是否拥有特定道具来判定是否可接取任务")]
		ItemInHand,
		/// <summary>
		/// 根据主角所属门派来判定是否可接取任务
		/// </summary>
		[Description("根据主角所属门派来判定是否可接取任务")]
		Occupation,
		/// <summary>
		/// 根据当前所处区域判定是否可接取任务
		/// </summary>
		[Description("根据当前所处区域判定是否可接取任务")]
		IsInArea,
		/// <summary>
		/// 绑定在野外区域中的事件上触发可接取的任务
		/// </summary>
		[Description("绑定在野外区域中的事件上触发可接取的任务")]
		IsBindedWithEvent
	}

	/// <summary>
	/// 任务步骤对话条件类型枚举
	/// </summary>
	public enum TaskDialogType {
		/// <summary>
		/// 普通谈话
		/// </summary>
		[Description("普通谈话")]
		JustTalk,
		/// <summary>
		/// 需要特定数量的物品(普通物品,任务物品,生活物品,击杀怪物的类型也做成掉落物品后再去交接任务)
		/// </summary>
		[Description("需要特定数量的物品")]
		SendItem,
		/// <summary>
		/// 护送Npc到指定场景
		/// </summary>
		[Description("护送Npc到指定场景")]
		ConvoyNpc,
		/// <summary>
		/// 是否使用过特定招式至少一次
		/// </summary>
		[Description("是否使用过特定招式至少一次")]
		UsedTheSkillOneTime,
		/// <summary>
		/// 是否成功使用招式暴击
		/// 招式暴击的比例可能有不同,用int值记录
		/// </summary>
		[Description("是否成功使用招式暴击")]
		WeaponPowerPlusSuccessed,
		/// <summary>
		/// 主角是否装备上特定武器
		/// </summary>
		[Description("主角是否装备上特定武器")]
		UsedTheWeapon,
		/// <summary>
		/// 主角是否装备上特定秘籍
		/// </summary>
		[Description("主角是否装备上特定秘籍")]
		UsedTheBook,
		/// <summary>
		/// 是否招募到特定伙伴
		/// </summary>
		[Description("是否招募到特定伙伴")]
		RecruitedThePartner,
		/// <summary>
		/// 特定战斗是否获胜
		/// </summary>
		[Description("特定战斗是否获胜")]
		FightWined,
		/// <summary>
		/// 抉择(使剧情产生分叉)
		/// </summary>
		[Description("抉择(使剧情产生分叉)")]
		Choice,
		/// <summary>
		/// 提示信息(用于显示步骤结果)
		/// </summary>
		[Description("提示信息(用于显示步骤结果)")]
		Notice,
		/// <summary>
		/// 区域大地图上动态生成的战斗事件是否获胜
		/// </summary>
		[Description("区域大地图上动态生成的战斗事件是否获胜")]
		EventFightWined,
		/// <summary>
		/// 需要特定数量的工坊资源
		/// </summary>
		[Description("需要特定数量的工坊资源")]
		SendResource,
		/// <summary>
		/// 当前时间是否为特定时辰
		/// </summary>
		[Description("当前时间是否为特定时辰")]
		TheHour,
		/// <summary>
		/// 往酒馆中添加一个非静态侠客
		/// </summary>
		[Description("往酒馆中添加一个非静态侠客")]
		PushRoleToWinshop,
		/// <summary>
		/// 创建一个绑定在区域地图上的事件上的任务
		/// </summary>
		[Description("创建一个绑定在区域地图上的事件上的任务")]
		CreateTaskIsBindedWithEvent
	}

	/// <summary>
	/// 用户当前位子状态枚举
	/// </summary>
	public enum UserPositionStatusType {
		/// <summary>
		/// 在城镇内
		/// </summary>
		InCity,
		/// <summary>
		/// 在大地图内
		/// </summary>
		InArea
	}

	/// <summary>
	/// 任务当前状态
	/// </summary>
	public enum TaskStateType {
		/// <summary>
		/// 条件未满足, 不可接取
		/// </summary>
		CanNotAccept = 0,
		/// <summary>
		/// 可接取
		/// </summary>
		CanAccept = 1,
		/// <summary>
		/// 已接取, 不能交付
		/// </summary>
		Accepted = 2,
		/// <summary>
		/// 已接取, 可以交付
		/// </summary>
		Ready = 3,
		/// <summary>
		/// 已完成
		/// </summary>
		Completed = 4
	}

	/// <summary>
	/// 任务步骤状态类型
	/// </summary>
	public enum TaskDialogStatusType {
		/// <summary>
		/// 初始状态
		/// </summary>
		Initial = 0,
		/// <summary>
		/// 等待执行
		/// </summary>
		HoldOn = 1,
		/// <summary>
		/// 已经完成(布尔是)
		/// </summary>
		ReadYes = 2,
		/// <summary>
		/// 已经完成(布尔非)
		/// </summary>
		ReadNo = 3
	}

	/// <summary>
	/// 侠客状态
	/// </summary>
	public enum RoleStateType {
		/// <summary>
		/// 未招募
		/// </summary>
		NotRecruited = 0,
		/// <summary>
		/// 正处于队伍中
		/// </summary>
		InTeam = 1,
		/// <summary>
		/// 替补中
		/// </summary>
		OutTeam = 2
	}

	/// <summary>
	/// 资源类型
	/// </summary>
    public enum ResourceType {
        /// <summary>
        /// 干粮
        /// </summary>
        [Description("干粮")]
        Food = 0,
		/// <summary>
		/// 小麦
		/// </summary>
		[Description("小麦")]
		Wheat = 1,
		/// <summary>
		/// 布料
		/// </summary>
        [Description("布料")]
		Cloth = 2,
		/// <summary>
		/// 木材
		/// </summary>
		[Description("木材")]
		Wood = 3,
		/// <summary>
		/// 铁
		/// </summary>
		[Description("铁")]
		Iron = 4,
		/// <summary>
		/// 银
		/// </summary>
		[Description("银")]
		SilverOre = 5,
		/// <summary>
		/// 钢
		/// </summary>
		[Description("钢")]
		Steel = 6,
		/// <summary>
		/// 银子
		/// </summary>
		[Description("银子")]
		Silver = 7,
		/// <summary>
		/// 钨
		/// </summary>
		[Description("钨")]
		Tungsten = 8,
		/// <summary>
		/// 玉
		/// </summary>
		[Description("玉")]
		Jade = 9,
		/// <summary>
		/// 赤铁
		/// </summary>
		[Description("赤铁")]
		RedSteel = 10,
		/// <summary>
		/// 百炼钢
		/// </summary>
		[Description("百炼钢")]
		RefinedSteel = 11,
		/// <summary>
		/// 钨钢
		/// </summary>
		[Description("钨钢")]
		TungstenSteel = 12,
		/// <summary>
		/// 乌金木
		/// </summary>
		[Description("乌金木")]
		Zingana = 13,
		/// <summary>
		/// 玄铁锭
		/// </summary>
		[Description("玄铁锭")]
		DarksteelIngot = 14
	}

	/// <summary>
	/// npc类型
	/// </summary>
	public enum NpcType {
		/// <summary>
		/// 普通npc
		/// </summary>
		[Description("普通npc")]
		Normal = 0,
		/// <summary>
		/// 战斗npc
		/// </summary>
		[Description("战斗npc")]
		Fight = 1,
        /// <summary>
        /// 任务达成npc
        /// </summary>
        [Description("任务达成npc")]
        AfterTask = 2
	}

	/// <summary>
	/// 秘籍状态类型
	/// </summary>
	public enum BookStateType {
		/// <summary>
		/// 还未获得
		/// </summary>
		Unread = 0,
		/// <summary>
		/// 已获得
		/// </summary>
		Read = 1
	}

    /// <summary>
    /// 兵器类型
    /// </summary>
    public enum WeaponType {
        /// <summary>
        /// 通用
        /// </summary>
        [Description("通用")]
        None = 0,
        /// <summary>
        /// 缠手
        /// </summary>
        [Description("缠手")]
        Glove = 1,
        /// <summary>
        /// 剑
        /// </summary>
        [Description("剑")]
        Sword = 2,
        /// <summary>
        /// 刀
        /// </summary>
        [Description("刀")]
        BroadSword = 3,
        /// <summary>
        /// 棍棒
        /// </summary>
        [Description("棍棒")]
        Stick = 4,
        /// <summary>
        /// 枪
        /// </summary>
        [Description("枪")]
        Spear = 5,
        /// <summary>
        /// 笛子
        /// </summary>
        [Description("笛子")]
        Flute = 6,
        /// <summary>
        /// 琴
        /// </summary>
        [Description("琴")]
        Zither = 7,
        /// <summary>
        /// 笔
        /// </summary>
        [Description("笔")]
        Pen = 8,
        /// <summary>
        /// 环刀
        /// </summary>
        [Description("环刀")]
        RingSword = 9,
        /// <summary>
        /// 暗器
        /// </summary>
        [Description("暗器")]
        Darts = 10,
        /// <summary>
        /// 鞭
        /// </summary>
        [Description("鞭")]
        Whip = 11,
        /// <summary>
        /// 扇子
        /// </summary>
        [Description("扇子")]
        Fan = 12,
        /// <summary>
        /// 特殊
        /// </summary>
        [Description("特殊")]
        Special = 13
    }

    /// <summary>
    /// 道具类型
    /// </summary>
    public enum PropType {
        /// <summary>
        /// 未知道具
        /// </summary>
        None = 0,
        /// <summary>
        /// 夜行衣
        /// </summary>
        NocturnalClothing = 1,
        /// <summary>
        /// 骏马
        /// </summary>
        Horse = 2,
        /// <summary>
        /// 镖师
        /// </summary>
        Bodyguard = 3,
        /// <summary>
        /// 石灰粉
        /// </summary>
        LimePowder = 4,
        /// <summary>
        /// 探子
        /// </summary>
        Scout = 5
    }
}
