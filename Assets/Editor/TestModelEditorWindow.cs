﻿#if UNITY_EDITOR
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
	public class ModelEditorWindow : EditorWindow {

		static ModelEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Test Model Editor")]
		static void OpenWindow() {
			Base.InitParams();
			Open();
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
			float width = 660;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (ModelEditorWindow)EditorWindow.GetWindowWithRect(typeof(ModelEditorWindow), size, true, "模型编辑器");
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

		static Dictionary<string, NpcData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, NpcData>();
			JObject obj = JsonManager.GetInstance().GetJson("Npcs", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<NpcData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<NpcData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<NpcData>();
			foreach(NpcData data in dataMapping.Values) {
				if (keyword != "") {
					if (data.Name.IndexOf(keyword) < 0) {
						continue;
					}
				}
				showListData.Add(data);
			}

			listNames = new List<string>();
			showListData.Sort((a, b) => a.Id.CompareTo(b.Id));
			for(int i = 0; i < showListData.Count; i++) {
				listNames.Add(showListData[i].Name);
				if (addedId == showListData[i].Id) {
					selGridInt = i;
					addedId = "";
				}
			}
		}

		void writeDataToJson() {
			JObject writeJson = new JObject();
			int index = 0;
			List<NpcData> datas = new List<NpcData>();
			foreach(NpcData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(NpcData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Npcs.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		NpcData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string name = "";

		short toolState = 0; //0正常 1添加 2删除

		string addId = "";
		string addName = "";
		//绘制窗口时调用
	    void OnGUI () {
			data = null;

			GUILayout.BeginArea(new Rect(5, 5, 200, 20));
			GUI.Label(new Rect(0, 0, 50, 18), "搜索名称:");
			searchKeyword = GUI.TextField(new Rect(55, 0, 100, 18), searchKeyword);
			if (GUI.Button(new Rect(160, 0, 30, 18), "搜索")) {
				selGridInt = 0;
				fetchData(searchKeyword);
			}
			GUILayout.EndArea();

			float listStartX = 5;
			float listStartY = 25;
			float scrollHeight = Screen.currentResolution.height - 110;
			if (listNames != null && listNames.Count > 0) {


				float contextHeight = listNames.Count * 21;
				//开始滚动视图  
				scrollPosition = GUI.BeginScrollView(new Rect(listStartX, listStartY, 200, scrollHeight), scrollPosition, new Rect(5, 5, 190, contextHeight), false, scrollHeight < contextHeight);

				selGridInt = GUILayout.SelectionGrid(selGridInt, listNames.ToArray(), 1, GUILayout.Width(190));
				selGridInt = selGridInt >= listNames.Count ? listNames.Count - 1 : selGridInt;
				data = showListData[selGridInt];
				if (selGridInt != oldSelGridInt) {
					oldSelGridInt = selGridInt;
					toolState = 0;
					showId = data.Id;
					name = data.Name;
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 300));
					GUI.Label(new Rect(0, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(65, 0, 150, 18), showId);
					GUI.Label(new Rect(0, 20, 60, 18), "xxx名称:");
					name = EditorGUI.TextField(new Rect(65, 20, 150, 18), name);
					if (GUI.Button(new Rect(0, 120, 100, 18), "修改")) {
						if (name == "") {
							this.ShowNotification(new GUIContent("xxx名不能为空!"));
							return;
						}
						data.Name = name;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 300, 500, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加xxx")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除xxx")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 20, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 20, 80, 18), addId);
				GUI.Label(new Rect(120, 20, 50, 18), "xxx名:");
				addName = GUI.TextField(new Rect(175, 20, 80, 18), addName);
				if (GUI.Button(new Rect(260, 20, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addName == "") {
						this.ShowNotification(new GUIContent("xxx名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}

					NpcData soundData = new NpcData();
					soundData.Id = addId;
					soundData.Name = addName;
					dataMapping.Add(soundData.Id, soundData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addName = "";
					oldSelGridInt = -1;
					this.ShowNotification(new GUIContent("添加成功"));
				}
				if (GUI.Button(new Rect(345, 20, 80, 18), "取消")) {
					toolState = 0;
				}
				break;
			case 2:
				if (GUI.Button(new Rect(0, 0, 80, 18), "确定删除")) {
					toolState = 0;
					if (data != null && dataMapping.ContainsKey(data.Id)) {
						dataMapping.Remove(data.Id);
						writeDataToJson();
						getData();
						fetchData(searchKeyword);
						oldSelGridInt = -1;
						this.ShowNotification(new GUIContent("删除成功"));
					}
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "取消")) {
					toolState = 0;
				}
				break;
			}
			GUILayout.EndArea();
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
//			EditorApplication.OpenScene("Assets/Scenes/Index.unity");
			Base.DestroyParams();
		}
	}
}
#endif