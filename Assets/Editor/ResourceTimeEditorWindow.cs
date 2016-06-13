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
	public class ResourceTimeEditorWindow : EditorWindow {

		static ResourceTimeEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Resource Value Explorer")]
		static void OpenWindow() {
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
			float width = 1060;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (ResourceTimeEditorWindow)EditorWindow.GetWindowWithRect(typeof(ResourceTimeEditorWindow), size, true, "资源价值查看器");
			}
			window.Show();
			window.position = size;
			InitData();
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


		static Dictionary<ResourceType, int> resourceTotalYieldNumMapping;
		static Dictionary<ResourceType, string> resourceNameMapping;
		static Dictionary<ResourceType, int> resourceNeedBaseResMapping;
		static Dictionary<ResourceType, int> resourceNeedSecondsMapping;
		static List<string> resourceNames;
		static void InitData() {
			WorkshopModel.Init();

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			resourceNameMapping = new Dictionary<ResourceType, string>();
			foreach(ResourceType type in Enum.GetValues(typeof(ResourceType))) {
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				resourceNameMapping.Add(type, attrib.Description);
			}

			resourceTotalYieldNumMapping = new Dictionary<ResourceType, int>();
			resourceNames = new List<string>();
			resourceNeedBaseResMapping = new Dictionary<ResourceType, int>();
			resourceNeedSecondsMapping = new Dictionary<ResourceType, int>();
			ResourceRelationshipData relation;
			for (int i = 0; i < WorkshopModel.Relationships.Count; i++) {
				relation = WorkshopModel.Relationships[i];
				resourceNames.Add(getResourceName(relation.Type));
				if (!resourceTotalYieldNumMapping.ContainsKey(relation.Type)) {
					resourceTotalYieldNumMapping.Add(relation.Type, relation.YieldNum);
				}
				resourceNeedBaseResMapping.Add(relation.Type, getNeedWheatNum(relation.Type));
				resourceNeedSecondsMapping.Add(relation.Type, getBeedSecond(relation.Type));
			}
		}

		/// <summary>
		/// 特定的资源一次单位生产所需要的小麦基础资源数
		/// </summary>
		/// <returns>The wheat number.</returns>
		/// <param name="relation">Relation.</param>
		static int getNeedWheatNum(ResourceType type) {
			ResourceRelationshipData relation = WorkshopModel.Relationships.Find(item => item.Type == type);
			if (relation == null) {
				return 0;
			}
			if (relation.Needs.Count == 0) {
				return relation.YieldNum;
			}
			int needNum = 0;
			for (int i = 0; i < relation.Needs.Count; i++) {
				needNum += (getNeedWheatNum(relation.Needs[i].Type) * (int)relation.Needs[i].Num);
			}
			return needNum;
		}

		/// <summary>
		/// 特定的资源一次单位生产所需要的时间
		/// </summary>
		/// <returns>The wheat number.</returns>
		/// <param name="relation">Relation.</param>
		static int getBeedSecond(ResourceType type) {
			ResourceRelationshipData relation = WorkshopModel.Relationships.Find(item => item.Type == type);
			if (relation == null) {
				return 0;
			}
			if (relation.Needs.Count == 0) {
				return (int)Mathf.Clamp(relation.YieldNum * (int)modifyResourceTimeout, modifyResourceTimeout, 36000000);
			}
			int needNum = 0;
			for (int i = 0; i < relation.Needs.Count; i++) {
				needNum += (int)Mathf.Clamp(getBeedSecond(relation.Needs[i].Type) * (int)relation.Needs[i].Num + (int)modifyResourceTimeout, modifyResourceTimeout, 36000000);
			}
			return (int)Mathf.Clamp(needNum, modifyResourceTimeout, 36000000);
		}

		static string getResourceName(ResourceType type) {
			if (resourceNameMapping.ContainsKey(type)) {
				return resourceNameMapping[type];
			}
			return "";
		}

		static float modifyResourceTimeout = 20;
		float workerNum = 0;
		List<string> resourceInfos = new List<string>();
		//绘制窗口时调用
	    void OnGUI () {
			GUILayout.BeginArea(new Rect(5, 5, 1050, Screen.currentResolution.height - 105));
			GUI.Label(new Rect(0, 0, 100, 18), "生产间隔时间(秒):");
			modifyResourceTimeout = EditorGUI.Slider(new Rect(105, 0, 200, 18), modifyResourceTimeout, 1, 600);
			GUI.Label(new Rect(310, 0, 100, 18), "单一工作家丁数:");
			workerNum = EditorGUI.Slider(new Rect(415, 0, 200, 18), workerNum, 1, 1000);
			if (GUI.Button(new Rect(620, 0, 60, 18), "计算")) {
				resourceInfos.Clear();
				ResourceRelationshipData relation;
				for (int i = 0; i < WorkshopModel.Relationships.Count; i++) {
					relation = WorkshopModel.Relationships[i];
					resourceInfos.Add(string.Format("需要材料1: {0}{1}, 需要材料2: {2}{3} (共计需要基础资源{4}{5}个, {6}个家丁生产{7}个{8}需要大约{9}秒)", 
						relation.Needs.Count > 0 ? getResourceName(relation.Needs[0].Type) : "无", relation.Needs.Count > 0 ? relation.Needs[0].Num + "个" : "",
						relation.Needs.Count > 1 ? getResourceName(relation.Needs[1].Type) : "无", relation.Needs.Count > 1 ? relation.Needs[1].Num + "个" : "",
						getResourceName(ResourceType.Wheat), resourceNeedBaseResMapping[relation.Type], 
						workerNum, relation.YieldNum, getResourceName(relation.Type), Statics.GetFullTime((int)Mathf.Clamp(resourceNeedSecondsMapping[relation.Type] / workerNum, modifyResourceTimeout, 3600000000))
					));
				}
			}
			float rowX = 0, rowY = 20;
			for (int i = 0; i < resourceNames.Count; i++) {
				GUI.Label(new Rect(rowX, rowY + i * 20, 80, 18), resourceNames[i]); 
				if (resourceInfos.Count > i) {
					GUI.Label(new Rect(rowX + 85, rowY + i * 20, 900, 18), resourceInfos[i]); 
				}
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