using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public class RoleInfoPanelCtrl : WindowCore<RoleInfoPanelCtrl, JArray> {
		List<RoleData> roleDataList;
		bool isFighting;
		List<Image> icons;
		List<Button> iconBtns;

		GameObject btnsObj;
		Button bagButton;
		Button booksButton;
		Button expButton;
		Button weaponsButton;

		GameObject booksObj;
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
			case "icon0":
				Debug.LogWarning("出战角色");
				break;
			case "icon1":
				Debug.LogWarning("1号待机角色");
				break;
			case "icon2":
				Debug.LogWarning("2号待机角色");
				break;
			case "icon3":
				Debug.LogWarning("3号待机角色");
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
				Debug.LogWarning("第1本书");
				break;
			case "book1":
				Debug.LogWarning("第2本书");
				break;
			case "book2":
				Debug.LogWarning("第3本书");
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
			RoleData roleData;
			for (int i = 0; i < roleDataList.Count; i++) {
				roleData = roleDataList[i];
			}
			btnsObj.SetActive(!isFighting);
			booksObj.SetActive(isFighting);
		}

		public static void Show(JArray data) {
			InstantiateView("Prefabs/UI/RoleInfoPanelView", "RoleInfoPanelCtrl", 0, -77.5f);
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
			Ctrl.MoveVertical(90 + 77.5f);
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.MoveVertical(-(90 + 77.5f), () => {
					Ctrl.Close();
				});
			}
		}
	}
}
