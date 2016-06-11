using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class BooksOfForbiddenAreaPanelCtrl : WindowCore<BooksOfForbiddenAreaPanelCtrl, JArray> {
		CanvasGroup bg;
		GridLayoutGroup grid;
		Button closeBtn;

		List<BookData> booksData;
		List<BookOfForbiddenAreaContainer> bookContainers;
		Object prefabObj;
		RoleData hostRoleData;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			bookContainers = new List<BookOfForbiddenAreaContainer>();
		}

		void onClick(GameObject e) {
			FadeOut();
		}

		public void UpdateData (List<BookData> books, RoleData host) {
			booksData = books;
			hostRoleData = host;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/BookOfForbiddenAreaContainer");
			}
			GameObject itemPrefab;
			BookData book;
			BookOfForbiddenAreaContainer container;
			for (int i = 0; i < booksData.Count; i++) {
				book = booksData[i];
				if (bookContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(grid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<BookOfForbiddenAreaContainer>();
					bookContainers.Add(container);
				}
				else {
					container = bookContainers[i];
				}
				container.UpdateData(book, hostRoleData);
				container.RefreshView();
			}
			RectTransform trans = grid.GetComponent<RectTransform>();
			float y = (grid.cellSize.y + grid.spacing.y) * Mathf.Ceil(bookContainers.Count / 3) - grid.spacing.y;
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
			Messenger.Broadcast(NotifyTypes.MakeCheckNewFlags); //判断城镇界面的新增提示
		}

		public static void Show(List<BookData> books, RoleData host) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/BooksOfForbiddenAreaPanelView", "BooksOfForbiddenAreaPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(books, host);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}
	}
}
