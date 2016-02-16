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
	public class TasksEditorWindow : EditorWindow {

		static TasksEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Tasks Editor")]
		static void OpenWindow() {
			Base.InitParams();
			InitParams();
			Open();
		}

		static List<TaskType> taskTypeEnums;
		static List<string> taskTypeStrs;
		static Dictionary<TaskType, int> taskTypeIndexMapping;


		static List<TaskDialogType> taskDialogTypeEnums;
		static List<string> taskDialogTypeStrs;
		static Dictionary<TaskDialogType, int> taskDialogTypeIndexMapping;
		static void InitParams() {
			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			int index;

			//加载全部的TaskType枚举类型
			taskTypeEnums = new List<TaskType>();
			taskTypeStrs = new List<string>();
			taskTypeIndexMapping = new Dictionary<TaskType, int>();
			index = 0;
			foreach(TaskType type in Enum.GetValues(typeof(TaskType))) {
				taskTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				taskTypeStrs.Add(attrib.Description);
				taskTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的TaskDialogType枚举类型
			taskDialogTypeEnums = new List<TaskDialogType>();
			taskDialogTypeStrs = new List<string>();
			taskDialogTypeIndexMapping = new Dictionary<TaskDialogType, int>();
			index = 0;
			foreach(TaskDialogType type in Enum.GetValues(typeof(TaskDialogType))) {
				taskDialogTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				taskDialogTypeStrs.Add(attrib.Description);
				taskDialogTypeIndexMapping.Add(type, index);
				index++;
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
			float width = 1060;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (TasksEditorWindow)EditorWindow.GetWindowWithRect(typeof(TasksEditorWindow), size, true, "任务编辑器");
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

		static Dictionary<string, TaskData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, TaskData>();
			JObject obj = JsonManager.GetInstance().GetJson("Tasks", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<TaskData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<TaskData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<TaskData>();
			foreach(TaskData data in dataMapping.Values) {
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
			List<TaskData> datas = new List<TaskData>();
			foreach(TaskData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(TaskData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Tasks.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		TaskData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string name = "";
		string belongToNpcId = "";
		string belongToAreaName = "";
		string frontTaskDataId = "";
		int taskTypeIndex;
		int oldTaskTypeIndex = -1;
		int taskTypeValueIndex;
		string stringValue;
		int intValue;
		int minIntValue;
		int maxIntValue;

		List<int> dialogTypeIndexes;
		List<string> dialogTalkMsgs;
		List<int> dialogIntValues;
		List<int> dialogBackYesTaskDataIdIndexes;
		List<int> dialogBackNoTaskDataIdIndexes;
		List<int> dialogIconIdIndex;
		List<string> dialogYesMsgs;
		List<string> dialogNoMsgs;

		short toolState = 0; //0正常 1添加 2删除

		string addId = "";
		string addName = "";

		int addDialogTypeIndex = 0;

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
					belongToNpcId = data.BelongToNpcId;
					belongToAreaName = data.BelongToAreaName;
					frontTaskDataId = data.FrontTaskDataId;
					taskTypeIndex = taskTypeIndexMapping.ContainsKey(data.Type) ? taskTypeIndexMapping[data.Type] : 0;
					taskTypeValueIndex = 0;
					switch (taskTypeEnums[taskTypeIndex]) {
					case TaskType.Gender:
						taskTypeValueIndex = Base.GenderTypeIndexMapping.ContainsKey((GenderType)data.IntValue) ? Base.GenderTypeIndexMapping[(GenderType)data.IntValue] : 0;
						break;
					case TaskType.ItemInHand:
						taskTypeValueIndex = Base.ItemDataIdIndexs.ContainsKey(data.StringValue) ? Base.ItemDataIdIndexs[data.StringValue] : 0;
						break;
					case TaskType.MoralRange:
						minIntValue = data.MinIntValue;
						maxIntValue = data.MaxIntValue;
						break;
					case TaskType.Occupation:
						taskTypeValueIndex = Base.OccupationTypeIndexMapping.ContainsKey((OccupationType)data.IntValue) ? Base.OccupationTypeIndexMapping[(OccupationType)data.IntValue] : 0;
						break;
					case TaskType.TheHour:
						taskTypeValueIndex = Base.TimeNames.Count > data.IntValue ? data.IntValue : 0;
						break;
					default:
						break;
					}

					//对话信息初始化
					dialogTypeIndexes = new List<int>();
					dialogTalkMsgs = new List<string>();
					dialogIntValues = new List<int>();
					dialogBackYesTaskDataIdIndexes = new List<int>();
					dialogBackNoTaskDataIdIndexes = new List<int>();
					dialogIconIdIndex = new List<int>();
					dialogYesMsgs = new List<string>();
					dialogNoMsgs = new List<string>();

					TaskDialogData dialog;
					for (int i = 0; i < data.Dialogs.Count; i++) {
						dialog = data.Dialogs[i];
						dialogTypeIndexes.Add(taskDialogTypeIndexMapping.ContainsKey(dialog.Type) ? taskDialogTypeIndexMapping[dialog.Type] : 0);
						dialogTalkMsgs.Add(dialog.TalkMsg);

					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 1000, 60));
					GUI.Label(new Rect(0, 0, 20, 18), "Id:");
					EditorGUI.TextField(new Rect(25, 0, 100, 18), showId);
					GUI.Label(new Rect(130, 0, 50, 18), "任务名称:");
					name = EditorGUI.TextField(new Rect(185, 0, 150, 18), name);
					GUI.Label(new Rect(340, 0, 50, 18), "绑定Npc:");
					belongToNpcId = EditorGUI.TextField(new Rect(395, 0, 100, 18), belongToNpcId);
					GUI.Label(new Rect(500, 0, 65, 18), "绑定大地图:");
					belongToAreaName = EditorGUI.TextField(new Rect(570, 0, 100, 18), belongToAreaName);
					GUI.Label(new Rect(675, 0, 50, 18), "前置任务:");
					frontTaskDataId = EditorGUI.TextField(new Rect(730, 0, 100, 18), frontTaskDataId);
					GUI.Label(new Rect(0, 20, 80, 18), "任务接取条件:");
					taskTypeIndex = EditorGUI.Popup(new Rect(85, 20, 200, 18), taskTypeIndex, taskTypeStrs.ToArray());
					if (taskTypeIndex != oldTaskTypeIndex){
						oldTaskTypeIndex = taskTypeIndex;
						stringValue = "";
						intValue = 0;
					}
					switch (taskTypeEnums[taskTypeIndex]) {
					case TaskType.Gender:
						GUI.Label(new Rect(290, 20, 60, 18), "要求性别为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 30, 18), taskTypeValueIndex, Base.GenderTypeStrs.ToArray());
						intValue = (int)Base.GenderTypeEnums[taskTypeValueIndex];
						break;
					case TaskType.ItemInHand:
						GUI.Label(new Rect(290, 20, 60, 18), "要求道具为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.ItemDataNames.ToArray());
						stringValue = Base.ItemDatas[taskTypeValueIndex].Id;
						break;
					case TaskType.MoralRange:
						GUI.Label(new Rect(290, 20, 60, 18), "道德点区间:");
						minIntValue = (int)EditorGUI.Slider(new Rect(355, 20, 180, 18), minIntValue, -10000, 10000);
						GUI.Label(new Rect(540, 20, 10, 18), "-:");
						maxIntValue = (int)EditorGUI.Slider(new Rect(555, 20, 180, 18), maxIntValue, -10000, 10000);
						break;
					case TaskType.Occupation:
						GUI.Label(new Rect(290, 20, 60, 18), "要求门派为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.OccupationTypeStrs.ToArray());
						intValue = (int)Base.OccupationTypeEnums[taskTypeValueIndex];
						break;
					case TaskType.TheHour:
						GUI.Label(new Rect(290, 20, 60, 18), "要求时辰为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.TimeNames.ToArray());
						intValue = taskTypeValueIndex;
						break;
					default:
						break;
					}
					if (GUI.Button(new Rect(0, 40, 100, 18), "修改任务基础属性")) {
						if (name == "") {
							this.ShowNotification(new GUIContent("任务名不能为空!"));
							return;
						}
						if (minIntValue > maxIntValue) {
							this.ShowNotification(new GUIContent("最小值不能大于最大值!"));
							return;
						}
						data.Name = name;
						data.BelongToNpcId = belongToNpcId;
						data.BelongToAreaName = belongToAreaName;
						data.Type = taskTypeEnums[taskTypeIndex];
						if (data.Type != TaskType.MoralRange) {
							minIntValue = 0;
							maxIntValue = 0;
						}
						data.FrontTaskDataId = frontTaskDataId;
						data.StringValue = stringValue;
						data.IntValue = intValue;
						data.MinIntValue = minIntValue;
						data.MaxIntValue = maxIntValue;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 70, 1000, 630));
					GUI.Label(new Rect(0, 0, 1000, 18), "|-----------任务步骤------------------------------------------------------------------------------------------------------------------------|");
					addDialogTypeIndex = EditorGUI.Popup(new Rect(0, 20, 100, 18), addDialogTypeIndex, taskDialogTypeStrs.ToArray());
					if (GUI.Button(new Rect(105, 20, 60, 18), "添加步骤")) {
						if (data.Dialogs.Count >= 20) {
							this.ShowNotification(new GUIContent("剧情的对话步骤不能超过20条!"));
							return;
						}
						TaskDialogData dialogData = new TaskDialogData();
						dialogData.Type = taskDialogTypeEnums[addDialogTypeIndex];
						data.Dialogs.Add(dialogData);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					float dialogsStartX = 0;
					float dialogsStartY = 40;
					for (int i = 0; i < dialogTypeIndexes.Count; i++) {
						GUILayout.BeginArea(new Rect(dialogsStartX, dialogsStartY + i * 50, 1000, 50));
						dialogTypeIndexes[i] = EditorGUI.Popup(new Rect(0, 0, 100, 18), dialogTypeIndexes[i], taskDialogTypeStrs.ToArray());
						GUILayout.EndArea();
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 700, 500, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加任务")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除任务")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 20, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 20, 80, 18), addId);
				GUI.Label(new Rect(120, 20, 50, 18), "任务名:");
				addName = GUI.TextField(new Rect(175, 20, 80, 18), addName);
				if (GUI.Button(new Rect(260, 20, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addName == "") {
						this.ShowNotification(new GUIContent("任务名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}

					TaskData soundData = new TaskData();
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