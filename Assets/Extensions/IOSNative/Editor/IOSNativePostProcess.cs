using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		if(IOSNativeSettings.Instance.EnableInAppsAPI) {

			string StoreKit = "StoreKit.framework";
			if(!ISDSettings.Instance.frameworks.Contains(StoreKit)) {
				ISDSettings.Instance.frameworks.Add(StoreKit);
			}

		}

		if(IOSNativeSettings.Instance.EnableGameCenterAPI) {
			
			string GameKit = "GameKit.framework";
			if(!ISDSettings.Instance.frameworks.Contains(GameKit)) {
				ISDSettings.Instance.frameworks.Add(GameKit);
			}
			
		}



		if(IOSNativeSettings.Instance.EnableSocialSharingAPI) {
		
			string Accounts = "Accounts.framework";
			if(!ISDSettings.Instance.frameworks.Contains(Accounts)) {
				ISDSettings.Instance.frameworks.Add(Accounts);
			}

			
			
			string SocialF = "Social.framework";
			if(!ISDSettings.Instance.frameworks.Contains(SocialF)) {
				ISDSettings.Instance.frameworks.Add(SocialF);
			}
			
			string MessageUI = "MessageUI.framework";
			if(!ISDSettings.Instance.frameworks.Contains(MessageUI)) {
				ISDSettings.Instance.frameworks.Add(MessageUI);
			}
		}


		if(IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			string MediaPlayer = "MediaPlayer.framework";
			if(!ISDSettings.Instance.frameworks.Contains(MediaPlayer)) {
				ISDSettings.Instance.frameworks.Add(MediaPlayer);
			}
		}
	

		if(IOSNativeSettings.Instance.EnableCameraAPI) {
			string MobileCoreServices = "MobileCoreServices.framework";
			if(!ISDSettings.Instance.frameworks.Contains(MobileCoreServices)) {
				ISDSettings.Instance.frameworks.Add(MobileCoreServices);
			}
		}

		if(IOSNativeSettings.Instance.EnableReplayKit) {
			string ReplayKit = "ReplayKit.framework";
			if(!ISDSettings.Instance.frameworks.Contains(ReplayKit)) {
				ISDSettings.Instance.frameworks.Add(ReplayKit);
			}
		}


		Debug.Log("ISN Postprocess Done");

	
	}
	#endif
}
