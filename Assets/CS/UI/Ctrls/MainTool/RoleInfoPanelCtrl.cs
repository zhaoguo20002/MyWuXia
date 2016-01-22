using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DG.Tweening;

namespace Game {
	public class RoleInfoPanelCtrl : WindowCore<RoleInfoPanelCtrl, JArray> {
		List<RoleData> roleDataList;
		/// <summary>
		/// 队伍数据列表
		/// </summary>
		/// <value>The role datas.</value>
		public List<RoleData> RoleDatas {
			get {
				return roleDataList;
			} 
		}

		/// <summary>
		/// 当前战斗角色数据
		/// </summary>
		/// <value>The current role.</value>
		public RoleData CurrentRole {
			get {
				return roleDataList.Count > 0 ? roleDataList[0] : null;
			}
		}

		bool isFighting;
		List<Image> icons;
		List<Button> iconBtns;
		List<Image> cdMasks;
		List<Image> disableSeatMasks;
		List<Image> disableBookMasks;

		GameObject btnsObj;
		Button bagButton;
		Button booksButton;
		Button expButton;
		Button weaponsButton;

		GameObject booksObj;
		List<Image> books;
		List<Button> bookBtns;

		float changeRoleCD = 30;
		bool canChangeRole;
		bool canChangeBook;

		bool disabed;

		protected override void Init () {
			icons = new List<Image>() { 
				GetChildImage("icon0"),
				GetChildImage("icon1"),
				GetChildImage("icon2"),
				GetChildImage("icon3")
			};
			iconBtns = new List<Button>() {
				GetChildButton("icon0"),
				GetChildButton("icon1"),
				GetChildButton("icon2"),
				GetChildButton("icon3")
			};
			cdMasks = new List<Image>() {
				GetChildImage("cdMask0"),
				GetChildImage("cdMask1"),
				GetChildImage("cdMask2"),
				GetChildImage("cdMask3")
			};
			disableSeatMasks = new List<Image>() {
				GetChildImage("disableSeatMask0"),
				GetChildImage("disableSeatMask1"),
				GetChildImage("disableSeatMask2"),
				GetChildImage("disableSeatMask3")
			};
			disableBookMasks = new List<Image>() {
				GetChildImage("disableBookMask0"),
				GetChildImage("disableBookMask1"),
				GetChildImage("disableBookMask2")
			};
			for (int i = 0; i < iconBtns.Count; i++) {
				EventTriggerListener.Get(iconBtns[i].gameObject).onClick += onClick;
			}
			btnsObj = GetChild("btns");
			bagButton = GetChildButton("bagButton");
			EventTriggerListener.Get(bagButton.gameObject).onClick += onClick;
			booksButton = GetChildButton("booksButton");
			EventTriggerListener.Get(booksButton.gameObject).onClick += onClick;
			expButton = GetChildButton("expButton");
			EventTriggerListener.Get(expButton.gameObject).onClick += onClick;
			weaponsButton = GetChildButton("weaponsButton");
			EventTriggerListener.Get(weaponsButton.gameObject).onClick += onClick;

			booksObj = GetChild("books");
			books = new List<Image>() { 
				GetChildImage("book0"),
				GetChildImage("book1"),
				GetChildImage("book2")
			};
			bookBtns = new List<Button>() {
				GetChildButton("bookBtn0"),
				GetChildButton("bookBtn1"),
				GetChildButton("bookBtn2")
			};
			for (int i = 0; i < bookBtns.Count; i++) {
				EventTriggerListener.Get(bookBtns[i].gameObject).onClick += onClick;
			}
			disabed = false;
		}

		void onClick(GameObject e) {
			if (disabed || !e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
//			case "icon0":
//				CallInBattle(0);
//				break;
			case "icon1":
				if (isFighting && canChangeRole) {
					if (cdMasks[1].fillAmount <= 0) {
						cdMasks[1].fillAmount = 1;
						cdMasks[1].DOFillAmount(0, changeRoleCD);
						CallInBattle(1);
					}
				}
				break;
			case "icon2":
				if (isFighting && canChangeRole) {
					if (cdMasks[2].fillAmount <= 0) {
						cdMasks[2].fillAmount = 1;
						cdMasks[2].DOFillAmount(0, changeRoleCD);
						CallInBattle(2);
					}
				}
				break;
			case "icon3":
				if (isFighting && canChangeRole) {
					if (cdMasks[3].fillAmount <= 0) {
						cdMasks[3].fillAmount = 1;
						cdMasks[3].DOFillAmount(0, changeRoleCD);
						CallInBattle(3);
					}
				}
				break;

			case "bagButton":
				Debug.LogWarning("背包");
				break;
			case "booksButton":
				Debug.LogWarning("秘籍");
				break;
			case "expButton":
				Debug.LogWarning("阅历");
				break;
			case "weaponsButton":
				Debug.LogWarning("兵器");
				break;

			case "bookBtn0":
				if (isFighting && canChangeBook) {
//					ChangeButtonColor(bookBtns[CurrentRole.SelectedBookIndex], new Color(0.2f, 0.2f, 0.2f, 1));
//					ChangeButtonColorToDefault(bookBtns[0]);
					books[CurrentRole.SelectedBookIndex].transform.DOScale(0.8f, 0.2f);
					books[0].transform.DOScale(1, 0.2f);
					Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 0);
				}
				break;
			case "bookBtn1":
				if (isFighting && canChangeBook) {
//					ChangeButtonColor(bookBtns[CurrentRole.SelectedBookIndex], new Color(0.2f, 0.2f, 0.2f, 1));
//					ChangeButtonColorToDefault(bookBtns[1]);
					books[CurrentRole.SelectedBookIndex].transform.DOScale(0.8f, 0.2f);
					books[1].transform.DOScale(1, 0.2f);
					Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 1);
				}
				break;
			case "bookBtn2":
				if (isFighting && canChangeBook) {
//					ChangeButtonColor(bookBtns[CurrentRole.SelectedBookIndex], new Color(0.2f, 0.2f, 0.2f, 1));
//					ChangeButtonColorToDefault(bookBtns[2]);
					books[CurrentRole.SelectedBookIndex].transform.DOScale(0.8f, 0.2f);
					books[2].transform.DOScale(1, 0.2f);
					Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 2);
				}
				break;

