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
	public class AreaEventDatasEditorWindow : EditorWindow {

		static AreaEventDatasEditorWindow window = null;
		static GameObject Prefab;
		[MenuItem ("Editors/Area Event Datas Editor")]
		static void OpenWindow() {
			JsonManager.GetInstance().Clear();
			Open();
			InitParams();
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
				window = (AreaEventDatasEditorWindow)EditorWindow.GetWindowWithRect(typeof(AreaEventDatasEditorWindow), size, true, "大地图事件编辑器");
			}
			window.Show();
			window.position = size;
			if (Prefab != null) {
				DestroyImmediate(Prefab);
				Prefab = null;
			}
			Prefab = new GameObject();
			Prefab.name = "pointer";
			Prefab.transform.position = Vector3.zero;
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

		static Dictionary<string, EventData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, EventData>();
			JObject obj = JsonManager.GetInstance().GetJson("AreaEventDatas", false);

			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<EventData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<EventData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<EventData>();
			foreach(EventData data in dataMapping.Values) {
				if (data.SceneId != sceneName) {
					continue;
				}
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

		static void writeDataToJson() {
			JObject writeJson = new JObject();
			int index = 0;
			List<EventData> datas = new List<EventData>();
			foreach(EventData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(EventData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaEventDatas.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}


		static List<SceneEventType> sceneEventTypeEnums;
		static List<string> sceneEventStrs;
		static Dictionary<SceneEventType, int> sceneEventIndexMapping;

		static List<string> allBirthPointNames;
		static Dictionary<string, int> allBirthPointIdIndexs;
		static List<EventData> allBirthPointEvents;

		static tk2dTileMap map;
		static string sceneName;

		static void InitParams() { 
			int index = 0;
			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的SceneEventType枚举类型
			sceneEventTypeEnums = new List<SceneEventType>();
			sceneEventStrs = new List<string>();
			sceneEventIndexMapping = new Dictionary<SceneEventType, int>();
			index = 0;
			foreach(SceneEventType type in Enum.GetValues(typeof(SceneEventType))) {
				sceneEventTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				sceneEventStrs.Add(attrib.Description);
				sceneEventIndexMapping.Add(type, index);
				index++;
			}

			//初始化地图中的静态事件地砖对应的事件关联数据
			map = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
			string[] fen = EditorApplication.currentScene.Split(new char[] { '/' });
			sceneName = fen[fen.Length - 1].Replace(".unity", "");
			tk2dRuntime.TileMap.TileInfo tile;
			EventData exsitEventData;
			EventData newEventData;
			string id;
			List<EventData> eventDatas = new List<EventData>();

			allBirthPointNames = new List<string>();
			allBirthPointIdIndexs = new Dictionary<string, int>();
			allBirthPointEvents = new List<EventData>();

			//获取旧数据
			dataMapping = new Dictionary<string, EventData>();
			JObject obj = JsonManager.GetInstance().GetJson("AreaEventDatas", false);
			EventData eventData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					eventData = JsonManager.GetInstance().DeserializeObject<EventData>(item.Value.ToString());
					if (eventData.SceneId == sceneName) {
						dataMapping.Add(eventData.Id, eventData);
					}
					//查找所有的出生点事件
					if (eventData.Type == SceneEventType.BirthPoint) {
						allBirthPointNames.Add(eventData.Name);
						allBirthPointIdIndexs.Add(eventData.Id, index);
						allBirthPointEvents.Add(eventData);
						index++;
					}
				}
			}

			for (int i = 0; i < map.width; i++) {
				for (int j = 0; j <  map.height; j++) {
					tile = map.GetTileInfoForTileId(map.GetTile(i, j, 1));
					if (tile != null) {
						id = sceneName + "_" + i + "_" + j;
						newEventData = new EventData();
						newEventData.Id = id;
						newEventData.SceneId = sceneName;
						if (dataMapping.ContainsKey(newEventData.Id)) {
							exsitEventData = dataMapping[newEventData.Id];
							newEventData.Name = exsitEventData.Name;
							newEventData.EventId = exsitEventData.EventId;
						}
						else {
							newEventData.Name = "默认事件点 - " + i + "," + j;
						}
						eventDatas.Add(newEventData);
					}
				}
			}
			//删除掉旧的数据
			dataMapping.Clear();
			JsonManager.GetInstance().Clear();
			getData();
			//添加新数据
			foreach(EventData item in eventDatas) {
				if (!dataMapping.ContainsKey(item.Id)) {
					dataMapping.Add(item.Id, item);
				}
			}
			//生成新数据
			writeDataToJson();
			getData();
		}

		EventData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string eventName = "";
		int sceneEventIndex = 0;
		string eventId;
		static int birthPointEventIdIndex = 0;
		//绘制窗口时调用
	    void OnGUI () {
			if (Prefab == null) {
				return;
			}
			else {
				Selection.activeGameObject = Prefab;
			}
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
					showId = data.Id;
					eventName = data.Name;
					sceneEventIndex = sceneEventIndexMapping[data.Type];
					eventId = data.EventId;
					string[] fen = showId.Split(new char[] { '_' });
					Prefab.transform.position = map.GetTilePosition(int.Parse(fen[1]), int.Parse(fen[2]));
					SceneView.lastActiveSceneView.pivot = Prefab.transform.position;
					birthPointEventIdIndex = allBirthPointIdIndexs.ContainsKey(data.EventId) ? allBirthPointIdIndexs[data.EventId] : 0;
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 300));
					GUI.Label(new Rect(0, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(65, 0, 150, 18), showId);
					GUI.Label(new Rect(0, 20, 60, 18), "事件名称:");
					eventName = EditorGUI.TextField(new Rect(65, 20, 150, 18), eventName);
					GUI.Label(new Rect(0, 40, 60, 18), "事件类型:");
					sceneEventIndex = EditorGUI.Popup(new Rect(65, 40, 150, 18), sceneEventIndex, sceneEventStrs.ToArray());

					if (sceneEventIndex == sceneEventIndexMapping[SceneEventType.EnterArea]) {
						birthPointEventIdIndex = EditorGUI.Popup(new Rect(220, 40, 150, 18), birthPointEventIdIndex, allBirthPointNames.ToArray());
						eventId = allBirthPointEvents[birthPointEventIdIndex].Id;
					}

					GUI.Label(new Rect(0, 60, 100, 18), "事件Id:");
					eventId = EditorGUI.TextField(new Rect(65, 60, 150, 18), eventId);

					if (GUI.Button(new Rect(0, 80, 80, 18), "修改事件")) {
						if (eventName == "") {
							this.ShowNotification(new GUIContent("事件名不能为空!"));
							return;
						}
						if (eventId == "") {
							this.ShowNotification(new GUIContent("事件Id不能为空!"));
							return;
						}
						data.Name = eventName;
						data.Type = sceneEventTypeEnums[sceneEventIndex];
						data.EventId = eventId;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					string[] fen = showId.Split(new char[] { '_' });
					if (fen.Length >= 3) {
						int i = int.Parse(fen[1]);
						int j = int.Parse(fen[2]);
						if (map.GetTileInfoForTileId(map.GetTile(i, j, 1)) == null) {
							if (GUI.Button(new Rect(85, 80, 300, 18), "该事件已从场景中被删除,点击清除此残余数据")) {
								if (!dataMapping.ContainsKey(data.Id)) {
									this.ShowNotification(new GUIContent("要删除的数据不存在!"));
									return;
								}
								dataMapping.Remove(data.Id);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("删除残余事件成功"));
							}
						}
					}
					GUILayout.EndArea();
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
			if (Prefab != null) {
				DestroyImmediate(Prefab);
				Prefab = null;
			}
		}
	}
}
#endif