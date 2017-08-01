using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(ISDSettings))]
public class ISDSettingsEditor : Editor
{

	
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");
	



	public override void OnInspectorGUI () {


		GUI.changed = false;


		GUI.enabled = false;

		EditorGUILayout.LabelField("IOS Deploy Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		Frameworks();
		EditorGUILayout.Space();
		Library ();
		EditorGUILayout.Space();
		LinkerFlags();
		EditorGUILayout.Space();
		CompilerFlags();
		EditorGUILayout.Space();
		PlistValues ();
		EditorGUILayout.Space();

		GUI.enabled = true;
		AboutGUI();

		if(GUI.changed) {
			DirtyEditor();
		}



	}


	private string newFreamwork = string.Empty;
	private void Frameworks() {
		

		ISDSettings.Instance.IsfwSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsfwSettingOpen, "Frameworks");

		if(ISDSettings.Instance.IsfwSettingOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {

				EditorGUILayout.HelpBox("No Frameworks added", MessageType.None);
			}


			foreach(string framework in ISDSettings.Instance.frameworks) {
				

				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(framework, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.frameworks.Remove(framework);
					return;
				}

				EditorGUILayout.EndHorizontal();


				EditorGUILayout.EndVertical ();
				
			}
				
		
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Framework");
			newFreamwork = EditorGUILayout.TextField(newFreamwork, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();





			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.frameworks.Contains(newFreamwork) && newFreamwork.Length > 0) {
					ISDSettings.Instance.frameworks.Add(newFreamwork);
					newFreamwork = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLibrary = string.Empty;
	void Library ()
	{
		ISDSettings.Instance.IsLibSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsLibSettingOpen, "Libraries");

		if(ISDSettings.Instance.IsLibSettingOpen)
		{
			if (ISDSettings.Instance.libraries.Count == 0) {
				
				EditorGUILayout.HelpBox("No Libraries added", MessageType.None);
			}

			foreach(string lib in ISDSettings.Instance.libraries) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(lib, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.libraries.Remove(lib);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical ();
				
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Fibrary");
			NewLibrary = EditorGUILayout.TextField(NewLibrary, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			
			
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.libraries.Contains(NewLibrary) && NewLibrary.Length > 0) {
					ISDSettings.Instance.libraries.Add(NewLibrary);
					NewLibrary = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLinkerFlag = string.Empty;
	private void LinkerFlags() {
		
		
		ISDSettings.Instance.IslinkerSettingOpne = EditorGUILayout.Foldout(ISDSettings.Instance.IslinkerSettingOpne, "Linker Flags");
		
		if(ISDSettings.Instance.IslinkerSettingOpne) {
			if (ISDSettings.Instance.linkFlags.Count == 0) {
				
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			

			foreach(string flasg in ISDSettings.Instance.linkFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.linkFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewLinkerFlag = EditorGUILayout.TextField(NewLinkerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			
			
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.linkFlags.Contains(NewLinkerFlag) && NewLinkerFlag.Length > 0) {
					ISDSettings.Instance.linkFlags.Add(NewLinkerFlag);
					NewLinkerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewCompilerFlag = string.Empty;
	private void CompilerFlags() {
		
		
		ISDSettings.Instance.IscompilerSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IscompilerSettingsOpen, "Compiler Flags");
		
		if(ISDSettings.Instance.IscompilerSettingsOpen) {
			if (ISDSettings.Instance.compileFlags.Count == 0) {
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			
			

			foreach(string flasg in ISDSettings.Instance.compileFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.compileFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewCompilerFlag = EditorGUILayout.TextField(NewCompilerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.compileFlags.Contains(NewCompilerFlag) && NewCompilerFlag.Length > 0) {
					ISDSettings.Instance.compileFlags.Add(NewCompilerFlag);
					NewCompilerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewPlistValue = string.Empty;
	private string NewPlistStringValue = string.Empty;
	private int NewPlistIntValue = 0;
	private float NewPlistFloatValue = 0;
	private bool NewPlistBoolValue = false;
	private PlistValueTypes type = PlistValueTypes.String;
	void PlistValues ()
	{
		ISDSettings.Instance.IsPlistSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsPlistSettingsOpen, "Plist values");
		
		if(ISDSettings.Instance.IsPlistSettingsOpen) {
			if (ISDSettings.Instance.plistkeys.Count == 0) {
				EditorGUILayout.HelpBox("No Plist values added", MessageType.None);
			}
			
			
			
			for(int i = 0; i < ISDSettings.Instance.plistkeys.Count; i++) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(ISDSettings.Instance.plistkeys[i] + ": " + ISDSettings.Instance.plistvalues[i], GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.plistkeys.Remove (ISDSettings.Instance.plistkeys[i]);
					ISDSettings.Instance.plistkeys.Remove (ISDSettings.Instance.plisttags[i]);
					ISDSettings.Instance.plistvalues.Remove (ISDSettings.Instance.plistvalues[i]);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
				
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Add New Value");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Key");
			NewPlistValue = EditorGUILayout.TextField(NewPlistValue);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.BeginChangeCheck ();
			type = (PlistValueTypes)EditorGUILayout.EnumPopup ("Type", type);
			if(EditorGUI.EndChangeCheck())
			{
				NewPlistStringValue = string.Empty;
				NewPlistIntValue = 0;
				NewPlistFloatValue = 0;
				NewPlistBoolValue = false;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Value");
			switch(type)
			{
			case PlistValueTypes.Boolean:
				NewPlistBoolValue = EditorGUILayout.Toggle (NewPlistBoolValue);
				break;
				
			case PlistValueTypes.Float:
				NewPlistFloatValue = EditorGUILayout.FloatField(NewPlistFloatValue);
				break;
				
			case PlistValueTypes.Integer:
				NewPlistIntValue = EditorGUILayout.IntField (NewPlistIntValue);
				break;
				
			case PlistValueTypes.String:
				NewPlistStringValue = EditorGUILayout.TextField (NewPlistStringValue);
				break;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.plistkeys.Contains(NewPlistValue) && NewPlistValue.Length > 0) {
					ISDSettings.Instance.plistkeys.Add (NewPlistValue);
					switch(type)
					{
					case PlistValueTypes.Boolean:
						ISDSettings.Instance.plistvalues.Add (NewPlistBoolValue.ToString ().ToLower ());
						ISDSettings.Instance.plisttags.Add (string.Empty);
						NewPlistBoolValue = false;
						break;

					case PlistValueTypes.Float:
						ISDSettings.Instance.plistvalues.Add (NewPlistFloatValue.ToString ());
						ISDSettings.Instance.plisttags.Add ("real");
						NewPlistFloatValue = 0;
						break;

					case PlistValueTypes.Integer:
						ISDSettings.Instance.plistvalues.Add (NewPlistIntValue.ToString ());
						ISDSettings.Instance.plisttags.Add ("integer");
						NewPlistIntValue = 0;
						break;

					case PlistValueTypes.String:
						ISDSettings.Instance.plistvalues.Add (NewPlistStringValue.ToString ());
						ISDSettings.Instance.plisttags.Add ("string");
						NewPlistStringValue = string.Empty;
						break;
					}
					NewPlistValue = string.Empty;
				}

			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private void AboutGUI() {
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, ISDSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Note: This version of IOS Deploy designed for Stan's Assets");
		EditorGUILayout.LabelField("plugins internal use only. If you want to use IOS Deploy  ");
		EditorGUILayout.LabelField("for your project needs, please, ");
		EditorGUILayout.LabelField("purchase a copy of IOS Deploy plugin.");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();

		if(GUILayout.Button("Documentation",  GUILayout.Width(150))) {
			Application.OpenURL("https://goo.gl/sOJFXJ");
		}

		if(GUILayout.Button("Purchase",  GUILayout.Width(150))) {
			Application.OpenURL("https://goo.gl/Nqbuuv");
		}

		EditorGUILayout.EndHorizontal();

	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}


	



	private static void DirtyEditor() {
			#if UNITY_EDITOR
		EditorUtility.SetDirty(ISDSettings.Instance);
			#endif
	}
}
