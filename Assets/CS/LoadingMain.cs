using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game;
using DG.Tweening.Core;
using DG.Tweening;
public class LoadingMain : MonoBehaviour {
	AsyncOperation asyncObj;
    string sceneName;
	//          Use this for initialization
    void Start() {
		SoundManager.GetInstance().StopBGM();
		sceneName = SceneManagerController.GetInstance().SceneName;
        Invoke("delayDo", Random.Range(0.6f, 1.2f));
//        StartCoroutine(loadScene());
	}

    void delayDo() {
        StartCoroutine(loadScene());
    }

	IEnumerator loadScene () {
        asyncObj = Application.LoadLevelAsync(sceneName);
        yield return asyncObj;
	}

    void OnDestroy() {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

	// Update is called once per frame
    //void Update () {
    //    Debug.Log(asyncObj.progress);
    //}
}
