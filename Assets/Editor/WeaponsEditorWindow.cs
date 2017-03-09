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
			float width = 660;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (WeaponsEditorWindow)EditorWindow.GetWindowWithRect(typeof(WeaponsEditorWindow), size, true, "兵器数据编辑器");
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

        static List<WeaponType> weaponTypeEnums;
        static List<string> weaponTypeStrs;
        static Dictionary<WeaponType, int> weaponTypeIndexMapping;

        static List<string> roleNames;
        static Dictionary<string, int> roleIdIndexesMapping;
        static List<RoleData> roles;

        static Dictionary<ResourceType, int> resourceNeedSecondsMapping;

        static float modifyResourceTimeout = 20;

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
					if (iconData.Name.IndexOf("兵器-") < 0) {
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

            roles = new List<RoleData>() { null };
            roleIdIndexesMapping = new Dictionary<string, int>() { { "", 0 } };
            roleNames = new List<string>() { "无" };

            obj = JsonManager.GetInstance().GetJson("RoleDatas", false);
            RoleData roleData;
            index = 1;
            foreach(var item in obj) {
                if (item.Key != "0") {
                    roleData = JsonManager.GetInstance().DeserializeObject<RoleData>(item.Value.ToString());
                    roleNames.Add(roleData.Name);
                    roleIdIndexesMapping.Add(roleData.Id, index);
                    roles.Add(roleData);
                    index++;
                }
            }

            WorkshopModel.Init();

            resourceNeedSecondsMapping = new Dictionary<ResourceType, int>();
            ResourceRelationshipData relation;
            for (int i = 0; i < WorkshopModel.Relationships.Count; i++) {
                relation = WorkshopModel.Relationships[i];
                resourceNeedSecondsMapping.Add(relation.Type, getBeedSecond(relation.Type));
            }
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

        static Dictionary<string, WeaponData> dataMapping;
		static void getData() {
            dataMapping = new Dictionary<string, WeaponData>();
			JObject obj = JsonManager.GetInstance().GetJson("Weapons", false);
            WeaponData weapon;
			foreach(var item in obj) {
				if (item.Key != "0") {
                    weapon = JsonManager.GetInstance().DeserializeObject<WeaponData>(item.Value.ToString());
                    dataMapping.Add(item.Value["Id"].ToString(), weapon);
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
			List<WeaponData> datas = new List<WeaponData>();
			foreach(WeaponData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			JObject weaponBuildingsOfWorkshopData = new JObject(); //工坊兵器锻造静态json数据
			foreach(WeaponData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				if (weaponBuildingsOfWorkshopData[data.BelongToCityId] == null) {
					weaponBuildingsOfWorkshopData[data.BelongToCityId] = new JArray(data.Id);
				}
				else {
					((JArray)weaponBuildingsOfWorkshopData[data.BelongToCityId]).Add(data.Id);
				}
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Weapons.json", JsonManager.GetInstance().SerializeObject(writeJson));
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "WeaponIdsOfWorkshopData.json", JsonManager.GetInstance().SerializeObject(weaponBuildingsOfWorkshopData));
		}

		WeaponData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string weaponName = "";
		int iconIndex = 0;
        int weaponTypeIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int qualityTypeIndex = 0;
		float[] rates;
		string weaponDesc = "";
		float weaponWidth = 0;
		int belongToCityIdIndex = 0;
		float physicsAttackPlus = 0;
		int fixedDamagePlus = 0;
		float damageRatePlus = 0;
		float attackSpeedPlus = 0;
		int occupationIndex = 0;
        bool justBelongToHost;
        int belongToRoleIdIndex = 0;
		List<int> needsTypeIndexes;
		List<float> needsNums;

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
                    weaponTypeIndex = weaponTypeIndexMapping.ContainsKey(data.Type) ? weaponTypeIndexMapping[data.Type] : 0;
					qualityTypeIndex = qualityTypeIndexMapping[data.Quality];
					rates = data.Rates;
					weaponDesc = data.Desc;
					weaponWidth = data.Width;
					belongToCityIdIndex = Base.AllCitySceneIdIndexs.ContainsKey(data.BelongToCityId) ? Base.AllCitySceneIdIndexs[data.BelongToCityId] : 0;
					physicsAttackPlus = data.PhysicsAttackPlus;
					fixedDamagePlus = data.FixedDamagePlus;
					damageRatePlus = data.DamageRatePlus;
					attackSpeedPlus = data.AttackSpeedPlus;
					occupationIndex = Base.OccupationTypeIndexMapping.ContainsKey(data.Occupation) ? Base.OccupationTypeIndexMapping[data.Occupation] : 0;
                    justBelongToHost = data.JustBelongToHost;
                    belongToRoleIdIndex = !string.IsNullOrEmpty(data.BelongToRoleId) && roleIdIndexesMapping.ContainsKey(data.BelongToRoleId) ? roleIdIndexesMapping[data.BelongToRoleId] : 0;
                    needsTypeIndexes = new List<int>();
					needsNums = new List<float>();
					foreach(ResourceData need in data.Needs) {
						needsTypeIndexes.Add(Base.ResourceTypeIndexMapping.ContainsKey(need.Type) ? Base.ResourceTypeIndexMapping[need.Type] : 0);
						needsNums.Add((float)need.Num);
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

                if (GUI.Button(new Rect(listStartX + 205, 5, 100, 18), "生成兵器Excel")) {
                    Excel outputXls = new Excel();
                    ExcelTable outputTable= new ExcelTable();
                    outputTable.TableName = "兵器数值";
                    string outputPath = ExcelEditor.DocsPath + "/兵器数值.xlsx";
                    outputXls.Tables.Add(outputTable);
                    outputXls.Tables[0] = new ExcelTable();
                    outputXls.Tables[0].TableName = "兵器数值";
                    outputXls.Tables[0].SetValue(1, 1, "1兵器id");
                    outputXls.Tables[0].SetValue(1, 2, "2兵器名称");
                    outputXls.Tables[0].SetValue(1, 3, "3暴击1.25倍");
                    outputXls.Tables[0].SetValue(1, 4, "4暴击1.5倍");
                    outputXls.Tables[0].SetValue(1, 5, "5暴击2倍");
                    outputXls.Tables[0].SetValue(1, 6, "6兵器长度");
                    outputXls.Tables[0].SetValue(1, 7, "7外功增量");
                    outputXls.Tables[0].SetValue(1, 8, "8固定伤害增量");
                    outputXls.Tables[0].SetValue(1, 9, "9伤害比例增量");
                    outputXls.Tables[0].SetValue(1, 10, "10攻速增量");
                    outputXls.Tables[0].SetValue(1, 11, "11材料一");
                    outputXls.Tables[0].SetValue(1, 12, "12材料一数量");
                    outputXls.Tables[0].SetValue(1, 13, "13材料二");
                    outputXls.Tables[0].SetValue(1, 14, "14材料二数量");
                    outputXls.Tables[0].SetValue(1, 15, "15材料三");
                    outputXls.Tables[0].SetValue(1, 16, "16材料三数量");
                    outputXls.Tables[0].SetValue(1, 17, "17材料四");
                    outputXls.Tables[0].SetValue(1, 18, "18材料四数量");
                    outputXls.Tables[0].SetValue(1, 19, "19材料五");
                    outputXls.Tables[0].SetValue(1, 20, "20材料五数量");
                    outputXls.Tables[0].SetValue(1, 21, "21所属城镇Id");
                    outputXls.Tables[0].SetValue(1, 22, "22所属角色Id");
                    outputXls.Tables[0].SetValue(1, 23, "23单一家丁生产耗时");
                    outputXls.Tables[0].SetValue(1, 24, "24单一家丁生产耗时");

                    int startIndex = 2;
                    int costSeconds;
                    foreach(WeaponData weapon in dataMapping.Values) {
                        outputXls.Tables[0].SetValue(startIndex, 1, weapon.Id);
                        outputXls.Tables[0].SetValue(startIndex, 2, weapon.Name);
                        outputXls.Tables[0].SetValue(startIndex, 3, weapon.Rates[1].ToString());
                        outputXls.Tables[0].SetValue(startIndex, 4, weapon.Rates[2].ToString());
                        outputXls.Tables[0].SetValue(startIndex, 5, weapon.Rates[3].ToString());
                        outputXls.Tables[0].SetValue(startIndex, 6, weapon.Width.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 7, weapon.PhysicsAttackPlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 8, weapon.FixedDamagePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 9, weapon.DamageRatePlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 10, weapon.AttackSpeedPlus.ToString());
                        outputXls.Tables[0].SetValue(startIndex, 11, weapon.Needs.Count > 0 ? weapon.Needs[0].Type.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 12, weapon.Needs.Count > 0 ? weapon.Needs[0].Num.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 13,  weapon.Needs.Count > 1 ? weapon.Needs[1].Type.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 14, weapon.Needs.Count > 1 ? weapon.Needs[1].Num.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 15,  weapon.Needs.Count > 2 ? weapon.Needs[2].Type.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 16, weapon.Needs.Count > 2 ? weapon.Needs[2].Num.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 17,  weapon.Needs.Count > 3 ? weapon.Needs[3].Type.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 18, weapon.Needs.Count > 3 ? weapon.Needs[3].Num.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 19,  weapon.Needs.Count > 4 ? weapon.Needs[4].Type.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 20, weapon.Needs.Count > 4 ? weapon.Needs[4].Num.ToString() : "无");
                        outputXls.Tables[0].SetValue(startIndex, 21, weapon.BelongToCityId);
                        outputXls.Tables[0].SetValue(startIndex, 22, weapon.BelongToRoleId);
                        costSeconds = 0;
                        for (int i = 0, len = weapon.Needs.Count; i < len; i++)
                        {
                            costSeconds += (int)((double)getBeedSecond(weapon.Needs[i].Type) * weapon.Needs[i].Num);
                        }
                        outputXls.Tables[0].SetValue(startIndex, 23, Statics.GetFullTime(costSeconds));
                        outputXls.Tables[0].SetValue(startIndex, 24, costSeconds.ToString());
                        startIndex++;
                    }

                    ExcelHelper.SaveExcel(outputXls, outputPath); //生成excel
                    this.ShowNotification(new GUIContent("兵器数值Excel已生成\n目录为:" + outputPath));
                }

                if (GUI.Button(new Rect(listStartX + 310, 5, 100, 18), "加载兵器Excel")) {
                    Excel loadExcel = ExcelHelper.LoadExcel(ExcelEditor.DocsPath + "/兵器数值.xlsx");
                    ExcelTable table = loadExcel.Tables[0];
                    WeaponData weapon;
                    for (int i = 2, len = table.NumberOfRows; i <= len; i++) {
                        if (dataMapping.ContainsKey(table.GetValue(i, 1).ToString())) {
                            weapon = dataMapping[table.GetValue(i, 1).ToString()];
                        } else {
                            weapon = new WeaponData();
                            weapon.Id = table.GetValue(i, 1).ToString();
                            dataMapping.Add(weapon.Id, weapon);
                        }
                        weapon.Name = table.GetValue(i, 2).ToString();
                        weapon.Rates[1] = float.Parse(table.GetValue(i, 3).ToString());
                        weapon.Rates[2] = float.Parse(table.GetValue(i, 4).ToString());
                        weapon.Rates[3] = float.Parse(table.GetValue(i, 5).ToString());
                        weapon.Width = float.Parse(table.GetValue(i, 6).ToString());
                        weapon.PhysicsAttackPlus = float.Parse(table.GetValue(i, 7).ToString());
                        weapon.FixedDamagePlus = int.Parse(table.GetValue(i, 8).ToString());
                        weapon.DamageRatePlus = float.Parse(table.GetValue(i, 9).ToString());
                        weapon.AttackSpeedPlus = float.Parse(table.GetValue(i, 10).ToString());
                        weapon.Needs.Clear();
                        if (table.GetValue(i, 11).ToString() != "无") {
                            weapon.Needs.Add(new ResourceData((ResourceType)Enum.Parse(typeof(ResourceType), table.GetValue(i, 11).ToString()), double.Parse(table.GetValue(i, 12).ToString())));
                        }
                        if (table.GetValue(i, 13).ToString() != "无") {
                            weapon.Needs.Add(new ResourceData((ResourceType)Enum.Parse(typeof(ResourceType), table.GetValue(i, 13).ToString()), double.Parse(table.GetValue(i, 14).ToString())));
                        }
                        if (table.GetValue(i, 15).ToString() != "无") {
                            weapon.Needs.Add(new ResourceData((ResourceType)Enum.Parse(typeof(ResourceType), table.GetValue(i, 15).ToString()), double.Parse(table.GetValue(i, 16).ToString())));
                        }
                        if (table.GetValue(i, 17).ToString() != "无") {
                            weapon.Needs.Add(new ResourceData((ResourceType)Enum.Parse(typeof(ResourceType), table.GetValue(i, 17).ToString()), double.Parse(table.GetValue(i, 18).ToString())));
                        }
                        if (table.GetValue(i, 19).ToString() != "无") {
                            weapon.Needs.Add(new ResourceData((ResourceType)Enum.Parse(typeof(ResourceType), table.GetValue(i, 19).ToString()), double.Parse(table.GetValue(i, 20).ToString())));
                        }
                    }
                    oldSelGridInt = -1;
                    fetchData(searchKeyword);
                    this.ShowNotification(new GUIContent("兵器数值Excel的数据已经导入"));
                }

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 600, 500));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 120, 120), iconTexture);
					}
					GUI.Label(new Rect(125, 0, 30, 18), "Id:");
                    EditorGUI.TextField(new Rect(155, 0, 50, 18), showId);
                    GUI.Label(new Rect(210, 0, 40, 18), "兵器名:");
                    weaponName = EditorGUI.TextField(new Rect(250, 0, 60, 18), weaponName);
                    GUI.Label(new Rect(125, 20, 60, 18), "类型:");
                    weaponTypeIndex = EditorGUI.Popup(new Rect(190, 20, 100, 18), weaponTypeIndex, weaponTypeStrs.ToArray());
					GUI.Label(new Rect(315, 0, 40, 18), "开启:");
					belongToCityIdIndex = EditorGUI.Popup(new Rect(340, 0, 100, 18), belongToCityIdIndex, Base.AllCitySceneNames.ToArray());
					GUI.Label(new Rect(295, 20, 60, 18), "门派:");
					occupationIndex = EditorGUI.Popup(new Rect(340, 20, 100, 18), occupationIndex, Base.OccupationTypeStrs.ToArray());
                    GUI.Label(new Rect(295, 40, 60, 18), "主角专属:");
                    justBelongToHost = EditorGUI.Toggle(new Rect(350, 40, 20, 18), justBelongToHost);
                    if (!justBelongToHost) {
                        GUI.Label(new Rect(295, 60, 60, 18), "主人:");
                        belongToRoleIdIndex = EditorGUI.Popup(new Rect(340, 60, 100, 18), belongToRoleIdIndex, roleNames.ToArray());
                    } else {
                        belongToRoleIdIndex = 0;
                    }
                    if (belongToRoleIdIndex > 0) {
                        //有唯一主人的兵器不能作为主角门派动态出现，应该是及时出现在工坊
                        justBelongToHost = false;

                    }
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
					GUI.Label(new Rect(125, 140, 100, 18), "兵器长度:");
					weaponWidth = EditorGUI.Slider(new Rect(230, 140, 180, 18), weaponWidth, 50, 300);
					GUI.Label(new Rect(0, 160, 100, 18), "外功增量:");
					physicsAttackPlus = EditorGUI.Slider(new Rect(0, 180, 180, 18), physicsAttackPlus, 0, 100000);
					GUI.Label(new Rect(200, 160, 100, 18), "固定伤害增量:");
					fixedDamagePlus = (int)EditorGUI.Slider(new Rect(200, 180, 180, 18), fixedDamagePlus, 0, 100000);
					GUI.Label(new Rect(0, 200, 100, 18), "伤害比例增量:");
					damageRatePlus = EditorGUI.Slider(new Rect(0, 220, 180, 18), damageRatePlus, 0, 20);
					GUI.Label(new Rect(200, 200, 100, 18), "攻速增量:");
					attackSpeedPlus = EditorGUI.Slider(new Rect(200, 220, 180, 18), attackSpeedPlus, -25, 25);
					for (int i = 0; i < needsTypeIndexes.Count; i++) {
						if (needsTypeIndexes.Count > i) {
							needsTypeIndexes[i] = EditorGUI.Popup(new Rect(0, 240 + i * 20, 100, 18), needsTypeIndexes[i], Base.ResourceTypeStrs.ToArray());
							needsNums[i] = EditorGUI.Slider(new Rect(105, 240 + i * 20, 180, 18), needsNums[i], 1, 1000);
							if (GUI.Button(new Rect(290, 240 + i * 20, 36, 18), "X")) {
								needsTypeIndexes.RemoveAt(i);
								needsNums.RemoveAt(i);
							}
						}
					}
					if (GUI.Button(new Rect(330, 240, 90, 18), "添加原材料")) {
						if (needsTypeIndexes.Count < 5) {
							needsTypeIndexes.Add(Base.ResourceTypeIndexMapping[ResourceType.Silver]);
							needsNums.Add(1);
						}
						else {
							this.ShowNotification(new GUIContent("原材料不能大于5个"));
						}
					}
					GUI.Label(new Rect(0, 360, 60, 18), "描述:");
					weaponDesc = GUI.TextArea(new Rect(45, 360, 370, 100), weaponDesc);

					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(332, 470, 80, 18), "修改兵器属性")) {
						if (weaponName == "") {
							this.ShowNotification(new GUIContent("兵器名不能为空!"));
							return;
						}
						data.Name = weaponName;
						data.IconId = icons[iconIndex].Id;
                        data.Type = weaponTypeEnums[weaponTypeIndex];
						data.Quality = qualityTypeEnums[qualityTypeIndex];
						data.Rates[1] = rates[1];
						data.Rates[2] = rates[2];
						data.Rates[3] = rates[3];
						data.Desc = weaponDesc;
						data.Width = weaponWidth;
						data.BeUsingByRoleId = "";
						data.BelongToCityId = Base.AllCityScenes[belongToCityIdIndex].Id;
						data.PhysicsAttackPlus = physicsAttackPlus;
						data.FixedDamagePlus = fixedDamagePlus;
						data.DamageRatePlus = damageRatePlus;
						data.AttackSpeedPlus = attackSpeedPlus;
						data.Occupation = Base.OccupationTypeEnums[occupationIndex];
                        data.JustBelongToHost = justBelongToHost;
                        data.BelongToRoleId = belongToRoleIdIndex > 0 ? roles[belongToRoleIdIndex].Id : "";
						data.Needs = new List<ResourceData>();
						for (int i = 0; i < needsTypeIndexes.Count; i++) {
							if (needsTypeIndexes.Count > i) {
								data.Needs.Add(new ResourceData(Base.ResourceTypeEnums[needsTypeIndexes[i]], (double)needsNums[i]));
							}
						}
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 500, 500, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加兵器")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除兵器")) {
					toolState = 2;
				}
				break;
			case 1:
				GUI.Label(new Rect(0, 0, 30, 18), "Id:");
				addId = GUI.TextField(new Rect(35, 0, 80, 18), addId);
				GUI.Label(new Rect(120, 0, 50, 18), "兵器名:");
				addWeaponName = GUI.TextField(new Rect(175, 0, 80, 18), addWeaponName);
				if (GUI.Button(new Rect(260, 0, 80, 18), "添加")) {
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addWeaponName == "") {
						this.ShowNotification(new GUIContent("兵器名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					WeaponData weapon = new WeaponData();
					weapon.Id = addId;
					weapon.Name = addWeaponName;
                    ResourceSrcData findIcon = icons.Find(item => item.Name.IndexOf(weapon.Name) >= 0);
                    if (findIcon != null) {
                        weapon.IconId = findIcon.Id;
                    }
					dataMapping.Add(weapon.Id, weapon);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
//					addId = "";
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