using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Game {
    public class LoadingBlockCtrl : WindowCore<LoadingBlockCtrl, UIModel> {
        CanvasGroup group;
        protected override void Init() {
            group = GetChildComponent<CanvasGroup>(gameObject, "group");
            group.DOFade(1, 0.5f).SetDelay(0.5f);
        }

        public static void Show() {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Comm/LoadingBlockView", "LoadingBlockCtrl", 0, 0, UIModel.FrameCanvas.transform);
            }
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Close();
            }
        }

        void OnDestroy() {
            group.DOKill();
        }
    }
}
