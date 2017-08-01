
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(IOSNativeSettings))]
public class IOSNativeSettingsEditor : Editor {




	GUIContent AppleIdLabel = new GUIContent("Apple Id [?]:", "Your Application Apple ID.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail");



	GUIContent SKPVDLabel = new GUIContent("Store Products View [?]:", "The SKStoreProductViewController class makes it possible to integrate purchasing from Apple’s iTunes, App and iBooks stores directly into iOS 6 applications with minimal coding work.");
	GUIContent CheckInternetLabel = new GUIContent("Check Internet Connection[?]:", "If set to true, the Internet connection will be checked before sending load request. Requests will be sent automatically if network becomes available.");
	GUIContent SendBillingFakeActions = new GUIContent("Send Fake Action In Editor[?]:", "Fake connect and purchase events will be fired in the editor, can be useful for testing your implementation in Editor.");

	GUIContent UseGCCaching  = new GUIContent("Use Requests Caching[?]:", "Requests to Game Center will be cached if no Internet connection is available. Requests will be resent on the next Game Center connect event.");

	GUIContent AutoLoadSmallImagesLoadTitle  = new GUIContent("Autoload Small Player Photo[?]:", "As soon as player info received, small player photo will be requested automatically");
	GUIContent AutoLoadBigmagesLoadTitle  = new GUIContent("Autoload Big Player Photo[?]:", "As soon as player info received, big player photo will be requested automatically");
	


	GUIContent DisablePluginLogsNote  = new GUIContent("Disable Plugin Logs[?]:", "All plugins 'Debug.Log' lines will be disabled if this option is enabled.");



	private static string IOSNotificationController_Path = "Extensions/IOSNative/Notifications/IOSNotificationController.cs";
	private static string DeviceTokenListener_Path = "Extensions/IOSNative/Notifications/DeviceTokenListener.cs";

	private static string GameCenterManager_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenterManager.cs";
	private static string GameCenter_TBM_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenter_TBM.cs";
	private static string GameCenter_RTM_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenter_RTM.cs";

	private static string IOSNativeMarketBridge_Path = "Extensions/IOSNative/Market/IOSNativeMarketBridge.cs";
	private static string IOSStoreProductView_Path = "Extensions/IOSNative/Market/IOSStoreProductView.cs";
	private static string ISN_Security_Path = "Extensions/IOSNative/Other/System/ISN_Security.cs";


	private static string iAdBannerControllerr_Path = "Extensions/IOSNative/iAd/iAdBannerController.cs";
	private static string iAdBanner_Path = "Extensions/IOSNative/iAd/iAdBanner.cs";

	private static string IOSSocialManager_Path = "Extensions/IOSNative/Social/IOSSocialManager.cs";

	private static string IOSCamera_Path = "Extensions/IOSNative/Other/Camera/IOSCamera.cs";


	private static string IOSVideoManager_Path = "Extensions/IOSNative/Other/VIdeo/IOSVideoManager.cs";
	private static string ISN_MediaController_Path = "Extensions/IOSNative/Other/Media/Controllers/ISN_MediaController.cs";


	private static string ISN_ReplayKit = "Extensions/IOSNative/Other/VIdeo/ISN_ReplayKit.cs";


	private IOSNativeSettings settings;

	void Awake() {
		#if !UNITY_WEBPLAYER
		UpdatePluginSettings();
		#endif
	}



