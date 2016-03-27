using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class BagPanelCtrl : WindowCore<BagPanelCtrl, JArray> {
		Image bg;
		Button block;
		GridLayoutGroup grid;
		Button closeBtn;
		Text silverText;
		Text totalText;

		List<ItemData> itemsData;
		double silverNum = 0;
		List<BagItemContainer> itemContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			silverText = GetChildText("SilverText");
			totalText = GetChildText("TotalText");
			itemContainers = new List<BagItemContainer>();
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData (List<ItemData> items, double silver) {
			itemsData = items;
			silverNum = silver;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/BagItemContainer");
			}
			GameObject itemPrefab;
			ItemData item;
			BagItemContainer container;
			for (int i = 0; i < itemsData.Count; i++) {
				item = itemsData[i];
				if (itemContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<BagItemContainer>();
					itemContainers.Add(container);
				}
				else {
					container = itemContainers[i];
				}
				container.UpdateData(item);
				container.RefreshView();
			}
			RectTransform trans = grid.GetComponent<RectTransform>();
			float y = (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(itemContainers.Count / 2) - grid.spacing.y;
			y = y < 0 ? 0 : y;
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, y);
			silverText.text = silverNum.ToString();
			totalText.text = string.Format("{0}/{1}", itemsData.Count, DbManager.Instance.MaxItemNumOfBag);
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

		public static void Show(List<ItemData> items, double silver) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/BagPanelView", "BagPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(items, silver);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
