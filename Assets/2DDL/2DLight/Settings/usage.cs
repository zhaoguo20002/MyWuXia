#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public class usage
{
	///*

	//[MenuItem("2DDL/Create/Changes")]
	public static void cancgeVal (){
		SerializedObject profile = new SerializedObject(AssetDatabase.LoadAssetAtPath<DynamicLightSetting>("Assets/2DDL/2DLight/Core/2ddlSettings.asset"));
		SerializedProperty prop = profile.FindProperty ("version");
		prop.stringValue = "12323";
		profile.ApplyModifiedProperties ();
		//profile.Update ();
	}
//*/



}
#endif