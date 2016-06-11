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
			Image iconImage;
			for (int i = 0; i < SkillIcons.Length; i++) {
				iconImage = SkillIcons[i];
				if (bookData.Skills.Count > i) {
					iconImage.gameObject.SetActive(true);
					iconImage.sprite = Statics.GetIconSprite(bookData.Skills[i].IconId);
				}
				else {
					iconImage.gameObject.SetActive(false);
				}
			}
			DescText.gameObject.SetActive(bookData.Skills.Count == 0);
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
