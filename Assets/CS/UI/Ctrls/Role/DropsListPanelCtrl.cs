using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
	public class DropsListPanelCtrl : WindowCore<DropsListPanelCtrl, JArray> {
		Image bg;
		Button block;
		GridLayoutGroup grid;

		List<DropData> dropsData;
		List<DropItemContainer> dropContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			dropContainers = new List<DropItemContainer>();
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData (List<DropData> drops) {
			dropsData = drops;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/DropItemContainer");
			}
			if (dropsData.Count > 0) {
				GameObject itemPrefab;
				DropItemContainer container;
				DropData drop;
				for (int i = 0; i < dropsData.Count; i++) {
					drop = dropsData[i];
					if (dropContainers.Count <= i) {
						itemPrefab = Statics.GetPrefabClone(prefabObj);
						MakeToParent(grid.transform, itemPrefab.transform);
						container = itemPrefab.GetComponent<DropItemContainer>();
						dropContainers.Add(container);
					}
					else {
						container = dropContainers[i];
					}
					container.UpdateData(drop);
					container.RefreshView();
				}
				RectTransform trans = grid.GetComponent<RectTransform>();
				trans.sizeDelta = new Vector2(trans.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * dropContainers.Count - grid.spacing.y);
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

		public static void Show(List<DropData> drops) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/DropsListPanelView", "DropsListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(drops);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
