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
	public class ItemDatasEditorWindow : EditorWindow {

		static ItemDatasEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Item Datas Editor")]
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
			float width = 860;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (ItemDatasEditorWindow)EditorWindow.GetWindowWithRect(typeof(ItemDatasEditorWindow), size, true, "物品编辑器");
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

		static List<ItemType> itemTypeEnums;
		static List<string> itemTypeStrs;
		static Dictionary<ItemType, int> itemTypeIndexMapping;

		static List<string> weaponNames;
		static Dictionary<string, int> weaponIdIndexs;
		static List<WeaponData> weapons;

		static List<string> bookNames;
		static Dictionary<string, int> bookIdIndexesMapping;
		static List<BookData> books;

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
					if (iconData.Name.IndexOf("物品-") < 0 && iconData.Name.IndexOf("物品资源-") < 0) {
						continue;
					}
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
                    if (iconPrefab != null) {
                        iconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
                        DestroyImmediate(iconPrefab);
                        iconNames.Add(iconData.Name);
                        iconIdIndexs.Add(iconData.Id, index);
                        icons.Add(iconData);
                        index++;
                    } else {
                        Debug.LogWarning(string.Format("解析icon资源出错: iconDataId: {0}, 路径: {1}", iconData.Id, JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src));
                    }
				}
			}

			weaponNames = new List<string>();
			weaponIdIndexs = new Dictionary<string, int>();
			weapons = new List<WeaponData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("Weapons", false);
			WeaponData weaponData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					weaponData = JsonManager.GetInstance().DeserializeObject<WeaponData>(item.Value.ToString());
					weaponNames.Add(weaponData.Name);
					weaponIdIndexs.Add(weaponData.Id, index);
					weapons.Add(weaponData);
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

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;

			//加载全部的ItemType枚举类型
			itemTypeEnums = new List<ItemType>();
			itemTypeStrs = new List<string>();
			itemTypeIndexMapping = new Dictionary<ItemType, int>();
			index = 0;
			foreach(ItemType type in Enum.GetValues(typeof(ItemType))) {
				itemTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				itemTypeStrs.Add(attrib.Description);
				itemTypeIndexMapping.Add(type, index);
				index++;
			}
		}

		static Dictionary<string, ItemData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, ItemData>();
			JObject obj = JsonManager.GetInstance().GetJson("ItemDatas", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<ItemData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<ItemData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<ItemData>();
			foreach(ItemData data in dataMapping.Values) {
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
			List<ItemData> datas = new List<ItemData>();
			foreach(ItemData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(ItemData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "ItemDatas.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		ItemData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string itemName = "";
		int iconIdIndex = 0;
		Texture iconTexture = null;
		int typeIndex = 0;
		int stringValueIndex = 0;
		string stringValue = "";
		string itemDesc = "";
		int maxNum = 0;
		int sellPrice = -1;
		bool canDiscard = true;
		int buyPrice = 1;
		int lv = 1;

		bool willDelete;
		bool willAdd;
		string addId = "";
		string addItemName = "";
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
					itemName = data.Name;
					if (iconIdIndexs.ContainsKey(data.IconId)) {
						iconIdIndex = iconIdIndexs[data.IconId];
					}
					else {
						iconIdIndex = 0;
					}
					if (iconTextureMappings.ContainsKey(data.IconId)) {
						iconTexture = iconTextureMappings[data.IconId];	
					}
					else {
						iconTexture = null;
					}	
					typeIndex = itemTypeIndexMapping[data.Type];
					switch (data.Type) {
					case ItemType.Weapon:
						stringValueIndex = !string.IsNullOrEmpty(data.StringValue) && weaponIdIndexs.ContainsKey(data.StringValue) ? weaponIdIndexs[data.StringValue] : 0;
						break;
					case ItemType.Book:
						stringValueIndex = !string.IsNullOrEmpty(data.StringValue) && bookIdIndexesMapping.ContainsKey(data.StringValue) ? bookIdIndexesMapping[data.StringValue] : 0;
						break;
					default:
						break;
					}
					itemDesc = data.Desc;
					maxNum = data.MaxNum;
					sellPrice = data.SellPrice;
					canDiscard = data.CanDiscard;
					buyPrice = data.BuyPrice;
					lv = data.Lv;
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 700, 270));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 60, 60), iconTexture);
					}
					showId = data.Id;
					GUI.Label(new Rect(65, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(110, 0, 100, 18), showId);
					GUI.Label(new Rect(215, 0, 40, 18), "名称:");
					itemName = EditorGUI.TextField(new Rect(260, 0, 100, 18), itemName);
					GUI.Label(new Rect(65, 20, 40, 18), "Icon:");
					iconIdIndex = EditorGUI.Popup(new Rect(110, 20, 100, 18), iconIdIndex, iconNames.ToArray());
					GUI.Label(new Rect(215, 20, 40, 18), "类型:");
					typeIndex = EditorGUI.Popup(new Rect(260, 20, 100, 18), typeIndex, itemTypeStrs.ToArray());
					switch (itemTypeEnums[typeIndex]) {
					case ItemType.Weapon:
						stringValueIndex = EditorGUI.Popup(new Rect(365, 20, 100, 18), stringValueIndex, weaponNames.ToArray());
						stringValue = weapons[stringValueIndex].Id;
						break;
					case ItemType.Book:
						stringValueIndex = EditorGUI.Popup(new Rect(365, 20, 100, 18), stringValueIndex, bookNames.ToArray());
						stringValue = books[stringValueIndex].Id;
						break;
					default:
						break;
					}
					GUI.Label(new Rect(65, 40, 40, 18), "描述:");
					itemDesc = GUI.TextArea(new Rect(110, 40, 250, 60), itemDesc);
					GUI.Label(new Rect(65, 105, 60, 18), "堆叠上限:");
					maxNum = (int)EditorGUI.Slider(new Rect(130, 105, 180, 18), maxNum, 1, 999);
					GUI.Label(new Rect(65, 125, 60, 18), "出售价格:");
					sellPrice = (int)EditorGUI.Slider(new Rect(130, 125, 180, 18), sellPrice, -1, 100000);
					GUI.Label(new Rect(65, 145, 60, 18), "购买价格:");
					buyPrice = (int)EditorGUI.Slider(new Rect(130, 145, 180, 18), buyPrice, 1, 100000);
					GUI.Label(new Rect(65, 165, 60, 18), "物品等级:");
					lv = (int)EditorGUI.Slider(new Rect(130, 165, 180, 18), lv, 1, 10);
					GUI.Label(new Rect(65, 185, 60, 18), "能否丢弃:");
					canDiscard = EditorGUI.Toggle(new Rect(130, 185, 18, 18), canDiscard);

					if (!willDelete) {
						if (GUI.Button(new Rect(0, 220, 80, 36), "修改")) {
							if (itemName == "") {
								this.ShowNotification(new GUIContent("名称不能为空!"));
								return;
							}
							data.Name = itemName;
							data.IconId = icons[iconIdIndex].Id;
							data.Type = itemTypeEnums[typeIndex];
							data.Desc = itemDesc;
							data.MaxNum = maxNum;
							data.SellPrice = sellPrice;
							data.CanDiscard = canDiscard;
							data.BuyPrice = buyPrice;
							data.ChangeToId = "";
							data.Lv = lv;
							data.StringValue = stringValue;
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改成功"));
						}
						if (GUI.Button(new Rect(85, 220, 80, 36), "删除")) {
							willDelete = true;
						}
					}
					else {
						if (GUI.Button(new Rect(0, 220, 80, 36), "确定删除")) {
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
						if (GUI.Button(new Rect(85, 220, 80, 36), "取消")) {
							willDelete = false;
						}
					}
					GUILayout.EndArea();
				}
			}
	    	
			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 275, 300, 160));
			GUI.Label(new Rect(0, 0, 300, 18), "|----添加新物品----------------------------------------------|");
			GUI.Label(new Rect(0, 20, 60, 18), "Id:");
			addId = EditorGUI.TextField(new Rect(65, 20, 200, 18), addId);
			GUI.Label(new Rect(0, 40, 60, 18), "名称:");
			addItemName = EditorGUI.TextField(new Rect(65, 40, 200, 18), addItemName);
			if (GUI.Button(new Rect(0, 85, 80, 36), "添加")) {
				if (addItemName == "") {
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
				ItemData newData = new ItemData();
				newData.Id = addId;
				newData.Name = addItemName;
				dataMapping.Add(addId, newData);
				writeDataToJson();
				addedId = addId;
				getData();
				fetchData(searchKeyword);
				addId = "";
				addItemName = "";
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
			
		}
	}
}
#endif