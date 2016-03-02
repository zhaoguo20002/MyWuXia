using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class GotSkills : MonoBehaviour {
		public Image[] SkillIconImages;
		List<string> skillIconIds;
		// Use this for initialization
		void Awake () {
			Clear();
		}
		
		/// <summary>
		/// 清除
		/// </summary>
		public void Clear() {
			if (SkillIconImages == null) {
				return;
			}
			for (int i = 0; i < SkillIconImages.Length; i++) {
				SkillIconImages[i].gameObject.SetActive(false);
			}
		}
		
		/// <summary>
		/// 设置Icon图标
		/// </summary>
		/// <param name="ids">Identifiers.</param>
		public void SetIconIds(List<string> ids) {
			skillIconIds = ids;
			for (int i = 0; i < SkillIconImages.Length; i++) {
				if (skillIconIds.Count > i && skillIconIds[i] != null) {
					SkillIconImages[i].sprite = Statics.GetIconSprite(skillIconIds[i]);
				}
			}
		}
		
		public void Pop(int index) {
			if (index < 0 || SkillIconImages == null || SkillIconImages.Length <= index) {
				return;
			}
			SkillIconImages[index].gameObject.SetActive(true);
		}
	}
}
