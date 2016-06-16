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
	public class ScenesEditorWindow : EditorWindow {

		static ScenesEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Scenes Editor")]
		static void OpenWindow() {
			Base.InitParams();
			InitParams();
			Open();
		}

		static List<string> storeNames;
		static Dictionary<string, int> storeIdIndexs;
		static List<StoreData> stores;

		static List<string> allNpcNames;
		static Dictionary<string, int> allNpcIdIndexs;
		static List<NpcData> allNpcs;
		static void InitParams() {
			storeNames = new List<string>();
			storeIdIndexs = new Dictionary<string, int>();
			stores = new List<StoreData>();
			int index = 0;
			JObject obj = JsonManager.GetInstance().GetJson("Stores", false);
			StoreData storeData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					storeData = JsonManager.GetInstance().DeserializeObject<StoreData>(item.Value.ToString());
					storeNames.Add(storeData.Name);
					storeIdIndexs.Add(storeData.Id, index);
					stores.Add(storeData);
					index++;
				}
			}

			allNpcNames = new List<string>();
			allNpcIdIndexs = new Dictionary<string, int>();
			allNpcs = new List<NpcData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("Npcs", false);
			NpcData npcData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					npcData = JsonManager.GetInstance().DeserializeObject<NpcData>(item.Value.ToString());
					allNpcNames.Add(npcData.Name);
					allNpcIdIndexs.Add(npcData.Id, index);
					allNpcs.Add(npcData);
					index++;
				}
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
			float width = 860;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (ScenesEditorWindow)EditorWindow.GetWindowWithRect(typeof(ScenesEditorWindow), size, true, "城镇场景编辑器");
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

		static Dictionary<string,  SceneData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string,  SceneData>();
			JObject obj = JsonManager.GetInstance().GetJson("Scenes", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject< SceneData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List< SceneData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List< SceneData>();
			foreach( SceneData data in dataMapping.Values) {
				if (keyword != "") {
					if (data.Name.IndexOf(keyword) < 0) {
						continue;
					}
				}
				showListData.Add(data);
			}

			listNames = new List<string>();
//			showListData.Sort((a, b) => a.Id.CompareTo(b.Id));
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
			List< SceneData> datas = new List< SceneData>();
			foreach( SceneData data in dataMapping.Values) {
				datas.Add(data);
			}
//			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(SceneData data in datas) {
				data.FloydIndex = index;
				if (index == 0) {
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				index++;
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Scenes.json", JsonManager.GetInstance().SerializeObject(writeJson));
			TextAsset disAsset = Resources.Load<TextAsset>("Data/Json/FloydDis");
			List<List<float>> dis = null;
			Dictionary<int, string> cityIndexToIdMapping = new Dictionary<int, string>(); //城镇在临接矩阵中的索引与id的关联表
			Dictionary<int, string> cityIndexToNameMapping = new Dictionary<int, string>(); //城镇在临接矩阵中的索引与城镇名的关联表
			//如果临接矩阵没有创建则创建
			if (disAsset == null) {
				dis = new List<List<float>>();
				for (int i = 0; i < datas.Count; i++) {
					dis.Add(new List<float>());
					for (int j = 0; j < datas.Count; j++) {
						dis[i].Add(1000);
					}
				}
				Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "FloydDis.json", JsonManager.GetInstance().SerializeObject(dis));
			}
			else {
				dis = JsonManager.GetInstance().DeserializeObject<List<List<float>>>(disAsset.text);
				if (dis.Count < datas.Count) {
					//新增列
					for (int i = 0; i < dis.Count; i++) {
						for (int j = dis.Count; j < datas.Count; j++) {
							dis[i].Add(1000);
						}
					}
					//新增行
					for (int i = dis.Count; i < datas.Count; i++) {
						dis.Add(new List<float>());
						for (int j = 0; j < datas.Count; j++) {
							dis[i].Add(1000);
						}
					}
					Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "FloydDis.json", JsonManager.GetInstance().SerializeObject(dis));
				}
			}
			for (int i = 0; i < datas.Count; i++) {
				if (!cityIndexToIdMapping.ContainsKey(datas[i].FloydIndex)) {
					cityIndexToIdMapping.Add(datas[i].FloydIndex, datas[i].Id);
					cityIndexToNameMapping.Add(datas[i].FloydIndex, datas[i].Name);
				}
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "SceneIndexToIds.json", JsonManager.GetInstance().SerializeObject(cityIndexToIdMapping));
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "SceneIndexToNames.json", JsonManager.GetInstance().SerializeObject(cityIndexToNameMapping));
		}

		 SceneData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string name = "";
		int storeIdIndex;
		List<NpcData> npcs;
		List<Texture> npcIconTextures;
		int bgmSoundIdIndex;
		string belongToAreaName;
		bool isInnDisplay;
		bool isYamenDisplay;
		bool isForbiddenAreaDisplay;
		bool isWinshopDisplay;
		bool isJustFightScene;
		static int addNpcIdIndex = 0;

		short toolState = 0; //0正常 1添加 2删除

		string addId = "";
		string addName = "";
		int addBelongToAreaNameIndex = 0;
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
					data.MakeJsonToModel();
					storeIdIndex = storeIdIndexs.ContainsKey(data.ResourceStoreId) ? storeIdIndexs[data.ResourceStoreId] : 0;
					npcs = data.Npcs;
					npcIconTextures = new List<Texture>();
					bgmSoundIdIndex = Base.SoundIdIndexs.ContainsKey(data.BgmSoundId) ? Base.SoundIdIndexs[data.BgmSoundId] : 0;
					belongToAreaName = data.BelongToAreaName;
					isInnDisplay = data.IsInnDisplay;
					isYamenDisplay = data.IsYamenDisplay;
					isForbiddenAreaDisplay = data.IsForbiddenAreaDisplay;
					isWinshopDisplay = data.IsWinshopDisplay;
					isJustFightScene = data.IsJustFightScene;
					foreach (NpcData npc in npcs) {
						npcIconTextures.Add(Base.IconTextureMappings.ContainsKey(npc.IconId) ? Base.IconTextureMappings[npc.IconId] : null);
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 700));
					GUI.Label(new Rect(0, 0, 60, 18), "Id:");
					EditorGUI.TextField(new Rect(65, 0, 150, 18), showId);
					GUI.Label(new Rect(220, 0, 100, 18), "所属区域:" + belongToAreaName);
					GUI.Label(new Rect(325, 0, 40, 18), "无传送:");
					isInnDisplay = EditorGUI.Toggle(new Rect(370, 0, 20, 18), isInnDisplay);
					GUI.Label(new Rect(395, 0, 40, 18), "无衙门:");
					isYamenDisplay = EditorGUI.Toggle(new Rect(440, 0, 20, 18), isYamenDisplay);
					GUI.Label(new Rect(465, 0, 40, 18), "无秘境:");
					isForbiddenAreaDisplay = EditorGUI.Toggle(new Rect(510, 0, 20, 18), isForbiddenAreaDisplay);
					GUI.Label(new Rect(535, 0, 40, 18), "无结识:");
					isWinshopDisplay = EditorGUI.Toggle(new Rect(580, 0, 20, 18), isWinshopDisplay);
					GUI.Label(new Rect(0, 20, 60, 18), "场景名称:");
					name = EditorGUI.TextField(new Rect(65, 20, 150, 18), name);
					GUI.Label(new Rect(220, 20, 60, 18), "战斗据点:");
					isJustFightScene = EditorGUI.Toggle(new Rect(285, 20, 20, 18), isJustFightScene);
					GUI.Label(new Rect(0, 40, 60, 18), "场景商店:");
					storeIdIndex = EditorGUI.Popup(new Rect(65, 40, 150, 18), storeIdIndex, storeNames.ToArray());
					GUI.Label(new Rect(0, 60, 60, 18), "背景音乐:");
					bgmSoundIdIndex = EditorGUI.Popup(new Rect(65, 60, 150, 18), bgmSoundIdIndex, Base.SoundNames.ToArray());
					GUI.Label(new Rect(0, 80, 80, 18), "添加Npc:");
					addNpcIdIndex = EditorGUI.Popup(new Rect(65, 80, 150, 18), addNpcIdIndex, allNpcNames.ToArray());
					if (GUI.Button(new Rect(220, 80, 36, 18), "+")) {
						if (allNpcs.Count > addNpcIdIndex) {
							NpcData addNpc = allNpcs[addNpcIdIndex];
							NpcData existNpc = npcs.Find((npc) => { return npc.Id == addNpc.Id; });
							if (addNpc.IsActive) {
								this.ShowNotification(new GUIContent("动态Npc不能常驻在城镇中!"));
								return;
							}
							if (existNpc != null) {
								this.ShowNotification(new GUIContent("不能重复添加同一个Npc!"));
								return;
							}
							if (npcs.Count >= 18) {
								this.ShowNotification(new GUIContent("一个场景中的Npc不能超过18个!"));
								return;
							}
							npcs.Add(addNpc);
							npcIconTextures.Add(Base.IconTextureMappings.ContainsKey(addNpc.IconId) ? Base.IconTextureMappings[addNpc.IconId] : null);
						}
					}

					GUI.Label(new Rect(0, 100, 60, 18), "常驻Npc:");
					float npcStartX = 0;
					float npcStartY = 120;
					float npcIconX;
					float npcIconY;
					NpcData npcData;
					for (int i = 0; i < npcs.Count; i++) {
						if (npcs.Count > i) {
							npcData = npcs[i];
							npcIconX = npcStartX + i % 4 * 160;
							npcIconY = npcStartY + Mathf.Ceil(i / 4) * 80;
							if (npcIconTextures[i] != null) {
								GUI.DrawTexture(new Rect(npcIconX, npcIconY, 50, 50), npcIconTextures[i]);
							}
							GUI.Label(new Rect(npcIconX + 55, npcIconY, 100, 18), npcData.Name);
							GUI.Label(new Rect(npcIconX + 55, npcIconY + 20, 100, 18), "Id:" + npcData.Id);
							GUI.Label(new Rect(npcIconX + 55, npcIconY + 40, 100, 18), "是否动态:" + npcData.IsActive);
							if (GUI.Button(new Rect(npcIconX + 7, npcIconY + 50, 36, 18), "X")) {
								npcs.RemoveAt(i);
								npcIconTextures.RemoveAt(i);
							}
						}
					}

					if (GUI.Button(new Rect(0, 660, 100, 18), "修改")) {
						if (name == "") {
							this.ShowNotification(new GUIContent("场景名不能为空!"));
							return;
						}
						data.Name = name;
						data.ResourceStoreId = stores[storeIdIndex].Id;
						data.ResourceNpcDataIds.Clear();
						foreach(NpcData npc in npcs) {
							data.ResourceNpcDataIds.Add(npc.Id);
						}
						data.Npcs.Clear();
						npcs.Clear();
						npcIconTextures.Clear();
						data.BgmSoundId = Base.Sounds[bgmSoundIdIndex].Id;
						data.BelongToAreaName = belongToAreaName;
						data.IsInnDisplay = isInnDisplay;
						data.IsYamenDisplay = isYamenDisplay;
						data.IsForbiddenAreaDisplay = isForbiddenAreaDisplay;
						data.IsWinshopDisplay = isWinshopDisplay;
						data.IsJustFightScene = isJustFightScene;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 700, 700, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加场景")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除场景")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 20, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 20, 80, 18), addId);
				GUI.Label(new Rect(120, 20, 50, 18), "场景名:");
				addName = GUI.TextField(new Rect(175, 20, 80, 18), addName);
				GUI.Label(new Rect(260, 20, 50, 18), "所属区域:");
				addBelongToAreaNameIndex = EditorGUI.Popup(new Rect(315, 20, 100, 18), addBelongToAreaNameIndex, Base.AllAreaSceneNames.ToArray());
				if (GUI.Button(new Rect(420, 20, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addName == "") {
						this.ShowNotification(new GUIContent("场景名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}

					SceneData sceneData = new  SceneData();
					sceneData.Id = addId;
					sceneData.Name = addName;
					sceneData.BelongToAreaName = Base.AllAreaSceneNames[addBelongToAreaNameIndex];
					dataMapping.Add(sceneData.Id, sceneData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addName = "";
					oldSelGridInt = -1;
					this.ShowNotification(new GUIContent("添加成功"));
				}
				if (GUI.Button(new Rect(505, 20, 80, 18), "取消")) {
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