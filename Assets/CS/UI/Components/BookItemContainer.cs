using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BookItemContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Image[] SkillIcons;
		public Text DescText;
		public Button UseBtn;
		public Button UnuseBtn;
		public Button ViewBtn;

		Image bg;

		BookData bookData;
		void Awake() {
			bg = GetComponent<Image>();
		}

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(UseBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(UnuseBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(ViewBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "UseBtn":
				Messenger.Broadcast<int>(NotifyTypes.UseBook, bookData.PrimaryKeyId);
				break;
			case "UnuseBtn":
				Messenger.Broadcast<int>(NotifyTypes.UnuseBook, bookData.PrimaryKeyId);
				break;
			case "ViewBtn":
				Messenger.Broadcast<BookData>(NotifyTypes.ShowBookDetailPanel, bookData);
				break;
			default:
				break;
			}
		}

		public void UpdateData(BookData book) {
			bookData = book;
			bookData.MakeJsonToModel();
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(bookData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
//			Image iconImage;
//			for (int i = 0; i < SkillIcons.Length; i++) {
//				iconImage = SkillIcons[i];
//				if (bookData.Skills.Count > i) {
//					iconImage.gameObject.SetActive(true);
//					iconImage.sprite = Statics.GetIconSprite(bookData.Skills[i].IconId);
//				}
//				else {
//					iconImage.gameObject.SetActive(false);
//				}
//			}
//			DescText.gameObject.SetActive(bookData.Skills.Count == 0);
            string attrsStr = "";
            if (bookData.MagicAttackPlus > 0)
            {
                attrsStr += string.Format("内功+{0} ", bookData.MagicAttackPlus);
            }
            if (bookData.MagicDefensePlus > 0)
            {
                attrsStr += string.Format("内防+{0} ", bookData.MagicDefensePlus);
            }
            if (bookData.PhysicsDefensePlus > 0)
            {
                attrsStr += string.Format("外防+{0} ", bookData.PhysicsDefensePlus);
            }
            if (bookData.DodgePlus > 0)
            {
                attrsStr += string.Format("轻功+{0} ", bookData.DodgePlus);
            }
            if (bookData.HurtCutRatePlus > 0)
            {
                attrsStr += string.Format("伤害减免{0}% ", (int)(bookData.HurtCutRatePlus * 100 + 0.5f));
            }
            if (bookData.MaxHPPlus > 0)
            {
                attrsStr += string.Format("气血+{0} ", bookData.MaxHPPlus);
            }
            if (bookData.CanNotMoveResistance > 0)
            {
                attrsStr += string.Format("抗定身+{0} ", bookData.CanNotMoveResistance);
            }
            if (bookData.ChaosResistance > 0)
            {
                attrsStr += string.Format("抗混乱+{0} ", bookData.ChaosResistance);
            }
            if (bookData.DisarmResistance > 0)
            {
                attrsStr += string.Format("抗缴械+{0} ", bookData.DisarmResistance);
            }
            if (bookData.DrugResistance > 0)
            {
                attrsStr += string.Format("抗毒+{0} ", bookData.DrugResistance);
            }
            if (bookData.SlowResistance > 0)
            {
                attrsStr += string.Format("抗迟缓+{0} ", bookData.SlowResistance);
            }
            if (bookData.VertigoResistance > 0)
            {
                attrsStr += string.Format("抗眩晕+{0} ", bookData.VertigoResistance);
            }
            DescText.text = string.Format("{0}\n{1}", bookData.GetCurrentSkill().Desc.Replace(" ", ""), "");
			if (bookData.BeUsingByRoleId != "") {
				UseBtn.gameObject.SetActive(false);
				UnuseBtn.gameObject.SetActive(true);
				bg.sprite = Statics.GetSprite("Border12");
			}
			else {
				UseBtn.gameObject.SetActive(true);
				UnuseBtn.gameObject.SetActive(false);
				bg.sprite = Statics.GetSprite("Border11");
			}
		}

	}
}
