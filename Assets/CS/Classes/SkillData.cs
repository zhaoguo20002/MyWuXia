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
		/// 额外招式索引Id集合
		/// </summary>
		public List<string> ResourceAddedSkillIds;
		/// <summary>
		/// 额外招式
		/// </summary>
		public List<SkillData> AddedSkillDatas;

		public SkillData() {
			IconId = "";
			BuffDatas = new List<BuffData>();
			DeBuffDatas = new List<BuffData>();
			ResourceAddedSkillIds = new List<string>();
			AddedSkillDatas = new List<SkillData>();
			Rate = 100;
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			AddedSkillDatas.Clear();
			for (int i = 0; i < ResourceAddedSkillIds.Count; i++) {
				AddedSkillDatas.Add(JsonManager.GetInstance().GetMapping<SkillData>("Skills", ResourceAddedSkillIds[i]));
			}
		}

		/// <summary>
		/// 将招式的各项属性翻译成文字显示出来
		/// </summary>
		public string GetSkillDesc() {
			string result = "将招式的各项属性翻译成文字显示出来";
			return result;
		}
	}
}
