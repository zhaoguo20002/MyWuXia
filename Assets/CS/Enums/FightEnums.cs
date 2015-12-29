using UnityEngine;
using System.Collections;

namespace Game {
	/// <summary>
	/// 门派类型
	/// </summary>
	public enum OccupationType {
		/// <summary>
		/// 无门无派
		/// </summary>
		None,
		/// <summary>
		/// 丐帮
		/// </summary>
		GaiBang,
		/// <summary>
		/// 少林
		/// </summary>
		ShaoLin,
		/// <summary>
		/// 全真
		/// </summary>
		QuanZhen,
		/// <summary>
		/// 逍遥
		/// </summary>
		XiaoYao,
		/// <summary>
		/// 大理
		/// </summary>
		DaLi,
		/// <summary>
		/// 岳家军
		/// </summary>
		YueJiaJun,
		/// <summary>
		/// 神兵营
		/// </summary>
		ShenBingYing,
		/// <summary>
		/// 唐门
		/// </summary>
		TangMen
	}

	/// <summary>
	/// 伤势类型
	/// </summary>
	public enum InjuryType {
		/// <summary>
		/// 健康
		/// </summary>
		None,
		/// <summary>
		/// 白伤(全属性降低10%)
		/// </summary>
		White,
		/// <summary>
		/// 黄伤(全属性降低20%)
		/// </summary>
		Yellow,
		/// <summary>
		/// 紫伤(全属性降低40%)
		/// </summary>
		Purple,
		/// <summary>
		/// 红伤(全属性降低80%)
		/// </summary>
		Red,
		/// <summary>
		/// 垂死(不能出战)
		/// </summary>
		Moribund
	}

	/// <summary>
	/// buff类型(buff/debuff通用)
	/// </summary>
	public enum BuffType {
		/// <summary>
		/// 无
		/// </summary>
		Normal,
		/// <summary>
		/// 增减益气血
		/// </summary>
		IncreaseHP,
		/// <summary>
		/// 增减益外功点数
		/// </summary>
		IncreasePhysicsAttack,
		/// <summary>
		/// 增减益外功比例
		/// </summary>
		IncreasePhysicsAttackRate,
		/// <summary>
		/// 增减益内功点数
		/// </summary>
		IncreaseMagicAttack,
		/// <summary>
		/// 增减益内功比例
		/// </summary>
		IncreaseMagicAttackRate,
		/// <summary>
		/// 增减益固定伤害值
		/// </summary>
		IncreaseFixedDamage,
		/// <summary>
		/// 增减益内功比例
		/// </summary>
		IncreaseDamageRate,
		/// <summary>
		/// 增减益减伤比例
		/// </summary>
		IncreaseHurtCutRate,
		/// <summary>
		/// 灼烧(持续掉血)
		/// </summary>
		Burn,
		/// <summary>
		/// 缴械(不能释放技能，可以切换门客)
		/// </summary>
		Disarm,
		/// <summary>
		/// 眩晕(当靶子)
		/// </summary>
		Vertigo,
		/// <summary>
		/// 定身(可以释放技能，不能切换门客)
		/// </summary>
		CanNotMove,
		/// <summary>
		/// 减速%(攻速降低)
		/// </summary>
		Slow,
		/// <summary>
		/// 混乱(自己出招有50%概率攻击自己)
		/// </summary>
		Chaos
	}

	public enum SkillType {
		/// <summary>
		/// 对己方增益
		/// </summary>
		Plus,
		/// <summary>
		/// 外功伤害
		/// </summary>
		PhysicsAttack,
		/// <summary>
		/// 内功伤害
		/// </summary>
		MagicAttack,
		/// <summary>
		/// 固定伤害
		/// </summary>
		FixedDamage
	}

	public enum FightType {
		/// <summary>
		/// 普通战斗(普通流程出发的战斗,如野外踩地雷或野外手动触发的怪物战斗等)
		/// </summary>
		Normal,
		/// <summary>
		/// 任务战斗(任务的其中一环战斗)
		/// </summary>
		Task
	}
}
