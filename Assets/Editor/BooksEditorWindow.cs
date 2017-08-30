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
			Base.InitParams();
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
		static List<QualityType> qualityTypeEnums;
		static List<string> qualityTypeStrs;
        static Dictionary<QualityType, int> qualityTypeIndexMapping;

        static List<WeaponType> weaponTypeEnums;
        static List<string> weaponTypeStrs;
        static Dictionary<WeaponType, int> weaponTypeIndexMapping;

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



            weaponTypeEnums = new List<WeaponType>();
            weaponTypeStrs = new List<string>();
            weaponTypeIndexMapping = new Dictionary<WeaponType, int>();
            index = 0;
            foreach(WeaponType type in Enum.GetValues(typeof(WeaponType))) {
                weaponTypeEnums.Add(type);
                fieldInfo = type.GetType().GetField(type.ToString());
                attribArray = fieldInfo.GetCustomAttributes(false);
                attrib = (DescriptionAttribute)attribArray[0];
                weaponTypeStrs.Add(attrib.Description);
                weaponTypeIndexMapping.Add(type, index);
                index++;
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
			List<BookData> datas = new List<BookData>();
			foreach(BookData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			JObject booksOfForbiddenAreaData = new JObject(); //秘境中秘籍静态json数据
			foreach(BookData data in datas) {
                data.Skills.Clear();
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				if (booksOfForbiddenAreaData[data.BelongToCityId] == null) {
					booksOfForbiddenAreaData[data.BelongToCityId] = new JArray(data.Id);
				}
				else {
					((JArray)booksOfForbiddenAreaData[data.BelongToCityId]).Add(data.Id);
				}
			}
            Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Books.json", DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(writeJson), JsonManager.DecderKey));
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "BooksOfForbiddenAreaData.json", JsonManager.GetInstance().SerializeObject(booksOfForbiddenAreaData));
		}

        void writeDataToJson2() {
            JObject writeJson = new JObject();
            int index = 0;
            foreach(SkillData data in allSkillDataMapping.Values) {
                if (index == 0) {
                    index++;
                    writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
                }
                writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
            }
            Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Skills.json", DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(writeJson), JsonManager.DecderKey));
        }

		BookData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string bookName = "";
		string bookDesc = "";
		int belongToCityIdIndex = 0;
		int qualityTypeIndex = 0;
		int maxHPPlus;
		float dodgePlus;
		float physicsDefensePlus;
		float hurtCutRatePlus;
		float magicAttackPlus;
		float magicDefensePlus;
        int drugResistance = 0;
        int disarmResistance = 0;
        int vertigoResistance = 0;
        int canNotMoveResistance = 0;
        int slowResistance = 0;
        int chaosResistance = 0;
		int occupationIndex = 0;
        int limitWeaponTypeIndex = 0;
		bool isMindBook;
		List<int> needsIdIndexes;
		List<int> needsNums;

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
					belongToCityIdIndex = Base.AllCitySceneIdIndexs.ContainsKey(data.BelongToCityId) ? Base.AllCitySceneIdIndexs[data.BelongToCityId] : 0;
					qualityTypeIndex = qualityTypeIndexMapping[data.Quality];
					maxHPPlus = data.MaxHPPlus;
					dodgePlus = data.DodgePlus;
					physicsDefensePlus = data.PhysicsDefensePlus;
					hurtCutRatePlus = data.HurtCutRatePlus;
					magicAttackPlus = data.MagicAttackPlus;
					magicDefensePlus = data.MagicDefensePlus;
                    drugResistance = data.DrugResistance;
                    disarmResistance = data.DisarmResistance;
                    vertigoResistance = data.VertigoResistance;
                    canNotMoveResistance = data.CanNotMoveResistance;
                    slowResistance = data.SlowResistance;
                    chaosResistance = data.ChaosResistance;
					occupationIndex = Base.OccupationTypeIndexMapping.ContainsKey(data.Occupation) ? Base.OccupationTypeIndexMapping[data.Occupation] : 0;
                    limitWeaponTypeIndex = weaponTypeIndexMapping.ContainsKey(data.LimitWeaponType) ? weaponTypeIndexMapping[data.LimitWeaponType] : 0;
                    isMindBook = data.IsMindBook;
					needsIdIndexes = new List<int>();
					needsNums = new List<int>();
					CostData cost;
					for (int i = 0; i < data.Needs.Count; i++) {
						cost = data.Needs[i];
						needsIdIndexes.Add(Base.ItemDataIdIndexs.ContainsKey(cost.Id) ? Base.ItemDataIdIndexs[cost.Id] : 0);
						needsNums.Add(cost.Num);
					}

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

                if (GUI.Button(new Rect(listStartX + 205, 5, 100, 18), "生成秘籍Excel")) {
                    Excel outputXls = new Excel();
                    ExcelTable outputTable= new ExcelTable();
                    outputTable.TableName = "秘籍数值";
                    string outputPath = ExcelEditor.DocsPath + "/秘籍数值.xlsx";
                    outputXls.Tables.Add(outputTable);

                    ExcelTable outputTable2= new ExcelTable();
                    outputTable2.TableName = "招式数值";
                    outputXls.Tables.Add(outputTable2);

                    ExcelTable outputTable3= new ExcelTable();
                    outputTable3.TableName = "增/减益数值";
                    outputXls.Tables.Add(outputTable3);


                    outputXls.Tables[0].SetValue(1, 1, "1秘籍id");
                    outputXls.Tables[0].SetValue(1, 2, "2秘籍名称");
                    outputXls.Tables[0].SetValue(1, 3, "3开启城镇id");
                    outputXls.Tables[0].SetValue(1, 4, "4定身抵抗");
                    outputXls.Tables[0].SetValue(1, 5, "5混乱抵抗");
                    outputXls.Tables[0].SetValue(1, 6, "6缴械抵抗");
                    outputXls.Tables[0].SetValue(1, 7, "7中毒抵抗");
                    outputXls.Tables[0].SetValue(1, 8, "8迟缓抵抗");
                    outputXls.Tables[0].SetValue(1, 9, "9眩晕抵抗");
                    outputXls.Tables[0].SetValue(1, 10, "10轻功增量");
                    outputXls.Tables[0].SetValue(1, 11, "11减伤比例增量");
                    outputXls.Tables[0].SetValue(1, 12, "12内功增量");
                    outputXls.Tables[0].SetValue(1, 13, "13内防增量");
                    outputXls.Tables[0].SetValue(1, 14, "14最大气血增量");
                    outputXls.Tables[0].SetValue(1, 15, "15外防增量");
                    outputXls.Tables[0].SetValue(1, 16, "16是否为心法");
                    outputXls.Tables[0].SetValue(1, 17, "17仅限兵器");
                    outputXls.Tables[0].SetValue(1, 18, "18所属门派");
                    outputXls.Tables[0].SetValue(1, 19, "19品质");
                    outputXls.Tables[0].SetValue(1, 20, "20招式id列表");

                    outputXls.Tables[1].SetValue(1, 1, "招式id");
                    outputXls.Tables[1].SetValue(1, 2, "所属秘籍");
                    outputXls.Tables[1].SetValue(1, 3, "招式名");
                    outputXls.Tables[1].SetValue(1, 4, "前置发招");
                    outputXls.Tables[1].SetValue(1, 5, "类型");
                    outputXls.Tables[1].SetValue(1, 6, "发招概率");
                    outputXls.Tables[1].SetValue(1, 7, "特效路径");
                    outputXls.Tables[1].SetValue(1, 8, "音效id");
                    outputXls.Tables[1].SetValue(1, 9, "CD时间");

                    outputXls.Tables[2].SetValue(1, 1, "招式id");
                    outputXls.Tables[2].SetValue(1, 2, "所属秘籍");
                    outputXls.Tables[2].SetValue(1, 3, "招式名");
                    outputXls.Tables[2].SetValue(1, 4, "前置发招");
                    outputXls.Tables[2].SetValue(1, 5, "类别");
                    outputXls.Tables[2].SetValue(1, 6, "类型");
                    outputXls.Tables[2].SetValue(1, 7, "概率");
                    outputXls.Tables[2].SetValue(1, 8, "持续招数");
                    outputXls.Tables[2].SetValue(1, 9, "值");
                    outputXls.Tables[2].SetValue(1, 10, "首回合生效");

                    int startIndex = 2;
                    int startIndex2 = 2;
                    int startIndex3 = 2;
                    SkillData skill;
                    SkillData addedSkill;
                    BuffData buff;
                    Dictionary<string, bool> createdSkillMapping = new Dictionary<string, bool>();
                    foreach(BookData book in dataMapping.Values) {
                        outputXls.Tables[0].SetValue(startIndex, 1, book.Id);
                        outputXls.Tables[0].SetValue(startIndex, 2, book.Name);
                        outputXls.Tables[0].SetValue(startIndex, 3, book.BelongToCityId);
                        outputXls.Tables[0].SetValue(startIndex, 4, book.CanNotMoveResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 5, book.ChaosResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 6, book.DisarmResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 7, book.DrugResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 8, book.SlowResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 9, book.VertigoResistance.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 10, book.DodgePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 11, book.HurtCutRatePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 12, book.MagicAttackPlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 13, book.MagicDefensePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 14, book.MaxHPPlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 15, book.PhysicsDefensePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 16, book.IsMindBook ? "是" : "否");
                        outputXls.Tables[0].SetValue(startIndex, 17, book.LimitWeaponType.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 18, book.Occupation.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 19, book.Quality.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 20, JsonManager.GetInstance().SerializeObject(book.ResourceSkillDataIds));
                        outputXls.Tables[0].SetValue(startIndex, 21, book.Desc);
                        startIndex++;

                        for (int i = 0, len = book.ResourceSkillDataIds.Count; i < len; i++) {
                            skill = allSkillDataMapping[book.ResourceSkillDataIds[i]];
                            if (!createdSkillMapping.ContainsKey(skill.Id)) {
                                createdSkillMapping.Add(skill.Id, true);
                                outputXls.Tables[1].SetValue(startIndex2, 1, skill.Id);
                                outputXls.Tables[1].SetValue(startIndex2, 2, book.Name);
                                outputXls.Tables[1].SetValue(startIndex2, 3, skill.Name);
                                outputXls.Tables[1].SetValue(startIndex2, 4, "无");
                                outputXls.Tables[1].SetValue(startIndex2, 5, skill.Type.ToString());
                                outputXls.Tables[1].SetValue(startIndex2, 6, skill.Rate.ToString());
                                outputXls.Tables[1].SetValue(startIndex2, 7, skill.EffectSrc);
                                outputXls.Tables[1].SetValue(startIndex2, 8, skill.EffectSoundId);
                                outputXls.Tables[1].SetValue(startIndex2, 9, skill.CDTime.ToString());
                                startIndex2++;

                                if (skill.BuffDatas != null) {
                                    for (int a = 0, alen = skill.BuffDatas.Count; a < alen; a++) {
                                        buff = skill.BuffDatas[a];
                                        outputXls.Tables[2].SetValue(startIndex3, 1, skill.Id);
                                        outputXls.Tables[2].SetValue(startIndex3, 2, book.Name);
                                        outputXls.Tables[2].SetValue(startIndex3, 3, skill.Name);
                                        outputXls.Tables[2].SetValue(startIndex3, 4, "无");
                                        outputXls.Tables[2].SetValue(startIndex3, 5, "增益");
                                        outputXls.Tables[2].SetValue(startIndex3, 6, buff.Type.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 7, buff.Rate.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 8, buff.RoundNumber.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 9, buff.Value.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 10, buff.FirstEffect ? "是" : "否");
                                        startIndex3++;
                                    }
                                }
                                if (skill.DeBuffDatas != null) {
                                    for (int a = 0, alen = skill.DeBuffDatas.Count; a < alen; a++) {
                                        buff = skill.DeBuffDatas[a];
                                        outputXls.Tables[2].SetValue(startIndex3, 1, skill.Id);
                                        outputXls.Tables[2].SetValue(startIndex3, 2, book.Name);
                                        outputXls.Tables[2].SetValue(startIndex3, 3, skill.Name);
                                        outputXls.Tables[2].SetValue(startIndex3, 4, "无");
                                        outputXls.Tables[2].SetValue(startIndex3, 5, "减益");
                                        outputXls.Tables[2].SetValue(startIndex3, 6, buff.Type.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 7, buff.Rate.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 8, buff.RoundNumber.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 9, buff.Value.ToString());
                                        outputXls.Tables[2].SetValue(startIndex3, 10, buff.FirstEffect ? "是" : "否");
                                        startIndex3++;
                                    }
                                }
                            }
                            for (int j = 0, len2 = skill.ResourceAddedSkillIds.Count; j < len2; j++) {
                                addedSkill = allSkillDataMapping[skill.ResourceAddedSkillIds[j]];
                                if (!createdSkillMapping.ContainsKey(addedSkill.Id)) {
                                    createdSkillMapping.Add(addedSkill.Id, true);
                                    outputXls.Tables[1].SetValue(startIndex2, 1, addedSkill.Id);
                                    outputXls.Tables[1].SetValue(startIndex2, 2, book.Name);
                                    outputXls.Tables[1].SetValue(startIndex2, 3, addedSkill.Name);
                                    outputXls.Tables[1].SetValue(startIndex2, 4, skill.Name);
                                    outputXls.Tables[1].SetValue(startIndex2, 5, addedSkill.Type.ToString());
                                    outputXls.Tables[1].SetValue(startIndex2, 6, addedSkill.Rate.ToString());
                                    outputXls.Tables[1].SetValue(startIndex2, 7, addedSkill.EffectSrc);
                                    outputXls.Tables[1].SetValue(startIndex2, 8, addedSkill.EffectSoundId);
                                    outputXls.Tables[1].SetValue(startIndex2, 9, addedSkill.CDTime.ToString());
                                    startIndex2++;

                                    if (addedSkill.BuffDatas != null) {
                                        for (int a = 0, alen = addedSkill.BuffDatas.Count; a < alen; a++) {
                                            buff = addedSkill.BuffDatas[a];
                                            outputXls.Tables[2].SetValue(startIndex3, 1, addedSkill.Id);
                                            outputXls.Tables[2].SetValue(startIndex3, 2, book.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 3, addedSkill.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 4, skill.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 5, "增益");
                                            outputXls.Tables[2].SetValue(startIndex3, 6, buff.Type.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 7, buff.Rate.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 8, buff.RoundNumber.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 9, buff.Value.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 10, buff.FirstEffect ? "是" : "否");
                                            startIndex3++;
                                        }
                                    }
                                    if (addedSkill.DeBuffDatas != null) {
                                        for (int a = 0, alen = addedSkill.DeBuffDatas.Count; a < alen; a++) {
                                            buff = addedSkill.DeBuffDatas[a];
                                            outputXls.Tables[2].SetValue(startIndex3, 1, addedSkill.Id);
                                            outputXls.Tables[2].SetValue(startIndex3, 2, book.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 3, addedSkill.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 4, skill.Name);
                                            outputXls.Tables[2].SetValue(startIndex3, 5, "减益");
                                            outputXls.Tables[2].SetValue(startIndex3, 6, buff.Type.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 7, buff.Rate.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 8, buff.RoundNumber.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 9, buff.Value.ToString());
                                            outputXls.Tables[2].SetValue(startIndex3, 10, buff.FirstEffect ? "是" : "否");
                                            startIndex3++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ExcelHelper.SaveExcel(outputXls, outputPath); //生成excel
                    this.ShowNotification(new GUIContent("秘籍数值Excel已生成\n目录为:" + outputPath));
                }

                if (GUI.Button(new Rect(listStartX + 310, 5, 100, 18), "加载秘籍Excel")) {
                    Excel loadExcel = ExcelHelper.LoadExcel(ExcelEditor.DocsPath + "/秘籍数值.xlsx");
                    ExcelTable table = loadExcel.Tables[0];
                    BookData book;
                    for (int i = 2, len = table.NumberOfRows; i <= len; i++) {
                        if (dataMapping.ContainsKey(table.GetValue(i, 1).ToString())) {
                            book = dataMapping[table.GetValue(i, 1).ToString()];
                            book.Name = table.GetValue(i, 2).ToString();
                            book.BelongToCityId = table.GetValue(i, 3).ToString();
                            book.CanNotMoveResistance = int.Parse(table.GetValue(i, 4).ToString());
                            book.ChaosResistance = int.Parse(table.GetValue(i, 5).ToString());
                            book.DisarmResistance = int.Parse(table.GetValue(i, 6).ToString());
                            book.DrugResistance = int.Parse(table.GetValue(i, 7).ToString());
                            book.SlowResistance = int.Parse(table.GetValue(i, 8).ToString());
                            book.VertigoResistance = int.Parse(table.GetValue(i, 9).ToString());
                            book.DodgePlus = float.Parse(table.GetValue(i, 10).ToString());
                            book.HurtCutRatePlus = float.Parse(table.GetValue(i, 11).ToString());
                            book.MagicAttackPlus = float.Parse(table.GetValue(i, 12).ToString());
                            book.MagicDefensePlus = float.Parse(table.GetValue(i, 13).ToString());
                            book.MaxHPPlus = int.Parse(table.GetValue(i, 14).ToString());
                            book.PhysicsDefensePlus = float.Parse(table.GetValue(i, 15).ToString());
                            book.IsMindBook = table.GetValue(i, 16).ToString() == "是";
                            book.LimitWeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), table.GetValue(i, 17).ToString());
                            book.Occupation = (OccupationType)Enum.Parse(typeof(OccupationType), table.GetValue(i, 18).ToString());
                            book.Quality = (QualityType)Enum.Parse(typeof(QualityType), table.GetValue(i, 19).ToString());
                            book.ResourceSkillDataIds = JsonManager.GetInstance().DeserializeObject<List<string>>(table.GetValue(i, 20).ToString());
                            book.Desc = table.GetValue(i, 21).ToString();
                        }
                    }

                    ExcelTable table2 = loadExcel.Tables[1];
                    SkillData skill;
                    for (int i = 2, len = table2.NumberOfRows; i <= len; i++) {
                        if (allSkillDataMapping.ContainsKey(table2.GetValue(i, 1).ToString())) {
                            skill = allSkillDataMapping[table2.GetValue(i, 1).ToString()];
                            skill.Name = table2.GetValue(i, 3).ToString();
                            skill.Type = (SkillType)Enum.Parse(typeof(SkillType), table2.GetValue(i, 5).ToString());
                            skill.Rate = float.Parse(table2.GetValue(i, 6).ToString());
                            skill.EffectSrc = table2.GetValue(i, 7).ToString();
                            skill.EffectSoundId = table2.GetValue(i, 8).ToString();
                            skill.CDTime = float.Parse(table2.GetValue(i, 9).ToString());
                            skill.BuffDatas.Clear();
                            skill.DeBuffDatas.Clear();
                        }
                    }

                    ExcelTable table3 = loadExcel.Tables[2];
                    BuffData buff;
                    BuffType buffType;
                    for (int i = 2, len = table3.NumberOfRows; i <= len; i++) {
                        if (allSkillDataMapping.ContainsKey(table3.GetValue(i, 1).ToString())) {
                            skill = allSkillDataMapping[table3.GetValue(i, 1).ToString()];
                            buffType = (BuffType)Enum.Parse(typeof(BuffType), table3.GetValue(i, 6).ToString());
//                            buff = null;
//                            if (table3.GetValue(i, 5).ToString() == "增益") {
//                                buff = skill.BuffDatas.Find(item => item.Type == buffType);
//                            } else {
//                                buff = skill.DeBuffDatas.Find(item => item.Type == buffType);
//                            }
//                            if (buff != null) {
//                                buff.Rate = float.Parse(table3.GetValue(i, 7).ToString());
//                                buff.RoundNumber = int.Parse(table3.GetValue(i, 8).ToString());
//                                buff.Value = float.Parse(table3.GetValue(i, 9).ToString());
//                                buff.FirstEffect = table3.GetValue(i, 10).ToString() == "是";
//                            }
                            buff = new BuffData();
                            buff.Type = buffType;
                            buff.Rate = float.Parse(table3.GetValue(i, 7).ToString());
                            buff.RoundNumber = int.Parse(table3.GetValue(i, 8).ToString());
                            buff.Timeout = (float)buff.RoundNumber;
                            buff.Value = float.Parse(table3.GetValue(i, 9).ToString());
                            buff.FirstEffect = table3.GetValue(i, 10).ToString() == "是";
                            if (table3.GetValue(i, 5).ToString() == "增益") {
                                skill.BuffDatas.Add(buff);
                            } else {
                                skill.DeBuffDatas.Add(buff);
                            }
                        }
                    }

                    oldSelGridInt = -1;
                    fetchData(searchKeyword);
                    writeDataToJson();
                    writeDataToJson2();
                    this.ShowNotification(new GUIContent("秘籍数值Excel的数据已经导入"));
                }

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 200));
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
					GUI.Label(new Rect(205, 20, 60, 18), "开启地:");
					belongToCityIdIndex = EditorGUI.Popup(new Rect(250, 20, 100, 18), belongToCityIdIndex, Base.AllCitySceneNames.ToArray());
					GUI.Label(new Rect(355, 0, 40, 18), "描述:");
					bookDesc = EditorGUI.TextArea(new Rect(400, 0, 400, 54), bookDesc);
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}

					GUI.Label(new Rect(55, 40, 40, 18), "气血:");
                    try {
                        maxHPPlus = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(100, 40, 60, 18), maxHPPlus.ToString())), 0, 100000);
                    }
                    catch(Exception e) {
                        maxHPPlus = 0;
                    }
					GUI.Label(new Rect(285, 40, 40, 18), "品质:");
					qualityTypeIndex = EditorGUI.Popup(new Rect(316, 40, 30, 18), qualityTypeIndex, qualityTypeStrs.ToArray());
					GUI.Label(new Rect(55, 60, 40, 18), "轻功:");
                    try {
                        dodgePlus = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(100, 60, 60, 18), dodgePlus.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        dodgePlus = 0;
                    }
					GUI.Label(new Rect(285, 60, 40, 18), "门派:");
                    occupationIndex = EditorGUI.Popup(new Rect(316, 60, 80, 18), occupationIndex, Base.OccupationTypeStrs.ToArray());
                    GUI.Label(new Rect(285, 80, 40, 18), "兵器:");
                    limitWeaponTypeIndex = EditorGUI.Popup(new Rect(316, 80, 80, 18), limitWeaponTypeIndex, weaponTypeStrs.ToArray());
					GUI.Label(new Rect(400, 60, 30, 18), "心法:");
					isMindBook = EditorGUI.Toggle(new Rect(435, 60, 20, 18), isMindBook);
					GUI.Label(new Rect(55, 80, 40, 18), "外防:");
                    try {
                        physicsDefensePlus = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(100, 80, 60, 18), physicsDefensePlus.ToString())), 0, 10000);
                    }
                    catch(Exception e) {
                        physicsDefensePlus = 0;
                    }
					GUI.Label(new Rect(55, 100, 40, 18), "减伤:");
					try {
                        hurtCutRatePlus = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(100, 100, 60, 18), hurtCutRatePlus.ToString())), 0, 1);
                    }
                    catch(Exception e) {
                        hurtCutRatePlus = 0;
                    }
					GUI.Label(new Rect(55, 120, 40, 18), "内功:");
                    try {
                        magicAttackPlus = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(100, 120, 60, 18), magicAttackPlus.ToString())), 0, 10000);
                    }
                    catch(Exception e) {
                        magicAttackPlus = 0;
                    }
					GUI.Label(new Rect(55, 140, 40, 18), "内防:");
                    try {
                        magicDefensePlus = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(100, 140, 60, 18), magicDefensePlus.ToString())), 0, 10000);
                    }
                    catch(Exception e) {
                        magicDefensePlus = 0;
                    }

                    GUI.Label(new Rect(165, 40, 50, 18), "中毒抵抗:");
                    try {
                        drugResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 40, 50, 18), drugResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        drugResistance = 0;
                    }
                    GUI.Label(new Rect(165, 60, 50, 18), "缴械抵抗:");
                    try {
                        disarmResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 60, 50, 18), disarmResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        disarmResistance = 0;
                    }
                    GUI.Label(new Rect(165, 80, 50, 18), "眩晕抵抗:");
                    try {
                        vertigoResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 80, 50, 18), vertigoResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        vertigoResistance = 0;
                    }
                    GUI.Label(new Rect(165, 100, 50, 18), "定身抵抗:");
                    try {
                        canNotMoveResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 100, 50, 18), canNotMoveResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        canNotMoveResistance = 0;
                    }
                    GUI.Label(new Rect(165, 120, 50, 18), "迟缓抵抗:");
                    try {
                        slowResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 120, 50, 18), slowResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        slowResistance = 0;
                    }
                    GUI.Label(new Rect(165, 140, 50, 18), "混乱抵抗:");
                    try {
                        chaosResistance = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(220, 140, 50, 18), chaosResistance.ToString())), 0, 100);
                    }
                    catch(Exception e) {
                        chaosResistance = 0;
                    }

					for (int i = 0; i < needsIdIndexes.Count; i++) {
						if (needsIdIndexes.Count > i) {
							needsIdIndexes[i] = EditorGUI.Popup(new Rect(300, 100 + i * 20, 100, 18), needsIdIndexes[i], Base.ItemDataNames.ToArray());
							needsNums[i] = (int)EditorGUI.Slider(new Rect(405, 100 + i * 20, 180, 18), needsNums[i], 1, 1000);
							if (GUI.Button(new Rect(590, 100 + i * 20, 36, 18), "X")) {
								needsIdIndexes.RemoveAt(i);
								needsNums.RemoveAt(i);
							}
						}
					}
					if (GUI.Button(new Rect(630, 100, 90, 18), "添加残卷")) {
						if (needsIdIndexes.Count < 5) {
							needsIdIndexes.Add(0);
							needsNums.Add(1);
						}
						else {
							this.ShowNotification(new GUIContent("残卷不能大于5个"));
						}
					}

					if (GUI.Button(new Rect(0, 165, 80, 18), "修改基础属性")) {
						if (bookName == "") {
							this.ShowNotification(new GUIContent("秘籍名不能为空!"));
							return;
						}
						data.Name = bookName;
						data.Desc = bookDesc;
						data.IconId = icons[iconIndex].Id;
						data.BeUsingByRoleId = "";
						data.BelongToCityId = Base.AllCityScenes[belongToCityIdIndex].Id;
						data.Quality = qualityTypeEnums[qualityTypeIndex];
						data.MaxHPPlus = maxHPPlus;
						data.DodgePlus = dodgePlus;
						data.PhysicsDefensePlus = physicsDefensePlus;
						data.HurtCutRatePlus = hurtCutRatePlus;
						data.MagicAttackPlus = magicAttackPlus;
						data.MagicDefensePlus = magicDefensePlus;
                        data.DrugResistance = drugResistance;
                        data.DisarmResistance = disarmResistance;
                        data.VertigoResistance = vertigoResistance;
                        data.CanNotMoveResistance = canNotMoveResistance;
                        data.SlowResistance = slowResistance;
                        data.ChaosResistance = chaosResistance;
						data.Occupation = Base.OccupationTypeEnums[occupationIndex];
                        data.LimitWeaponType = weaponTypeEnums[limitWeaponTypeIndex];
						data.IsMindBook = isMindBook;
						data.Needs = new List<CostData>();
						for (int i = 0; i < needsIdIndexes.Count; i++) {
							if (needsIdIndexes.Count > i) {
								data.Needs.Add(new CostData(Base.ItemDatas[needsIdIndexes[i]].Id, needsNums[i]));
							}
						}
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();

					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 205, 800, 500));
					GUI.Label(new Rect(0, 0, 800, 18), "|----------秘籍招式--------------------------------------------------------------------------------------------------------------|");
					GUI.Label(new Rect(0, 20, 75, 18), "选择秘籍招式:");
					addSkillIdIndex = EditorGUI.Popup(new Rect(80, 20, 100, 18), addSkillIdIndex, allSkillDataNames.ToArray());
					if (GUI.Button(new Rect(185, 20, 80, 18), "添加新招式")) {
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

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 700, 300, 60));
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
                    //默认选择有关联的icon
                    addSkillIdIndex = iconNames.FindIndex(item => item.IndexOf(addBookName) >= 0);
                    addSkillIdIndex = addSkillIdIndex >= 0 ? addSkillIdIndex : 0;
                    addBookData.IconId = icons[addSkillIdIndex].Id;
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