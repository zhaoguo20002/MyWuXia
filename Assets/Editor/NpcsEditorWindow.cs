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
	public class NpcsEditorWindow : EditorWindow {

		static NpcsEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Npcs Editor")]
		static void OpenWindow() {
			Base.InitParams();
			InitParams();
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
				window = (NpcsEditorWindow)EditorWindow.GetWindowWithRect(typeof(NpcsEditorWindow), size, true, "Npc编辑器");
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

		static List<string> allFightNames;
		static Dictionary<string, int> allFightIdIndexs;
        static List<FightData> allFights;

        static List<string> allTaskNames;
        static Dictionary<string, int> allTaskIdIndexs;
        static List<TaskData> allTasks;

		static void InitParams() { 
			allFightIdIndexs = new Dictionary<string, int>();
			allFightNames = new List<string>();
			allFights = new List<FightData>();
			JObject obj = JsonManager.GetInstance().GetJson("Fights", false);
			FightData fightData;
			int index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					fightData = JsonManager.GetInstance().DeserializeObject<FightData>(item.Value.ToString());
					if (fightData.Type == FightType.Scene) { //场景中的npc的战斗
						allFightNames.Add(fightData.Name);
						allFightIdIndexs.Add(fightData.Id, index);
						allFights.Add(fightData);
						index++;
					}
				}
			}

            allTaskIdIndexs = new Dictionary<string, int>();
            allTaskNames = new List<string>();
            allTasks = new List<TaskData>();
            obj = JsonManager.GetInstance().GetJson("Tasks", false);
            TaskData taskData;
            index = 0;
            foreach(var item in obj) {
                if (item.Key != "0") {
                    taskData = JsonManager.GetInstance().DeserializeObject<TaskData>(item.Value.ToString());
                    allTaskNames.Add(taskData.Name);
                    allTaskIdIndexs.Add(taskData.Id, index);
                    allTasks.Add(taskData);
                    index++;
                }
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
		string defaultDialogMsg = "";
		int iconIdIndex;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		bool isActive;
		int npcTypeIndex = 0;
		int fightIdIndex = 0;
        int taskIdIndex = 0;

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
					defaultDialogMsg = data.DefaultDialogMsg;
					iconIdIndex = Base.IconIdIndexs.ContainsKey(data.IconId) ? Base.IconIdIndexs[data.IconId] : 0;
					iconTexture = Base.IconTextureMappings.ContainsKey(data.IconId) ? Base.IconTextureMappings[data.IconId] : null;
					isActive = data.IsActive;
					npcTypeIndex = Base.NpcTypeIndexMapping.ContainsKey(data.Type) ? Base.NpcTypeIndexMapping[data.Type] : 0;
					fightIdIndex = allFightIdIndexs.ContainsKey(data.CurrentFightId) ? allFightIdIndexs[data.CurrentFightId] : 0;
                    taskIdIndex = allTaskIdIndexs.ContainsKey(data.ShowAfterTaskId) ? allTaskIdIndexs[data.ShowAfterTaskId] : 0;
				}
				//结束滚动视图  
				GUI.EndScrollView();
				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 300));
					GUI.Label(new Rect(0, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(65, 0, 150, 18), showId);
					GUI.Label(new Rect(0, 20, 60, 18), "Npc名称:");
					name = EditorGUI.TextField(new Rect(65, 20, 150, 18), name);
					GUI.Label(new Rect(220, 0, 50, 18), "一句话:");
					defaultDialogMsg = EditorGUI.TextArea(new Rect(265, 0, 180, 36), defaultDialogMsg);
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 40, 50, 50), iconTexture);
					}
					iconIdIndex = EditorGUI.Popup(new Rect(65, 40, 150, 18), iconIdIndex, Base.IconNames.ToArray());
					if (oldIconIndex != iconIdIndex) {
						oldIconIndex = iconIdIndex;
						iconTexture = Base.IconTextureMappings[Base.Icons[iconIdIndex].Id];
					}
					GUI.Label(new Rect(65, 60, 60, 18), "Npc类型:");
					npcTypeIndex = EditorGUI.Popup(new Rect(130, 60, 80, 18), npcTypeIndex, Base.NpcTypeStrs.ToArray());
                    if (Base.NpcTypeEnums[npcTypeIndex] == NpcType.Fight) {
                        fightIdIndex = EditorGUI.Popup(new Rect(215, 60, 150, 18), fightIdIndex, allFightNames.ToArray());
                    } else if (Base.NpcTypeEnums[npcTypeIndex] == NpcType.AfterTask) {
                        taskIdIndex = EditorGUI.Popup(new Rect(215, 60, 150, 18), taskIdIndex, allTaskNames.ToArray());
                    }
					GUI.Label(new Rect(220, 40, 50, 18), "动态Npc:");
					isActive = EditorGUI.Toggle(new Rect(275, 40, 18, 18), isActive);
					if (GUI.Button(new Rect(0, 120, 100, 18), "修改")) {
						if (name == "") {
							this.ShowNotification(new GUIContent("Npc名不能为空!"));
							return;
						}
						data.Name = name;
						data.DefaultDialogMsg = defaultDialogMsg;
						data.IconId = Base.Icons[iconIdIndex].Id;
						data.IsActive = isActive;
						data.Type = Base.NpcTypeEnums[npcTypeIndex];
                        data.CurrentFightId = allFights.Count > fightIdIndex ? allFights[fightIdIndex].Id : "";
                        data.ShowAfterTaskId = allTasks[taskIdIndex].Id;
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
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加Npc")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除Ncp")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 20, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 20, 80, 18), addId);
				GUI.Label(new Rect(120, 20, 50, 18), "Npc名:");
				addName = GUI.TextField(new Rect(175, 20, 80, 18), addName);
				if (GUI.Button(new Rect(260, 20, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addName == "") {
						this.ShowNotification(new GUIContent("Npc名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}

					NpcData npcData = new NpcData();
					npcData.Id = addId;
					npcData.Name = addName;
                    ResourceSrcData findIcon = Base.Icons.Find(item => item.Name.IndexOf(npcData.Name) >= 0);
                    if (findIcon != null) {
                        npcData.IconId = findIcon.Id;
                    }
					dataMapping.Add(npcData.Id, npcData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
//					addId = "";
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
			Base.DestroyParams();
		}
	}
}
#endif