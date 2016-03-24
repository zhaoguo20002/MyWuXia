using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BookOfForbiddenAreaContainer : ComponentCore {
		public Image Icon;
		public Text Name;
		public Image Flag;
		public Button Btn;
		public Button MakeBtn;

		BookData bookData;
		string costStr;

		// Use this for initialization
		void Start () {
			if (Icon == null || Name == null || Flag == null || Btn == null) {
				enabled = false;
			}
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
			EventTriggerListener.Get(MakeBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
			case "MakeBtn":
				if (bookData.State == BookStateType.Unread) {
					ConfirmCtrl.Show(string.Format("是否将{0}拼合成<color=\"{1}\">{2}</color>进行研读？", costStr, Statics.GetQualityColorString(bookData.Quality), bookData.Name), () => {
						Messenger.Broadcast<int>(NotifyTypes.ReadBook, bookData.PrimaryKeyId);
					});
				}
				else {
					AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>已经研读过", Statics.GetQualityColorString(bookData.Quality), bookData.Name), null);
				}
				break;
			case "Btn":
				Debug.LogWarning("查看");
				break;
			default:
				break;
			}
		}
		
		public void UpdateData(BookData book) {
			bookData = book;
			costStr = "";
			CostData cost;
			for (int i = 0; i < bookData.Needs.Count; i++) {
				cost = bookData.Needs[i];
				costStr += string.Format("{0}张{1} ", cost.Num, JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", cost.Id).Name);
			}
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(bookData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
			Flag.gameObject.SetActive(bookData.State == BookStateType.Read);
			MakeButtonEnable(MakeBtn, bookData.State == BookStateType.Unread);
		}
		
	}
}
