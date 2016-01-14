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
	public class WeaponsEditorWindow : EditorWindow {

		static WeaponsEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Weapons Editor")]
		static void OpenWindow() {
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
				window = (WeaponsEditorWindow)EditorWindow.GetWindowWithRect(typeof(WeaponsEditorWindow), size, true, "武器数据编辑器");
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

		static Dictionary<string, Texture> iconTextureMappings;
		static List<string> iconNames;
		static Dictionary<string, int> iconIdIndexs;
		static List<ResourceSrcData> icons;
		static List<QualityType> qualityTypeEnums;
		static List<string> qualityTypeStrs;
		static Dictionary<QualityType, int> qualityTypeIndexMapping;

		static void InitParams() { 
			//加载全部的icon对象
			iconTextureMappings = new Dictionary<string, Texture>();
			iconNames = new List<string>();
			iconIdIndexs = new Dictionary<string, int>();
			icons = new List<ResourceSrcData>();
			int index = 0;
			JObject obj = JsonManager.GetInstance().GetJson("Icons", false);
			ResourceSrcData iconData;
			GameObject iconPrefab;
			foreach(var item in obj) {
				if (item.Key != "0") {
					iconData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
					if (iconData.Name.IndexOf("武器-") < 0) {
						continue;
					}
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
					iconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(iconPrefab);
					iconNames.Add(iconData.Name);
					iconIdIndexs.Add(iconData.Id, index);
					icons.Add(iconData);
					index++;
				}
			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的QualityType枚举类型
			qualityTypeEnums = new List<QualityType>();
			qualityTypeStrs = new List<string>();
			qualityTypeIndexMapping = new Dictionary<QualityType, int>();
			index = 0;
			foreach(QualityType type in Enum.GetValues(typeof(QualityType))) {
				qualityTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				qualityTypeStrs.Add(attrib.Description);
				qualityTypeIndexMapping.Add(type, index);
				index++;
			}
		}

		static Dictionary<string, WeaponData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, WeaponData>();
			JObject obj = JsonManager.GetInstance().GetJson("Weapons", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<WeaponData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<WeaponData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<WeaponData>();
			foreach(WeaponData data in dataMapping.Values) {
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
			foreach(WeaponData data in dataMapping.Values) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Weapons.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		WeaponData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string weaponName = "";
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int qualityTypeIndex = 0;
		float[] rates;
		string weaponDesc = "";

		short toolState = 0; //0正常 1添加 2删除

		string addId = "";
		string addWeaponName = "";
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
					weaponName = data.Name;
					if (iconIdIndexs.ContainsKey(data.IconId)) {
						iconIndex = iconIdIndexs[data.IconId];
					}
					else {
						iconIndex = 0;
					}
					if (iconTextureMappings.ContainsKey(data.IconId)) {
						iconTexture = iconTextureMappings[data.IconId];	
					}
					else {
						iconTexture = null;
					}	
					qualityTypeIndex = qualityTypeIndexMapping[data.Quality];
					rates = data.Rates;
					weaponDesc = data.Desc;
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 280));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 120, 120), iconTexture);
					}
					GUI.Label(new Rect(125, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(190, 0, 100, 18), showId);
					GUI.Label(new Rect(125, 20, 60, 18), "武器名称:");
					weaponName = EditorGUI.TextField(new Rect(190, 20, 100, 18), weaponName);
					GUI.Label(new Rect(125, 40, 60, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(190, 40, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(125, 60, 60, 18), "品质:");
					qualityTypeIndex = EditorGUI.Popup(new Rect(190, 60, 100, 18), qualityTypeIndex, qualityTypeStrs.ToArray());
					GUI.Label(new Rect(125, 80, 100, 18), "25%追加伤害:");
					rates[1] = EditorGUI.Slider(new Rect(230, 80, 180, 18), rates[1], 0, 1);
					GUI.Label(new Rect(125, 100, 100, 18), "50%追加伤害:");
					rates[2] = EditorGUI.Slider(new Rect(230, 100, 180, 18), rates[2], 0, 1);
					GUI.Label(new Rect(125, 120, 100, 18), "100%追加伤害:");
					rates[3] = EditorGUI.Slider(new Rect(230, 120, 180, 18), rates[3], 0, 1);
					GUI.Label(new Rect(0, 140, 60, 18), "描述:");
					weaponDesc = GUI.TextArea(new Rect(45, 140, 370, 100), weaponDesc);

					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(332, 250, 80, 18), "修改武器属性")) {
						if (weaponName == "") {
							this.ShowNotification(new GUIContent("武器名不能为空!"));
							return;
						}
						data.Name = weaponName;
						data.IconId = icons[iconIndex].Id;
						data.Quality = qualityTypeEnums[qualityTypeIndex];
						data.Rates[1] = rates[1];
						data.Rates[2] = rates[2];
						data.Rates[3] = rates[3];
						data.Desc = weaponDesc;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 280, 500, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加武器")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除武器")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 0, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 0, 80, 18), addId);
				GUI.Label(new Rect(120, 0, 50, 18), "武器名:");
				addWeaponName = GUI.TextField(new Rect(175, 0, 80, 18), addWeaponName);
				if (GUI.Button(new Rect(260, 0, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addWeaponName == "") {
						this.ShowNotification(new GUIContent("武器名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					WeaponData weapon = new WeaponData();
					weapon.Id = addId;
					weapon.Name = addWeaponName;
					dataMapping.Add(weapon.Id, weapon);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addWeaponName = "";
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
			
		}
	}
}
#endif