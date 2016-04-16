#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Game;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel;
using System;

namespace GameEditor {
	public class ToolsEditorWindow : Editor {

		static ModelEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/CreateRandomNames")]
		static void OpenWindow() {
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/FirstNames");
			string firstNamesStr = asset.text.Replace(" ", "|");
			firstNamesStr = firstNamesStr.Replace("\n", "|");
			string[] fen = firstNamesStr.Split(new char[] { '|' });
			if (fen.Length > 0) {
				List<string> firstNames = new List<string>();
				for (int i = 0; i < fen.Length; i++) {
					if (!string.IsNullOrEmpty(fen[i])) {
						firstNames.Add(fen[i]);
					}
				}
				Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "FirstNamesList.json", JsonManager.GetInstance().SerializeObject(firstNames));
				Debug.LogWarning("创建姓完毕！");
			}

			asset = Resources.Load<TextAsset>("Data/Json/SecondNames");
			string secondNamesStr = asset.text.Replace(" ", "|");
			secondNamesStr = secondNamesStr.Replace("\n", "|");
			string[] fen2 = secondNamesStr.Split(new char[] { '|' });
			if (fen2.Length > 0) {
				List<string> secondNames = new List<string>();
				for (int i = 0; i < fen2.Length; i++) {
					if (!string.IsNullOrEmpty(fen2[i])) {
						secondNames.Add(fen2[i]);
					}
				}
				Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "SecondNamesList.json", JsonManager.GetInstance().SerializeObject(secondNames));
				Debug.LogWarning("创建名完毕！");
			}
			asset = null;
		}
	}
}
#endif