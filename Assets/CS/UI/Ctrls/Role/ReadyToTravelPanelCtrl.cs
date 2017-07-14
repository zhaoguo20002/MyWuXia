using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class ReadyToTravelPanelCtrl : WindowCore<ReadyToTravelPanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button startBtn;
		Button backBtn;
		Image foodIcon;
		Text foodNumText;
		List<Image> selectedIcons;
        List<Image> seatCovers;
		Text totalText;

		ReadyToTraveRoleContainer hostRoleContainer;
		List<ReadyToTraveRoleContainer> roleContainers;

		List<RoleData> rolesData;
		List<RoleData> selectedRolesData;
        UserData userData;
		ItemData foodData;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			startBtn = GetChildButton("StartBtn");
			backBtn = GetChildButton("BackBtn");
			EventTriggerListener.Get(startBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(backBtn.gameObject).onClick = onClick;
			foodIcon = GetChildImage("FoodIcon");
			foodNumText = GetChildText("FoodNumText");
			selectedIcons = new List<Image>() {
				GetChildImage("icon0"),
				GetChildImage("icon1"),
				GetChildImage("icon2"),
                GetChildImage("icon3"),
                GetChildImage("icon4"),
                GetChildImage("icon5")
            };
            for (int i = 0, len = selectedIcons.Count; i < len; i++)
            {
                EventTriggerListener.Get(selectedIcons[i].gameObject).onClick = onClick;
            }
            seatCovers = new List<Image>() { 
                GetChildImage("seatCover0"),
                GetChildImage("seatCover1"),
                GetChildImage("seatCover2"),
                GetChildImage("seatCover3"),
                GetChildImage("seatCover4"),
                GetChildImage("seatCover5")
            };
            for (int i = 0, len = seatCovers.Count; i < len; i++)
            {
                EventTriggerListener.Get(seatCovers[i].gameObject).onClick = onClick;
            }
			totalText = GetChildText("TotalText");
			hostRoleContainer = GetChildComponent<ReadyToTraveRoleContainer>(gameObject, "ReadyToTraveRoleContainer");
			roleContainers = new List<ReadyToTraveRoleContainer>();
			selectedRolesData = new List<RoleData>();
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "StartBtn":
                    if (!startBtn.enabled)
                    {
                        return;
                    }
                    if (rolesData.Count > 0)
                    {
                        if (rolesData[0].Injury == InjuryType.Moribund)
                        {
                            AlertCtrl.Show("你现在伤重生命垂危不能继续闯荡江湖!", null);
                        }
                        else
                        {
                            if (foodData.Num <= 0)
                            {
                                AlertCtrl.Show("没有干粮如何闯荡江湖!", null);
                            }
                            else
                            {
                                JArray ids = new JArray();
                                for (int i = 0; i < selectedRolesData.Count; i++)
                                {
                                    ids.Add(selectedRolesData[i].PrimaryKeyId);
                                }
                                Messenger.Broadcast<JArray>(NotifyTypes.ChangeRolesSeatNo, ids);
                            }
                        }
                    }
                    break;
                case "BackBtn":
                    FadeOut();
                    break;
                case "seatCover2":
                    AlertCtrl.Show("前往[临安集市]后开启");
                    break;
                case "seatCover3":
                    AlertCtrl.Show("前往[观前街]后开启");
                    break;
                case "seatCover4":
                    AlertCtrl.Show("前往[归云庄]后开启");
                    break;
                case "seatCover5":
                    AlertCtrl.Show("前往[金国领地]后开启");
                    break;
                default:
                    if (e.name.IndexOf("icon") == 0)
                    {
                        int index = int.Parse(e.name.Replace("icon", ""));
                        if (selectedRolesData.Count > index)
                        {
                            Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, selectedRolesData[index]);
                        }
                    }
                    break;
            }
		}

        public void UpdateData(List<RoleData> roles, UserData user) {
			rolesData = roles;
            userData = user;
            foodData = userData.AreaFood;
			if (rolesData.Count > 0) {
				selectedRolesData.Add(rolesData[0]);
				for (int i = 1; i < rolesData.Count; i++) {
					if (rolesData[i].State == RoleStateType.InTeam) {
						selectedRolesData.Add(rolesData[i]);
					}
				}
			}
		}

		public override void RefreshView () {
			if (rolesData.Count > 0) {
				hostRoleContainer.UpdateData(rolesData[0]);
				hostRoleContainer.RefreshView();
				if (prefabObj == null) {
					prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/ReadyToTraveRoleContainer");
				}
				GameObject itemPrefab;
				RoleData role;
				ReadyToTraveRoleContainer container;
				for (int i = 1; i < rolesData.Count; i++) {
					role = rolesData[i];
					if (roleContainers.Count <= (i - 1)) {
						itemPrefab = Statics.GetPrefabClone(prefabObj);
						MakeToParent(grid.transform, itemPrefab.transform);
						container = itemPrefab.GetComponent<ReadyToTraveRoleContainer>();
						roleContainers.Add(container);
					}
					else {
						container = roleContainers[i - 1];
					}
					container.UpdateData(role);
					container.RefreshView();
				}
				RectTransform trans = grid.GetComponent<RectTransform>();
				float y = (grid.cellSize.y + grid.spacing.y) * roleContainers.Count - grid.spacing.y;
				y = y < 0 ? 0 : y;
				trans.sizeDelta = new Vector2(trans.sizeDelta.x, y);
			}
            foodIcon.sprite = Statics.GetIconSprite("600001");
			foodNumText.text = string.Format("{0}/{1}", foodData.Num, foodData.MaxNum);
			RefreshSelectedRolesView();
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
			Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
		}

		/// <summary>
		/// 选中角色
		/// </summary>
		/// <param name="role">Role.</param>
		public void SelectRole(RoleData role) {
			if (selectedRolesData.FindIndex(item => item.Id == role.Id) < 0) {
				selectedRolesData.Add(role);
			}
		}

		/// <summary>
		/// 取消选中角色
		/// </summary>
		/// <param name="role">Role.</param>
		public void UnSelectRole(RoleData role) {
			int index = selectedRolesData.FindIndex(item => item.Id == role.Id);
			if (selectedRolesData.Count > index) {
				selectedRolesData.RemoveAt(index);
			}
		}

		/// <summary>
		/// 刷新角色
		/// </summary>
		public void RefreshSelectedRolesView() {
			int totalHP = 0;
			for (int i = 0; i < selectedIcons.Count; i++) {
				if (selectedRolesData.Count > i) {
					selectedIcons[i].gameObject.SetActive(true);
					selectedIcons[i].sprite = Statics.GetIconSprite(selectedRolesData[i].IconId);
					totalHP += selectedRolesData[i].MaxHP;
				}
				else {
					selectedIcons[i].gameObject.SetActive(false);
				}
			}
            totalText.text = string.Format("总气血:{0}  干粮:{1}/{2}  可出战侠客数:{3}", totalHP, foodData.Num, foodData.MaxNum, userData.MaxRoleNum);
			for (int i = 1; i < rolesData.Count; i++) {
                roleContainers[i - 1].EnableSelectBtn(selectedRolesData.Count < userData.MaxRoleNum);
			}
            for (int i = 0, len = seatCovers.Count; i < len; i++)
            {
                seatCovers[i].gameObject.SetActive(i >= userData.MaxRoleNum);
            }
		}

        public static void Show(List<RoleData> roles, UserData user) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/ReadyToTravelPanelView", "ReadyToTravelPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(roles, user);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}

		/// <summary>
		/// 选中角色
		/// </summary>
		/// <param name="role">Role.</param>
		public static void MakeSelectRole(RoleData role) {
			if (Ctrl != null) {
				Ctrl.SelectRole(role);
				Ctrl.RefreshSelectedRolesView();
			}
		}

		/// <summary>
		/// 取消选中角色
		/// </summary>
		/// <param name="role">Role.</param>
		public static void MakeUnSelectRole(RoleData role) {
			if (Ctrl != null) {
				Ctrl.UnSelectRole(role);
				Ctrl.RefreshSelectedRolesView();
			}
		}
	}
}
