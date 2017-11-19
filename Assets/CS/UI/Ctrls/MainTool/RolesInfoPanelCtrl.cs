using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DG.Tweening;

namespace Game {
    public class RolesInfoPanelCtrl : WindowCore<RolesInfoPanelCtrl, JArray> {
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

		List<Image> icons;
		List<Button> iconBtns;
		List<Image> injuryImages;

		Button bagButton;
		Button booksButton;
		Button weaponsButton;
        Image weaponsRedPointImage;
        Image booksRedPointImage;
        Image bagRedPointImage;

		GameObject booksObj;
		List<Image> books;
		List<Button> bookBtns;
		List<Image> mindBookMasks;

		float changeRoleCD = 30;
		bool canChangeRole;
		bool canChangeBook;

		bool disabed;
        int viewedRoleIndex = -1;

		protected override void Init () {
			icons = new List<Image>() { 
				GetChildImage("icon0"),
				GetChildImage("icon1"),
				GetChildImage("icon2"),
                GetChildImage("icon3"),
                GetChildImage("icon4"),
                GetChildImage("icon5")
			};
			iconBtns = new List<Button>() {
				GetChildButton("icon0"),
				GetChildButton("icon1"),
				GetChildButton("icon2"),
                GetChildButton("icon3"),
                GetChildButton("icon4"),
                GetChildButton("icon5")
			};
			injuryImages = new List<Image>() {
				GetChildImage("injuryImage0"),
				GetChildImage("injuryImage1"),
				GetChildImage("injuryImage2"),
                GetChildImage("injuryImage3"),
                GetChildImage("injuryImage4"),
                GetChildImage("injuryImage5")
			};
			for (int i = 0; i < iconBtns.Count; i++) {
				EventTriggerListener.Get(iconBtns[i].gameObject).onClick += onClick;
			}
			bagButton = GetChildButton("bagButton");
			EventTriggerListener.Get(bagButton.gameObject).onClick += onClick;
			booksButton = GetChildButton("booksButton");
			EventTriggerListener.Get(booksButton.gameObject).onClick += onClick;
			weaponsButton = GetChildButton("weaponsButton");
			EventTriggerListener.Get(weaponsButton.gameObject).onClick += onClick;
            weaponsRedPointImage = GetChildImage("weaponsRedPointImage");
            booksRedPointImage = GetChildImage("booksRedPointImage");
            bagRedPointImage = GetChildImage("bagRedPointImage");
		}

		void onClick(GameObject e) {
			if (disabed || !e.GetComponent<Button>().enabled) {
				return;
			}
            switch (e.name)
            {
                case "icon0":
                    if (roleDataList.Count > 0)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[0]);
                        viewedRoleIndex = 0;
                    }
                    break;
                case "icon1":
                    if (roleDataList.Count > 1)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[1]);
                        viewedRoleIndex = 1;
                    }
                    break;
                case "icon2":
                    if (roleDataList.Count > 2)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[2]);
                        viewedRoleIndex = 2;
                    }
                    break;
                case "icon3":
                    if (roleDataList.Count > 3)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[3]);
                        viewedRoleIndex = 3;
                    }
                    break;
                case "icon4":
                    if (roleDataList.Count > 4)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[4]);
                        viewedRoleIndex = 4;
                    }
                    break;
                case "icon5":
                    if (roleDataList.Count > 5)
                    {
                        Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[5]);
                        viewedRoleIndex = 5;
                    }
                    break;
                case "bagButton":
                    Messenger.Broadcast(NotifyTypes.GetBagPanelData);
                    PlayerPrefs.SetString("AddedNewItemFlag", "");
                    RefreshRedPoint();
                    break;
                case "booksButton":
                    Messenger.Broadcast(NotifyTypes.GetBooksListPanelData);
                    PlayerPrefs.SetString("AddedNewBookFlag", "");
                    RefreshRedPoint();
                    break;
                case "weaponsButton":
                    Messenger.Broadcast(NotifyTypes.GetWeaponsListPanelData);
                    PlayerPrefs.SetString("AddedNewWeaponFlag", "");
                    RefreshRedPoint();
                    break;
                default:
                    break;
            }
		}

		public void UpdateData (object obj, bool isfighting) {
			disabed = false;
			JArray data = (JArray)obj;
			roleDataList = new List<RoleData>(); 
			JArray itemData;
			RoleData role;
			for (int i = 0; i < data.Count; i++) {
				itemData = (JArray)data[i];
				role = JsonManager.GetInstance().DeserializeObject<RoleData>(itemData[1].ToString());
				role.MakeJsonToModel();
				role.Injury = (InjuryType)((int)itemData[3]);
				role.IsDie = false;
				roleDataList.Add(role);
			}
		}

		public override void RefreshView () {
            RoleData roleData;
            Image icon;
            for (int i = 0; i < icons.Count; i++) {
                icon = icons[i];
                if (roleDataList.Count > i) {
                    icon.gameObject.SetActive(true);
                    roleData = roleDataList[i];
                    icon.sprite = Statics.GetIconSprite(roleData.IconId);
                    icon.color = roleData.IsDie ? Color.red : Color.white;
                    if (roleData.Injury != InjuryType.None) {
                        injuryImages[i].gameObject.SetActive(true);
                        injuryImages[i].color = Statics.GetInjuryColor(roleData.Injury);
                    }
                    else {
                        injuryImages[i].gameObject.SetActive(false);
                    }
                }
                else {
                    icon.gameObject.SetActive(false);
                    injuryImages[i].gameObject.SetActive(false);
                }
            }
            RefreshRedPoint();
        }

        public void RefreshRedPoint() {
            weaponsRedPointImage.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString("AddedNewWeaponFlag")));
            booksRedPointImage.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString("AddedNewBookFlag")));
            bagRedPointImage.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString("AddedNewItemFlag")));
        }

        public void ReviewRole() {
            if (viewedRoleIndex >= 0 && roleDataList.Count > viewedRoleIndex)
            {
                Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleDataList[viewedRoleIndex]);
            }
        }

		public static void Show(JArray data, bool isfighting = true) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/RolesInfoPanelView", "RolesInfoPanelCtrl", 0, -52.5f, UIModel.FrameCanvas.transform);
				Ctrl.MoveVertical(65 + 52.5f);
			}
			Ctrl.UpdateData(data, isfighting);
			Ctrl.RefreshView();
		}

		public static void MoveDown() {
			if (Ctrl != null) {
				Ctrl.MoveVertical(-(65 + 52.5f), () => {
					Ctrl.Close();
				});
			}
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
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

        public static void MakeRefreshRedPoint() {
            if (Ctrl != null)
            {
                Ctrl.RefreshRedPoint();
            }
        }

        public static void MakeReviewRole() {
            if (Ctrl != null)
            {
                Ctrl.ReviewRole();
            }
        }
	}
}
