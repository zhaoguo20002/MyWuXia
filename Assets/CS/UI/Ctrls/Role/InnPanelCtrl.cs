using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class InnPanelCtrl : WindowCore<InnPanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button closeBtn;

		List<FloydResult> resultsData;
		double silverNum = 0;
		List<InnItemContainer> innContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			innContainers = new List<InnItemContainer>();
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "CloseBtn":
				FadeOut();
				break;
			default:
				break;
			}
		}

		public void UpdateData (List<FloydResult> results) {
			resultsData = results;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/InnItemContainer");
			}
			GameObject itemPrefab;
			FloydResult result;
			InnItemContainer container;
			for (int i = 0; i < resultsData.Count; i++) {
				result = resultsData[i];
				if (innContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<InnItemContainer>();
					innContainers.Add(container);
				}
				else {
					container = innContainers[i];
				}
				container.UpdateData(result);
				container.RefreshView();
			}
//			RectTransform trans = grid.GetComponent<RectTransform>();
//			float y = (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(innContainers.Count / 2) - grid.spacing.y;
//			y = y < 0 ? 0 : y;
//			trans.sizeDelta = new Vector2(trans.sizeDelta.x, y);
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
		}

		public static void Show(List<FloydResult> items) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/InnPanelView", "InnPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(items);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
