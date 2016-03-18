using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class RolesOfWinShopPanelCtrl : WindowCore<RolesOfWinShopPanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button closeBtn;

		List<RoleData> rolesData;
		List<RoleOfWinShopContainer> roleContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn ");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			roleContainers = new List<RoleOfWinShopContainer>();
		}

		void onClick(GameObject e) {
			FadeOut();
		}

		public void UpdateData (List<RoleData> roles) {
			rolesData = roles;
		}

		public override void RefreshView () {
			for (int i = 0; i < roleContainers.Count; i++) {
				Destroy(roleContainers[i].gameObject);
			}
			roleContainers.Clear();
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/RoleOfWinShopContainer");
			}
			GameObject itemPrefab;
			RoleData role;
			RoleOfWinShopContainer container;
			for (int i = 0; i < rolesData.Count; i++) {
				role = rolesData[i];
				itemPrefab = Statics.GetPrefabClone(prefabObj);
				itemPrefab.name = i.ToString();
				MakeToParent(grid.transform, itemPrefab.transform);
				container = itemPrefab.GetComponent<RoleOfWinShopContainer>();
				container.UpdateData(role);
				container.RefreshView();
				roleContainers.Add(container);
			}
			grid.GetComponent<RectTransform>().sizeDelta = new Vector2(480, (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(roleContainers.Count / 3) - grid.spacing.y);
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
				InstantiateView("Prefabs/UI/Role/RolesOfWinShopPanelView", "RolesOfWinShopPanelCtrl");
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
