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
using System.IO;

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

		[MenuItem ("Editors/Clear Cache")]
		static void ClearCache() {
			PlayerPrefs.DeleteAll();
			Debug.LogWarning("PlayerPrefs全部清除！");
		}

        [MenuItem("Editors/MakeCreateSelectedSpriteToIcon")]
        static void DoMakeCreateSelectedSpriteToIcon () {
            UnityEngine.Object[] selections = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            if (selections.Length > 0) {
                string path;
                foreach(var se in selections) {
                    path = AssetDatabase.GetAssetPath(se);
                    //判断是否选中的是文件夹
                    if (Path.GetExtension(path) == "" && path.IndexOf("Assets/Avatars/") == 0) {
                        string[] filePathes =  Directory.GetFiles(path);
                        string aimPath = path.Replace("Avatars", "Resources/Prefabs/Avatars");
                        //先删除原来的icon
                        for (int i = 0, len = filePathes.Length; i < len; i++) {
                            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePathes[i]);
                            if (sprite != null) {
                                GameObject newObj = new GameObject();
                                newObj.name = sprite.name;
                                SpriteRenderer render = newObj.AddComponent<SpriteRenderer>();
                                render.sprite = sprite;
                                PrefabUtility.CreatePrefab(aimPath + "/" + newObj.name + ".prefab", newObj);
                                Debug.Log(string.Format("{0}创建完毕", newObj.name));
                                DestroyImmediate(newObj);
                            }
                        }
                    }
                    Debug.Log(string.Format("目录{0}下的Sprite创建Prefab完毕", path));
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem ("Editors/AppNocturnalClothings")]
        static void AddNocturnalClothings() {
            DbManager.Instance.AddProp(PropType.NocturnalClothing, 10);
            Debug.LogWarning("添加了10件夜行衣！");
        }

	}
}
#endif