	public static void UpdatePluginSettings() {


		if(!IsInstalled || !IsUpToDate) {
			return;
		}


		ChnageDefineState(IOSNotificationController_Path, 		"PUSH_ENABLED", IOSNativeSettings.Instance.EnablePushNotificationsAPI);
		ChnageDefineState(DeviceTokenListener_Path,		 	"PUSH_ENABLED", IOSNativeSettings.Instance.EnablePushNotificationsAPI);


		ChnageDefineState(GameCenterManager_Path, 				"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);
		ChnageDefineState(GameCenter_TBM_Path, 				"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);
		ChnageDefineState(GameCenter_RTM_Path, 				"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);

		ChnageDefineState(IOSNativeMarketBridge_Path, 			"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);
		ChnageDefineState(IOSStoreProductView_Path, 			"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);
		ChnageDefineState(ISN_Security_Path, 					"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);



		ChnageDefineState(iAdBannerControllerr_Path, 			"IAD_API", IOSNativeSettings.Instance.EnableiAdAPI);
		ChnageDefineState(iAdBanner_Path, 						"IAD_API", IOSNativeSettings.Instance.EnableiAdAPI);

		ChnageDefineState(IOSSocialManager_Path, 				"SOCIAL_API", IOSNativeSettings.Instance.EnableSocialSharingAPI);

		ChnageDefineState(IOSCamera_Path, 						"CAMERA_API", IOSNativeSettings.Instance.EnableCameraAPI);

		ChnageDefineState(IOSVideoManager_Path, 				"VIDEO_API", IOSNativeSettings.Instance.EnableMediaPlayerAPI);
		ChnageDefineState(ISN_MediaController_Path, 			"VIDEO_API", IOSNativeSettings.Instance.EnableMediaPlayerAPI);

		ChnageDefineState(ISN_ReplayKit, 						"REPLAY_KIT", IOSNativeSettings.Instance.EnableReplayKit);





		if(!IOSNativeSettings.Instance.EnableGameCenterAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_GameCenter");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
		}


		if(!IOSNativeSettings.Instance.EnableInAppsAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_InApp");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_InApp.mm");
		}


		if(!IOSNativeSettings.Instance.EnableiAdAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_iAd");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_iAd.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_iAd.mm");
		}


		if(!IOSNativeSettings.Instance.EnableCameraAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_Camera");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Camera.mm");
		}

		if(!IOSNativeSettings.Instance.EnableSocialSharingAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_SocialGate");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
		}

		if(!IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_Media");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Media.mm");
		}

		if(!IOSNativeSettings.Instance.EnableReplayKit) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_ReplayKit");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
		}

	}

	
	private static void ChnageDefineState(string file, string tag, bool IsEnabled) {
		
		string content = FileStaticAPI.Read(file);
		
		int endlineIndex;
		endlineIndex = content.IndexOf(System.Environment.NewLine);
		if(endlineIndex == -1) {
			endlineIndex = content.IndexOf("\n");
		}
		string TagLine = content.Substring(0, endlineIndex);
		
		if(IsEnabled) {
			content 	= content.Replace(TagLine, "#define " + tag);
		} else {
			content 	= content.Replace(TagLine, "//#define " + tag);
		}
		
		FileStaticAPI.Write(file, content);
		
	}

	public override void OnInspectorGUI()  {


		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing IOS Native Settings not available with web player platfrom. Please switch to any other platform under Build Settings menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To IOS Platfrom",  GUILayout.Width(150))) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		}
		EditorGUILayout.EndHorizontal();

		if(Application.isEditor) {
			return;
		}

		#endif


		settings = target as IOSNativeSettings;

		GUI.changed = false;



		GeneralOptions();

		EditorGUILayout.HelpBox("(Optional) Services Settings", MessageType.None);
		APISettings();
		EditorGUILayout.Space();
		OtherSettins();
		EditorGUILayout.Space();
		BillingSettings();
		EditorGUILayout.Space();
		GameCenterSettings();
		EditorGUILayout.Space();
		CameraSettins();
		EditorGUILayout.Space();

		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}

	}




