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
	}
}
