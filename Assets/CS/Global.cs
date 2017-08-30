using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;

public class Global : MonoBehaviour {
    public static float FrameCost = 0.033f;
	public bool TestModel = false;
	GameObject UICamera;
	GameObject showFps;
	GameObject UICanvas;
	GameObject FrameCanvas;
	GameObject FontCanvas;
	GameObject UIEventSystem;
	void Awake () {
		QualitySettings.vSyncCount = -1;
		QualitySettings.maxQueuedFrames = 0;
		Application.targetFrameRate = 30;
        FrameCost = (float)Statics.ClearError(1.0d / (double)Application.targetFrameRate); //每一帧消耗的固定时间
		UICamera = GameObject.Find("UICamera");
		if (UICamera != null) {
			DontDestroyOnLoad(UICamera);
		}
		showFps = GameObject.Find("ShowFPS");
		if (showFps != null) {
			DontDestroyOnLoad(showFps);
		}
		UICanvas = GameObject.Find("UICanvas");
		if (UICanvas != null) {
			DontDestroyOnLoad(UICanvas);
		}
		FrameCanvas = GameObject.Find("FrameCanvas");
		if (FrameCanvas != null) {
			DontDestroyOnLoad(FrameCanvas);
		}
		FontCanvas = GameObject.Find("FontCanvas");
		if (FontCanvas != null) {
			DontDestroyOnLoad(FontCanvas);
		}
		UIEventSystem = GameObject.Find("UIEventSystem");
		if (UIEventSystem != null) {
			DontDestroyOnLoad(UIEventSystem);
		}

		DontDestroyOnLoad(gameObject);
		Statics.Init();
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DESStatics.Init();
	}

	void Start() {
		if (TestModel) {
			return;
		}
		Messenger.Broadcast(NotifyTypes.ShowMainPanel);
	}

//    System.DateTime backDelayDate = System.DateTime.MinValue;
//    double backDelayTimeout = 12;
    public void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause: " + pause);
        if (pause) { //程序进入后台
            MaiHandler.StopSession();
//            backDelayDate = System.DateTime.Now;
        }
        else { //程序被唤醒
            MaiHandler.StartSession();
//            //大于等于5级每次切换回游戏都弹一次插屏广告
//            if (DbManager.Instance.HostData != null && DbManager.Instance.HostData.Lv >= 5 && (System.DateTime.Now - backDelayDate).TotalSeconds > backDelayTimeout) {
//                backDelayDate = System.DateTime.Now;
//                MaiHandler.ShowInterstitial(false);
//                MaiHandler.SendEvent("StartInterstitialForBack", DbManager.Instance.HostData.Lv.ToString());
//            }
        }
    }

	/// <summary>
	/// 处理场景加载完成回调
	/// </summary>
	/// <param name="level">Level.</param>
	void OnLevelWasLoaded(int level) {
		if (UICamera != null) {
			UICamera.GetComponent<Camera>().clearFlags = level > 0 ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
		}
		if (level > 1) {
			if (UserModel.CurrentUserData != null) {
				DbManager.Instance.CheckEnterArea(UserModel.CurrentUserData.CurrentAreaSceneName);
			}
		}
    }
}