			default:
				break;
			}
		}

		public void UpdateData (object obj, bool isfighting) {
			JArray data = (JArray)obj;
			roleDataList = new List<RoleData>(); 
			JArray itemData;
			for (int i = 0; i < data.Count; i++) {
				itemData = (JArray)data[i];
				roleDataList.Add(JsonManager.GetInstance().DeserializeObject<RoleData>(itemData[1].ToString()));
			}
			isFighting = isfighting;
			ChangeRoleEnable(true);
			ChangeBookEnable(true);
		}

		public void UpdateData(List<RoleData> roleDatas, bool isfighting) {
			roleDataList = roleDatas;
			isFighting = isfighting;
			ChangeRoleEnable(true);
			ChangeBookEnable(true);
			CallInBattle(0);
		}

		public override void RefreshView () {
			for (int i = 0; i < cdMasks.Count; i++) {
				cdMasks[i].DOKill();
				cdMasks[i].fillAmount = 0;
			}
			btnsObj.SetActive(!isFighting);
			booksObj.SetActive(isFighting);
		}

		public void ChangeRoleEnable(bool enable) {
			canChangeRole = enable;
			for(int i = 0; i < disableSeatMasks.Count; i++) {
				disableSeatMasks[i].gameObject.SetActive(!canChangeRole);
			}
		}

		public void ChangeBookEnable(bool enable) {
			canChangeBook = enable;
			for(int i = 0; i < disableBookMasks.Count; i++) {
				disableBookMasks[i].gameObject.SetActive(!canChangeBook);
			}
		}

		void refreshRoles() {
			RoleData roleData;
			Image icon;
			for (int i = 0; i < icons.Count; i++) {
				icon = icons[i];
				if (roleDataList.Count > i) {
					icon.gameObject.SetActive(true);
					roleData = roleDataList[i];
					icon.sprite = Statics.GetIconSprite(roleData.IconId);
				}
				else {
					icon.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// 呼叫角色战斗
		/// </summary>
		/// <param name="index">Index.</param>
		public void CallInBattle(int index) {
			if (index < 0 || index >= roleDataList.Count) {
				return;
			}
			//处理头像
			RoleData currentRole = roleDataList[index];
			roleDataList[index] = roleDataList[0];
			roleDataList[0] = currentRole;

			//处理书
			RoleData fightingRole = roleDataList[0];
			Image book;
			Button bookBtn;
			for (int i = 0; i < books.Count; i++) {
				book = books[i];
				if (fightingRole.Books != null && fightingRole.Books.Count > i) {
					book.gameObject.SetActive(true);
					book.sprite = Statics.GetIconSprite(fightingRole.Books[i].IconId);
					bookBtn = bookBtns[i];
					if (currentRole.SelectedBookIndex == i) {
//						ChangeButtonColorToDefault(bookBtn);
						book.transform.DOScale(1, 0.2f);
					}
					else {
//						ChangeButtonColor(bookBtn, new Color(0.8f, 0.8f, 0.2f, 1));
						book.transform.DOScale(0.8f, 0.5f);
					}
				}
				else {
					book.gameObject.SetActive(false);
				}
			}
			Messenger.Broadcast<RoleData>(NotifyTypes.ChangeCurrentTeamRoleInBattle, roleDataList[0]);
			refreshRoles();
		}

		/// <summary>
		/// 使界面交互失效
		/// </summary>
		/// <param name="dis">If set to <c>true</c> dis.</param>
		public void Disable(bool dis) {
			disabed = dis;
		}

		public static void Show(JArray data, bool isfighting = true) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/RoleInfoPanelView", "RoleInfoPanelCtrl", 0, -77.5f);
				Ctrl.MoveVertical(90 + 77.5f);
			}
			Ctrl.UpdateData(data, isfighting);
			Ctrl.RefreshView();
		}

		public static void Show(List<RoleData> roleDatas, bool isfighting = true) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/RoleInfoPanelView", "RoleInfoPanelCtrl", 0, -77.5f);
				Ctrl.MoveVertical(90 + 77.5f);
			}
			Ctrl.UpdateData(roleDatas, isfighting);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.MoveVertical(-(90 + 77.5f), () => {
					Ctrl.Close();
				});
			}
		}

		public static List<RoleData> GetRoleDatas() {
			if (Ctrl != null) {
				return Ctrl.RoleDatas;
			}
			return null;
		}

		public static RoleData GetCurrentRoleData() {
			if (Ctrl != null) {
				return Ctrl.CurrentRole;
			}
			return null;
		}

		public static void MakeChangeRoleEnable(bool enable) {
			if (Ctrl != null) {
				Ctrl.ChangeRoleEnable(enable);
			}
		}

		public static void MakeChangeBookEnable(bool enable) {
			if (Ctrl != null) {
				Ctrl.ChangeBookEnable(enable);
			}
		}

		public static void MakeDisable(bool dis) {
			if (Ctrl != null) {
				Ctrl.Disable(dis);
			}
		}
	}
}
