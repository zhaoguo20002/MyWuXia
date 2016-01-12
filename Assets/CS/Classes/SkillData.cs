using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class SkillData {
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// The name.
		/// </summary>
		public string Name;
		/// <summary>
		/// 技能类型
		/// </summary>
		public SkillType Type;
		/// <summary>
		/// 图标Id
		/// </summary>
		public string IconId;
		/// <summary>
		/// The buff datas.
		/// </summary>
		public List<BuffData> BuffDatas;
		/// <summary>
		/// The de buff datas.
		/// </summary>
		public List<BuffData> DeBuffDatas;
		/// <summary>
		/// 发招概率[0-100]
		/// </summary>
		public float Rate;
		/// <summary>
		/// 额外招式
		/// </summary>
		public List<SkillData> AddedSkillDatas;

		public SkillData() {
			IconId = "";
			BuffDatas = new List<BuffData>();
			DeBuffDatas = new List<BuffData>();
			AddedSkillDatas = new List<SkillData>();
			Rate = 100;
		}
	}
}
