using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
	public class BookListPanelCtrl : WindowCore<BookListPanelCtrl, JArray> {
		Image bg;
		Button block;
		GridLayoutGroup grid;
		Button closeBtn;

		List<BookData> booksData;
		List<BookItemContainer> bookContainers;
		Object prefabObj;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			bookContainers = new List<BookItemContainer>();
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData (List<BookData> books) {
			booksData = books;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/BookItemContainer");
			}
			if (booksData.Count > 0) {
				BookData book;
				GameObject itemPrefab;
				BookItemContainer container;
				for (int i = 0; i < booksData.Count; i++) {
					book = booksData[i];
					if (bookContainers.Count <= i) {
						itemPrefab = Statics.GetPrefabClone(prefabObj);
						MakeToParent(grid.transform, itemPrefab.transform);
						container = itemPrefab.GetComponent<BookItemContainer>();
						bookContainers.Add(container);
					}
					else {
						container = bookContainers[i];
					}
					container.UpdateData(book);
					container.RefreshView();
				}
				RectTransform trans = grid.GetComponent<RectTransform>();
				trans.sizeDelta = new Vector2(trans.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * bookContainers.Count - grid.spacing.y);
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

		public static void Show(List<BookData> books) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/BookListPanelView", "BookListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(books);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
