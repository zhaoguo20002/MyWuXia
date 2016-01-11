#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GameEditor {
	public class Base {
		/// <summary>
		/// Reads the names.
		/// </summary>
		/// <returns>The names.</returns>
		public static List<string> ReadNames() {
			List<string> temp = new List<string>();
			foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
				if (S.enabled) {
					string name = S.path.Substring(S.path.LastIndexOf('/')+1);
					name = name.Substring(0,name.Length-6);
					temp.Add(name);
					Debug.LogWarning(S.path);
				}
			}
			return temp;
		}

		/// <summary>
		/// Tops the view.
		/// </summary>
		public static void TopView() {
			SetView(new Vector3(90, 0, 0), Vector3.zero);
		}

		/// <summary>
		/// Sets the view.
		/// </summary>
		/// <param name="rota">Rota.</param>
		public static void SetView(Vector3 rota, Vector3 pos, float size = 10, bool orthographic = true) {
			SceneView.lastActiveSceneView.rotation = Quaternion.Euler(rota.x, rota.y, rota.z);
			SceneView.lastActiveSceneView.pivot = pos;
			SceneView.lastActiveSceneView.size = size; // unit
			SceneView.lastActiveSceneView.orthographic = orthographic; // or false
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="fileName">File name.</param>
		/// <param name="text">Text.</param>
		public static void CreateFile(string path, string fileName, string text) {
			StreamWriter sw;
			FileInfo file = new FileInfo(path + "//" + fileName);
//			if (!file.Exists) {
			sw = file.CreateText();
//			}
//			else {
//				sw = file.AppendText(); 
//			}
			sw.Write(text);
			sw.Close();
			sw.Dispose();
			AssetDatabase.Refresh();
		}
	}
}
#endif