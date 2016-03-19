using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class AlertCtrl : WindowCore<AlertCtrl, JArray> {
		Image bg;
		Button block;
		Text msg;
		Button sureBtn;
		Text sureBtnText;

		string _msg;
		string _sureBtnValue;
		System.Action _sureCallback;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			msg = GetChildText("Msg");
			sureBtn = GetChildButton("SureBtn");
			EventTriggerListener.Get(sureBtn.gameObject).onClick = onClick;
			sureBtnText = GetChildText("SureBtnText");
		}

		void onClick(GameObject e) {
			Back();
			if (_sureCallback != null) {
				_sureCallback();
				_sureCallback = null;
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

		public void UpdateData(string msg, System.Action sureCallback, string sureBtnVale) {
			_msg = msg;
			_sureCallback = sureCallback;
			_sureBtnValue = sureBtnVale;
		}

		public override void RefreshView () {
			msg.text = _msg;
			sureBtnText.text = _sureBtnValue;
		}

		public static void Show(string msg, System.Action sureCallback, string sureBtnVale = "不行") {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Comm/AlertView", "AlertCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(msg, sureCallback, sureBtnVale);
			Ctrl.RefreshView();
		}

	}
}
