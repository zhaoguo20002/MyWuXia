using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

		GameObject btnsObj;
		Button bagButton;
		Button booksButton;
		Button expButton;
		Button weaponsButton;

		GameObject booksObj;
		List<Image> books;
		List<Button> bookBtns;

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
				GetChildButton("book0"),
				GetChildButton("book1"),
				GetChildButton("book2")
			};
			for (int i = 0; i < bookBtns.Count; i++) {
				EventTriggerListener.Get(bookBtns[i].gameObject).onClick += onClick;
			}

			MakeButtonEnable(bookBtns[2], false);
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
//			case "icon0":
//				CallInBattle(0);
//				break;
			case "icon1":
				CallInBattle(1);
				break;
			case "icon2":
				CallInBattle(2);
				break;
			case "icon3":
				CallInBattle(3);
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

			case "book0":
				Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 0);
				break;
			case "book1":
				Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 1);
				break;
			case "book2":
				Messenger.Broadcast<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, 2);
				break;

			default:
				break;
			}
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
			roleDataList = new List<RoleData>(); 
			JArray itemData;
			for (int i = 0; i < data.Count; i++) {
				itemData = (JArray)data[i];
				roleDataList.Add(JsonManager.GetInstance().DeserializeObject<RoleData>(itemData[1].ToString()));
			}
			isFighting = true;
		}

		public override void RefreshView () {
			btnsObj.SetActive(!isFighting);
			booksObj.SetActive(isFighting);
			CallInBattle(0);
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
				}
				else {
					book.gameObject.SetActive(false);
				}
			}
			Messenger.Broadcast<RoleData>(NotifyTypes.ChangeCurrentTeamRoleInBattle, roleDataList[0]);
			refreshRoles();
		}

		public static void Show(JArray data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/RoleInfoPanelView", "RoleInfoPanelCtrl", 0, -77.5f);
				Ctrl.MoveVertical(90 + 77.5f);
			}
			Ctrl.UpdateData(data);
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
	}
}
