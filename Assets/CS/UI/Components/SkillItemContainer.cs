using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class SkillItemContainer : MonoBehaviour {
		public Image SkillIconImage;
		public Text DescText;
		public Image FlagImage;

		SkillData skillData;
		bool showFlag;

		public void UpdateData(SkillData skill, bool flag = true) {
			skillData = skill;
			showFlag = flag;
		}

		public void RefreshView() {
			SkillIconImage.sprite = Statics.GetIconSprite(skillData.IconId);
			DescText.text = skillData.Desc;
			FlagImage.gameObject.SetActive(showFlag);
		}
	}
}
