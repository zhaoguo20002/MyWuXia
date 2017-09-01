using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
    public class PropsMallPanelCtrl : WindowCore<PropsMallPanelCtrl, JArray> {
		Image bg;
		Button block;
        Button closeBtn;
        PropItemContainer payContainer;
        PropItemContainer freeContainer;
        Text descText;

        PropData propData;

		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
            payContainer = GetChildComponent<PropItemContainer>(gameObject, "payContainer");
            freeContainer = GetChildComponent<PropItemContainer>(gameObject, "freeContainer");
            descText = GetChildText("descText");
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

        public void UpdateData(PropData data) {
            propData = data;
        }

        public override void RefreshView()
        {
            switch (propData.Type)
            {
                case PropType.NocturnalClothing:
                    descText.text = "道具：夜行衣\n效果：一件夜行衣能避免一场野外战斗。";
                    break;
                case PropType.Bodyguard:
                    descText.text = "道具：镖师\n效果：一位镖师能够抵消一位侠客受伤。";
                    break;
                case PropType.LimePowder:
                    descText.text = "道具：石灰粉\n效果：石灰粉有50%概率能脱离战斗。";
                    break;
                case PropType.Scout:
                    descText.text = "道具：探子\n效果：探子可以追踪未知的任务目标。";
                    break;
                default:
                    break;
            }
            payContainer.UpdateData(propData);
            payContainer.RefreshView();
            freeContainer.UpdateData(propData);
            freeContainer.RefreshView();
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

        public static void Show(PropData data) {
			if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/PropsMallPanelView", "PropsMallPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData(data);
            Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
