using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class ConfirmCtrl : WindowCore<ConfirmCtrl, JArray> {
		Image bg;
		Button block;
		Text msg;
		Button sureBtn;
		Text sureBtnText;
		Button cancelBtn;
		Text cancelBtnText;

		string _msg;
		string _sureBtnValue;
		string _cancelBtnValue;
		System.Action _sureCallback;
		System.Action _cancelCallback;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;;
			msg = GetChildText("Msg");
			sureBtn = GetChildButton("SureBtn");
			EventTriggerListener.Get(sureBtn.gameObject).onClick = onClick;
			sureBtnText = GetChildText("SureBtnText");
			cancelBtn = GetChildButton("CancelBtn");
			EventTriggerListener.Get(cancelBtn.gameObject).onClick = onClick;
			cancelBtnText = GetChildText("CancelBtnText");
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "SureBtn":
				Back();
				if (_sureCallback != null) {
					_sureCallback();
					_sureCallback = null;
				}
				break;
			case "CancelBtn":
			default:
				Back();
				if (_cancelCallback != null) {
					_cancelCallback();
					_cancelCallback = null;
				}
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

		public void UpdateData(string msg, System.Action sureCallback, System.Action cancelCallback, string sureBtnVale, string cancelBtnVale) {
			_msg = msg;
			_sureCallback = sureCallback;
			_cancelCallback = cancelCallback;
			_sureBtnValue = sureBtnVale;
			_cancelBtnValue = cancelBtnVale;
		}

		public override void RefreshView () {
			msg.text = _msg;
			sureBtnText.text = _sureBtnValue;
			cancelBtnText.text = _cancelBtnValue;
		}

		public static void Show(string msg, System.Action sureCallback, System.Action cancelCallback = null, string sureBtnVale = "不行", string cancelBtnVale = "好的") {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Comm/ConfirmView", "ConfirmCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(msg, sureCallback, cancelCallback, sureBtnVale, cancelBtnVale);
			Ctrl.RefreshView();
		}

	}
}
