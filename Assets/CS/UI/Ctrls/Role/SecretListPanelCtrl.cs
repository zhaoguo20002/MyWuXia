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
            if (secretsData.Count <= 0)
            {
                return;
            }
            SecretItemContainer itemScript = item.GetComponent<SecretItemContainer>();
            SecretData currentSecret = secretsData[item.ItemIndex];
            itemScript.UpdateData(currentSecret);
            itemScript.RefreshView();
            itemScript.StateText.gameObject.SetActive(false);
            if (hasSecretsData == null)
            {
                itemScript.StudyBtn.gameObject.SetActive(false);
                itemScript.ForgetBtn.gameObject.SetActive(false);
                if (currentSecret.Quality < QualityType.FlashRed)
                {
                    itemScript.MixBtn.gameObject.SetActive(true);
                    //闪红以下品质4张及4张以上相同类型相同品质的诀要就可以融合
                    List<SecretData> sameSecrets = secretsData.FindAll(sec => sec.Type == currentSecret.Type && sec.Quality == currentSecret.Quality);
                    itemScript.MixBtn.gameObject.SetActive(sameSecrets != null && sameSecrets.Count >= 4);
                }
                else
                {
                    itemScript.MixBtn.gameObject.SetActive(false);
                    itemScript.StateText.gameObject.SetActive(true);
                }
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
            if (bookData != null)
            {
                DbManager.Instance.StudySecret(bookData, data);
            }
        }

        void forget(SecretData data) {
            if (bookData != null)
            {
                DbManager.Instance.ForgetSecret(bookData, data);
            }
        }

        void mix(SecretData data) {
            if (DbManager.Instance.MixSecrets(data))
            {
                Messenger.Broadcast<BookData, List<SecretData>>(NotifyTypes.GetSecretListPanelData, null, null);
            }
        }

        public void UpdateData (List<SecretData> secrets, BookData book, List<SecretData> hasSecrets) {
            secretsData = secrets;
            bookData = book;
            hasSecretsData = hasSecrets;
            if (hasSecretsData != null)
            {
                secretsData.Sort((a, b) => b.Quality.CompareTo(a.Quality));
                secretsData.InsertRange(0, hasSecretsData);
            }
            else
            {
                //排序，能融合的排前面，之后按品质倒序排列
                secretsData.Sort((a, b) =>
                {
                    List<SecretData> sameSecretsA = secretsData.FindAll(sec => sec.Type == a.Type && sec.Quality == a.Quality);
                    List<SecretData> sameSecretsB = secretsData.FindAll(sec => sec.Type == b.Type && sec.Quality == b.Quality);

                    if (sameSecretsA.Count >= 4)
                    {
                        if (sameSecretsB.Count >= 4)
                        {
                            return b.Quality.CompareTo(a.Quality);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (sameSecretsB.Count >= 4)
                        {
                            return 1;
                        }
                        else
                        {
                            return b.Quality.CompareTo(a.Quality);
                        }
                    }
                });
            }
		}

		public override void RefreshView () {
            if (secretsData.Count > 0)
            {
                emptyImage.gameObject.SetActive(false);
            }
            else
            {
                emptyImage.gameObject.SetActive(true);
            }
            ScrollView.SetListItemCount(secretsData.Count);
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
