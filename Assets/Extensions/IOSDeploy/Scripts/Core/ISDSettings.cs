using UnityEngine;
using System.IO;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class ISDSettings : ScriptableObject
{

	public const string VERSION_NUMBER = "1.6";

	public bool IsfwSettingOpen;
	public bool IsLibSettingOpen;
	public bool IslinkerSettingOpne;
	public bool IscompilerSettingsOpen;
	public bool IsPlistSettingsOpen = true;

	public List<string> frameworks =  new List<string>();
	public List<string> libraries =  new List<string>();
	public List<string> compileFlags =  new List<string>();
	public List<string> linkFlags =  new List<string>();
	public List<string> plistkeys = new List<string> ();
	public List<string> plisttags = new List<string> ();
	public List<string> plistvalues = new List<string> ();


	private const string ISDAssetPath = "Extensions/IOSDeploy/Resources";
	private const string ISDAssetName = "ISDSettingsResource";
	private const string ISDAssetExtension = ".asset";

	private static ISDSettings instance;

	public static ISDSettings Instance
	{
		get
		{
			if(instance == null)
			{
				instance = Resources.Load(ISDAssetName) as ISDSettings;
				if(instance == null)
				{
					instance = CreateInstance<ISDSettings>();
					#if UNITY_EDITOR


					FileStaticAPI.CreateFolder(ISDAssetPath);
					
					string fullPath = Path.Combine(Path.Combine("Assets", ISDAssetPath), ISDAssetName + ISDAssetExtension );
					
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif

				}
			}
			return instance;
		}
	}


}
