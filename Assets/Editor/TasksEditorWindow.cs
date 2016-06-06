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

		static Vector2 dialogScrollPosition;

		static List<TaskType> taskTypeEnums;
		static List<string> taskTypeStrs;
		static Dictionary<TaskType, int> taskTypeIndexMapping;


		static List<TaskDialogType> taskDialogTypeEnums;
		static List<string> taskDialogTypeStrs;
		static Dictionary<TaskDialogType, int> taskDialogTypeIndexMapping;

		static List<NpcData> npcs;
		static List<string> npcNames;
		static Dictionary<string, int> npcIdIndexesMapping;

		static List<string> fightNames;
		static Dictionary<string, int> fightIdIndexesMapping;
		static List<FightData> fights;

		static List<string> roleNames;
		static Dictionary<string, int> roleIdIndexesMapping;
		static List<RoleData> roles;

		static List<string> notStaticRoleNames;
		static Dictionary<string, int> notStaticRoleIdIndexesMapping;
		static List<RoleData> notStaticRoles;

		static List<string> bookNames;
		static Dictionary<string, int> bookIdIndexesMapping;
		static List<BookData> books;

		static List<string> skillNames;
		static Dictionary<string, int> skillIdIndexesMapping;
		static List<SkillData> skills;

		static List<string> weaponNames;
		static Dictionary<string, int> weaponIdIndexesMapping;
		static List<WeaponData> weapons;

		static List<string> allCitySceneNames;
		static Dictionary<string, int> allCitySceneIdIndexs;
		static List<SceneData> allCityScenes;

		static void InitParams() {
			dialogScrollPosition = new Vector2(0, 2000);
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

			npcs = new List<NpcData>();
			npcNames = new List<string>();
			npcIdIndexesMapping = new Dictionary<string, int>();
			JObject obj = JsonManager.GetInstance().GetJson("Npcs", false);
			NpcData npcData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					npcData = JsonManager.GetInstance().DeserializeObject<NpcData>(item.Value.ToString());
					if (npcData.Type == NpcType.Normal) {
						npcNames.Add(npcData.Name);
						npcIdIndexesMapping.Add(npcData.Id, index);
						npcs.Add(npcData);
						index++;
					}
				}
			}

			fights = new List<FightData>();
			fightIdIndexesMapping = new Dictionary<string, int>();
			fightNames = new List<string>();
			obj = JsonManager.GetInstance().GetJson("Fights", false);
			FightData fightData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					fightData = JsonManager.GetInstance().DeserializeObject<FightData>(item.Value.ToString());
					if (fightData.Type == FightType.Task) {
						fightNames.Add(fightData.Name);
						fightIdIndexesMapping.Add(fightData.Id, index);
						fights.Add(fightData);
						index++;
					}
				}
			}

			roles = new List<RoleData>();
			roleIdIndexesMapping = new Dictionary<string, int>();
			roleNames = new List<string>();

			notStaticRoles = new List<RoleData>();
			notStaticRoleIdIndexesMapping = new Dictionary<string, int>();
			notStaticRoleNames = new List<string>();

			obj = JsonManager.GetInstance().GetJson("RoleDatas", false);
			RoleData roleData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					roleData = JsonManager.GetInstance().DeserializeObject<RoleData>(item.Value.ToString());
					roleNames.Add(roleData.Name);
					roleIdIndexesMapping.Add(roleData.Id, index);
					roles.Add(roleData);
					if (!roleData.IsStatic) {
						notStaticRoleNames.Add(roleData.Name);
						notStaticRoleIdIndexesMapping.Add(roleData.Id, index);
						notStaticRoles.Add(roleData);
					}
					index++;
				}
			}

			books = new List<BookData>();
			bookIdIndexesMapping = new Dictionary<string, int>();
			bookNames = new List<string>();
			obj = JsonManager.GetInstance().GetJson("Books", false);
			BookData bookData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					bookData = JsonManager.GetInstance().DeserializeObject<BookData>(item.Value.ToString());
					bookNames.Add(bookData.Name);
					bookIdIndexesMapping.Add(bookData.Id, index);
					books.Add(bookData);
					index++;
				}
			}

			skills = new List<SkillData>();
			skillIdIndexesMapping = new Dictionary<string, int>();
			skillNames = new List<string>();
			obj = JsonManager.GetInstance().GetJson("Skills", false);
			SkillData skillData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					skillData = JsonManager.GetInstance().DeserializeObject<SkillData>(item.Value.ToString());
					skillNames.Add(skillData.Name);
					skillIdIndexesMapping.Add(skillData.Id, index);
					skills.Add(skillData);
					index++;
				}
			}

			weapons = new List<WeaponData>();
			weaponIdIndexesMapping = new Dictionary<string, int>();
			weaponNames = new List<string>();
			obj = JsonManager.GetInstance().GetJson("Weapons", false);
			WeaponData weaponData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					weaponData = JsonManager.GetInstance().DeserializeObject<WeaponData>(item.Value.ToString());
					weaponNames.Add(weaponData.Name);
					weaponIdIndexesMapping.Add(weaponData.Id, index);
					weapons.Add(weaponData);
					index++;
				}
			}

			allCitySceneIdIndexs = new Dictionary<string, int>();
			allCitySceneNames = new List<string>();
			allCityScenes = new List<SceneData>();
			obj = JsonManager.GetInstance().GetJson("Scenes", false);
			SceneData sceneData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					sceneData = JsonManager.GetInstance().DeserializeObject<SceneData>(item.Value.ToString());
					allCitySceneNames.Add(sceneData.Name);
					allCitySceneIdIndexs.Add(sceneData.Id, index);
					allCityScenes.Add(sceneData);
					index++;
				}
			}
		}

		static void DestroyParams() {
			taskTypeEnums.Clear();
			taskTypeStrs.Clear();
			taskTypeIndexMapping.Clear();
			taskDialogTypeEnums.Clear();
			taskDialogTypeStrs.Clear();
			taskDialogTypeIndexMapping.Clear();
			npcs.Clear();
			npcNames.Clear();
			npcIdIndexesMapping.Clear();
			fights.Clear();
			fightNames.Clear();
			fightIdIndexesMapping.Clear();
			roleNames.Clear();
			roleIdIndexesMapping.Clear();
			roles.Clear();
			bookNames.Clear();
			bookIdIndexesMapping.Clear();
			books.Clear();
			skillNames.Clear();
			skillIdIndexesMapping.Clear();
			skills.Clear();
			weaponNames.Clear();
			weaponIdIndexesMapping.Clear();
			weapons.Clear();
			allCitySceneIdIndexs.Clear();
			allCitySceneNames.Clear();
			allCityScenes.Clear();
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
		static List<string> taskDataNames;
		static Dictionary<string, int> taskDataIdIndexs;
		static List<TaskData> taskDatas;
		static void getData() {
			dataMapping = new Dictionary<string, TaskData>();
			taskDataNames = new List<string>();
			taskDataIdIndexs = new Dictionary<string, int>();
			taskDatas = new List<TaskData>();
			JObject obj = JsonManager.GetInstance().GetJson("Tasks", false);
			int index = 0;
			TaskData taskData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					taskData = JsonManager.GetInstance().DeserializeObject<TaskData>(item.Value.ToString());
					dataMapping.Add(item.Value["Id"].ToString(), taskData);
					taskDataNames.Add(taskData.Name);
					taskDataIdIndexs.Add(taskData.Id, index);
					taskDatas.Add(taskData);
					index++;
				}
			}
			taskDataNames.Add("无");
			taskDataIdIndexs.Add("0", index);
			TaskData noneTaskData = new TaskData();
			noneTaskData.Id = "0";
			taskDatas.Add(noneTaskData);
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
		string desc = "";
		bool canRepeat = false;
		bool isInaugurationTask = false;
		int inaugurationOccupationIndex = 0;
		int belongToNpcIdIndex;
		int belongToSceneIdIndex;
		int frontTaskDataIdIndex;
		int taskTypeIndex;
		int oldTaskTypeIndex = -1;
		int taskTypeValueIndex;
		string stringValue;
		int intValue;
		int minIntValue;
		int maxIntValue;

		List<int> dialogTypeIndexes;
		List<string> dialogTalkMsgs;
		List<int> dialogBackYesTaskDataIdIndexes;
		List<int> dialogBackNoTaskDataIdIndexes;
		List<int> dialogIconIdIndex;
		List<string> dialogYesMsgs;
		List<string> dialogNoMsgs;
		List<int> stringValueIndexes;
		List<string> stringValues;
		List<int> intValues;
		List<int> protectNpcIdIndexes;
		List<int> protectNpcToSceneNameIndexes;

		List<int> dropItemDataIdIndexs;
		List<float> dropRates;
		List<int> dropNums;
		List<int> dropMaxNums;

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
					desc = data.Desc;
					canRepeat = data.CanRepeat;
					isInaugurationTask = data.IsInaugurationTask;
					inaugurationOccupationIndex =  Base.OccupationTypeIndexMapping.ContainsKey(data.InaugurationOccupation) ? Base.OccupationTypeIndexMapping[data.InaugurationOccupation] : 0;
					belongToNpcIdIndex = npcIdIndexesMapping.ContainsKey(data.BelongToNpcId) ? npcIdIndexesMapping[data.BelongToNpcId] : 0;
					belongToSceneIdIndex = allCitySceneIdIndexs.ContainsKey(data.BelongToSceneId) ? allCitySceneIdIndexs[data.BelongToSceneId] : 0;
					frontTaskDataIdIndex = taskDataIdIndexs.ContainsKey(data.FrontTaskDataId) ? taskDataIdIndexs[data.FrontTaskDataId] : 0;
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
					case TaskType.IsInArea:
						taskTypeValueIndex = Base.AllAreaSceneNameIndexesMapping.ContainsKey(data.StringValue) ? Base.AllAreaSceneNameIndexesMapping[data.StringValue] : 0;
						break;
					default:
						break;
					}

					//对话信息初始化
					dialogTypeIndexes = new List<int>();
					dialogTalkMsgs = new List<string>();
					dialogBackYesTaskDataIdIndexes = new List<int>();
					dialogBackNoTaskDataIdIndexes = new List<int>();
					dialogIconIdIndex = new List<int>();
					dialogYesMsgs = new List<string>();
					dialogNoMsgs = new List<string>();
					stringValueIndexes = new List<int>();
					stringValues = new List<string>();
					intValues = new List<int>();
					protectNpcIdIndexes = new List<int>();
					protectNpcToSceneNameIndexes = new List<int>();

					TaskDialogData dialog;
					int stringValueIndex;
					int protectNpcIdIndex;
					int protectNpcToSceneNameIndex;
					for (int i = 0; i < data.Dialogs.Count; i++) {
						dialog = data.Dialogs[i];
						dialogTypeIndexes.Add(taskDialogTypeIndexMapping.ContainsKey(dialog.Type) ? taskDialogTypeIndexMapping[dialog.Type] : 0);
						dialogTalkMsgs.Add(dialog.TalkMsg);
						dialogBackYesTaskDataIdIndexes.Add(taskDataIdIndexs.ContainsKey(dialog.BackYesTaskDataId) ? taskDataIdIndexs[dialog.BackYesTaskDataId] : 0);
						dialogBackNoTaskDataIdIndexes.Add(taskDataIdIndexs.ContainsKey(dialog.BackNoTaskDataId) ? taskDataIdIndexs[dialog.BackNoTaskDataId] : 0);
						dialogIconIdIndex.Add(Base.IconIdIndexs.ContainsKey(dialog.IconId) ? Base.IconIdIndexs[dialog.IconId] : 0);
						dialogYesMsgs.Add(dialog.YesMsg);
						dialogNoMsgs.Add(dialog.NoMsg);
						stringValueIndex = 0;
						protectNpcIdIndex = 0;
						protectNpcToSceneNameIndex = 0;
						switch (dialog.Type) {
						case TaskDialogType.SendItem:
							stringValueIndex = Base.ItemDataIdIndexs.ContainsKey(dialog.StringValue) ? Base.ItemDataIdIndexs[dialog.StringValue] : 0;
							break;
						case TaskDialogType.ConvoyNpc:
							string[] fen = dialog.StringValue.Split(new char[] { '_' });
							if (fen.Length == 2) {
								protectNpcIdIndex = npcIdIndexesMapping.ContainsKey(fen[0]) ? npcIdIndexesMapping[fen[0]] : 0;
								protectNpcToSceneNameIndex = Base.AllAreaSceneNameIndexesMapping.ContainsKey(fen[1]) ? Base.AllAreaSceneNameIndexesMapping[fen[1]] : 0;
							}
							break;
						case TaskDialogType.FightWined:
						case TaskDialogType.EventFightWined:
							stringValueIndex = fightIdIndexesMapping.ContainsKey(dialog.StringValue) ? fightIdIndexesMapping[dialog.StringValue] : 0;
							break;
						case TaskDialogType.RecruitedThePartner:
							stringValueIndex = roleIdIndexesMapping.ContainsKey(dialog.StringValue) ? roleIdIndexesMapping[dialog.StringValue] : 0;
							break;
						case TaskDialogType.UsedTheBook:
							stringValueIndex = bookIdIndexesMapping.ContainsKey(dialog.StringValue) ? bookIdIndexesMapping[dialog.StringValue] : 0;
							break;
						case TaskDialogType.UsedTheSkillOneTime:
							stringValueIndex = skillIdIndexesMapping.ContainsKey(dialog.StringValue) ? skillIdIndexesMapping[dialog.StringValue] : 0;
							break;
						case TaskDialogType.UsedTheWeapon:
							stringValueIndex = weaponIdIndexesMapping.ContainsKey(dialog.StringValue) ? weaponIdIndexesMapping[dialog.StringValue] : 0;
							break;
						case TaskDialogType.SendResource:
							if (dialog.StringValue != "") {
								ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), dialog.StringValue);
								stringValueIndex = Base.ResourceTypeIndexMapping.ContainsKey(resourceType) ? Base.ResourceTypeIndexMapping[resourceType] : 0;
							}
							else {
								stringValueIndex = 0;
							}
							break;
						case TaskDialogType.PushRoleToWinshop:
							stringValueIndex = notStaticRoleIdIndexesMapping.ContainsKey(dialog.StringValue) ? roleIdIndexesMapping[dialog.StringValue] : 0;
							break;
						default:
							break;
						}
						stringValueIndexes.Add(stringValueIndex);
						stringValues.Add("");
						intValues.Add(dialog.IntValue);
						protectNpcIdIndexes.Add(protectNpcIdIndex);
						protectNpcToSceneNameIndexes.Add(protectNpcToSceneNameIndex);

						dropItemDataIdIndexs = new List<int>();
						dropRates = new List<float>();
						dropNums = new List<int>();
						dropMaxNums = new List<int>();
						foreach(DropData drop in data.Rewards) {
							if (Base.ItemDataIdIndexs.ContainsKey(drop.ResourceItemDataId)) {
								dropItemDataIdIndexs.Add(Base.ItemDataIdIndexs[drop.ResourceItemDataId]);
							}
							else {
								dropItemDataIdIndexs.Add(0);
							}
							dropRates.Add(drop.Rate);
							dropNums.Add(drop.Num);
							if (drop.Item != null) {
								dropMaxNums.Add(drop.Item.MaxNum);
							}
							else {
								dropMaxNums.Add(999);
							}
						}
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 1000, 80));
					GUI.Label(new Rect(0, 0, 20, 18), "Id:");
					EditorGUI.TextField(new Rect(25, 0, 100, 18), showId);
					GUI.Label(new Rect(130, 0, 50, 18), "任务名称:");
					name = EditorGUI.TextField(new Rect(185, 0, 150, 18), name);
					GUI.Label(new Rect(340, 0, 50, 18), "绑定Npc:");
					belongToNpcIdIndex = EditorGUI.Popup(new Rect(395, 0, 100, 18), belongToNpcIdIndex, npcNames.ToArray());
					GUI.Label(new Rect(500, 0, 55, 18), "绑定城镇:");
					belongToSceneIdIndex = EditorGUI.Popup(new Rect(560, 0, 100, 18), belongToSceneIdIndex, allCitySceneNames.ToArray());
					GUI.Label(new Rect(665, 0, 50, 18), "前置任务:");
					frontTaskDataIdIndex = EditorGUI.Popup(new Rect(720, 0, 100, 18), frontTaskDataIdIndex, taskDataNames.ToArray());
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
						if (Base.GenderTypeEnums.Count <= taskTypeValueIndex) {
							taskTypeValueIndex = 0;
						}
						intValue = (int)Base.GenderTypeEnums[taskTypeValueIndex];
						break;
					case TaskType.ItemInHand:
						GUI.Label(new Rect(290, 20, 60, 18), "要求道具为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.ItemDataNames.ToArray());
						if (Base.ItemDatas.Count <= taskTypeValueIndex) {
							taskTypeValueIndex = 0;
						}
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
						if (Base.OccupationTypeEnums.Count <= taskTypeValueIndex) {
							taskTypeValueIndex = 0;
						}
						intValue = (int)Base.OccupationTypeEnums[taskTypeValueIndex];
						break;
					case TaskType.TheHour:
						GUI.Label(new Rect(290, 20, 60, 18), "要求时辰为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.TimeNames.ToArray());
						intValue = taskTypeValueIndex;
						break;
					case TaskType.IsInArea:
						GUI.Label(new Rect(290, 20, 60, 18), "要求区域为:");
						taskTypeValueIndex = EditorGUI.Popup(new Rect(355, 20, 100, 18), taskTypeValueIndex, Base.AllAreaSceneNames.ToArray());
						if (Base.AllAreaSceneNames.Count <= taskTypeValueIndex) {
							taskTypeValueIndex = 0;
						}
						stringValue = Base.AllAreaSceneNames[taskTypeValueIndex];
						break;
					default:
						break;
					}
					GUI.Label(new Rect(0, 40, 60, 18), "一句话描述:");
					desc = EditorGUI.TextField(new Rect(65, 40, 400, 18), desc);
					GUI.Label(new Rect(470, 40, 30, 18), "重复:");
					canRepeat = EditorGUI.Toggle(new Rect(500, 40, 30, 18), canRepeat);
					GUI.Label(new Rect(535, 40, 60, 18), "就职任务:");
					isInaugurationTask = EditorGUI.Toggle(new Rect(600, 40, 30, 18), isInaugurationTask);
					if (isInaugurationTask) {
						inaugurationOccupationIndex = EditorGUI.Popup(new Rect(650, 40, 100, 18), inaugurationOccupationIndex, Base.OccupationTypeStrs.ToArray());
					}

					if (GUI.Button(new Rect(0, 60, 100, 18), "修改任务基础属性")) {
						if (name == "") {
							this.ShowNotification(new GUIContent("任务名不能为空!"));
							return;
						}
						if (desc == "") {
							this.ShowNotification(new GUIContent("一句话描述不能为空!"));
							return;
						}
						if (minIntValue > maxIntValue) {
							this.ShowNotification(new GUIContent("最小值不能大于最大值!"));
							return;
						}
						if (taskDatas[frontTaskDataIdIndex].Id == data.Id) {
							this.ShowNotification(new GUIContent("前置任务Id不能喝当前任务Id一致!"));
							return;
						}
						data.Name = name;
						data.Desc = desc;
						data.BelongToNpcId = npcs[belongToNpcIdIndex].Id;
						data.BelongToSceneId = allCityScenes[belongToSceneIdIndex].Id;
						data.Type = taskTypeEnums[taskTypeIndex];
						if (data.Type != TaskType.MoralRange) {
							minIntValue = 0;
							maxIntValue = 0;
						}
						data.FrontTaskDataId = taskDatas[frontTaskDataIdIndex].Id;
						data.StringValue = stringValue;
						data.IntValue = intValue;
						data.MinIntValue = minIntValue;
						data.MaxIntValue = maxIntValue;
						data.CanRepeat = canRepeat;
						data.IsInaugurationTask = isInaugurationTask;
						data.InaugurationOccupation = isInaugurationTask ? Base.OccupationTypeEnums[inaugurationOccupationIndex] : OccupationType.None;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 90, 1000, 830));
					GUI.Label(new Rect(0, 0, 1000, 18), "|-----------任务步骤------------------------------------------------------------------------------------------------------------------------|");
					addDialogTypeIndex = EditorGUI.Popup(new Rect(0, 20, 100, 18), addDialogTypeIndex, taskDialogTypeStrs.ToArray());
					if (GUI.Button(new Rect(105, 20, 60, 18), "添加步骤")) {
						if (data.Dialogs.Count >= 20) {
							this.ShowNotification(new GUIContent("剧情的对话步骤不能超过20条!"));
							return;
						}
						TaskDialogData dialogData = new TaskDialogData();
						dialogData.Type = taskDialogTypeEnums[addDialogTypeIndex];
						dialogData.TalkMsg = "步骤对话";
						dialogData.YesMsg = "步骤完成对话";
						data.Dialogs.Add(dialogData);
						dialogScrollPosition = new Vector2(0, 2000);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					float dialogContextHeight = dialogTypeIndexes.Count * 70;
					float dialogsStartX = 0;
					float dialogsStartY = 45;
					//开始滚动视图  
					dialogScrollPosition = GUI.BeginScrollView(new Rect(dialogsStartX, dialogsStartY, 800, 500), dialogScrollPosition, new Rect(0, 0, 780, dialogContextHeight), false, 500 < dialogContextHeight);
					for (int i = 0; i < dialogTypeIndexes.Count; i++) {
						GUILayout.BeginArea(new Rect(dialogsStartX, i * 70, 1000, 60));
						GUI.Label(new Rect(0, 0, 40, 18), string.Format("第{0}步:", i + 1));
						dialogTypeIndexes[i] = EditorGUI.Popup(new Rect(45, 0, 100, 18), dialogTypeIndexes[i], taskDialogTypeStrs.ToArray());
						GUI.Label(new Rect(150, 0, 50, 18), "头像Icond:");
						dialogIconIdIndex[i] = EditorGUI.Popup(new Rect(205, 0, 100, 18), dialogIconIdIndex[i], Base.IconNames.ToArray());

						GUI.Label(new Rect(0, 20, 40, 18), "对话:");
						dialogTalkMsgs[i] = EditorGUI.TextArea(new Rect(45, 20, 160, 40), dialogTalkMsgs[i]);
						if (taskDialogTypeEnums[dialogTypeIndexes[i]] != TaskDialogType.JustTalk && 
							taskDialogTypeEnums[dialogTypeIndexes[i]] != TaskDialogType.Notice) {
							GUI.Label(new Rect(210, 20, 40, 18), "完成后:");
							dialogYesMsgs[i] = EditorGUI.TextArea(new Rect(255, 20, 160, 40), dialogYesMsgs[i]);
						}
						else {
							dialogYesMsgs[i] = "空";
						}

						switch (taskDialogTypeEnums[dialogTypeIndexes[i]]) {
						case TaskDialogType.Choice:
							GUI.Label(new Rect(310, 0, 65, 18), "抉择是指向:");
							dialogBackYesTaskDataIdIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 150, 18), dialogBackYesTaskDataIdIndexes[i], taskDataNames.ToArray());
							GUI.Label(new Rect(535, 0, 65, 18), "抉择非指向:");
							dialogBackNoTaskDataIdIndexes[i] = EditorGUI.Popup(new Rect(605, 0, 150, 18), dialogBackNoTaskDataIdIndexes[i], taskDataNames.ToArray());
							GUI.Label(new Rect(420, 20, 40, 18), "抉择非:");
							dialogNoMsgs[i] = EditorGUI.TextArea(new Rect(465, 20, 160, 40), dialogNoMsgs[i]);
							break;
						case TaskDialogType.SendItem:
							GUI.Label(new Rect(310, 0, 65, 18), "需要的物品:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], Base.ItemDataNames.ToArray());
							if ( Base.ItemDatas.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = Base.ItemDatas[stringValueIndexes[i]].Id;
							GUI.Label(new Rect(485, 0, 50, 18), "物品数量:");
							intValues[i] = (int)EditorGUI.Slider(new Rect(550, 0, 180, 18), intValues[i], 1, 999);
							break;
						case TaskDialogType.ConvoyNpc:
							GUI.Label(new Rect (310, 0, 65, 18), "护送的Npc:");
							protectNpcIdIndexes[i] = EditorGUI.Popup(new Rect (380, 0, 100, 18), protectNpcIdIndexes[i], npcNames.ToArray());
							GUI.Label(new Rect(485, 0, 50, 18), "送到场景:");
							protectNpcToSceneNameIndexes[i] = EditorGUI.Popup(new Rect(550, 0, 100, 18), protectNpcToSceneNameIndexes[i], Base.AllAreaSceneNames.ToArray());
							stringValues[i] = npcs[protectNpcIdIndexes[i]].Id + "_" + Base.AllAreaSceneNames[protectNpcToSceneNameIndexes[i]];
							break;
						case TaskDialogType.FightWined:
						case TaskDialogType.EventFightWined:
							GUI.Label (new Rect (310, 0, 65, 18), "需战斗获胜:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], fightNames.ToArray());
							if (fights.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = fights[stringValueIndexes[i]].Id;
							break;
						case TaskDialogType.RecruitedThePartner:
							GUI.Label(new Rect(310, 0, 65, 18), "招募的侠客:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], roleNames.ToArray());
							if (roles.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = roles[stringValueIndexes[i]].Id;
							break;
						case TaskDialogType.UsedTheBook:
							GUI.Label(new Rect(310, 0, 65, 18), "装备上秘籍:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], bookNames.ToArray());
							if (books.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = books[stringValueIndexes[i]].Id;
							break;
						case TaskDialogType.UsedTheSkillOneTime:
							GUI.Label(new Rect(310, 0, 65, 18), "使用过招式:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], skillNames.ToArray());
							if (skills.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = skills[stringValueIndexes[i]].Id;
							break;
						case TaskDialogType.UsedTheWeapon:
							GUI.Label(new Rect(310, 0, 65, 18), "装备上武器:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], weaponNames.ToArray());
							if (weapons.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = weapons[stringValueIndexes[i]].Id;
							break;
						case TaskDialogType.WeaponPowerPlusSuccessed:
							stringValues[i] = "";
							GUI.Label(new Rect(310, 0, 170, 18), "程度(1为黄色, 2为橙色, 3为红色):");
							intValues[i] = (int)EditorGUI.Slider(new Rect(485, 0, 180, 18), intValues[i], 1, 3);
							break;
						case TaskDialogType.SendResource:
							GUI.Label(new Rect(310, 0, 65, 18), "需要的资源:");
							stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], Base.ResourceTypeStrs.ToArray());
							if (Base.ResourceTypeEnums.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = Base.ResourceTypeEnums[stringValueIndexes[i]].ToString();
							GUI.Label(new Rect(485, 0, 50, 18), "资源数量:");
							intValues[i] = (int)EditorGUI.Slider(new Rect(550, 0, 180, 18), intValues[i], 1, 99999);
							break;
						case TaskDialogType.TheHour:
							GUI.Label(new Rect(310, 0, 65, 18), "要求时辰为:");
							intValues[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), intValues[i], Base.TimeNames.ToArray());
							break;
						case TaskDialogType.PushRoleToWinshop:
							GUI.Label(new Rect(310, 0, 65, 18), "出现的侠客:");
							if (notStaticRoles.Count > 0) {
								stringValueIndexes[i] = EditorGUI.Popup(new Rect(380, 0, 100, 18), stringValueIndexes[i], notStaticRoleNames.ToArray());
							}
							if (notStaticRoles.Count <= stringValueIndexes[i]) {
								stringValueIndexes[i] = 0;
							}
							stringValues[i] = notStaticRoles[stringValueIndexes[i]].Id;
							break;
						default:
							stringValues[i] = "";
							intValues[i] = 0;
							break;
						}

						if (GUI.Button(new Rect(630, 20, 40, 40), "修改")) {
							if (string.IsNullOrEmpty(dialogTalkMsgs[i])) {
								this.ShowNotification(new GUIContent("对话不能为空!"));
								return;
							}
							if (string.IsNullOrEmpty(dialogYesMsgs[i])) {
								this.ShowNotification(new GUIContent("完成后的对话不能为空!"));
								return;
							}
							if (taskDialogTypeEnums[dialogTypeIndexes[i]] == TaskDialogType.Choice && string.IsNullOrEmpty(dialogNoMsgs[i])) {
								this.ShowNotification(new GUIContent("选择布尔非后的对话不能为空!"));
								return;
							}
							data.Dialogs[i].Type = taskDialogTypeEnums[dialogTypeIndexes[i]];
							data.Dialogs[i].IconId = Base.Icons[dialogIconIdIndex[i]].Id;
							data.Dialogs[i].TalkMsg = dialogTalkMsgs[i];
							data.Dialogs[i].YesMsg = dialogYesMsgs[i];
							data.Dialogs[i].NoMsg = dialogNoMsgs[i];
							data.Dialogs[i].StringValue = stringValues[i];
							data.Dialogs[i].IntValue = intValues[i];
							data.Dialogs[i].BackYesTaskDataId = dialogBackYesTaskDataIdIndexes[i] >= 0 ? taskDatas[dialogBackYesTaskDataIdIndexes[i]].Id : "";
							data.Dialogs[i].BackNoTaskDataId = dialogBackNoTaskDataIdIndexes[i] >= 0 ? taskDatas[dialogBackNoTaskDataIdIndexes[i]].Id : "";
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改成功"));
						}
						if (GUI.Button(new Rect(675, 20, 40, 40), "删除")) {
							if (data.Dialogs.Count > i) {
								data.Dialogs.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("删除成功"));
							}
							else {
								this.ShowNotification(new GUIContent("待删除数据不存在"));
							}
						}
						if (i > 0) {
							if (GUI.Button(new Rect(720, 20, 40, 20), "上移")) {
								TaskDialogData removeDialog = data.Dialogs[i];
								data.Dialogs.RemoveAt(i);
								data.Dialogs.Insert(i - 1, removeDialog);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
							}
						}
						if (i < dialogTypeIndexes.Count - 1) {
							if (GUI.Button(new Rect(720, 40, 40, 20), "下移")) {
								TaskDialogData removeDialog = data.Dialogs[i];
								data.Dialogs.Insert(i + 2, removeDialog);
								data.Dialogs.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
							}
						}
						GUILayout.EndArea();
					}
					GUI.EndScrollView();
					GUI.Label(new Rect(0, 550, 1000, 18), "|-----------任务奖励------------------------------------------------------------------------------------------------------------------------|");
					if (GUI.Button(new Rect(0, 570, 120, 18), "添加新的奖励")) {
						if (data.Rewards.Count >= 5) {
							this.ShowNotification(new GUIContent("一个任务最多添加5个奖励物品!"));
							return;
						}
						DropData newDrop = new DropData();
						data.Rewards.Add(newDrop);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("添加任务奖励成功"));
					}
					float rewardsStartX = 0;
					float rewardsStartY = 590;
					for (int i = 0; i < data.Rewards.Count; i++) {
						if (dropItemDataIdIndexs.Count <= i) {
							continue;
						}	
						GUI.Label(new Rect(rewardsStartX, rewardsStartY + i * 40, 40, 18), "物品名:");
						dropItemDataIdIndexs[i] = EditorGUI.Popup(new Rect(rewardsStartX + 45, rewardsStartY + i * 40, 165, 18), dropItemDataIdIndexs[i], Base.ItemDataNames.ToArray());
						GUI.Label(new Rect(rewardsStartX, rewardsStartY + 20 + i * 40, 40, 18), "数量:");
						dropNums[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(rewardsStartX + 45, rewardsStartY + 20 + i * 40, 60, 18), dropNums[i].ToString())), 1, dropMaxNums[i]);
						GUI.Label(new Rect(rewardsStartX + 110, rewardsStartY + 20 + i * 40, 40, 18), "概率:");
						dropRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(rewardsStartX + 155, rewardsStartY + 20 + i * 40, 60, 18), dropRates[i].ToString())), 0, 100);
						if (GUI.Button(new Rect(rewardsStartX + 220, rewardsStartY + i * 40, 40, 36), "修改")) {
							data.Rewards[i].ResourceItemDataId = Base.ItemDatas[dropItemDataIdIndexs[i]].Id;
							data.Rewards[i].Num = dropNums[i];
							data.Rewards[i].Rate = dropRates[i];
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改任务奖励成功"));
						}
						if (GUI.Button(new Rect(rewardsStartX + 265, rewardsStartY + i * 40, 40, 36), "-")) {
							if (data.Rewards.Count > i) {
								data.Rewards.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("修改任务奖励成功"));
							}
							else {
								this.ShowNotification(new GUIContent("要删除的数据不存在!"));
							}
						}	
					}

					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 900, 500, 60));
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
				GUI.Label(new Rect(0, 0, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 0, 80, 18), addId);
				GUI.Label(new Rect(120, 0, 50, 18), "任务名:");
				addName = GUI.TextField(new Rect(175, 0, 80, 18), addName);
				if (GUI.Button(new Rect(260, 0, 80, 18), "添加")) {
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
				if (GUI.Button(new Rect(345, 0, 80, 18), "取消")) {
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
			DestroyParams();
		}
	}
}
#endif