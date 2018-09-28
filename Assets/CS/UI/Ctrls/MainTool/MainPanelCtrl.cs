using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using UnityEngine.UI;

namespace Game {
	public class MainPanelCtrl : WindowCore<MainPanelCtrl, JArray> {
		string version = "2.3";
		Image bg;
		Image logoBackImage;
		Image logoImage;
		Image poetryImage;
		Image bottomImage;
		Image title1Image;
		Image title2Image;
		Image title3Image;
		Text versionText;
		Image progressImage;
		Button enterButton;
		Button loadRecordsButton;
		Button settingButton;

		protected override void Init () {
			bg = GetComponent<Image>();
			logoBackImage = GetChildImage("LogoBackImage");
			logoImage = GetChildImage("LogoImage");
			poetryImage = GetChildImage("PoetryImage");
			bottomImage = GetChildImage("BottomImage");
			versionText = GetChildText("VersionText");
			progressImage = GetChildImage("ProgressImage");
			enterButton = GetChildButton("EnterButton");
			EventTriggerListener.Get(enterButton.gameObject).onClick = onClick;
			loadRecordsButton = GetChildButton("LoadRecordsButton");
			EventTriggerListener.Get(loadRecordsButton.gameObject).onClick = onClick;
			settingButton = GetChildButton("SettingButton");
			EventTriggerListener.Get(settingButton.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "EnterButton":
				loadRecordsButton.gameObject.SetActive(false);
				settingButton.gameObject.SetActive(false);
				enterButton.gameObject.SetActive(false);
				progressImage.rectTransform.DOKill();
				progressImage.rectTransform.localScale = new Vector3(0, 1, 1);
				progressImage.rectTransform.DOScaleX(1, 1).OnComplete(() => {
					Messenger.Broadcast(NotifyTypes.EnterGame);
				});
				break;
			case "LoadRecordsButton":
				Messenger.Broadcast(NotifyTypes.GetRecordListData);
				break;
			case "SettingButton":
				Messenger.Broadcast<bool>(NotifyTypes.ShowSettingPanel, false);
				break;
			default:
				break;
			}
		}

		public override void RefreshView () {
			SoundManager.GetInstance().PlayBGM("bgm0003");
			bg.color = Color.black;
			logoBackImage.DOFade(1, 0);
			logoImage.DOFade(0, 0);
			logoImage.rectTransform.anchoredPosition = new Vector2(-7, -100);
			poetryImage.DOFade(0, 0);
			bottomImage.DOFade(0, 0);


			bg.DOColor(new Color(0.937f, 0.937f, 0.937f, 1), 1).SetDelay(0.5f);
			logoBackImage.DOFade(0, 1).SetDelay(0.5f);
			logoImage.DOFade(1, 1).SetDelay(0.5f);
			logoImage.rectTransform.DOAnchorPos(new Vector2(220, -20), 1).SetEase(Ease.OutQuad).SetDelay(2);
			bottomImage.DOFade(1, 2).SetDelay(3);
			poetryImage.DOFade(1, 1).SetDelay(3);


			loadRecordsButton.gameObject.SetActive(true);
			loadRecordsButton.image.rectTransform.anchoredPosition = new Vector2(10 + 640, 45);
			loadRecordsButton.image.rectTransform.DOAnchorPos(new Vector2(10, 45), 0.5f).SetEase(Ease.OutQuad).SetDelay(3);
			settingButton.gameObject.SetActive(true);
			settingButton.image.rectTransform.anchoredPosition = new Vector2(134 + 640, 45);
			settingButton.image.rectTransform.DOAnchorPos(new Vector2(134, 45), 0.5f).SetEase(Ease.OutQuad).SetDelay(3.25f);
			enterButton.gameObject.SetActive(true);
			enterButton.image.rectTransform.anchoredPosition = new Vector2(430 + 640, 45);
			enterButton.image.rectTransform.DOAnchorPos(new Vector2(430, 45), 0.5f).SetEase(Ease.OutQuad).SetDelay(3.5f);

			versionText.text = string.Format("版本:{0}", version);
			versionText.DOFade(0, 0);
			versionText.DOFade(1, 1).SetDelay(4);
			progressImage.rectTransform.localScale = new Vector3(0, 1, 1);
		}

		public static void Show() {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/MainPanelView", "MainPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
			}
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
