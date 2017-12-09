using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class WeaponData {
		/// <summary>
		/// 数据主键id
		/// </summary>
		public int PrimaryKeyId;
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// The name.
		/// </summary>
		public string Name;
		/// <summary>
		/// The desc.
		/// </summary>
		public string Desc;
		/// <summary>
		/// Icon图标id
		/// </summary>
		public string IconId;
        /// <summary>
        /// 兵器类型
        /// </summary>
        public WeaponType Type;
		/// <summary>
		/// 品质
		/// </summary>
		public QualityType Quality;
		/// <summary>
		/// 武器宽度
		/// </summary>
		public float Width;
		/// <summary>
		/// 武器威力增量倍率集合
		/// </summary>
		public float[] Rates;

		/// <summary>
		/// 正在使用该兵器的角色id
		/// </summary>
		public string BeUsingByRoleId;

		/// <summary>
		/// 开启城镇id
		/// </summary>
		public string BelongToCityId;

		/// <summary>
		/// 锻造兵器需要的材料
		/// </summary>
		public List<ResourceData> Needs;

		/// <summary>
		/// 外功增量
		/// </summary>
		public float PhysicsAttackPlus;

		/// <summary>
		/// 攻速增量
		/// </summary>
		public float AttackSpeedPlus;

		/// <summary>
		/// 固定伤害值增量
		/// </summary>
		public int FixedDamagePlus;

		/// <summary>
		/// 伤害比例增量[0-1]
		/// </summary>
		public float DamageRatePlus;
		/// <summary>
		/// 门派
		/// </summary>
		public OccupationType Occupation;
        /// <summary>
        /// 是否只能由主角装备(用于区分各自门派所对应需要打造的兵器)
        /// </summary>
        public bool JustBelongToHost;
        /// <summary>
        /// 专属于侠客的角色数据id
        /// </summary>
        public string BelongToRoleId;
        /// <summary>
        /// 兵器buff列表
        /// </summary>
        public List<WeaponBuffData> Buffs;
        /// <summary>
        /// 兵器强化等级
        /// </summary>
        public int LV;

		public WeaponData() {
			Desc = "";
			IconId = "";
            Type = WeaponType.Glove;
			Width = 100;
			Rates = new float[] { 1, 0, 0, 0 };
			BeUsingByRoleId = "";
			BelongToCityId = "";
			Needs = new List<ResourceData>();
			PhysicsAttackPlus = 0;
			AttackSpeedPlus = 0;
			FixedDamagePlus = 0;
			DamageRatePlus = 0;
            JustBelongToHost = false;
            BelongToRoleId = "";
		}

        public void Init(int lv) {
            LV = lv;
            Buffs = new List<WeaponBuffData>();
            switch (Id)
            {
                case "100106": //啸天狂龙
                    Buffs.Add(new WeaponBuffData("buff_100106_0", WeaponBuffType.PAUpWhenHPDown, 100, 0.01f, 0.05f + (LV * 0.05f)));
                    Buffs.Add(new WeaponBuffData("buff_100106_1", WeaponBuffType.ReboundInjuryWhenHPDown, 100, 0.1f + (LV * 0.1f)));
                    break;
                case "101106": //神威
                    Buffs.Add(new WeaponBuffData("buff_101106_0", WeaponBuffType.PAMultipleIncrease, 5 + (LV * 5f)));
                    break;
                case "102206": //承影
                    Buffs.Add(new WeaponBuffData("buff_102206_0", WeaponBuffType.MAMultipleIncreaseWhenBeMissed, 100, 1 + (LV * 0.1f), 10));
                    break;
                case "100206": //天谴
                    Buffs.Add(new WeaponBuffData("buff_100206_0", WeaponBuffType.BreachAttack, 100, 2 + (LV * 0.4f)));
                    break;
                case "104106": //伏虎
                    Buffs.Add(new WeaponBuffData("buff_104106_0", WeaponBuffType.InvincibleWall, 10 + (LV * 9f), 0, 0, 5 + (LV * 0.5f), 20));
                    break;
                case "105106": //清音
                    Buffs.Add(new WeaponBuffData("buff_105106_0", WeaponBuffType.AttackAbsorption, 10 + (LV * 9f), 0, 0, 0, 30));
                    break;
                default:
                    break;
            }
        }

        public string GetBuffDesc() {
            string desc = "";
            if (Buffs != null)
            {
                WeaponBuffData buff;
                for (int i = 0, len = Buffs.Count; i < len; i++)
                {
                    buff  = Buffs[i];
                    if (desc != "")
                    {
                        desc += ",";
                    }
                    switch (buff.Type)
                    {
                        case WeaponBuffType.AttackAbsorption:
                            desc += string.Format("{0}%概率触发攻击吸收气墙,气墙回血一次后消失,cd{1}秒", (int)((buff.Rate * 100d + 0.005d) / 100), buff.CDTime);
                            break;
                        case WeaponBuffType.BreachAttack:
                            desc += string.Format("对处于无视内功攻击状态下的敌人造成大幅伤害(基础内功提高{0}%)", ((buff.FloatValue0 * 10000d + 0.005d) / 100).ToString("0.0"));
                            break;
                        case WeaponBuffType.InvincibleWall:
                            desc += string.Format("{0}%概率触发无敌气墙,持续{1}秒,cd20秒", (int)((buff.Rate * 100d + 0.005d) / 100), buff.Timeout);
                            break;
                        case WeaponBuffType.MAMultipleIncreaseWhenBeMissed:
                            desc += string.Format("自身闪避后增加基础内功{0}%,最高叠加至{1}%,命中敌人后内功叠加消失", ((buff.FloatValue0 * 10000d + 0.005d) / 100).ToString("0.0"), ((buff.FloatValue1 * 10000d + 0.005d) / 100).ToString("0.0"));
                            break;
                        case WeaponBuffType.PAMultipleIncrease:
                            desc += string.Format("每次攻击{0}%概率基础外功增加100%,最高叠加至500%", (int)((buff.Rate * 100d + 0.005d) / 100));
                            break;
                        case WeaponBuffType.PAUpWhenHPDown:
                            desc += string.Format("气血每降低{0}%基础外功增加{1}%", ((buff.FloatValue0 * 10000d + 0.005d) / 100).ToString("0.0"), ((buff.FloatValue1 * 10000d + 0.005d) / 100).ToString("0.0"));
                            break;
                        case WeaponBuffType.ReboundInjuryWhenHPDown:
                            desc += string.Format("生命低于30%时附加反伤{0}%伤害效果", ((buff.FloatValue0 * 10000d + 0.005d) / 100).ToString("0.0"));
                            break;
                    }
                }
            }
            return desc;
        }
	}
}
