using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
	public class WeaponListPanelCtrl : WindowCore<WeaponListPanelCtrl, JArray> {
		Image bg;
		Button block;
		GridLayoutGroup grid;
		Button closeBtn;

		List<WeaponData> weaponsData;
		List<WeaponItemContainer> weaponContainers;
		Object prefabObj;
		WeaponItemContainer hostWeaponItemContainer;
		RoleData hostRoleData;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			weaponContainers = new List<WeaponItemContainer>();
			hostWeaponItemContainer = GetChildComponent<WeaponItemContainer>(gameObject, "HostWeaponItemContainer");
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData (List<WeaponData> weapons, RoleData host) {
			weaponsData = weapons;
			hostRoleData = host;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/WeaponItemContainer");
			}
			if (weaponsData.Count > 0) {
				WeaponData weapon = weaponsData[0];
				if (weapon != null) {
					hostWeaponItemContainer.gameObject.SetActive(true);
					hostWeaponItemContainer.UpdateData(weapon, weapon, hostRoleData);
					hostWeaponItemContainer.RefreshView();
				}
				else {
					hostWeaponItemContainer.gameObject.SetActive(false);
				}
				if (weaponsData.Count > 1) {
					GameObject itemPrefab;
					WeaponItemContainer container;
					for (int i = 1; i < weaponsData.Count; i++) {
						weapon = weaponsData[i];
						if (weaponContainers.Count <= (i - 1)) {
							itemPrefab = Statics.GetPrefabClone(prefabObj);
							MakeToParent(grid.transform, itemPrefab.transform);
							container = itemPrefab.GetComponent<WeaponItemContainer>();
							weaponContainers.Add(container);
						}
						else {
							container = weaponContainers[i - 1];
						}
						container.UpdateData(weapon, weaponsData[0], hostRoleData);
						container.RefreshView();
					}
					//移除多余的container
					if (weaponContainers.Count > weaponsData.Count - 1) {
						for (int i = weaponContainers.Count - 1; i >= weaponsData.Count - 1; i--) {
							Destroy(weaponContainers[i].gameObject);
							weaponContainers.RemoveAt(i);
						}
					}
					RectTransform trans = grid.GetComponent<RectTransform>();
					trans.sizeDelta = new Vector2(trans.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * weaponContainers.Count - grid.spacing.y);
				}
			}
		}

		public void Pop() {
			bg.transform.DOScale(0, 0);
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

		public static void Show(List<WeaponData> weapons, RoleData host) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/WeaponListPanelView", "WeaponListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(weapons, host);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
