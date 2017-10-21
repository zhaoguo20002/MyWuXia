using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BookOfForbiddenAreaContainer : ComponentCore {
        public Image Icon;
        public Image FlashImage;
		public Image NewFlag;
		public Text Name;
		public Image Flag;
		public Button Btn;
		public Button MakeBtn;

		BookData bookData;
		RoleData hostRoleData;
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
            switch (e.name)
            {
                case "MakeBtn":
                    if (bookData.Occupation == OccupationType.None || bookData.Occupation == hostRoleData.Occupation)
                    {
                        if (bookData.State == BookStateType.Unread)
                        {
                            string noticeMsg = costStr != "" ? string.Format("是否将{0}拼合成<color=\"{1}\">{2}</color>进行研读？", costStr, Statics.GetQualityColorString(bookData.Quality), bookData.Name) : string.Format("是否研读<color=\"{0}\">{1}</color>？", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
                            ConfirmCtrl.Show(noticeMsg, () =>
                            {
                                Messenger.Broadcast<int>(NotifyTypes.ReadBook, bookData.PrimaryKeyId);
                            });
                        }
                        else
                        {
                            AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>已经研读过", Statics.GetQualityColorString(bookData.Quality), bookData.Name), null);
                        }
                    }
                    else
                    {
                        AlertCtrl.Show(string.Format("非{0}弟子不得研习<color=\"{1}\">{2}</color>!", Statics.GetOccupationName(bookData.Occupation), Statics.GetQualityColorString(bookData.Quality), bookData.Name));
                    }
                    break;
                case "Btn":
                    Messenger.Broadcast<BookData>(NotifyTypes.ShowBookDetailPanel, bookData);
                    break;
                default:
                    break;
            }
			viewedNewFlag();
		}

		void viewedNewFlag() {
			if (NewFlag.gameObject.activeSelf) {
				PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "BookIdOfCurrentForbiddenAreaNewFlagIsHide_" + bookData.Id, "true"); //让新增提示消失
				NewFlag.gameObject.SetActive(false);
			}
		}
		
		public void UpdateData(BookData book, RoleData host) {
			bookData = book;
			hostRoleData = host;
			costStr = "";
			CostData cost;
			for (int i = 0; i < bookData.Needs.Count; i++) {
				cost = bookData.Needs[i];
				costStr += string.Format("{0}张{1} ", cost.Num, JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", cost.Id).Name);
			}
		}
		
		public void RefreshView() {
            Icon.sprite = Statics.GetIconSprite(bookData.IconId);
            FlashImage.gameObject.SetActive(((int)bookData.Quality) >= ((int)QualityType.FlashGold));
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
			Flag.gameObject.SetActive(bookData.State == BookStateType.Read);
			MakeButtonEnable(MakeBtn, bookData.State == BookStateType.Unread);
			//判断是否为新增秘籍，控制新增标记显示隐藏
			NewFlag.gameObject.SetActive(string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "BookIdOfCurrentForbiddenAreaNewFlagIsHide_" + bookData.Id)));
		}
		
	}
}
