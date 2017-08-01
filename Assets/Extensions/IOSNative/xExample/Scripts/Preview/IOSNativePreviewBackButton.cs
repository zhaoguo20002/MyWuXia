using UnityEngine;
using System.Collections;

public class IOSNativePreviewBackButton : BaseIOSFeaturePreview {


	private string initialSceneName = "scene";

	public static IOSNativePreviewBackButton Create() {
		return new GameObject("BackButton").AddComponent<IOSNativePreviewBackButton>();
	} 


	void Awake() {
		DontDestroyOnLoad(gameObject);
		initialSceneName = Application.loadedLevelName;
	}


	void OnGUI() {
		float bw = 120;
		float x = Screen.width - bw * 1.2f ;
		float y = bw * 0.2f;


		if(!Application.loadedLevelName.Equals(initialSceneName)) {
			Color customColor = GUI.color;
			GUI.color = Color.green;

			if(GUI.Button(new Rect(x, y, bw, bw * 0.4f), "Back")) {
				Application.LoadLevel(initialSceneName);
			}

			GUI.color = customColor;

		}
	}
	 


}
