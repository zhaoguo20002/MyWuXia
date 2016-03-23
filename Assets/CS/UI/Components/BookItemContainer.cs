using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BookItemContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Button UseBtn;
		public Button UnuseBtn;

		Image bg;

		BookData bookData;
		void Awake() {
			bg = GetComponent<Image>();
		}

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(UseBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(UnuseBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "UseBtn":
				Messenger.Broadcast<int>(NotifyTypes.UseBook, bookData.PrimaryKeyId);
				break;
			case "UnuseBtn":
				Messenger.Broadcast<int>(NotifyTypes.UnuseBook, bookData.PrimaryKeyId);
				break;
			default:
				break;
			}
		}

		public void UpdateData(BookData book) {
			bookData = book;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(bookData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
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
