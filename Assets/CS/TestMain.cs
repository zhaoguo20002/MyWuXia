using UnityEngine;
using System.Collections;
using Game;

public class TestMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Statics.Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (GUI.Button(new Rect(50, 50, 100, 30), "播放")) {
			SoundManager.GetInstance().PlayBGM("");
		}
		if (GUI.Button(new Rect(155, 50, 100, 30), "停止")) {
			SoundManager.GetInstance().StopBGM();
		}
		if (GUI.Button(new Rect(260, 50, 100, 30), "暂停")) {
			SoundManager.GetInstance().PauseBGM();
		}
		if (GUI.Button(new Rect(365, 50, 100, 30), "继续")) {
			SoundManager.GetInstance().UnPauseBGM();
		}
		if (GUI.Button(new Rect(470, 50, 100, 30), "静音")) {
			SoundManager.GetInstance().ToggleBGM();
		}
		if (GUI.Button(new Rect(575, 50, 100, 30), "音效")) {
			SoundManager.GetInstance().PushSound("");
		}
	}
}
