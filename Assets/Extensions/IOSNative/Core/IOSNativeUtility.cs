//#define SA_DEBUG_MODE
using UnityEngine;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



public class IOSNativeUtility {


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_RedirectToAppStoreRatingPage(string appId);

	[DllImport ("__Internal")]
	private static extern void _ISN_ShowPreloader();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_HidePreloader();


	[DllImport ("__Internal")]
	private static extern void _ISN_SetApplicationBagesNumber(int count);
	

	#endif


	public static void RedirectToAppStoreRatingPage() {
		RedirectToAppStoreRatingPage(IOSNativeSettings.Instance.AppleId);
	}

	public static void RedirectToAppStoreRatingPage(string appleId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_RedirectToAppStoreRatingPage(appleId);
		#endif
	}

	public static void SetApplicationBagesNumber(int count) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_SetApplicationBagesNumber(count);
		#endif
	}



	public static void ShowPreloader() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowPreloader();
		#endif
	}
	
	public static void HidePreloader() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_HidePreloader();
		#endif
	}


}
