#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using Game;
using UnityEditor;

public class SoundTestMain : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Statics.Init();
		SoundManager.GetInstance().PushSound(PlayerPrefs.GetString("SoundEditorPlaySoundId"));
	}
}
#endif