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
		Text totalText;

		ReadyToTraveRoleContainer hostRoleContainer;
		List<ReadyToTraveRoleContainer> roleContainers;

		List<RoleData> rolesData;
		List<RoleData> selectedRolesData;
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
			totalText = GetChildText("TotalText");
			hostRoleContainer = GetChildComponent<ReadyToTraveRoleContainer>(gameObject, "ReadyToTraveRoleContainer");
			roleContainers = new List<ReadyToTraveRoleContainer>();
			selectedRolesData = new List<RoleData>();
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
			case "StartBtn":
				if (rolesData.Count > 0) {
					if (rolesData[0].Injury == InjuryType.Moribund) {
						AlertCtrl.Show("你现在伤重生命垂危不能继续闯荡江湖!", null);
					}
					else {
						if (foodData.Num <= 0) {
							AlertCtrl.Show("没有干粮如何闯荡江湖!", null);
						}
						else {
							JArray ids = new JArray();
							for (int i = 0; i < selectedRolesData.Count; i++) {
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
			default:
				break;
			}
		}

		public void UpdateData(List<RoleData> roles, ItemData food) {
			rolesData = roles;
			foodData = food;
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
			totalText.text = string.Format("总气血:{0}    干粮:{1}/{2}", totalHP, foodData.Num, foodData.MaxNum);
			for (int i = 1; i < rolesData.Count; i++) {
				roleContainers[i - 1].EnableSelectBtn(selectedRolesData.Count < 6);
			}
		}

		public static void Show(List<RoleData> roles, ItemData food) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/ReadyToTravelPanelView", "ReadyToTravelPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(roles, food);
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
