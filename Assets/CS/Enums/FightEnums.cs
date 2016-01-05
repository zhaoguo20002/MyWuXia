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
		Normal = 0,
		/// <summary>
		/// 增减益气血
		/// </summary>
		IncreaseHP = 1,
		/// <summary>
		/// 增减益气血上限
		/// </summary>
		IncreaseMaxHP = 2,
		/// <summary>
		/// 增减益气血上限比例
		/// </summary>
		IncreaseMaxHPRate = 3,
		/// <summary>
		/// 增减益外功点数
		/// </summary>
		IncreasePhysicsAttack = 4,
		/// <summary>
		/// 增减益外功比例
		/// </summary>
		IncreasePhysicsAttackRate = 5,
		/// <summary>
		/// 增减益外防点数
		/// </summary>
		IncreasePhysicsDefense = 6,
		/// <summary>
		/// 增减益外防比例
		/// </summary>
		IncreasePhysicsDefenseRate = 7,
		/// <summary>
		/// 增减益内功点数
		/// </summary>
		IncreaseMagicAttack = 8,
		/// <summary>
		/// 增减益内功比例
		/// </summary>
		IncreaseMagicAttackRate = 9,
		/// <summary>
		/// 增减益内防点数
		/// </summary>
		IncreaseMagicDefense = 10,
		/// <summary>
		/// 增减益内防比例
		/// </summary>
		IncreaseMagicDefenseRate = 11,
		/// <summary>
		/// 增减益固定伤害值
		/// </summary>
		IncreaseFixedDamage = 12,
		/// <summary>
		/// 增减益伤害比例
		/// </summary>
		IncreaseDamageRate = 13,
		/// <summary>
		/// 增减益减伤比例
		/// </summary>
		IncreaseHurtCutRate = 14,
		/// <summary>
		/// 中毒(持续掉血10%)
		/// </summary>
		Drug = 15,
		/// <summary>
		/// 缴械(不能释放技能，可以切换门客)
		/// </summary>
		Disarm = 16,
		/// <summary>
		/// 眩晕(当靶子)
		/// </summary>
		Vertigo = 17,
		/// <summary>
		/// 定身(可以释放技能，不能切换门客)
		/// </summary>
		CanNotMove = 18,
		/// <summary>
		/// 迟缓(减速%攻速降低)
		/// </summary>
		Slow = 19,
		/// <summary>
		/// 混乱(自己出招有50%概率攻击自己)
		/// </summary>
		Chaos = 20,
		/// <summary>
		/// 疾走(加速%攻速提高)
		/// </summary>
		Fast = 21
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
