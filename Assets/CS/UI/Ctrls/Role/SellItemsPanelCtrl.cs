using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class SellItemsPanelCtrl : WindowCore<SellItemsPanelCtrl, JArray> {
		Button block;
		Image bg;
		Button closeBtn;
		Button sellBtn;
		GridLayoutGroup grid;
		Text silverText;

		List<ItemData> itemsData;
		List<SellItemContainer> itemContainers;
		Object prefabObj;
		JArray selectedItemsId;
		double silverNum;

		protected override void Init () {
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			bg = GetChildImage("Bg");
			bg.rectTransform.anchoredPosition = new Vector2(0, -bg.rectTransform.sizeDelta.y);
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			sellBtn = GetChildButton("SellBtn");
			EventTriggerListener.Get(sellBtn.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			silverText = GetChildText("SilverText");
			itemContainers = new List<SellItemContainer>();
			selectedItemsId = new JArray();
			silverNum = 0;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "Block":
			case "CloseBtn":
				SlideDown();
				break;
			case "SellBtn":
				if (selectedItemsId.Count > 0) {
					ConfirmCtrl.Show("确定将选中物品卖出?", () => {
						Messenger.Broadcast<JArray>(NotifyTypes.SellItems, selectedItemsId);
					});
				}
				else {
					AlertCtrl.Show("请先选择要卖出的物品!");
				}
				break;
			default:
				break;
			}
		}

		public void SlideUp() {
			bg.rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
		}

		public void SlideDown() {
			bg.rectTransform.DOAnchorPos(new Vector2(0, -bg.rectTransform.sizeDelta.y), 0.5f).OnComplete(() => {
				Close();
			});
		}

		public void UpdateData(List<ItemData> items) {
			itemsData = items;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/SellItemContainer");
			}
			GameObject itemPrefab;
			ItemData item;
			SellItemContainer container;
			for (int i = 0; i < itemsData.Count; i++) {
				item = itemsData[i];
				if (itemContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<SellItemContainer>();
					itemContainers.Add(container);
				}
				else {
					container = itemContainers[i];
				}
				container.UpdateData(item);
				container.RefreshView();
			}
			RectTransform trans = grid.GetComponent<RectTransform>();
			float y = (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(itemContainers.Count / 5) - grid.spacing.y;
			y = y < 0 ? 0 : y;
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, y);
			MakeSelectedItem();
		}

		public void UpdateSelectedItemsData() {
			selectedItemsId.Clear();
			silverNum = 0;
			SellItemContainer container;
			for (int i = 0; i < itemContainers.Count; i++) {
				container = itemContainers[i];
				if (container.Checked) {
					selectedItemsId.Add(container.Item.PrimaryKeyId);
					silverNum += container.Item.SellPrice * container.Item.Num;
				}
			}
		}

		public void RefreshSelectedItemsView() {
			silverText.text = silverNum.ToString();
		}

		public static void Show(List<ItemData> items) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/SellItemsPanelView", "SellItemsPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.SlideUp();
			}
			Ctrl.UpdateData(items);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.SlideDown();
			}
		}

		public static void MakeSelectedItem() {
			if (Ctrl != null) {
				Ctrl.UpdateSelectedItemsData();
				Ctrl.RefreshSelectedItemsView();
			}
		}
		
	}
}
