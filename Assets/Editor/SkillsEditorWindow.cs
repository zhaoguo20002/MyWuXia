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
	public class SkillsEditorWindow : EditorWindow {

		static SkillsEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Skills Editor")]
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
				window = (SkillsEditorWindow)EditorWindow.GetWindowWithRect(typeof(SkillsEditorWindow), size, true, "武功招式编辑器");
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

		static List<string> soundNames;
		static Dictionary<string, int> soundIdIndexs;
		static List<SoundData> sounds;

		static List<SkillType> skillTypeEnums;
		static List<string> skillTypeStrs;
		static Dictionary<SkillType, int> skillTypeIndexMapping;
		static List<BuffType> buffTypeEnums;
		static List<string> buffTypeStrs;
		static Dictionary<BuffType, int> buffTypeIndexMapping;

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
					if (iconData.Name.IndexOf("招式-") < 0) {
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

			soundNames = new List<string>();
			soundIdIndexs = new Dictionary<string, int>();
			sounds = new List<SoundData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("Sounds", false);
			SoundData soundData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					soundData = JsonManager.GetInstance().DeserializeObject<SoundData>(item.Value.ToString());
					if (soundData.Name.IndexOf("音效-") < 0) {
						continue;
					}
					soundNames.Add(soundData.Name);
					soundIdIndexs.Add(soundData.Id, index);
					sounds.Add(soundData);
					index++;
				}
			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的SkillType枚举类型
			skillTypeEnums = new List<SkillType>();
			skillTypeStrs = new List<string>();
			skillTypeIndexMapping = new Dictionary<SkillType, int>();
			index = 0;
			foreach(SkillType type in Enum.GetValues(typeof(SkillType))) {
				skillTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				skillTypeStrs.Add(attrib.Description);
				skillTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的BuffType枚举类型
			buffTypeEnums = new List<BuffType>();
			buffTypeStrs = new List<string>();
			buffTypeIndexMapping = new Dictionary<BuffType, int>();
			index = 0;
			foreach(BuffType type in Enum.GetValues(typeof(BuffType))) {
				buffTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				buffTypeStrs.Add(attrib.Description);
				buffTypeIndexMapping.Add(type, index);
				index++;
			}
		}

		static Dictionary<string, SkillData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, SkillData>();
			JObject obj = JsonManager.GetInstance().GetJson("Skills", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<SkillData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<SkillData> showListData;
		static List<string> listNames;
		static List<string> allDataNames;
		static List<SkillData> allSkillDatas;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<SkillData>();
			allDataNames = new List<string>(){ "无额外招式" };
			allSkillDatas = new List<SkillData>() { null };
			foreach(SkillData data in dataMapping.Values) {
				allDataNames.Add(data.Name);
				allSkillDatas.Add(data);
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
			foreach(SkillData data in dataMapping.Values) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Skills.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		SkillData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string skillName = "";
		string effectSrc = "";
		int effectSoundIdIndex = 0;
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int skillTypeIndex = 0;
		float rate = 0;

		int buffGridIndex = 0;
		List<int> theBuffTypeIndexs;
		List<float> theBuffRates;
		List<int> theBuffRoundNumbers;
		List<float> theBuffValues;
		List<bool> theBuffFirstEffects;

		List<int> theDeBuffTypeIndexs;
		List<float> theDeBuffRates;
		List<int> theDeBuffRoundNumbers;
		List<float> theDeBuffValues;
		List<bool> theDeBuffFirstEffects;

		int addBuffOrDeBuffTypeIndex = 0;
		float addBuffOrDeBuffRate = 100;
		int addBuffOrDeBuffRoundNumber = 0;
		float addBuffOrDeBuffValue = 1;
		bool addBuffOrDeBuffFirstEffect = true;

		int addedSkillIndex = 0;

		short toolState; //0正常 1增加 2删除

		string addId = "";
		string addSkillName = "";
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
					skillName = data.Name;
					effectSrc = data.EffectSrc;
					effectSoundIdIndex = soundIdIndexs.ContainsKey(data.EffectSoundId) ? soundIdIndexs[data.EffectSoundId] : 0;

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
					skillTypeIndex = skillTypeIndexMapping[data.Type];
					rate = data.Rate;

					theBuffTypeIndexs = new List<int>();
					theBuffRates = new List<float>();
					theBuffRoundNumbers = new List<int>();
					theBuffValues = new List<float>();
					theBuffFirstEffects = new List<bool>();
					foreach(BuffData buff in data.BuffDatas) {
						theBuffTypeIndexs.Add(buffTypeIndexMapping[buff.Type]);
						theBuffRates.Add(buff.Rate);
						theBuffRoundNumbers.Add(buff.RoundNumber);
						theBuffValues.Add(buff.Value);
						theBuffFirstEffects.Add(buff.FirstEffect);
					}

					theDeBuffTypeIndexs = new List<int>();
					theDeBuffRates = new List<float>();
					theDeBuffRoundNumbers = new List<int>();
					theDeBuffValues = new List<float>();
					theDeBuffFirstEffects = new List<bool>();
					foreach(BuffData deBuff in data.DeBuffDatas) {
						theDeBuffTypeIndexs.Add(buffTypeIndexMapping[deBuff.Type]);
						theDeBuffRates.Add(deBuff.Rate);
						theDeBuffRoundNumbers.Add(deBuff.RoundNumber);
						theDeBuffValues.Add(deBuff.Value);
						theDeBuffFirstEffects.Add(deBuff.FirstEffect);
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 250));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 50, 50), iconTexture);
					}
					showId = data.Id;
					GUI.Label(new Rect(55, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(100, 0, 100, 18), showId);
					GUI.Label(new Rect(205, 0, 40, 18), "名称:");
					skillName = EditorGUI.TextField(new Rect(250, 0, 100, 18), skillName);
					GUI.Label(new Rect(355, 0, 50, 18), "特效路径:");
					Rect prefabRect = new Rect(410, 0, 200, 18);
					effectSrc = EditorGUI.TextField(prefabRect, effectSrc);
					// 判断当前鼠标正拖拽某对象或者在拖拽的过程中松开了鼠标按键 
					// 同时还需要判断拖拽时鼠标所在位置处于文本输入框内 
					if ((Event.current.type == EventType.DragUpdated
						|| Event.current.type == EventType.DragExited)) {
						// 判断是否拖拽了文件 
						if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
							string sfxPath = DragAndDrop.paths [0];
							// 拖拽的过程中，松开鼠标之后，拖拽操作结束，此时就可以使用获得的 sfxPath 变量了 
							if (!string.IsNullOrEmpty (sfxPath) && Event.current.type == EventType.DragExited) {
								DragAndDrop.AcceptDrag ();
								if (sfxPath.IndexOf(".prefab") >= 0) {
									if (prefabRect.Contains (Event.current.mousePosition)) {
										if (sfxPath.IndexOf("Assets/Resources/") == 0) {
											sfxPath = sfxPath.Replace("Assets/Resources/", "");
											sfxPath = sfxPath.Replace(".prefab", "");
											effectSrc = sfxPath;
										}
										else {
											this.ShowNotification(new GUIContent("只能使用放在Resources目录下的角色模型预设!"));
										}
									}
								}
							}
						}
					}
					GUI.Label(new Rect(55, 20, 40, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(100, 20, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(205, 20, 40, 18), "类型:");
					skillTypeIndex = EditorGUI.Popup(new Rect(250, 20, 100, 18), skillTypeIndex, skillTypeStrs.ToArray());
					GUI.Label(new Rect(355, 20, 50, 18), "音效:");
					effectSoundIdIndex = EditorGUI.Popup(new Rect(410, 20, 100, 18), effectSoundIdIndex, soundNames.ToArray());
					GUI.Label(new Rect(55, 40, 40, 18), "概率:");
					rate = EditorGUI.Slider(new Rect(100, 40, 180, 18), rate, 0, 100);
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(0, 65, 80, 18), "修改基础属性")) {
						if (skillName == "") {
							this.ShowNotification(new GUIContent("招式名不能为空!"));
							return;
						}
						data.Name = skillName;
						data.IconId = icons[iconIndex].Id;
						data.Type = skillTypeEnums[skillTypeIndex];
						data.Rate = rate;
						data.Desc = createSkillDesc(data);
						data.EffectSrc = effectSrc;
						data.EffectSoundId = sounds[effectSoundIdIndex].Id;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					buffGridIndex = GUI.SelectionGrid(new Rect(0, 90, 80, 50), buffGridIndex, new string[2]{ "Buff", "DeBuff" }, 1);
					GUI.Label(new Rect(85, 90, 40, 18), "类型:");
					addBuffOrDeBuffTypeIndex = EditorGUI.Popup(new Rect(130, 90, 100, 18), addBuffOrDeBuffTypeIndex, buffTypeStrs.ToArray());
					GUI.Label(new Rect(235, 90, 40, 18), "概率:");
					addBuffOrDeBuffRate = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, 90, 40, 18), addBuffOrDeBuffRate.ToString())), 0, 100);
					GUI.Label(new Rect(325, 90, 40, 18), "回合:");
					addBuffOrDeBuffRoundNumber = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, 90, 40, 18), addBuffOrDeBuffRoundNumber.ToString())), 0, 10);
					GUI.Label(new Rect(415, 90, 40, 18), "数值:");
					if (buffGridIndex == 0) {
						addBuffOrDeBuffValue = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, 90, 80, 18), addBuffOrDeBuffValue.ToString())), 0, getBuffValueRangeTop(buffTypeEnums[addBuffOrDeBuffTypeIndex]));
					}
					else {
						addBuffOrDeBuffValue = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, 90, 80, 18), addBuffOrDeBuffValue.ToString())), -getBuffValueRangeTop(buffTypeEnums[addBuffOrDeBuffTypeIndex]), 0);
					}
					GUI.Label(new Rect(545, 90, 70, 18), "首回合生效:");
					addBuffOrDeBuffFirstEffect = EditorGUI.Toggle(new Rect(620, 90, 30, 18), addBuffOrDeBuffFirstEffect);
					if (GUI.Button(new Rect(655, 90, 40, 18), "+")) {
						List<BuffData> buffs = buffGridIndex == 0 ? data.BuffDatas : data.DeBuffDatas;
						if (buffs.Count < 5) {
							BuffData buff = buffs.Find((item) => { return item.Type == buffTypeEnums[addBuffOrDeBuffTypeIndex]; });
							if (buff == null) {
								BuffData newBuff = new BuffData();
								newBuff.Type = buffTypeEnums[addBuffOrDeBuffTypeIndex];
								newBuff.Rate = addBuffOrDeBuffRate;
								newBuff.RoundNumber = addBuffOrDeBuffRoundNumber;
								newBuff.Value = addBuffOrDeBuffValue;
								newBuff.FirstEffect = addBuffOrDeBuffFirstEffect;
								buffs.Add(newBuff);
								data.Desc = createSkillDesc(data);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								addBuffOrDeBuffTypeIndex = 0;
								addBuffOrDeBuffRate = 100;
								addBuffOrDeBuffRoundNumber = 0;
								addBuffOrDeBuffValue = 1;
								addBuffOrDeBuffFirstEffect = true;
								this.ShowNotification(new GUIContent("添加成功"));
							}
							else {
								this.ShowNotification(new GUIContent("Buff或DeBuff类型已存在, 不能添加!"));
							}
						}
						else {
							this.ShowNotification(new GUIContent("Buff或DeBuff已到上限,不能添加!"));
						}
					}
					float buffsStartY = 110;
					if (buffGridIndex == 0) {
						for (int i = 0; i < theBuffTypeIndexs.Count; i++) {
							GUI.Label(new Rect(85, buffsStartY + i * 20, 40, 18), "类型:");
							theBuffTypeIndexs[i] = EditorGUI.Popup(new Rect(130, buffsStartY + i * 20, 100, 18), theBuffTypeIndexs[i], buffTypeStrs.ToArray());
							GUI.Label(new Rect(235, buffsStartY + i * 20, 40, 18), "概率:");
							theBuffRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, buffsStartY + i * 20, 40, 18), theBuffRates[i].ToString())), 0, 100);
							GUI.Label(new Rect(325, buffsStartY + i * 20, 40, 18), "回合:");
							theBuffRoundNumbers[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, buffsStartY + i * 20, 40, 18), theBuffRoundNumbers[i].ToString())), 0, 10);
							GUI.Label(new Rect(415, buffsStartY + i * 20, 40, 18), "数值:");
							theBuffValues[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, buffsStartY + i * 20, 80, 18), theBuffValues[i].ToString())), 0, getBuffValueRangeTop(buffTypeEnums[theBuffTypeIndexs[i]]));
							GUI.Label(new Rect(545, buffsStartY + i * 20, 70, 18), "首回合生效:");
							theBuffFirstEffects[i] = EditorGUI.Toggle(new Rect(620, buffsStartY + i * 20, 30, 18), theBuffFirstEffects[i]);
							if (GUI.Button(new Rect(655, buffsStartY + i * 20, 40, 18), "修改")) {
								if (data.BuffDatas.Count > i) {
									int buffIndex = data.BuffDatas.FindIndex((item) => { return item.Type == buffTypeEnums[theBuffTypeIndexs[i]]; });
									if (buffIndex >= 0 && buffIndex != i) {
										this.ShowNotification(new GUIContent("Buff或DeBuff类型已存在, 不能修改!"));
										return;
									}
									data.BuffDatas[i].Type = buffTypeEnums[theBuffTypeIndexs[i]];
									data.BuffDatas[i].Rate = theBuffRates[i];
									data.BuffDatas[i].RoundNumber = theBuffRoundNumbers[i];
									data.BuffDatas[i].Value = theBuffValues[i];
									data.BuffDatas[i].FirstEffect = theBuffFirstEffects[i];
									data.Desc = createSkillDesc(data);
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("修改成功"));
								}
							}
							if (GUI.Button(new Rect(700, buffsStartY + i * 20, 40, 18), "-")) {
								if (data.BuffDatas.Count > i) {
									data.BuffDatas.RemoveAt(i);
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("删除成功"));
								}
							}
						}
					}
					else {
						for (int i = 0; i < theDeBuffTypeIndexs.Count; i++) {
							GUI.Label(new Rect(85, buffsStartY + i * 20, 40, 18), "类型:");
							theDeBuffTypeIndexs[i] = EditorGUI.Popup(new Rect(130, buffsStartY + i * 20, 100, 18), theDeBuffTypeIndexs[i], buffTypeStrs.ToArray());
							GUI.Label(new Rect(235, buffsStartY + i * 20, 40, 18), "概率:");
							theDeBuffRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, buffsStartY + i * 20, 40, 18), theDeBuffRates[i].ToString())), 0, 100);
							GUI.Label(new Rect(325, buffsStartY + i * 20, 40, 18), "回合:");
							theDeBuffRoundNumbers[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, buffsStartY + i * 20, 40, 18), theDeBuffRoundNumbers[i].ToString())), 0, 10);
							GUI.Label(new Rect(415, buffsStartY + i * 20, 40, 18), "数值:");
							theDeBuffValues[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, buffsStartY + i * 20, 80, 18), theDeBuffValues[i].ToString())), -getBuffValueRangeTop(buffTypeEnums[theDeBuffTypeIndexs[i]]), 0);
							GUI.Label(new Rect(545, buffsStartY + i * 20, 70, 18), "首回合生效:");
							theDeBuffFirstEffects[i] = EditorGUI.Toggle(new Rect(620, buffsStartY + i * 20, 30, 18), theDeBuffFirstEffects[i]);
							if (GUI.Button(new Rect(655, buffsStartY + i * 20, 40, 18), "修改")) {
								if (data.DeBuffDatas.Count > i) {
									int buffIndex = data.DeBuffDatas.FindIndex((item) => { return item.Type == buffTypeEnums[theDeBuffTypeIndexs[i]]; });
									if (buffIndex >= 0 && buffIndex != i) {
										this.ShowNotification(new GUIContent("Buff或DeBuff类型已存在, 不能修改!"));
										return;
									}
									data.DeBuffDatas[i].Type = buffTypeEnums[theDeBuffTypeIndexs[i]];
									data.DeBuffDatas[i].Rate = theDeBuffRates[i];
									data.DeBuffDatas[i].RoundNumber = theDeBuffRoundNumbers[i];
									data.DeBuffDatas[i].Value = theDeBuffValues[i];
									data.DeBuffDatas[i].FirstEffect = theDeBuffFirstEffects[i];
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("修改成功"));
								}
							}
							if (GUI.Button(new Rect(700, buffsStartY + i * 20, 40, 18), "-")) {
								if (data.DeBuffDatas.Count > i) {
									data.DeBuffDatas.RemoveAt(i);
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("删除成功"));
								}
							}
						}
					}
					GUILayout.EndArea();


					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 245, 800, 100));
					GUI.Label(new Rect(0, 0, 800, 18), "|--------额外招式-----------------------------------------------------------------------|");
					addedSkillIndex = EditorGUI.Popup(new Rect(0, 20, 100, 18), addedSkillIndex, allDataNames.ToArray());
					if (GUI.Button(new Rect(105, 20, 100, 18), "新增额外招式")) {
						if (addedSkillIndex <= 0 || allSkillDatas.Count <= addedSkillIndex) {
							return;
						}
						if (data.ResourceAddedSkillIds.Count >= 3) {
							this.ShowNotification(new GUIContent("一个招式只能拥有最多3个额外招式!"));
							return;
						}
						if (allSkillDatas[addedSkillIndex].Id == data.Id) {
							this.ShowNotification(new GUIContent("额外招式不能和主招式相同!"));
							return;
						}
						data.ResourceAddedSkillIds.Add(allSkillDatas[addedSkillIndex].Id);
						data.Desc = createSkillDesc(data);
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						addedSkillIndex = 0;
						this.ShowNotification(new GUIContent("添加成功"));
					}
					for (int i = 0; i < data.ResourceAddedSkillIds.Count; i++) {
						GUI.Label(new Rect(0, 40 + i * 20, 100, 18), dataMapping[data.ResourceAddedSkillIds[i]].Name);
						GUI.Label(new Rect(105, 40 + i * 20, 100, 18), string.Format("触发概率: {0}%", dataMapping[data.ResourceAddedSkillIds[i]].Rate));
						if (GUI.Button(new Rect(210, 40 + i * 20, 40, 18), "-")) {
							if (data.ResourceAddedSkillIds.Count > i) {
								data.ResourceAddedSkillIds.RemoveAt(i);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								addedSkillIndex = 0;
								this.ShowNotification(new GUIContent("删除成功"));
							}
						}
					}
					GUILayout.EndArea();
				}
				
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 360, 300, 60));
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
				GUI.Label(new Rect(140, 0, 60, 18), "招式名:");
				addSkillName = EditorGUI.TextField(new Rect(205, 0, 100, 18), addSkillName);
				if (GUI.Button(new Rect(0, 20, 60, 18), "确定添加")) {
					toolState = 0;
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addSkillName == "") {
						this.ShowNotification(new GUIContent("招式名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					SkillData addSkillData = new SkillData();
					addSkillData.Id = addId;
					addSkillData.Name = addSkillName;
					dataMapping.Add(addId, addSkillData);
					addSkillData.Desc = createSkillDesc(addSkillData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addSkillName = "";
					addSkillData.Desc = createSkillDesc(addSkillData);
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
			case BuffType.IncreaseDamageRate:
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

		string getBuffDesc(BuffData buff) {
			string rateStr = buff.Rate >= 100 ? "" : "<color=\"#A64DFF\">" + buff.Rate + "%</color>概率";
			string firstEffectStr = buff.FirstEffect ? "" : "下回合起";
			string roundRumberStr;
			string roundRumberStr2;
			if (!buff.FirstEffect && buff.RoundNumber <= 0) {
				roundRumberStr = "<color=\"#B20000\">无效</color>";
				roundRumberStr2 = "<color=\"#B20000\">无效</color>";
			} 
			else {
				roundRumberStr = buff.RoundNumber <= 0 ? "1回合" : (buff.RoundNumber + "回合");
				roundRumberStr2 = buff.RoundNumber <= 0 ? "持续1回合" : "持续" + (buff.RoundNumber + "回合");
			}
			switch(buff.Type) {
			case BuffType.CanNotMove:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">定身</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Chaos:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">混乱</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Disarm:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">缴械</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Drug:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">中毒</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Fast:
				return string.Format("{0}{1}触发<color=\"#FF9326\">疾走</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Slow:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">迟缓</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.Vertigo:
				return string.Format("{0}{1}致敌<color=\"#FF9326\">眩晕</color>{2}", rateStr, firstEffectStr, roundRumberStr);
			case BuffType.IncreaseDamageRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF4DFF\">最终伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreaseFixedDamage:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF4DFF\">固定伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreaseHP:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#00FF00\">气血值</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreaseHurtCutRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF4DFF\">己方所受伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreaseMagicAttack:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#2693FF\">内功</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreaseMagicAttackRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#2693FF\">内功</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreaseMagicDefense:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#73B9FF\">内防</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreaseMagicDefenseRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#73B9FF\">内防</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreaseMaxHP:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreaseMaxHPRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreasePhysicsAttack:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF0000\">外功</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreasePhysicsAttackRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF0000\">外功</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.IncreasePhysicsDefense:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF7373\">外防</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
			case BuffType.IncreasePhysicsDefenseRate:
				return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, "<color=\"#FF7373\">外防</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100)) + "%", roundRumberStr2);
			case BuffType.Normal:
				return "无";
			default:
				return "";
			}
		}

		string createSkillDesc(SkillData skill, bool isAddedSkill = false) {
			string typeStr = "";
			switch (skill.Type) {
			case SkillType.PhysicsAttack:
				typeStr = "<color=\"#FF0000\">外功招式</color>";
				break;
			case SkillType.MagicAttack:
				typeStr = "<color=\"#2693FF\">内功招式</color>";
				break;
			case SkillType.FixedDamage:
				typeStr = "<color=\"#BFCFFF\">固定伤害</color>";
				break;
			case SkillType.Plus:
				typeStr = "<color=\"#00FF00\">增益招式</color>";
				break;
			default:
				break;
			}
			string buffDesc = "";
			foreach(BuffData buff in skill.BuffDatas) {
				buffDesc += " " + getBuffDesc(buff) + ",";
			}
			foreach(BuffData deBuff in skill.DeBuffDatas) {
				buffDesc += " " + getBuffDesc(deBuff) + ",";
			}
			if (buffDesc.Length > 1) {
				buffDesc = buffDesc.Remove(buffDesc.Length - 1, 1);
			}
			string addSkillDesc = "";
			if (!isAddedSkill) {
				List<string> titles = new List<string>() { "一", "二", "三" };
				int index = 0;
				foreach(string addSkillId in skill.ResourceAddedSkillIds) {
					if (index >= 3) {
						break;
					}
					if (dataMapping.ContainsKey(addSkillId)) {
						addSkillDesc += "\n变招" + titles[index] + ", " + createSkillDesc(dataMapping[addSkillId], true);
					}
					index++;
				}
			}
			return string.Format("{0}{1}\n[{2}]{3}{4}", skill.Name, skill.Rate >= 100 ? "" : ("(发招概率:<color=\"#A64DFF\">" + skill.Rate + "%</color>)"), typeStr, buffDesc, addSkillDesc);
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