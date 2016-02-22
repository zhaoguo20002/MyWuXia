using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;

public class Global : MonoBehaviour {
	public bool TestModel = false;
	GameObject UICamera;
	GameObject showFps;
	GameObject UICanvas;
	GameObject FrameCanvas;
	GameObject FontCanvas;
	GameObject UIEventSystem;
	void Awake () {
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
	}

	void Start() {
		if (TestModel) {
			return;
		}
		QualitySettings.vSyncCount = -1;
		QualitySettings.maxQueuedFrames = 0;
		Application.targetFrameRate = 30;
		Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
		Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.CallUserData, (userData) => {
			Messenger.Broadcast<string>(NotifyTypes.GoToScene, userData.CurrentAreaSceneName);
		});
	}

	/// <summary>
	/// 处理场景加载完成回调
	/// </summary>
	/// <param name="level">Level.</param>
	void OnLevelWasLoaded(int level) {
		if (UICamera != null) {
			UICamera.GetComponent<Camera>().clearFlags = level > 0 ? CameraClearFlags.Depth : CameraClearFlags.SolidColor;
		}
    }
}
