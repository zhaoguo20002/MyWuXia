using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class StorePanelCtrl : WindowCore<StorePanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button closeBtn;
		Text silverText;
		Button sellBtn;

		List<ItemData> itemsData;
		double silverNum = 0;
		List<StoreItemContainer> itemContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			silverText = GetChildText("SilverText");
			sellBtn = GetChildButton("SellBtn");
			EventTriggerListener.Get(sellBtn.gameObject).onClick = onClick;
			itemContainers = new List<StoreItemContainer>();
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "CloseBtn":
				FadeOut();
				break;
			case "SellBtn":
				Messenger.Broadcast(NotifyTypes.GetSellItemsPanelData);
				break;
			default:
				break;
			}
		}

		public void UpdateData (List<ItemData> items, double silver) {
			itemsData = items;
            itemsData.Sort((a, b) => b.Id.CompareTo(a.Id));
			UpdateData(silver);
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/StoreItemContainer");
			}
			GameObject itemPrefab;
			ItemData item;
			StoreItemContainer container;
			for (int i = 0; i < itemsData.Count; i++) {
				item = itemsData[i];
				if (itemContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<StoreItemContainer>();
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
			RefreshSilverNumView();
		}

		public void UpdateData(double silver) {
			silverNum = silver;
		}

		public void RefreshSilverNumView() {
			silverText.text = silverNum.ToString();
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
		}

		/// <summary>
		/// 购买物品成功回调
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="silver">Silver.</param>
		public void BuyItemEcho(string msg, double silver) {
			silverNum = silver;
			Statics.CreatePopMsg(Vector3.zero, msg, Color.white, 30);
			silverText.rectTransform.anchoredPosition3D = new Vector3(40, 0, 0);
			silverText.transform.DOKill();
			silverText.transform.DOShakePosition(0.5f, 5);
			silverText.text = silverNum.ToString();
		}

		public static void Show(List<ItemData> items, double silver) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/StorePanelView", "StorePanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(items, silver);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}

		/// <summary>
		/// 购买物品成功回调
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="silver">Silver.</param>
		public static void MakeBuyItemEcho(string msg, double silver) {
			if (Ctrl != null) {
				Ctrl.BuyItemEcho(msg, silver);
			}
		}

		/// <summary>
		/// 更新剩余银子数
		/// </summary>
		/// <param name="silver">Silver.</param>
		public static void MakeChangeSilverNum(double silver) {
			if (Ctrl != null) {
				Ctrl.UpdateData(silver);
				Ctrl.RefreshSilverNumView();
			}
		}
	}
}
