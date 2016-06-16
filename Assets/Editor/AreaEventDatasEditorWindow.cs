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
			Base.InitParams();
			Open();
			InitParams();
			getMeetEnemyRates();
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

		static Dictionary<string, List<RateData>> meetEnemyRatesMapping;
		static List<RateData> currentMeetEnemyRates;
		static void getMeetEnemyRates() {
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/AreaMeetEnemys");
			meetEnemyRatesMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<string, List<RateData>>>(asset.text);
		}

		static Dictionary<string, SizeData> areaSizeMapping;

		static List<SceneEventType> sceneEventTypeEnums;
		static List<string> sceneEventStrs;
		static Dictionary<SceneEventType, int> sceneEventIndexMapping;

		static List<SceneEventOpenType> sceneEventOpenTypeEnums;
		static List<string> sceneEventOpenStrs;
		static Dictionary<SceneEventOpenType, int> sceneEventOpenIndexMapping;

		static List<string> allBirthPointNames;
		static Dictionary<string, int> allBirthPointIdIndexs;
		static List<EventData> allBirthPointEvents;

		static List<string> allCitySceneNames;
		static Dictionary<string, int> allCitySceneIdIndexs;
		static List<SceneData> allCityScenes;

		static List<string> allFightNames;
		static Dictionary<string, int> allFightIdIndexs;
		static List<FightData> allFights;

		static tk2dTileMap map;
		static string sceneName;
		static string areaName;
		static JObject areaData;

		static void InitParams() { 
			//读取区域大地图宽高数据表
			areaSizeMapping = new Dictionary<string, SizeData>();
			JObject obj = JsonManager.GetInstance().GetJson("AreaSizeDatas", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					areaSizeMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<SizeData>(item.Value.ToString()));
				}
			}
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

			//加载全部的SceneEventOpeType枚举类型
			sceneEventOpenTypeEnums = new List<SceneEventOpenType>();
			sceneEventOpenStrs = new List<string>();
			sceneEventOpenIndexMapping = new Dictionary<SceneEventOpenType, int>();
			index = 0;
			foreach(SceneEventOpenType type in Enum.GetValues(typeof(SceneEventOpenType))) {
				sceneEventOpenTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				sceneEventOpenStrs.Add(attrib.Description);
				sceneEventOpenIndexMapping.Add(type, index);
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

			//处理区域大地图中文名
			areaData = JsonManager.GetInstance().GetJson("AreaNames", false);
			if (areaData[sceneName] == null) {
				areaData[sceneName] = new JObject();
				areaData[sceneName]["Id"] = sceneName;
				areaData[sceneName]["Name"] = sceneName;
				areaName = sceneName;
			}
			else {
				areaName = areaData[sceneName]["Name"].ToString();
			}
			writeAreaDataToJson();

			//获取旧数据
			dataMapping = new Dictionary<string, EventData>();
			obj = JsonManager.GetInstance().GetJson("AreaEventDatas", false);
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

			allCitySceneIdIndexs = new Dictionary<string, int>();
			allCitySceneNames = new List<string>();
			allCityScenes = new List<SceneData>();
			obj = JsonManager.GetInstance().GetJson("Scenes", false);
			SceneData sceneData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					sceneData = JsonManager.GetInstance().DeserializeObject<SceneData>(item.Value.ToString());
					if (sceneData.BelongToAreaName == sceneName) {
						allCitySceneNames.Add(sceneData.Name);
						allCitySceneIdIndexs.Add(sceneData.Id, index);
						allCityScenes.Add(sceneData);
						index++;
					}
				}
			}

			allFightIdIndexs = new Dictionary<string, int>();
			allFightNames = new List<string>();
			allFights = new List<FightData>();
			obj = JsonManager.GetInstance().GetJson("Fights", false);
			FightData fightData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					fightData = JsonManager.GetInstance().DeserializeObject<FightData>(item.Value.ToString());
					if (fightData.Type == FightType.Normal) {
						allFightNames.Add(fightData.Name);
						allFightIdIndexs.Add(fightData.Id, index);
						allFights.Add(fightData);
						index++;
					}
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
			JObject cityPosObj = new JObject(); //记录城镇坐标的json表数据
			foreach(EventData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				if (data.Type == SceneEventType.EnterCity) {
					if (cityPosObj[data.EventId] == null) {
						cityPosObj[data.EventId] = data.Id;
					}
				}
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaEventDatas.json", JsonManager.GetInstance().SerializeObject(writeJson));
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaCityPosDatas.json", JsonManager.GetInstance().SerializeObject(cityPosObj));
			if (areaSizeMapping.ContainsKey(sceneName)) {
				areaSizeMapping[sceneName].Width = map.width;
				areaSizeMapping[sceneName].Height = map.height;
			}
			else {
				areaSizeMapping.Add(sceneName, new SizeData(sceneName, map.width, map.height));
			}
			JObject sizeObj = new JObject();
			index = 0;
			foreach(SizeData data in areaSizeMapping.Values) {
				if (index == 0) {
					index++;
					sizeObj["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObject(data));
				}
				sizeObj[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObject(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaSizeDatas.json", JsonManager.GetInstance().SerializeObject(sizeObj));
		}

		static void writeAreaDataToJson() {
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaNames.json", JsonManager.GetInstance().SerializeObject(areaData));
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
		static int citySceneIdIndex = 0;
		static int fightEventIdIndex = 0;
		static int eventOpenTypeIndex = 0;
		static string eventNotice = "";
		static int openKeyIndex = 0;
		static string openKey = "";

		//绘制窗口时调用
	    void OnGUI () {
			if (Prefab == null) {
				return;
			}
			else {
				Selection.activeGameObject = Prefab;
			}
			data = null;

			GUILayout.BeginArea(new Rect(5, 5, 800, 20));
			GUI.Label(new Rect(0, 0, 50, 18), "搜索名称:");
			searchKeyword = GUI.TextField(new Rect(55, 0, 100, 18), searchKeyword);
			if (GUI.Button(new Rect(160, 0, 30, 18), "搜索")) {
				selGridInt = 0;
				fetchData(searchKeyword);
			}
			GUI.Label(new Rect(195, 0, 60, 18), "区域名:");
			areaName = GUI.TextField(new Rect(260, 0, 100, 18), areaName);
			if (GUI.Button(new Rect(365, 0, 40, 18), "改名")) {
				if (areaData[sceneName] != null) {
					areaData[sceneName]["Name"] = areaName;
					writeAreaDataToJson();
					this.ShowNotification(new GUIContent("改名成功"));
				}
				else {
					this.ShowNotification(new GUIContent("区域名称数据不存在!"));
				}
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
					switch(data.Type) {
					case SceneEventType.EnterArea:
						birthPointEventIdIndex = allBirthPointIdIndexs.ContainsKey(data.EventId) ? allBirthPointIdIndexs[data.EventId] : 0;
						break;
					case SceneEventType.EnterCity:
						citySceneIdIndex = allCitySceneIdIndexs.ContainsKey(data.EventId) ? allCitySceneIdIndexs[data.EventId] : 0;
						break;
					case SceneEventType.Battle:
						fightEventIdIndex = allFightIdIndexs.ContainsKey(data.EventId) ? allFightIdIndexs[data.EventId] : 0;
						break;
					default:
						break;
					}
					eventOpenTypeIndex = sceneEventOpenIndexMapping.ContainsKey(data.OpenType) ? sceneEventOpenIndexMapping[data.OpenType] : 0;
					switch (sceneEventOpenTypeEnums[eventOpenTypeIndex]) {
					case SceneEventOpenType.FightWined:
						if (allFightNames.Count <= openKeyIndex) {
							openKeyIndex = 0;
						}
						openKeyIndex = allFightIdIndexs.ContainsKey(data.OpenKey) ? allFightIdIndexs[data.OpenKey] : 0;
						break;
					case SceneEventOpenType.NeedItem:
						if (Base.ItemDataNames.Count <= openKeyIndex) {
							openKeyIndex = 0;
						}
						openKeyIndex = Base.ItemDataIdIndexs.ContainsKey(data.OpenKey) ? Base.ItemDataIdIndexs[data.OpenKey] : 0;
						break;
					default:
						openKey = "";
						break;
					}
					eventNotice = data.Notice;
					if (meetEnemyRatesMapping.ContainsKey(sceneName)) {
						currentMeetEnemyRates = meetEnemyRatesMapping[sceneName];
					}
					else {
						currentMeetEnemyRates = new List<RateData>();
						meetEnemyRatesMapping.Add(sceneName, currentMeetEnemyRates);
					}
					for (int i = 0; i < currentMeetEnemyRates.Count; i++) {
						currentMeetEnemyRates[i].IdIndex = allFightIdIndexs.ContainsKey(currentMeetEnemyRates[i].Id) ? allFightIdIndexs[currentMeetEnemyRates[i].Id] : 0;
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 300));
					GUI.Label(new Rect(0, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(65, 0, 150, 18), showId);
					GUI.Label(new Rect(220, 0, 50, 18), "开启条件:");
					eventOpenTypeIndex = EditorGUI.Popup(new Rect(275, 0, 80, 18), eventOpenTypeIndex, sceneEventOpenStrs.ToArray());
					switch (sceneEventOpenTypeEnums[eventOpenTypeIndex]) {
					case SceneEventOpenType.FightWined:
						if (allFightNames.Count <= openKeyIndex) {
							openKeyIndex = 0;
						}
						openKeyIndex = EditorGUI.Popup(new Rect(360, 0, 150, 18), openKeyIndex, allFightNames.ToArray());
						openKey = allFights[openKeyIndex].Id;
						eventNotice = EditorGUI.TextArea(new Rect(515, 0, 200, 36), eventNotice);
						break;
					case SceneEventOpenType.NeedItem:
						if (Base.ItemDataNames.Count <= openKeyIndex) {
							openKeyIndex = 0;
						}
						openKeyIndex = EditorGUI.Popup(new Rect(360, 0, 150, 18), openKeyIndex, Base.ItemDataNames.ToArray());
						openKey = Base.ItemDatas[openKeyIndex].Id;
						eventNotice = EditorGUI.TextArea(new Rect(515, 0, 200, 36), eventNotice);
						break;
					default:
						openKey = "";
						break;
					}
					GUI.Label(new Rect(0, 20, 60, 18), "事件名称:");
					eventName = EditorGUI.TextField(new Rect(65, 20, 150, 18), eventName);
					GUI.Label(new Rect(0, 40, 60, 18), "事件类型:");
					sceneEventIndex = EditorGUI.Popup(new Rect(65, 40, 150, 18), sceneEventIndex, sceneEventStrs.ToArray());

					switch(sceneEventTypeEnums[sceneEventIndex]) {
					case SceneEventType.EnterArea:
						birthPointEventIdIndex = EditorGUI.Popup(new Rect(220, 40, 150, 18), birthPointEventIdIndex, allBirthPointNames.ToArray());
						eventId = allBirthPointEvents[birthPointEventIdIndex].Id;
						break;
					case SceneEventType.EnterCity:
						citySceneIdIndex = EditorGUI.Popup(new Rect(220, 40, 150, 18), citySceneIdIndex, allCitySceneNames.ToArray());
						eventId = allCityScenes[citySceneIdIndex].Id;
						break;
					case SceneEventType.Battle:
						fightEventIdIndex = EditorGUI.Popup(new Rect(220, 40, 150, 18), fightEventIdIndex, allFightNames.ToArray());
						eventId = allFights[fightEventIdIndex].Id;
						break;
					default:
						break;
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
						data.OpenType = sceneEventOpenTypeEnums[eventOpenTypeIndex];
						data.OpenKey = openKey;
						data.Notice = eventNotice;
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

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 305, 800, 500));
					GUI.Label(new Rect(0, 0, 60, 18), "随机遇敌:");
					RateData rateData;
					for (int i = 0; i < currentMeetEnemyRates.Count; i++) {
						if (currentMeetEnemyRates.Count <= i) {
							continue;
						}
						rateData = currentMeetEnemyRates[i];
						GUI.Label(new Rect(0, 20 + i * 20, 60, 18), "遇敌战斗:");
						rateData.IdIndex = EditorGUI.Popup(new Rect(65, 20 + i * 20, 150, 18), rateData.IdIndex, allFightNames.ToArray());
						rateData.Id = allFights[rateData.IdIndex].Id;
						GUI.Label(new Rect(220, 20 + i * 20, 60, 18), "遇敌概率:");
						rateData.Rate = EditorGUI.Slider(new Rect(285, 20 + i * 20, 180, 18), rateData.Rate, 0, 100);
						if (GUI.Button(new Rect(470, 20 + i * 20, 40, 18), "删除")) {
							currentMeetEnemyRates.RemoveAt(i);
						}
					}
					if (GUI.Button(new Rect(0, 180, 60, 18), "修改战斗")) {
//						if (currentMeetEnemyRates.Count == 0) {
//							this.ShowNotification(new GUIContent("还没有添加任何随机战斗!"));
//							return;
//						}
						Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "AreaMeetEnemys.json", JsonManager.GetInstance().SerializeObject(meetEnemyRatesMapping));
						this.ShowNotification(new GUIContent("修改随机遇敌数据成功"));
					}
					if (GUI.Button(new Rect(65, 180, 60, 18), "添加战斗")) {
						if (currentMeetEnemyRates.Count >= 8) {
							this.ShowNotification(new GUIContent("每个区域最多出现8种战斗!"));
							return;
						}	
						currentMeetEnemyRates.Add(new RateData(100, "0", 0));
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