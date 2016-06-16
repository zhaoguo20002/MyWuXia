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
	public class SceneFloydDisEditorWindow : EditorWindow {

		static SceneFloydDisEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Scene Floyd Dis Editor")]
		static void OpenWindow() {
			InitParams();
			Open();
		}

		static List<List<float>> dis;
		static Dictionary<int, string> cityIndexToNameMapping; //城镇在临接矩阵中的索引与城镇名的关联表
		static List<string> sceneNames = new List<string>();
		static void InitParams() {
			sceneNames = new List<string>();
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/SceneIndexToNames");
			cityIndexToNameMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<int, string>>(asset.text);
			foreach(string name in cityIndexToNameMapping.Values) {
				sceneNames.Add(name);
			}
		}

		// Use this for initialization
		void Start () {
			
		}

		// Update is called once per frame
		void Update () {

		}

		/// <summary>
		/// Open the specified pos.
		/// </summary>
		public static void Open() {
			float width = 860;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (SceneFloydDisEditorWindow)EditorWindow.GetWindowWithRect(typeof(SceneFloydDisEditorWindow), size, true, "城镇寻路编辑器");
			}
			window.Show();
			window.position = size;
			getData();
		}

		/// <summary>
		/// Hide this instance.
		/// </summary>
		public static void Hide() {
			if (window != null) {
				window.Close();
				window = null;
			}
		}

		static void getData() {
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/FloydDis");
			dis = JsonManager.GetInstance().DeserializeObject<List<List<float>>>(asset.text);
		}


		void writeDataToJson() {
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "FloydDis.json", JsonManager.GetInstance().SerializeObject(dis));
		}

		string currentName;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		List<float> rowData;
		Vector2 scrollContentPosition;
		//绘制窗口时调用
	    void OnGUI () {
			float listStartX = 5;
			float listStartY = 25;
			float scrollHeight = Screen.currentResolution.height - 160;
			if (dis != null && sceneNames != null && sceneNames.Count > 0) {
				float contextHeight = sceneNames.Count * 21;
				//开始滚动视图  
				scrollPosition = GUI.BeginScrollView(new Rect(listStartX, listStartY, 200, scrollHeight), scrollPosition, new Rect(0, 0, 180, contextHeight), false, scrollHeight < contextHeight);

				selGridInt = GUILayout.SelectionGrid(selGridInt, sceneNames.ToArray(), 1, GUILayout.Width(190));
				selGridInt = selGridInt >= sceneNames.Count ? sceneNames.Count - 1 : selGridInt;
				currentName = sceneNames[selGridInt];
				if (selGridInt != oldSelGridInt) {
					oldSelGridInt = selGridInt;
					rowData = dis[selGridInt];
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (!string.IsNullOrEmpty(currentName)) {
//					GUILayout.BeginArea(new Rect(listStartX + 215, listStartY, 600, 700));
					scrollContentPosition = GUI.BeginScrollView(new Rect(listStartX + 215, listStartY, 600, scrollHeight), scrollContentPosition, new Rect(0, 0, 580, contextHeight), false, scrollHeight < contextHeight);
					for (int i = 0; i < rowData.Count; i++) {
						if (sceneNames.Count > i) {
							if (i != selGridInt) {
								GUI.Label(new Rect(0, i * 21, 60, 18), string.Format("{0}:", sceneNames[i]));
								rowData[i] = EditorGUI.Slider(new Rect(65, i * 21, 200, 18), rowData[i], 1, 100000);
							}
							else {
								GUI.Label(new Rect(0, i * 21, 200, 18), string.Format("{0}: N/A", sceneNames[i]));
								rowData[i] = 100000;
							}
							
						}
					}
					GUI.EndScrollView();

					if (GUI.Button(new Rect(listStartX + 215, scrollHeight + 20, 100, 18), "修改")) {
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						this.ShowNotification(new GUIContent("修改成功"));
					}
//					GUILayout.EndArea();
				}
			}
	    }

		/// <summary>
		/// 当窗口获得焦点时调用一次
		/// </summary>
		void OnFocus() {

		}

		/// <summary>
		/// 当窗口丢失焦点时调用一次
		/// </summary>
		void OnLostFocus() {

		}

		/// <summary>
		/// 当Hierarchy视图中的任何对象发生改变时调用一次
		/// </summary>
		void OnHierarchyChange() {

		}

		/// <summary>
		/// 当Project视图中的资源发生改变时调用一次
		/// </summary>
		void OnProjectChange() {

		}

		/// <summary>
		/// 这里开启窗口的重绘，不然窗口信息不会刷新
		/// </summary>
		void OnInspectorUpdate() {
			this.Repaint();
		}

		/// <summary>
		/// 当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		/// </summary>
		void OnSelectionChange() {
//			foreach(Transform t in Selection.transforms) {
//				//有可能是多选，这里开启一个循环打印选中游戏对象的名称
//				Debug.Log("OnSelectionChange" + t.name);
//			}
		}

		/// <summary>
		/// 当窗口关闭时调用
		/// </summary>
		void OnDestroy() {
			
		}
	}
}
#endif