	private void GeneralOptions() {

		if(!IsInstalled) {
			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(120))) {
				ISN_Plugin_Install();
			}
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
		}

		if(IsInstalled) {
			if(!IsUpToDate) {

				
				EditorGUILayout.HelpBox("Update Required \nResources version: " + SA_VersionsManager.ISN_StringVersionId + " Plugin version: " + IOSNativeSettings.VERSION_NUMBER, MessageType.Warning);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;

				if(CurrentMagorVersion != SA_VersionsManager.ISN_MagorVersion) {
					if(GUILayout.Button("How to update",  GUILayout.Width(250))) {
						Application.OpenURL("https://goo.gl/GsUcA0");
					}
				} else {
					if(GUILayout.Button("Upgrade Resources",  GUILayout.Width(250))) {
						ISN_Plugin_Install();
					}
				}

				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				
			} else {
				EditorGUILayout.HelpBox("IOS Native Plugin v" + IOSNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);

			}
		}
		
		
		EditorGUILayout.Space();



		EditorGUILayout.HelpBox("(Required) Application Data", MessageType.None);

	

		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AppleIdLabel);
		settings.AppleId	 	= EditorGUILayout.TextField(settings.AppleId);
		if(settings.AppleId.Length > 0) {
			settings.AppleId		= settings.AppleId.Trim();
		}

		EditorGUILayout.EndHorizontal();




		EditorGUILayout.Space();

	}


	public static void APISettings() {
		//IOSNativeSettings.
		IOSNativeSettings.Instance.ExpandAPISettings = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ExpandAPISettings, "IOS Native Libs");
		if(IOSNativeSettings.Instance.ExpandAPISettings) {
			EditorGUI.indentLevel++;


			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			GUI.enabled = false;
			EditorGUILayout.Toggle("ISN Basic Features",  true);
			GUI.enabled = true;

			IOSNativeSettings.Instance.EnableGameCenterAPI = EditorGUILayout.Toggle("Game Center",  IOSNativeSettings.Instance.EnableGameCenterAPI);
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableInAppsAPI = EditorGUILayout.Toggle("In-App Purchases",  IOSNativeSettings.Instance.EnableInAppsAPI);
			IOSNativeSettings.Instance.EnableSocialSharingAPI = EditorGUILayout.Toggle("Social Sharing",  IOSNativeSettings.Instance.EnableSocialSharingAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableCameraAPI = EditorGUILayout.Toggle("Camera And Gallery",  IOSNativeSettings.Instance.EnableCameraAPI);
			IOSNativeSettings.Instance.EnableiAdAPI = EditorGUILayout.Toggle("iAd",  IOSNativeSettings.Instance.EnableiAdAPI);
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableMediaPlayerAPI = EditorGUILayout.Toggle("Media Player",  IOSNativeSettings.Instance.EnableMediaPlayerAPI);
			IOSNativeSettings.Instance.EnablePushNotificationsAPI = EditorGUILayout.Toggle("Push Notifications",  IOSNativeSettings.Instance.EnablePushNotificationsAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableReplayKit = EditorGUILayout.Toggle("Replay Kit ",  IOSNativeSettings.Instance.EnableReplayKit);

			EditorGUILayout.EndHorizontal();


			if(EditorGUI.EndChangeCheck()) {
				UpdatePluginSettings();
			}
		
		
			EditorGUI.indentLevel--;
		}
	}




	public static void CameraAndGallery() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max Loaded Image Size");
		IOSNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(IOSNativeSettings.Instance.MaxImageLoadSize);
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Loaded Image Format");
		IOSNativeSettings.Instance.GalleryImageFormat	 	= (IOSGalleryLoadImageFormat) EditorGUILayout.EnumPopup(IOSNativeSettings.Instance.GalleryImageFormat);
		EditorGUILayout.EndHorizontal();


		if(IOSNativeSettings.Instance.GalleryImageFormat == IOSGalleryLoadImageFormat.JPEG) {
			GUI.enabled = true;
		} else {
			GUI.enabled = false;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("JPEG Compression Rate");
		IOSNativeSettings.Instance.JPegCompressionRate	 	= EditorGUILayout.Slider(IOSNativeSettings.Instance.JPegCompressionRate, 0f, 1f);
		EditorGUILayout.EndHorizontal();
		GUI.enabled = true;

	}





	private void GameCenterSettings() {
		IOSNativeSettings.Instance.ShowGCParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowGCParams, "Game Center");
		if (IOSNativeSettings.Instance.ShowGCParams) {
		
			EditorGUI.indentLevel++;


			IOSNativeSettings.Instance.ShowUsersParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowUsersParams, "Players");
			if (IOSNativeSettings.Instance.ShowUsersParams) {
			
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(AutoLoadBigmagesLoadTitle);
				IOSNativeSettings.Instance.AutoLoadUsersBigImages = EditorGUILayout.Toggle(IOSNativeSettings.Instance.AutoLoadUsersBigImages);
				EditorGUILayout.EndHorizontal();
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(AutoLoadSmallImagesLoadTitle);
				IOSNativeSettings.Instance.AutoLoadUsersSmallImages = EditorGUILayout.Toggle(IOSNativeSettings.Instance.AutoLoadUsersSmallImages);
				EditorGUILayout.EndHorizontal();
			}

			IOSNativeSettings.Instance.ShowAchievementsParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowAchievementsParams, "Achievements");
			if (IOSNativeSettings.Instance.ShowAchievementsParams) {
				if(IOSNativeSettings.Instance.RegisteredAchievementsIds.Count == 0) {
					EditorGUILayout.HelpBox("No Achievement IDs Registered", MessageType.Info);
				}
				
				
				int i = 0;
				foreach(string str in IOSNativeSettings.Instance.RegisteredAchievementsIds) {
					EditorGUILayout.BeginHorizontal();
					IOSNativeSettings.Instance.RegisteredAchievementsIds[i]	 	= EditorGUILayout.TextField(IOSNativeSettings.Instance.RegisteredAchievementsIds[i]);
					if(IOSNativeSettings.Instance.RegisteredAchievementsIds[i].Length > 0) {
						IOSNativeSettings.Instance.RegisteredAchievementsIds[i]		= IOSNativeSettings.Instance.RegisteredAchievementsIds[i].Trim();
					}

					if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
						IOSNativeSettings.Instance.RegisteredAchievementsIds.Remove(str);
						break;
					}
					EditorGUILayout.EndHorizontal();
					i++;
				}
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add",  GUILayout.Width(80))) {
					IOSNativeSettings.Instance.RegisteredAchievementsIds.Add("");
				}
				EditorGUILayout.EndHorizontal();


				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(UseGCCaching);
				IOSNativeSettings.Instance.UseGCRequestCaching = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UseGCRequestCaching);
				EditorGUILayout.EndHorizontal();
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Save progress in PlayerPrefs[?]");
				IOSNativeSettings.Instance.UsePPForAchievements = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UsePPForAchievements);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Read More",  GUILayout.Width(100))) {
					Application.OpenURL("http://goo.gl/3nq260");
				}
				EditorGUILayout.EndHorizontal();

			}

			EditorGUI.indentLevel--;




		}
	}

	private void CameraSettins() {
		IOSNativeSettings.Instance.ShowCameraAndGalleryParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowCameraAndGalleryParams, "Camera And Gallery");
		if (IOSNativeSettings.Instance.ShowCameraAndGalleryParams) {
			
			CameraAndGallery();
		}
	}

	private void OtherSettins() {


		
		IOSNativeSettings.Instance.ShowOtherParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowOtherParams, "More Actions");
		if (IOSNativeSettings.Instance.ShowOtherParams) {




			EditorGUI.BeginChangeCheck();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(DisablePluginLogsNote);
			IOSNativeSettings.Instance.DisablePluginLogs = EditorGUILayout.Toggle(IOSNativeSettings.Instance.DisablePluginLogs);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Remove IOS Native",  GUILayout.Width(140))) {
				SA_RemoveTool.RemovePlugins();
			}
			EditorGUILayout.EndHorizontal();



		

		}
	}


	private void BillingSettings() {

		IOSNativeSettings.Instance.ShowStoreKitParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowStoreKitParams, "Billing Settings");
		if(IOSNativeSettings.Instance.ShowStoreKitParams) {

			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No In-App Products Added", MessageType.Warning);
			}
		

			int i = 0;
			foreach(string str in settings.InAppProducts) {
				EditorGUILayout.BeginHorizontal();
				settings.InAppProducts[i]	 	= EditorGUILayout.TextField(settings.InAppProducts[i]);
				if(settings.InAppProducts[i].Length > 0) {
					settings.InAppProducts[i]		= settings.InAppProducts[i].Trim();
				}
			
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.InAppProducts.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.InAppProducts.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(SendBillingFakeActions);
			settings.SendFakeEventsInEditor = EditorGUILayout.Toggle(settings.SendFakeEventsInEditor);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(CheckInternetLabel);
			settings.checkInternetBeforeLoadRequest = EditorGUILayout.Toggle(settings.checkInternetBeforeLoadRequest);
			EditorGUILayout.EndHorizontal();






			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(SKPVDLabel);

			/*****************************************/

			if(settings.DefaultStoreProductsView.Count == 0) {
				EditorGUILayout.HelpBox("No Default Store Products View Added", MessageType.Info);
			}
			
			
			i = 0;
			foreach(string str in settings.DefaultStoreProductsView) {
				EditorGUILayout.BeginHorizontal();
				settings.DefaultStoreProductsView[i]	 	= EditorGUILayout.TextField(settings.DefaultStoreProductsView[i]);
				if(settings.DefaultStoreProductsView[i].Length > 0) {
					settings.DefaultStoreProductsView[i]		= settings.DefaultStoreProductsView[i].Trim();
				}

				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.DefaultStoreProductsView.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.DefaultStoreProductsView.Add("");
			}
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.Space();

		}
	}




	private void AboutGUI() {


		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, IOSNativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(IOSNativeSettings.Instance);
		#endif
	}


	public static bool IsInstalled {
		get {
			return SA_VersionsManager.Is_ISN_Installed;
		}
	}
	
	
	public static bool IsUpToDate {
		get {
			
			if(CurrentVersion == SA_VersionsManager.ISN_Version) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	public static int CurrentVersion {
		get {
			return SA_VersionsManager.ParceVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	public static int CurrentMagorVersion {
		get {
			return SA_VersionsManager.ParceMagorVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	
	public static int Version {
		get {
			return SA_VersionsManager.ISN_Version;
		}
	}


	public static void ISN_Plugin_Install() {

		PluginsInstalationUtil.IOS_UpdatePlugin();
		UpdateVersionInfo();
	}

	public static void UpdateVersionInfo() {
		FileStaticAPI.Write(SA_VersionsManager.ISN_VERSION_INFO_PATH, IOSNativeSettings.VERSION_NUMBER);
	}
	
	
}
