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
	public class FightsEditorWindow : EditorWindow {

		static FightsEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Fights Editor")]
		static void OpenWindow() {
			JsonManager.GetInstance().Clear();
			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex0", 0);
			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex1", 0);
			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex2", 0);
			EditorApplication.OpenScene("Assets/Scenes/FightTest.unity");
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
			float width = 760;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (FightsEditorWindow)EditorWindow.GetWindowWithRect(typeof(FightsEditorWindow), size, true, "战斗编辑器");
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

		static List<string> roleNames;
		static Dictionary<string, int> roleIdIndexs;
		static List<RoleData> roles;


		static List<string> itemNames;
		static Dictionary<string, int> itemIdIndexs;
		static List<ItemData> items;

		static List<FightType> fightTypeEnums;
		static List<string> fightTypeStrs;
		static Dictionary<FightType, int> fightTypeIndexMapping;

		static int testRoleIdIndex0 = 0;
		static int testRoleIdIndex1 = 0;
		static int testRoleIdIndex2 = 0;

		static void InitParams() { 
			int index = 0;
			roleNames = new List<string>();
			roleIdIndexs = new Dictionary<string, int>();
			roles = new List<RoleData>();
			JObject obj = JsonManager.GetInstance().GetJson("RoleDatas", false);
			RoleData roleData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					roleData = JsonManager.GetInstance().DeserializeObject<RoleData>(item.Value.ToString());
					roleNames.Add(roleData.Name);
					roleIdIndexs.Add(roleData.Id, index);
					roles.Add(roleData);
					index++;
				}
			}

			itemNames = new List<string>();
			itemIdIndexs = new Dictionary<string, int>();
			items = new List<ItemData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("ItemDatas", false);
			ItemData itemData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					itemData = JsonManager.GetInstance().DeserializeObject<ItemData>(item.Value.ToString());
					itemNames.Add(itemData.Name);
					itemIdIndexs.Add(itemData.Id, index);
					items.Add(itemData);
					index++;
				}
			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;

			//加载全部的FightType枚举类型
			fightTypeEnums = new List<FightType>();
			fightTypeStrs = new List<string>();
			fightTypeIndexMapping = new Dictionary<FightType, int>();
			index = 0;
			foreach(FightType type in Enum.GetValues(typeof(FightType))) {
				fightTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				fightTypeStrs.Add(attrib.Description);
				fightTypeIndexMapping.Add(type, index);
				index++;
			}

			testRoleIdIndex0 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex0");
			testRoleIdIndex1 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex1");
			testRoleIdIndex2 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex2");
		}

		static Dictionary<string, FightData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, FightData>();
			JObject obj = JsonManager.GetInstance().GetJson("Fights", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<FightData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<FightData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<FightData>();
			foreach(FightData data in dataMapping.Values) {
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
			foreach(FightData data in dataMapping.Values) {
				data.Enemys.Clear();
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Fights.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		FightData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string fightName = "";
		int typeIndex = 0;
		List<RoleData> enemyDatas;
		List<int> dropItemDataIdIndexs;
		List<float> dropRates;
		List<int> dropNums;
		List<int> dropMaxNums;

		int addEnemyIdIndex = 0;

		bool willDelete;
		bool willAdd;
		string addId = "";
		string addFightName = "";
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
					willDelete = false;
					fightName = data.Name;
					typeIndex = fightTypeIndexMapping[data.Type];
					data.MakeJsonToModel();
					enemyDatas = new List<RoleData>();
					foreach(RoleData enemy in data.Enemys) {
						enemyDatas.Add(enemy);
					}
					dropItemDataIdIndexs = new List<int>();
					dropRates = new List<float>();
					dropNums = new List<int>();
					dropMaxNums = new List<int>();
					foreach(DropData drop in data.Drops) {
						if (itemIdIndexs.ContainsKey(drop.ResourceItemDataId)) {
							dropItemDataIdIndexs.Add(itemIdIndexs[drop.ResourceItemDataId]);
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
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 555, 500));
					showId = data.Id;
					GUI.Label(new Rect(0, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(45, 0, 100, 18), showId);
					GUI.Label(new Rect(150, 0, 40, 18), "名称:");
					fightName = EditorGUI.TextField(new Rect(195, 0, 100, 18), fightName);
					GUI.Label(new Rect(0, 20, 40, 18), "类型:");
					typeIndex = EditorGUI.Popup(new Rect(45, 20, 100, 18), typeIndex, fightTypeStrs.ToArray());

					GUI.Label(new Rect(0, 40, 40, 18), "敌人:");
					addEnemyIdIndex = EditorGUI.Popup(new Rect(45, 40, 100, 18), addEnemyIdIndex, roleNames.ToArray());
					if (GUI.Button(new Rect(150, 40, 40, 18), "添加")) {
						if (data.ResourceEnemyIds.Count >= 10) {
							this.ShowNotification(new GUIContent("一场战斗最多添加10个敌人!"));
							return;
						}
						data.ResourceEnemyIds.Add(roles[addEnemyIdIndex].Id);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("添加敌人成功"));
					}
					for (int i = 0; i < enemyDatas.Count; i++) {
						GUI.Label(new Rect(45, 60 + i * 20, 100, 18), enemyDatas[i].Name);
						if (GUI.Button(new Rect(150, 60 + i * 20, 40, 18), "-")) {
							if (data.ResourceEnemyIds.Count > i) {
								data.ResourceEnemyIds.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("删除敌人成功"));
							}
						}
					}

					GUI.Label(new Rect(205, 40, 75, 18), "掉落物:");
					if (GUI.Button(new Rect(390, 40, 120, 18), "添加新的掉落物")) {
						if (data.Drops.Count >= 5) {
							this.ShowNotification(new GUIContent("一场战斗最多添加5个掉落物!"));
							return;
						}
						DropData newDrop = new DropData();
						data.Drops.Add(newDrop);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("添加掉落物成功"));
					}
					for (int i = 0; i < data.Drops.Count; i++) {
						if (dropItemDataIdIndexs.Count <= i) {
							continue;
						}	
						GUI.Label(new Rect(205, 60 + i * 40, 40, 18), "物品名:");
						dropItemDataIdIndexs[i] = EditorGUI.Popup(new Rect(255, 60 + i * 40, 165, 18), dropItemDataIdIndexs[i], itemNames.ToArray());
						GUI.Label(new Rect(205, 80 + i * 40, 40, 18), "数量:");
						dropNums[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(250, 80 + i * 40, 60, 18), dropNums[i].ToString())), 1, dropMaxNums[i]);
						GUI.Label(new Rect(315, 80 + i * 40, 40, 18), "概率:");
						dropRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(360, 80 + i * 40, 60, 18), dropRates[i].ToString())), 0, 100);
						if (GUI.Button(new Rect(425, 60 + i * 40, 40, 36), "修改")) {
							data.Drops[i].ResourceItemDataId = items[dropItemDataIdIndexs[i]].Id;
							data.Drops[i].Num = dropNums[i];
							data.Drops[i].Rate = dropRates[i];
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改掉落物成功"));
						}
						if (GUI.Button(new Rect(470, 60 + i * 40, 40, 36), "-")) {
							if (data.Drops.Count > i) {
								data.Drops.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("修改掉落物成功"));
							}
							else {
								this.ShowNotification(new GUIContent("要删除的数据不存在!"));
							}
						}	
					}

					if (!willDelete) {
						if (GUI.Button(new Rect(0, 460, 80, 36), "修改")) {
							if (fightName == "") {
								this.ShowNotification(new GUIContent("名称不能为空!"));
								return;
							}
							data.Name = fightName;
							data.Type = fightTypeEnums[typeIndex];
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改成功"));
						}
						if (GUI.Button(new Rect(85, 460, 80, 36), "删除")) {
							willDelete = true;
						}
						if (GUI.Button(new Rect(170, 460, 80, 36), "预览")) {
							PlayerPrefs.SetString("FightEditorCurrentId", data.Id);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex0", testRoleIdIndex0);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex1", testRoleIdIndex1);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex2", testRoleIdIndex2);
							PlayerPrefs.SetString("FightEditorTestRoleId0", roles[testRoleIdIndex0].Id);
							PlayerPrefs.SetString("FightEditorTestRoleId1", roles[testRoleIdIndex1].Id);
							PlayerPrefs.SetString("FightEditorTestRoleId2", roles[testRoleIdIndex2].Id);
							EditorApplication.isPlaying = true;
						}
						testRoleIdIndex0 = EditorGUI.Popup(new Rect(255, 460, 90, 18), testRoleIdIndex0, roleNames.ToArray());
						testRoleIdIndex1 = EditorGUI.Popup(new Rect(360, 460, 90, 18), testRoleIdIndex1, roleNames.ToArray());
						testRoleIdIndex2 = EditorGUI.Popup(new Rect(465, 460, 90, 18), testRoleIdIndex2, roleNames.ToArray());
					}
					else {
						if (GUI.Button(new Rect(0, 460, 80, 36), "确定删除")) {
							if (!dataMapping.ContainsKey(data.Id)) {
								this.ShowNotification(new GUIContent("要删除的数据不存在!"));
								return;
							}
							dataMapping.Remove(data.Id);
							writeDataToJson();
							selGridInt = 0;
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("删除成功"));
							willDelete = false;
						}
						if (GUI.Button(new Rect(85, 460, 80, 36), "取消")) {
							willDelete = false;
						}
					}
					GUILayout.EndArea();
				}
			}
			else {
				if (GUI.Button(new Rect(10, 30, 100, 50), "结束预览")) {
					EditorApplication.isPlaying = false;
					if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorCurrentId"))) {
						addedId = PlayerPrefs.GetString("FightEditorCurrentId");
						PlayerPrefs.SetString("FightEditorCurrentId", "");
					}
					InitParams();
					oldSelGridInt = -1;
					getData();
					fetchData(searchKeyword);
				}
			}
	    	
			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 505, 400, 160));
			GUI.Label(new Rect(0, 0, 300, 18), "|----添加新战斗----------------------------------------------|");
			GUI.Label(new Rect(0, 20, 60, 18), "Id:");
			addId = EditorGUI.TextField(new Rect(65, 20, 200, 18), addId);
			GUI.Label(new Rect(0, 40, 60, 18), "名称:");
			addFightName = EditorGUI.TextField(new Rect(65, 40, 200, 18), addFightName);
			if (GUI.Button(new Rect(0, 85, 80, 36), "添加")) {
				if (addFightName == "") {
					this.ShowNotification(new GUIContent("名称不能为空!"));
					return;
				}
				if (addId == "") {
					this.ShowNotification(new GUIContent("Id不能为空!"));
					return;
				}
				if (dataMapping.ContainsKey(addId)) {
					this.ShowNotification(new GUIContent("Id已存在!"));
					return;
				}
				FightData newData = new FightData();
				newData.Id = addId;
				newData.Name = addFightName;
				dataMapping.Add(addId, newData);
				writeDataToJson();
				addedId = addId;
				getData();
				fetchData(searchKeyword);
				addId = "";
				addFightName = "";
				this.ShowNotification(new GUIContent("添加成功"));
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
			EditorApplication.OpenScene("Assets/Scenes/Index.unity");
		}
	}
}
#endif