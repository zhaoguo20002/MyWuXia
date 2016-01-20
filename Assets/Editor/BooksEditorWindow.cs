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
	public class BooksEditorWindow : EditorWindow {

		static BooksEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Books Editor")]
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
			float width = 1024;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (BooksEditorWindow)EditorWindow.GetWindowWithRect(typeof(BooksEditorWindow), size, true, "秘籍编辑器");
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
		static Dictionary<string, SkillData> allSkillDataMapping;
		static List<SkillData> allSkillDatas;
		static Dictionary<string, int> allSkillDataIndexs;
		static List<string> allSkillDataNames;

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
					if (iconData.Name.IndexOf("秘籍-") < 0) {
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

			allSkillDataMapping = new Dictionary<string, SkillData>();
			allSkillDatas = new List<SkillData>();
			SkillData skill;
			obj = JsonManager.GetInstance().GetJson("Skills", false);
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					skill = JsonManager.GetInstance().DeserializeObject<SkillData>(item.Value.ToString());
					allSkillDataMapping.Add(skill.Id, skill);
					allSkillDatas.Add(skill);
				}
			}
			allSkillDataIndexs = new Dictionary<string, int>();
			allSkillDataNames = new List<string>();
			allSkillDatas.Sort((a, b) => a.Id.CompareTo(b.Id));
			allSkillDatas.Insert(0, null);
			for(int i = 0; i < allSkillDatas.Count; i++) {
				if (allSkillDatas[i] == null) {
					allSkillDataIndexs.Add("", i);
					allSkillDataNames.Add("无");
				}
				else {
					allSkillDataIndexs.Add(allSkillDatas[i].Id, i);
					allSkillDataNames.Add(allSkillDatas[i].Name);
				}
			}
		}

		static Dictionary<string, BookData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, BookData>();
			JObject obj = JsonManager.GetInstance().GetJson("Books", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<BookData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<BookData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<BookData>();
			foreach(BookData data in dataMapping.Values) {
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
			foreach(BookData data in dataMapping.Values) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Books.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		BookData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string bookName = "";
		string bookDesc = "";
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;

		List<Texture> skillIconTextures;
		List<int> skillIdIndexes;
		List<string> skillDescs;
		int addSkillIdIndex = 0;

		short toolState; //0正常 1增加 2删除

		string addId;
		string addBookName;
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
					showId = data.Id;
					bookName = data.Name;
					bookDesc = data.Desc;
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
					skillIconTextures = new List<Texture>();
					skillIdIndexes = new List<int>();
					skillDescs = new List<string>();
					data.MakeJsonToModel();
					GameObject iconPrefab;
					foreach(SkillData skillData in data.Skills) {
						//处理招式图标
						iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", skillData.IconId).Src);
						skillIconTextures.Add(iconPrefab.GetComponent<Image>().sprite.texture);
						DestroyImmediate(iconPrefab);
						//处理招式Id索引
						skillIdIndexes.Add(allSkillDataIndexs[skillData.Id]);
						Debug.LogWarning(skillData.Desc);
						skillDescs.Add(Statics.StripHtml(skillData.Desc));
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 100));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 50, 50), iconTexture);
					}
					showId = data.Id;
					GUI.Label(new Rect(55, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(100, 0, 100, 18), showId);
					GUI.Label(new Rect(205, 0, 40, 18), "名称:");
					bookName = EditorGUI.TextField(new Rect(250, 0, 100, 18), bookName);
					GUI.Label(new Rect(55, 20, 40, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(100, 20, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(355, 0, 40, 18), "描述:");
					bookDesc = EditorGUI.TextArea(new Rect(400, 0, 400, 80), bookDesc);
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(0, 65, 80, 18), "修改基础属性")) {
						if (bookName == "") {
							this.ShowNotification(new GUIContent("秘籍名不能为空!"));
							return;
						}
						data.Name = bookName;
						data.Desc = bookDesc;
						data.IconId = icons[iconIndex].Id;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 105, 800, 500));
					GUI.Label(new Rect(0, 0, 800, 18), "|----------秘籍招式--------------------------------------------------------------------------------------------------------------|");
					GUI.Label(new Rect(0, 20, 50, 18), "选择秘籍招式:");
					addSkillIdIndex = EditorGUI.Popup(new Rect(55, 20, 100, 18), addSkillIdIndex, allSkillDataNames.ToArray());
					if (GUI.Button(new Rect(160, 20, 80, 18), "添加新招式")) {
						if (addSkillIdIndex <= 0) {
							return;
						}	
						if (data.ResourceSkillDataIds.Count >= 5) {
							this.ShowNotification(new GUIContent("一本秘籍做多只能创建五个招式!"));
							return;
						}
						int index = data.ResourceSkillDataIds.FindIndex((item) => { return item == allSkillDatas[addSkillIdIndex].Id; });
						if (index >= 0) {
							this.ShowNotification(new GUIContent("此招式已存在!"));
							return;
						}
						data.ResourceSkillDataIds.Add(allSkillDatas[addSkillIdIndex].Id);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("添加成功"));
					}
					for (int i = 0; i < skillIconTextures.Count; i++) {
						GUI.DrawTexture(new Rect(0, 40 + i * 85, 50, 50), skillIconTextures[i]);
						skillIdIndexes[i] = EditorGUI.Popup(new Rect(55, 40 + i * 85, 100, 18), skillIdIndexes[i], allSkillDataNames.ToArray());
						GUI.TextArea(new Rect(55, 60 + i * 85, 600, 60), skillDescs[i]);
						if (GUI.Button(new Rect(665, 60 + i * 85, 50, 60), "修改")) {
							if (skillIdIndexes[i] <= 0) {
								return;
							}
							int index = data.ResourceSkillDataIds.FindIndex((item) => { return item == allSkillDatas[skillIdIndexes[i]].Id; });
							if (index >= 0 && index != i) {
								this.ShowNotification(new GUIContent("招式已存在, 不能修改!"));
								return;
							}
							if (data.ResourceSkillDataIds.Count > i) {
								data.ResourceSkillDataIds[i] = allSkillDatas[skillIdIndexes[i]].Id;
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("修改成功"));
							}
						}
						if (GUI.Button(new Rect(720, 60 + i * 85, 50, 60), "-")) {
							if (data.ResourceSkillDataIds.Count > i) {
								data.ResourceSkillDataIds.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								this.ShowNotification(new GUIContent("删除成功"));
							}
						}
					}
					GUILayout.EndArea();
				}
				
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 600, 300, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除")) {
					toolState = 2;
				}
				break;

			case 1:
				GUI.Label(new Rect(0, 0, 30, 18), "Id:");
				addId = EditorGUI.TextField(new Rect(35, 0, 100, 18), addId);
				GUI.Label(new Rect(140, 0, 60, 18), "秘籍名:");
				addBookName = EditorGUI.TextField(new Rect(205, 0, 100, 18), addBookName);
				if (GUI.Button(new Rect(0, 20, 60, 18), "确定添加")) {
					toolState = 0;
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addBookName == "") {
						this.ShowNotification(new GUIContent("秘籍名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					BookData addBookData = new BookData();
					addBookData.Id = addId;
					addBookData.Name = addBookName;
					dataMapping.Add(addId, addBookData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addBookName = "";
					this.ShowNotification(new GUIContent("添加成功"));
				}
				if (GUI.Button(new Rect(65, 20, 60, 18), "取消")) {
					toolState = 0;
				}
				break;

			case 2:
				if (GUI.Button(new Rect(0, 0, 60, 18), "确定删除")) {
					toolState = 0;
					if (!dataMapping.ContainsKey(data.Id)) {
						this.ShowNotification(new GUIContent("待删除的数据不存在!"));
						return;
					}
					dataMapping.Remove(data.Id);
					writeDataToJson();
					selGridInt = 0;
					oldSelGridInt = -1;
					getData();
					fetchData(searchKeyword);
					this.ShowNotification(new GUIContent("删除成功"));
				}
				if (GUI.Button(new Rect(65, 0, 60, 18), "取消")) {
					toolState = 0;
				}
				break;
			default:
				break;
			}
			GUILayout.EndArea();
	    }

		/// <summary>
		/// 根据buff类型获取数值上限
		/// </summary>
		/// <returns>The buff value range top.</returns>
		/// <param name="buffType">Buff type.</param>
		float getBuffValueRangeTop(BuffType buffType) {
			switch (buffType) {
			case BuffType.Fast:	
			case BuffType.Slow:
			case BuffType.IncreaseHurtCutRate:
			case BuffType.IncreaseMagicAttackRate:
			case BuffType.IncreaseMagicDefenseRate:
			case BuffType.IncreaseMaxHPRate:
			case BuffType.IncreasePhysicsAttackRate:
			case BuffType.IncreasePhysicsDefenseRate:		
				return 1;
			default:
				return 100000;
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