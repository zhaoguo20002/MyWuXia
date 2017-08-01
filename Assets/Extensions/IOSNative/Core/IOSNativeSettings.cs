using UnityEngine;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class IOSNativeSettings : ScriptableObject {

	public const string VERSION_NUMBER = "8.0";

	public string AppleId = "XXXXXXXXX";

	public List<string> InAppProducts = new List<string>();
	public bool SendFakeEventsInEditor = true;
	public List<string> RegisteredAchievementsIds = new List<string>();
	public List<string> DefaultStoreProductsView = new List<string>();


	public bool checkInternetBeforeLoadRequest = false;
	public bool ShowStoreKitParams = false;
	public bool ShowGCParams = false;
	public bool ShowAchievementsParams = false;
	public bool ShowUsersParams = false;
	public bool ShowOtherParams = false;
	public bool ShowCameraAndGalleryParams = false;



	public bool ExpandAPISettings = true;


	public bool EnableGameCenterAPI = true;
	public bool EnableInAppsAPI = true;
	public bool EnableCameraAPI = true;
	public bool EnableSocialSharingAPI = true;
	public bool EnableMediaPlayerAPI = true;
	public bool EnableiAdAPI = true;
	public bool EnableReplayKit = false;


	public bool EnablePushNotificationsAPI = false;


	public bool DisablePluginLogs = false;


	public bool UseGCRequestCaching = false;
	public bool UsePPForAchievements = false;


	public bool AutoLoadUsersSmallImages = true;
	public bool AutoLoadUsersBigImages = false;


	public int  MaxImageLoadSize = 512;
	public float JPegCompressionRate = 0.8f;
	public IOSGalleryLoadImageFormat GalleryImageFormat = IOSGalleryLoadImageFormat.JPEG;




	private const string ISNSettingsAssetName = "IOSNativeSettings";
	private const string ISNSettingsPath = "Extensions/IOSNative/Resources";
	private const string ISNSettingsAssetExtension = ".asset";

	private static IOSNativeSettings instance = null;

	
	public static IOSNativeSettings Instance {

		
		get {
			if (instance == null) {
				instance = Resources.Load(ISNSettingsAssetName) as IOSNativeSettings;
				
				if (instance == null) {
					
					// If not found, autocreate the asset object.
					instance = CreateInstance<IOSNativeSettings>();
					#if UNITY_EDITOR

					FileStaticAPI.CreateFolder(ISNSettingsPath);
				
					/*string properPath = Path.Combine(Application.dataPath, ISNSettingsPath);
					if (!Directory.Exists(properPath)) {
						AssetDatabase.CreateFolder("Extensions/", "IOSNative");
						AssetDatabase.CreateFolder("Extensions/IOSNative", "Resources");
					}
					*/
					
					string fullPath = Path.Combine(Path.Combine("Assets", ISNSettingsPath),
					                               ISNSettingsAssetName + ISNSettingsAssetExtension
					                               );
					
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}

}
