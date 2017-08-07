using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
    public class HelpPanelCtrl : WindowCore<HelpPanelCtrl, JArray> {
		Image bg;
		Button block;
		Button closeBtn;

		bool showBackMainTool;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
            switch (e.name)
            {
                case "Block":
                case "CloseBtn":
                    Back();
                    break;
                default:
                    break;
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

		public static void Show() {
			if (Ctrl == null) {
                InstantiateView("Prefabs/UI/MainTool/HelpPanelView", "HelpPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
