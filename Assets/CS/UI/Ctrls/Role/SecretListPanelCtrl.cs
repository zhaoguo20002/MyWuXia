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
            itemScript.UpdateData(secretsData[item.ItemIndex]);
            itemScript.RefreshView();
        }

		void onClick(GameObject e) {
			Back();
		}

        public void UpdateData (List<SecretData> secrets) {
            secretsData = secrets;
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

        public static void Show(List<SecretData> secrets) {
			if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/SecretListPanelView", "SecretListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData(secrets);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
