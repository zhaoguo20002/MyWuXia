#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Game;
using UnityEngine.UI;

namespace GameEditor {
	public class HalfBodysEditorWindow : EditorWindow {

		static HalfBodysEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/HalfBodys Editor")]
		static void OpenWindow() {
			JsonManager.GetInstance().Clear();
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
				window = (HalfBodysEditorWindow)EditorWindow.GetWindowWithRect(typeof(HalfBodysEditorWindow), size, true, "半身像编辑器");
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

		static Dictionary<string, ResourceSrcData> srcDataMapping;
		static void getData() {
			srcDataMapping = new Dictionary<string, ResourceSrcData>();
			JObject obj = JsonManager.GetInstance().GetJson("HalfBodys", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					srcDataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<ResourceSrcData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<ResourceSrcData>();
			foreach(ResourceSrcData data in srcDataMapping.Values) {
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
			List<ResourceSrcData> datas = new List<ResourceSrcData>();
			foreach(ResourceSrcData data in srcDataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(ResourceSrcData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "HalfBodys.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		ResourceSrcData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string src = "";
		string srcName = "";
		Texture iconTexture = null;
		bool willDelete = false;

		string addId = "";
		string addSrcName = "";
		Sprite addSprite = null;
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
					src = data.Src;
					srcName = data.Name;
					GameObject showImgObj = Statics.GetPrefabClone(src);
					iconTexture = showImgObj.GetComponent<Image>().sprite.texture;
					DestroyImmediate(showImgObj);
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 300, 250));
					GUI.DrawTexture(new Rect(0, 0, 162, 130), iconTexture);
					showId = data.Id;
					GUI.Label(new Rect(0, 135, 60, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(65, 135, 100, 18), showId);
					GUI.Label(new Rect(0, 155, 60, 18), "名称:");
					srcName = EditorGUI.TextField(new Rect(65, 155, 100, 18), srcName);
					GUI.Label(new Rect(0, 175, 60, 18), "资源路径:");
					EditorGUI.TextField(new Rect(65, 175, 200, 18), src);

					if (!willDelete) {
						if (GUI.Button(new Rect(0, 195, 80, 36), "修改")) {
							if (srcName == "") {
								this.ShowNotification(new GUIContent("名称不能为空!"));
								return;
							}
							if (src == "") {
								this.ShowNotification(new GUIContent("路径不能为空!"));
								return;
							}
							data.Name = srcName;
//							data.Src = src;
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改成功"));
						}
						if (GUI.Button(new Rect(85, 195, 80, 36), "删除")) {
							willDelete = true;
						}
					}
					else {
						if (GUI.Button(new Rect(0, 195, 80, 36), "确定删除")) {
							if (!srcDataMapping.ContainsKey(data.Id)) {
								this.ShowNotification(new GUIContent("要删除的数据不存在!"));
								return;
							}
							srcDataMapping.Remove(data.Id);
							writeDataToJson();
							selGridInt = 0;
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							FileUtil.DeleteFileOrDirectory("Assets/Resources/" + data.Src + ".prefab");
							AssetDatabase.Refresh();
							this.ShowNotification(new GUIContent("删除成功"));
							willDelete = false;
						}
						if (GUI.Button(new Rect(85, 195, 80, 36), "取消")) {
							willDelete = false;
						}
					}
					GUILayout.EndArea();

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 255, 300, 160));
					GUI.Label(new Rect(0, 0, 300, 18), "|----添加新数据----------------------------------------------|");
					GUI.Label(new Rect(0, 20, 60, 18), "Id:");
					addId = EditorGUI.TextField(new Rect(65, 20, 200, 18), addId);
					GUI.Label(new Rect(0, 40, 60, 18), "名称:");
					addSrcName = EditorGUI.TextField(new Rect(65, 40, 200, 18), addSrcName);
					addSprite = EditorGUI.ObjectField(new Rect(0, 60, 268, 18), "添加半身像Sprite", addSprite, typeof(Sprite), true) as Sprite;
					if (GUI.Button(new Rect(0, 85, 80, 36), "添加")) {
						if (addSrcName == "") {
							this.ShowNotification(new GUIContent("名称不能为空!"));
							return;
						}
						if (addId == "") {
							this.ShowNotification(new GUIContent("Id不能为空!"));
							return;
						}
						if (addSprite == null) {
							this.ShowNotification(new GUIContent("请选择图形!"));
							return;
						} 
						if (srcDataMapping.ContainsKey(addId)) {
							this.ShowNotification(new GUIContent("Id已存在!"));
							return;
						}
						GameObject newObj = new GameObject();
						newObj.name = addId;
						Image img = newObj.AddComponent<Image>();
						img.sprite = addSprite;
						img.SetNativeSize();
						PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/UI/HalfBodys/" + addId + ".prefab", newObj);
						DestroyImmediate(newObj);
						AssetDatabase.Refresh();
						ResourceSrcData srcData = new ResourceSrcData();
						srcData.Id = addId;
						srcData.Name = addSrcName;
						srcData.Src = "Prefabs/UI/HalfBodys/" + addId;
						srcDataMapping.Add(addId, srcData);
						writeDataToJson();
                        addedId = addId;
                        oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
//						addId = "";
//						addSrcName = "";
						addSprite = null;
						this.ShowNotification(new GUIContent("添加成功"));
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
			
		}
	}
}
#endif