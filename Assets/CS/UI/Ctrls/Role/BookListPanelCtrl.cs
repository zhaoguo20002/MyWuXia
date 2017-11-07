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
        ScrollRect scroll;
        Image emptyImage;
        Button allSecretBtn;
        Image secretsRedPointImage;

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
            scroll = GetChildScrollRect("List");
            emptyImage = GetChildImage("emptyImage");
            allSecretBtn = GetChildButton("allSecretBtn");
            EventTriggerListener.Get(allSecretBtn.gameObject).onClick = onClick;
            secretsRedPointImage = GetChildImage("secretsRedPointImage");
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "allSecretBtn":
                    Messenger.Broadcast<List<SecretData>>(NotifyTypes.GetSecretListPanelData, null);
                    PlayerPrefs.SetString("AddedNewSecretFlag", "");
                    refreshRedPoints();
                    break;
                default:
                    Back();
                    break;
            }
		}

		public void UpdateData (List<BookData> books) {
			booksData = books;
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/BookItemContainer");
			}
            if (booksData.Count > 0)
            {
                emptyImage.gameObject.SetActive(false);
                BookData book;
                GameObject itemPrefab;
                BookItemContainer container;
                for (int i = 0; i < booksData.Count; i++)
                {
                    book = booksData[i];
                    if (bookContainers.Count <= i)
                    {
                        itemPrefab = Statics.GetPrefabClone(prefabObj);
                        MakeToParent(grid.transform, itemPrefab.transform);
                        container = itemPrefab.GetComponent<BookItemContainer>();
                        bookContainers.Add(container);
                    }
                    else
                    {
                        container = bookContainers[i];
                    }
                    container.Index = i;
                    container.UpdateData(book);
                    container.RefreshView();
                }
                RectTransform trans = grid.GetComponent<RectTransform>();
                trans.sizeDelta = new Vector2(trans.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * bookContainers.Count - grid.spacing.y);
                scroll.verticalNormalizedPosition = 1;
            }
            else
            {
                emptyImage.gameObject.SetActive(true);
            }
            refreshRedPoints();
        }

        void refreshRedPoints() {
            secretsRedPointImage.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString("AddedNewSecretFlag")));
        }

        void sendUseBook(int index) {
            BookData book = booksData[index];
            BookData findBook;
            if (!book.IsMindBook)
            {
                if (!book.IsLostKnowledge)
                {
                    findBook = booksData.Find(item => item.BeUsingByRoleId != "" && item.IsMindBook == false && item.IsLostKnowledge == false);
                    if (findBook != null)
                    {
                        AlertCtrl.Show("只能随身携带一本秘籍！");
                        return;
                    }
                }
                else
                {
                    findBook = booksData.Find(item => item.BeUsingByRoleId != "" && item.IsMindBook == false && item.IsLostKnowledge == true);
                    if (findBook != null)
                    {
                        AlertCtrl.Show("只能随身携带一本绝学！");
                        return;
                    }
                }
                Messenger.Broadcast<int>(NotifyTypes.UseBook, book.PrimaryKeyId);
            }
            else
            {
                findBook = booksData.Find(item => item.BeUsingByRoleId != "" && item.IsMindBook == false && item.IsLostKnowledge == false);
                if (findBook == null)
                {
                    AlertCtrl.Show("没有秘籍在身不能使用心法！");
                    return;
                }
                Messenger.Broadcast<int>(NotifyTypes.UseBook, book.PrimaryKeyId);
            }
        }

        void sendUnUseBook(int index) {
            BookData book = booksData[index];
            Messenger.Broadcast<int>(NotifyTypes.UnuseBook, book.PrimaryKeyId);
            if (!book.IsMindBook && !book.IsLostKnowledge)
            {
                //卸下秘籍的同时要清除掉心法
                List<BookData> equipedBooks = booksData.FindAll(item => item.IsMindBook && item.BeUsingByRoleId != "");
                for (int i = equipedBooks.Count - 1; i >= 0; i--)
                {
                    Messenger.Broadcast<int>(NotifyTypes.UnuseBook, equipedBooks[i].PrimaryKeyId);
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
