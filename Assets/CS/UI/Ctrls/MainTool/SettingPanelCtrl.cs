using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
	public class SettingPanelCtrl : WindowCore<SettingPanelCtrl, JArray> {
		Image bg;
		Button block;
		Button closeBtn;
		Button bGMOpenBtn;
		Button bGMCloseBtn;
		Button soundOpenBtn;
		Button soundCloseBtn;
		Button loadRecordListBtn;
		Button backToMainMenuBtn;
        Button cameraBtn;
        Button helpBtn;
        Button praiseBtn;
        Toggle effectToggle;
        Button raidersBtn;

		bool showBackMainTool;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;

			bGMOpenBtn = GetChildButton("BGMOpenBtn");
			EventTriggerListener.Get(bGMOpenBtn.gameObject).onClick = onClick;
			bGMCloseBtn = GetChildButton("BGMCloseBtn");
			EventTriggerListener.Get(bGMCloseBtn.gameObject).onClick = onClick;
			soundOpenBtn = GetChildButton("SoundOpenBtn");
			EventTriggerListener.Get(soundOpenBtn.gameObject).onClick = onClick;
			soundCloseBtn = GetChildButton("SoundCloseBtn");
			EventTriggerListener.Get(soundCloseBtn.gameObject).onClick = onClick;
			loadRecordListBtn = GetChildButton("LoadRecordListBtn");
			EventTriggerListener.Get(loadRecordListBtn.gameObject).onClick = onClick;
			backToMainMenuBtn = GetChildButton("BackToMainMenuBtn");
			EventTriggerListener.Get(backToMainMenuBtn.gameObject).onClick = onClick;
            cameraBtn = GetChildButton("CameraBtn");
            EventTriggerListener.Get(cameraBtn.gameObject).onClick = onClick;
            helpBtn = GetChildButton("HelpBtn");
            EventTriggerListener.Get(helpBtn.gameObject).onClick = onClick;
            praiseBtn = GetChildButton("PraiseBtn");
            EventTriggerListener.Get(praiseBtn.gameObject).onClick = onClick;
            effectToggle = GetChildToggle("effectToggle");
            effectToggle.onValueChanged.AddListener((check) => {
                PlayerPrefs.SetString("EffectSwitchOffFlag", check ? "" : "true");
            });
            effectToggle.isOn = string.IsNullOrEmpty(PlayerPrefs.GetString("EffectSwitchOffFlag"));

            raidersBtn = GetChildButton("raidersBtn");
            EventTriggerListener.Get(raidersBtn.gameObject).onClick = onClick;
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
                case "BGMOpenBtn":
                    SoundManager.GetInstance().EnableBGM();
//				Messenger.Broadcast(NotifyTypes.PlayBgm);
                    refreshBGMAndSoundView();
                    break;
                case "BGMCloseBtn":
                    SoundManager.GetInstance().DisableBGM();
                    refreshBGMAndSoundView();
                    break;
                case "SoundOpenBtn":
                    SoundManager.GetInstance().EnableSound();
                    refreshBGMAndSoundView();
                    break;
                case "SoundCloseBtn":
                    SoundManager.GetInstance().DisableSound();
                    refreshBGMAndSoundView();
                    break;
                case "LoadRecordListBtn":
                    Messenger.Broadcast(NotifyTypes.GetRecordListData);
                    break;
                case "BackToMainMenuBtn":
                    Messenger.Broadcast(NotifyTypes.ShowMainPanel);
                    break;
                case "CameraBtn":
                    MaiHandler.ShowInterstitial();
                    MaiHandler.SendEvent("StartInterstitialForHelp", DbManager.Instance.HostData.Lv.ToString());
                    break;
                case "HelpBtn":
                    HelpPanelCtrl.Show();
                    break;
                case "PraiseBtn":
                    const string APP_ID = "1274001919";
                    var url = string.Format(
                        "https://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={0}&pageNumber=0&sortOrdering=2&type=Purple+Software&mt=8",
                        APP_ID);
                    Application.OpenURL(url);
                    break;
                case "raidersBtn":
                    Application.OpenURL("http://mywuxia.lofter.com");
                    break;
                default:
                    break;
            }
		}

		public void UpdateData(bool flag = true) {
			showBackMainTool = flag;
		}

		void refreshBGMAndSoundView() {
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("DisableBGM"))) {
				bGMOpenBtn.gameObject.SetActive(false);
				bGMCloseBtn.gameObject.SetActive(true);
			}
			else {
				bGMOpenBtn.gameObject.SetActive(true);
				bGMCloseBtn.gameObject.SetActive(false);
			}
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("DisableSound"))) {
				soundOpenBtn.gameObject.SetActive(false);
				soundCloseBtn.gameObject.SetActive(true);
			}
			else {
				soundOpenBtn.gameObject.SetActive(true);
				soundCloseBtn.gameObject.SetActive(false);
			}
		}

		public override void RefreshView () {
			refreshBGMAndSoundView();
			MakeButtonEnable(loadRecordListBtn, showBackMainTool);
			MakeButtonEnable(backToMainMenuBtn, showBackMainTool);
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

		public static void Show(bool flag = true) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/SettingPanelView", "SettingPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(flag);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
