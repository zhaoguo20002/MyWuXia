using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class HospitalPanelCtrl : WindowCore<HospitalPanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button closeBtn;

		List<RoleData> rolesData;
		List<RoleOfHospitalContainer> roleContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			roleContainers = new List<RoleOfHospitalContainer>();
		}

		void onClick(GameObject e) {
			FadeOut();
		}

		public void UpdateData (List<RoleData> roles) {
			rolesData = roles;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/RoleOfHospitalContainer");
			}
			GameObject itemPrefab;
			RoleData role;
			RoleOfHospitalContainer container;
			for (int i = 0; i < rolesData.Count; i++) {
				role = rolesData[i];
				if (roleContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<RoleOfHospitalContainer>();
					roleContainers.Add(container);
				}
				else {
					container = roleContainers[i];
				}
				container.UpdateData(role);
				container.RefreshView();
			}
			RectTransform trans = grid.GetComponent<RectTransform>();
			float y = (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(roleContainers.Count / 3) - grid.spacing.y;
			y = y < 0 ? 0 : y;
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, y);
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
		}

		public static void Show(List<RoleData> roles) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/HospitalPanelView", "HospitalPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(roles);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}
	}
}
