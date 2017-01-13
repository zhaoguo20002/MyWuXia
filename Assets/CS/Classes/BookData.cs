using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class BookData {
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
		/// 引用的技能id集合
		/// </summary>
		public List<string> ResourceSkillDataIds;
		/// <summary>
		/// 武功招式集合
		/// </summary>
		public List<SkillData> Skills;
		/// <summary>
		/// Icon Id
		/// </summary>
		public string IconId;
		/// <summary>
		/// 品质
		/// </summary>
		public QualityType Quality;

		int currentSkillIndex;
		/// <summary>
		/// 获取当前技能索引
		/// </summary>
		/// <value>The index of the current skill.</value>
		public int CurrentSkillIndex {
			get {
				return currentSkillIndex;
			}
				
		}

		/// <summary>
		/// 正在使用该秘籍的角色id
		/// </summary>
		public string BeUsingByRoleId;

		/// <summary>
		/// 开启城镇id
		/// </summary>
		public string BelongToCityId;

		/// <summary>
		/// 所需的秘籍残卷
		/// </summary>
		public List<CostData> Needs;

		/// <summary>
		/// 最大气血增量
		/// </summary>
		public int MaxHPPlus;

		/// <summary>
		/// 外防增量
		/// </summary>
		public float PhysicsDefensePlus;

		/// <summary>
		/// 内功增量
		/// </summary>
		public float MagicAttackPlus;

		/// <summary>
		/// 内防增量
		/// </summary>
		public float MagicDefensePlus;

		/// <summary>
		/// 减伤比例增量[0-1]
		/// </summary>
		public float HurtCutRatePlus;

		/// <summary>
		/// 轻功增量
		/// </summary>
        public float DodgePlus;

        /// <summary>
        /// 中毒抵抗
        /// </summary>
        public int DrugResistance;
        /// <summary>
        /// 缴械抵抗
        /// </summary>
        public int DisarmResistance;
        /// <summary>
        /// 眩晕抵抗
        /// </summary>
        public int VertigoResistance;
        /// <summary>
        /// 定身抵抗
        /// </summary>
        public int CanNotMoveResistance;
        /// <summary>
        /// 迟缓抵抗
        /// </summary>
        public int SlowResistance;
        /// <summary>
        /// 混乱抵抗
        /// </summary>
        public int ChaosResistance;

		/// <summary>
		/// 秘籍状态
		/// </summary>
		public BookStateType State;
		/// <summary>
		/// 门派
		/// </summary>
		public OccupationType Occupation;
		/// <summary>
		/// 是否为心法(心法只加属性不能在战斗中切换)
		/// </summary>
		public bool IsMindBook;
        /// <summary>
        /// 只限装备某种类型的兵器时才能使用秘籍
        /// </summary>
        public WeaponType LimitWeaponType;

		public BookData() {
			ResourceSkillDataIds = new List<string>();
			Skills = new List<SkillData>();
			Desc = "";
			IconId = "";
			currentSkillIndex = 0;
			BeUsingByRoleId = "";
			BelongToCityId = "";
			Needs = new List<CostData>();
			MaxHPPlus = 0;
			PhysicsDefensePlus = 0;
			MagicAttackPlus = 0;
			MagicDefensePlus = 0;
			HurtCutRatePlus = 0;
            DodgePlus = 0;
            DrugResistance = 0;
            DisarmResistance = 0;
            VertigoResistance = 0;
            CanNotMoveResistance = 0;
            SlowResistance = 0;
            ChaosResistance = 0;
			IsMindBook = false;
            LimitWeaponType = WeaponType.None;
		}

		/// <summary>
		/// 获取当前技能
		/// </summary>
		/// <returns>The current skill data.</returns>
		public SkillData GetCurrentSkill() {
			if (Skills == null || Skills.Count == 0) {
				return null;
			}
//			return Skills[currentSkillIndex].GetRealSkill();
            return Skills[currentSkillIndex];
		}

		/// <summary>
		/// 使用下一个技能
		/// </summary>
		/// <returns>The skill.</returns>
		public SkillData NextSkill() {
			if (Skills == null || Skills.Count == 0) {
				return null;
			}
			currentSkillIndex++;
			currentSkillIndex %= Skills.Count;
			return Skills[currentSkillIndex];
		}

		/// <summary>
		/// 重头来
		/// </summary>
		public SkillData Restart() {
			if (Skills == null || Skills.Count == 0) {
				return null;
			}
			currentSkillIndex = 0;
			return Skills[currentSkillIndex];
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Skills.Clear();
			SkillData skill;
			for (int i = 0; i < ResourceSkillDataIds.Count; i++) {
				skill = JsonManager.GetInstance().GetMapping<SkillData>("Skills", ResourceSkillDataIds[i]);
				skill.MakeJsonToModel();
				Skills.Add(skill);
			}
		}

		/// <summary>
		/// 获取当前秘籍的技能Icon Id列表
		/// </summary>
		/// <returns>The skill icon identifiers.</returns>
		public List<string> GetSkillIconIds() {
			List<string> ids = new List<string>();
			for (int i = 0; i < Skills.Count; i++) {
				ids.Add(Skills[i].IconId);
			}
			return ids;
		}
	}
}