using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//Attach the script to the empty gameobject on your sceneS
public class iAdIOSInterstitial : MonoBehaviour {


	// --------------------------------------
	// Unity Events
	// --------------------------------------


	void Start() {
		ShowBanner();
	}




	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		iAdBannerController.instance.StartInterstitialAd();
	}



	// --------------------------------------
	// GET / SET
	// --------------------------------------



	public string sceneBannerId {
		get {
			return Application.loadedLevelName + "_" + this.gameObject.name;
		}
	}

	
}
