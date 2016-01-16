using UnityEngine;
using System.Collections;
using Game;
using System.Collections.Generic;

public class FightTestMain : MonoBehaviour {
	GameObject UICanvas;
	GameObject FrameCanvas;
	GameObject FontCanvas;
	GameObject UIEventSystem;
	GameObject showFps;
	void Awake () {
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

	// Use this for initialization
	void Start () {
		List<RoleData> roleDatas = new List<RoleData>();
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId0")));
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId1")));
		roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId2")));
		for (int i= 0; i< roleDatas.Count; i++) {
			roleDatas[i].MakeJsonToModel();
		}
		Messenger.Broadcast<RoleData, string>(NotifyTypes.CreateTestBattle, roleDatas[0], PlayerPrefs.GetString("FightEditorCurrentId"));
		RoleInfoPanelCtrl.Show(roleDatas);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
