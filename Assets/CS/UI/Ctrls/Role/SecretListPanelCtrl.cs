using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;
using SuperScrollView;

namespace Game {
    public class SecretListPanelCtrl : WindowCore<SecretListPanelCtrl, JArray> {
        public LoopListView ScrollView;
		Image bg;
		Button block;
		Button closeBtn;
        Image emptyImage;

        List<SecretData> secretsData;
        BookData bookData;
        List<SecretData> hasSecretsData;
		protected override void Init () {
            ScrollView.InitListView(0, null, OnItemUpdated);
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
            emptyImage = GetChildImage("emptyImage");
		}

        void OnItemUpdated(LoopListViewItem item)
        {
            SecretItemContainer itemScript = item.GetComponent<SecretItemContainer>();
            SecretData currentSecret = secretsData[item.ItemIndex];
            itemScript.UpdateData(currentSecret);
            itemScript.RefreshView();
            if (hasSecretsData == null)
            {
                itemScript.StudyBtn.gameObject.SetActive(false);
                itemScript.ForgetBtn.gameObject.SetActive(false);
                itemScript.MixBtn.gameObject.SetActive(true);
            }
            else
            {
                if (hasSecretsData.FindIndex(sec => sec.PrimaryKeyId == currentSecret.PrimaryKeyId) >= 0)
                {
                    itemScript.ForgetBtn.gameObject.SetActive(true);
                }
                else
                {
                    itemScript.StudyBtn.gameObject.SetActive(true);
                }
                itemScript.MixBtn.gameObject.SetActive(false);
            }
        }

		void onClick(GameObject e) {
			Back();
		}

        void study(SecretData data) {
            DbManager.Instance.StudySecret(bookData, data);
        }

        public void UpdateData (List<SecretData> secrets, BookData book, List<SecretData> hasSecrets) {
            secretsData = secrets;
            bookData = book;
            hasSecretsData = hasSecrets;
            if (hasSecretsData != null)
            {
                secretsData.InsertRange(0, hasSecretsData);
            }
		}

		public override void RefreshView () {
            if (secretsData.Count > 0)
            {
                emptyImage.gameObject.SetActive(false);
                ScrollView.SetListItemCount(secretsData.Count);
            }
            else
            {
                emptyImage.gameObject.SetActive(true);
            }
		}

        void sendUseBook(int index) {
            
        }

        void sendUnUseBook(int index) {
            
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

        public static void Show(List<SecretData> secrets, BookData book, List<SecretData> hasSecrets) {
			if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/SecretListPanelView", "SecretListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData(secrets, book, hasSecrets);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
