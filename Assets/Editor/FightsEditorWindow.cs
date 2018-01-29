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
//			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex0", 0);
//			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex1", 0);
//			PlayerPrefs.SetInt("FightEditorTestRoleIdIndex2", 0);
			EditorApplication.OpenScene("Assets/Scenes/FightTest.unity");
            InitParams();
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

        static Dictionary<string, bool> usedFights;

		static int testRoleIdIndex0 = 0;
		static int testRoleIdIndex1 = 0;
		static int testRoleIdIndex2 = 0;
        static int testRoleIdIndex3 = 0;
        static int testRoleIdIndex4 = 0;
        static int testRoleIdIndex5 = 0;

        static int hostWeaponLv;
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
            roleNames.Add("无");
            roles.Add(new RoleData());

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
            testRoleIdIndex3 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex3");
            testRoleIdIndex4 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex4");
            testRoleIdIndex5 = PlayerPrefs.GetInt("FightEditorTestRoleIdIndex5");

            TextAsset asset = Resources.Load<TextAsset>("Data/Json/AreaMeetEnemys");
            Dictionary<string, List<RateData>> meetEnemyRatesMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<string, List<RateData>>>(asset.text);
            usedFights = new Dictionary<string, bool>();
            foreach (List<RateData> fights in meetEnemyRatesMapping.Values)
            {
                for (int i = 0, len = fights.Count; i < len; i++)
                {
                    if (!usedFights.ContainsKey(fights[i].Id))
                    {
                        usedFights.Add(fights[i].Id, true);
                    }
                }
            }

            hostWeaponLv = PlayerPrefs.GetInt("TestHostWeaponLv");
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
        static bool areaEnemyOnly = false;
		static void fetchData(string keyword = "") {
			showListData = new List<FightData>();
			foreach(FightData data in dataMapping.Values) {
                if (!data.IsStatic && keyword != "") {
					if (data.Name.IndexOf(keyword) < 0) {
						continue;
					}
				}
                if (data.IsStatic || !areaEnemyOnly || usedFights.ContainsKey(data.Id))
                {
                    if (showListData.FindIndex(item => item.Id == data.Id) < 0)
                    {
                        showListData.Add(data);
                    }
                }
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
			List<FightData> datas = new List<FightData>();
			foreach(FightData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			foreach(FightData data in datas) {
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
        bool isStatic;
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

            GUILayout.BeginArea(new Rect(205, 5, 600, 20));
            if (GUI.Button(new Rect(0, 0, 80, 18), "加载战斗配表")) {
                string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
                Excel xls =  ExcelHelper.LoadExcel(path);
                ExcelTable table = xls.Tables[0];
                List<string> areaIds = new List<string>();
                List<string> areaNames = new List<string>();
                List<List<RoleData>> enemys = new List<List<RoleData>>();
                string areaName;
                RoleData enemy;
                for (int i = 189; i <= table.NumberOfRows; i++) {
                    areaName = table.GetValue(i, 1).ToString();
                    if (areaName.IndexOf("x") >= 0) {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(areaName)) {
                        string[] fen = areaName.Split(new char[] { '|' });
                        areaIds.Add(fen[1]);
                        areaNames.Add(fen[0]);
                        enemys.Add(new List<RoleData>());
                    } else {
                        if (!string.IsNullOrEmpty(table.GetValue(i, 21).ToString())) {
                            enemy = new RoleData();
                            enemy.TeamName = "Enemy";
                            enemy.IsKnight = false;
                            enemy.IsBoss = table.GetValue(i, 40).ToString() == "是";
                            enemy.Id = table.GetValue(i, 21).ToString();
                            enemy.Name = table.GetValue(i, 22).ToString();
                            enemy.Lv = int.Parse(table.GetValue(i, 23).ToString());
                            enemy.DifLv4HP = int.Parse(table.GetValue(i, 26).ToString());
                            enemy.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 28).ToString());
                            enemy.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 30).ToString());
                            enemy.DifLv4MagicAttack = int.Parse(table.GetValue(i, 32).ToString());
                            enemy.DifLv4MagicDefense = int.Parse(table.GetValue(i, 34).ToString());
                            enemy.DifLv4Dodge = int.Parse(table.GetValue(i, 36).ToString());
                            enemy.Desc = table.GetValue(i, 37).ToString(); //记录武功类型 0为外功 1为内功
                            //处理兵器秘籍
                            if (!string.IsNullOrEmpty(table.GetValue(i, 38).ToString())) {
                                enemy.ResourceWeaponDataId = table.GetValue(i, 38).ToString();
                            }
                            if (!string.IsNullOrEmpty(table.GetValue(i, 39).ToString())) {
                                string[] fen = table.GetValue(i, 39).ToString().Split(new char[] { '|' });
                                foreach (string f in fen) {
                                    enemy.ResourceBookDataIds.Add(f);
                                }
                            }
                            enemy.Init();
                            //                Debug.Log(JsonManager.GetInstance().SerializeObject(enemy));
                            enemys[enemys.Count - 1].Add(enemy);
                        }
                    }
                }
                string id;
                int plusId;
                FightData fight;
                RoleData enemy1;
                RoleData enemy2;
                //跳过临安郊外和临安府
                for (int i = 0, len = areaIds.Count; i < len; i++) {
                    id = areaIds[i];
                    plusId = 1031 + i;
                    for (int j = 0, len2 = enemys[i].Count; j < len2; j++) {
                        enemy1 = enemys[i][j];
                        if (!dataMapping.ContainsKey(id + plusId)) {
                            fight = new FightData();
                            dataMapping.Add(id + plusId, fight);
                        } else {
                            fight = dataMapping[id + plusId];
                        }
                        fight.Id = id + plusId;
                        fight.Name = areaNames[i] + "-" + enemy1.Name;
                        fight.ResourceEnemyIds.Clear();
                        fight.ResourceEnemyIds.Add(enemy1.Id);
                        plusId++;
                        if (!enemy1.IsBoss) {
                            if (!dataMapping.ContainsKey(id + plusId)) {
                                fight = new FightData();
                                dataMapping.Add(id + plusId, fight);
                            } else {
                                fight = dataMapping[id + plusId];
                            }
                            fight.Id = id + plusId;
                            fight.Name = areaNames[i] + "-" + enemy1.Name + "*2";
                            fight.ResourceEnemyIds.Clear();
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            plusId++;
                            if (!dataMapping.ContainsKey(id + plusId)) {
                                fight = new FightData();
                                dataMapping.Add(id + plusId, fight);
                            } else {
                                fight = dataMapping[id + plusId];
                            }
                            fight.Id = id + plusId;
                            fight.Name = areaNames[i] + "-" + enemy1.Name + "*3";
                            fight.ResourceEnemyIds.Clear();
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            plusId++;
                        }
                    }
                    for (int j = 0, len2 = enemys[i].Count; j < len2; j++) {
                        enemy1 = enemys[i][j];
                        if (enemy1.IsBoss) {
                            continue;
                        }
                        for (int k = 0, len3 = enemys[i].Count; k < len3; k++) {
                            enemy2 = enemys[i][k];
                            if (enemy1.Id == enemy2.Id) {
                                continue;
                            }
                            if (!dataMapping.ContainsKey(id + plusId)) {
                                fight = new FightData();
                                dataMapping.Add(id + plusId, fight);
                            } else {
                                fight = dataMapping[id + plusId];
                            }
                            fight.Id = id + plusId;
                            fight.Name = areaNames[i] + "-" + enemy1.Name + "*1" + enemy2.Name + "*1";
                            fight.ResourceEnemyIds.Clear();
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy2.Id);
                            plusId++;
                            if (!dataMapping.ContainsKey(id + plusId)) {
                                fight = new FightData();
                                dataMapping.Add(id + plusId, fight);
                            } else {
                                fight = dataMapping[id + plusId];
                            }
                            fight.Id = id + plusId;
                            fight.Name = areaNames[i] + "-" + enemy1.Name + "*2" + enemy2.Name + "*1";
                            fight.ResourceEnemyIds.Clear();
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy2.Id);
                            plusId++;
                            if (!dataMapping.ContainsKey(id + plusId)) {
                                fight = new FightData();
                                dataMapping.Add(id + plusId, fight);
                            } else {
                                fight = dataMapping[id + plusId];
                            }
                            fight.Id = id + plusId;
                            fight.Name = areaNames[i] + "-" + enemy1.Name + "*3" + enemy2.Name + "*1";
                            fight.ResourceEnemyIds.Clear();
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy1.Id);
                            fight.ResourceEnemyIds.Add(enemy2.Id);
                            plusId++;

                            if (!enemy2.IsBoss) {
                                fight.Id = id + plusId;
                                fight.Name = areaNames[i] + "-" + enemy1.Name + "*2" + enemy2.Name + "*2";
                                fight.ResourceEnemyIds.Clear();
                                fight.ResourceEnemyIds.Add(enemy1.Id);
                                fight.ResourceEnemyIds.Add(enemy1.Id);
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                plusId++;
                                if (!dataMapping.ContainsKey(id + plusId)) {
                                    fight = new FightData();
                                    dataMapping.Add(id + plusId, fight);
                                } else {
                                    fight = dataMapping[id + plusId];
                                }
                                fight.Id = id + plusId;
                                fight.Name = areaNames[i] + "-" + enemy2.Name + "*1" + enemy1.Name + "*1";
                                fight.ResourceEnemyIds.Clear();
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy1.Id);
                                plusId++;
                                if (!dataMapping.ContainsKey(id + plusId)) {
                                    fight = new FightData();
                                    dataMapping.Add(id + plusId, fight);
                                } else {
                                    fight = dataMapping[id + plusId];
                                }
                                fight.Id = id + plusId;
                                fight.Name = areaNames[i] + "-" + enemy2.Name + "*2" + enemy1.Name + "*1";
                                fight.ResourceEnemyIds.Clear();
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy1.Id);
                                plusId++;
                                if (!dataMapping.ContainsKey(id + plusId)) {
                                    fight = new FightData();
                                    dataMapping.Add(id + plusId, fight);
                                } else {
                                    fight = dataMapping[id + plusId];
                                }
                                fight.Id = id + plusId;
                                fight.Name = areaNames[i] + "-" + enemy2.Name + "*3" + enemy1.Name + "*1";
                                fight.ResourceEnemyIds.Clear();
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy2.Id);
                                fight.ResourceEnemyIds.Add(enemy1.Id);
                                plusId++;
                            }
                        }
                    }
                }
                oldSelGridInt = -1;
//                getData();
                fetchData(searchKeyword);
                this.ShowNotification(new GUIContent("加载完成!请点击保存进行数据持久化!"));
            }

            areaEnemyOnly = GUI.Toggle(new Rect(90, 0, 120, 20), areaEnemyOnly, "只显示区域图战斗");

            if (GUI.Button(new Rect(215, 0, 80, 18), "创建最优数据"))
            {
                JObject writeJson = new JObject();
                int index = 0;
                Debug.Log(showListData.Count + "," + dataMapping.Count);
                foreach(FightData showData in showListData) {
                    showData.Enemys.Clear();
                    if (index == 0) {
                        index++;
                        writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(showData));
                    }
                    writeJson[showData.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(showData));
                }
                Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Fights.json", JsonManager.GetInstance().SerializeObject(writeJson));
                Debug.Log("创建最优数据成功");
            }

            if (GUI.Button(new Rect(300, 0, 80, 18), "创建测试诀要"))
            {
                string path = ExcelEditor.DocsPath + "/测试诀要.xlsx";
                Excel xls = ExcelHelper.LoadExcel(path);
                ExcelTable table = xls.Tables[0];
                List<List<SecretData>> createSecrets = new List<List<SecretData>>() {
                    new List<SecretData>(),
                    new List<SecretData>(),
                    new List<SecretData>(),
                    new List<SecretData>(),
                    new List<SecretData>(),
                    new List<SecretData>()
                };
                SecretData createSecret;
                for (int i = 2; i <= table.NumberOfRows; i++)
                {
                    createSecret = new SecretData();
                    createSecret.Type = (SecretType)int.Parse(table.GetValue(i, 2).ToString());
                    createSecret.Quality = (QualityType)int.Parse(table.GetValue(i, 3).ToString());
                    createSecrets[int.Parse(table.GetValue(i, 1).ToString())].Add(createSecret);
                }
                Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "TestSecrets.json", JsonManager.GetInstance().SerializeObject(createSecrets));
                Debug.Log("创建测试诀要成功");
            }

            try {
                hostWeaponLv = int.Parse(EditorGUI.TextField(new Rect(385, 0, 30, 18), hostWeaponLv.ToString()));
            }
            catch{
                hostWeaponLv = 0;
            }
            if (GUI.Button(new Rect(420, 0, 100, 18), "修改闪金兵器等级"))
            {
                PlayerPrefs.SetInt("TestHostWeaponLv", hostWeaponLv);
                Debug.Log("修改闪金兵器等级为" + hostWeaponLv);
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
                    isStatic = data.IsStatic;
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
                    GUI.Label(new Rect(150, 20, 40, 18), "静态:");
                    isStatic = EditorGUI.Toggle(new Rect(195, 20, 100, 18), isStatic);

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
                        dropNums[i] = int.Parse(EditorGUI.TextField(new Rect(250, 80 + i * 40, 60, 18), dropNums[i].ToString()));
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
                            data.IsStatic = isStatic;
							writeDataToJson();
							oldSelGridInt = -1;
							getData();
							fetchData(searchKeyword);
							this.ShowNotification(new GUIContent("修改成功"));
						}
						if (GUI.Button(new Rect(85, 460, 80, 36), "删除")) {
							willDelete = true;
						}
						if (GUI.Button(new Rect(170, 460, 80, 18), "预览")) {
							PlayerPrefs.SetString("FightEditorCurrentId", data.Id);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex0", testRoleIdIndex0);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex1", testRoleIdIndex1);
							PlayerPrefs.SetInt("FightEditorTestRoleIdIndex2", testRoleIdIndex2);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex3", testRoleIdIndex3);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex4", testRoleIdIndex4);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex5", testRoleIdIndex5);
							PlayerPrefs.SetString("FightEditorTestRoleId0", roles[testRoleIdIndex0].Id);
							PlayerPrefs.SetString("FightEditorTestRoleId1", roles[testRoleIdIndex1].Id);
							PlayerPrefs.SetString("FightEditorTestRoleId2", roles[testRoleIdIndex2].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId3", roles[testRoleIdIndex3].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId4", roles[testRoleIdIndex4].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId5", roles[testRoleIdIndex5].Id);
							EditorApplication.isPlaying = true;
						}
                        if (GUI.Button(new Rect(170, 480, 80, 18), "演算"))
                        {
                            PlayerPrefs.SetString("FightEditorCurrentId", data.Id);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex0", testRoleIdIndex0);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex1", testRoleIdIndex1);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex2", testRoleIdIndex2);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex3", testRoleIdIndex3);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex4", testRoleIdIndex4);
                            PlayerPrefs.SetInt("FightEditorTestRoleIdIndex5", testRoleIdIndex5);
                            PlayerPrefs.SetString("FightEditorTestRoleId0", roles[testRoleIdIndex0].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId1", roles[testRoleIdIndex1].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId2", roles[testRoleIdIndex2].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId3", roles[testRoleIdIndex3].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId4", roles[testRoleIdIndex4].Id);
                            PlayerPrefs.SetString("FightEditorTestRoleId5", roles[testRoleIdIndex5].Id);

                            List<RoleData> roleDatas = new List<RoleData>();
//                            List<List<SecretData>> secrets = new List<List<SecretData>>();
                            TextAsset asset = Resources.Load<TextAsset>("Data/Json/TestSecrets");
                            List<List<SecretData>> secrets = JsonManager.GetInstance().DeserializeObject<List<List<SecretData>>>(asset.text);
                            if(!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId0"))) {
                                RoleData hostData = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId0"));
                                hostData.CurrentWeaponLV = PlayerPrefs.GetInt("TestHostWeaponLv");
                                roleDatas.Add(hostData);
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[0].ResourceBookDataIds));
                            }
                            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId1"))) {
                                roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId1")));
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[1].ResourceBookDataIds));
                            }
                            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId2"))) {
                                roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId2")));
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[2].ResourceBookDataIds));
                            }
                            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId3"))) {
                                roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId3")));
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[3].ResourceBookDataIds));
                            }
                            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId4"))) {
                                roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId4")));
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[4].ResourceBookDataIds));
                            }
                            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("FightEditorTestRoleId5"))) {
                                roleDatas.Add(JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", PlayerPrefs.GetString("FightEditorTestRoleId5")));
//                                secrets.Add(DbManager.Instance.GetSecretsBelongBooks(roleDatas[5].ResourceBookDataIds));
                            }
                            FightData fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", PlayerPrefs.GetString("FightEditorCurrentId"));
                            fightData.MakeJsonToModel();
                            BattleLogic.Instance.Init(roleDatas, secrets, fightData.Enemys);
                            BattleLogic.Instance.AutoFight = true;
                            while (!BattleLogic.Instance.IsFail() && !BattleLogic.Instance.IsWin()) {
                                BattleLogic.Instance.Action();
                            }
                            List<BattleProcess> processes = new List<BattleProcess>();
                            while (BattleLogic.Instance.GetProcessCount() > 0) {
                                processes.Add(BattleLogic.Instance.PopProcess());
                            }
                            Debug.Log("过程:");
                            for (int i = 0, len = processes.Count; i < len; i++)
                            {
                                Debug.Log(processes[i].Result);
                            }
                            Debug.Log("结果:");
                            RoleData enemyData;
                            string plusStr;
                            for (int i = 0, len = BattleLogic.Instance.EnemysData.Count; i < len; i++)
                            {
                                enemyData = BattleLogic.Instance.EnemysData[i];
                                Debug.Log(string.Format("{0}{1}气血{2}: {3}% [{4},{5}], {6}", ("(" + enemyData.Id + ")" + (enemyData.HP > 0 ? "" : "[<color=\"#FF0000\">阵亡</color>]")), enemyData.Name, enemyData.HP, (int)(enemyData.HPRate * 100), enemyData.Weapon.Name, enemyData.GetCurrentBook().Name, enemyData.PhysicsDefense));
                                plusStr = "";
                                if (enemyData.AttackSpeedPlus != 0)
                                {
                                    plusStr += " 攻速增量" + enemyData.AttackSpeedPlus;
                                }
                                if (enemyData.DamageRatePlus != 0)
                                {
                                    plusStr += " 伤害比例增量" + enemyData.DamageRatePlus;
                                }
                                if (enemyData.DodgePlus != 0)
                                {
                                    plusStr += " 轻功增量" + enemyData.DodgePlus;
                                }
                                if (enemyData.FixedDamagePlus != 0)
                                {
                                    plusStr += " 固定伤害增量" + enemyData.FixedDamagePlus;
                                }
                                if (enemyData.HurtCutRatePlus != 0)
                                {
                                    plusStr += " 减伤比例增量" + enemyData.HurtCutRatePlus;
                                }
                                if (enemyData.MagicAttackPlus != 0)
                                {
                                    plusStr += " 内功增量" + enemyData.MagicAttackPlus;
                                }
                                if (enemyData.MagicDefensePlus != 0)
                                {
                                    plusStr += " 内防增量" + enemyData.MagicDefensePlus;
                                }
                                if (enemyData.MaxHPPlus != 0)
                                {
                                    plusStr += " 最大气血增量" + enemyData.MaxHPPlus;
                                }
                                if (enemyData.PhysicsAttackPlus != 0)
                                {
                                    plusStr += " 外功增量" + enemyData.PhysicsAttackPlus;
                                }
                                if (enemyData.PhysicsDefensePlus != 0)
                                {
                                    plusStr += " 外防增量" + enemyData.PhysicsDefensePlus;
                                }
                                if (enemyData.DrugResistance != 0)
                                {
                                    plusStr += " 中毒抵抗" + enemyData.DrugResistance;
                                }
                                if (enemyData.DisarmResistance != 0)
                                {
                                    plusStr += " 缴械抵抗" + enemyData.DisarmResistance;
                                }
                                if (enemyData.VertigoResistance != 0)
                                {
                                    plusStr += " 眩晕抵抗" + enemyData.VertigoResistance;
                                }
                                if (enemyData.CanNotMoveResistance != 0)
                                {
                                    plusStr += " 定身抵抗" + enemyData.CanNotMoveResistance;
                                }
                                if (enemyData.SlowResistance != 0)
                                {
                                    plusStr += " 迟缓抵抗" + enemyData.SlowResistance;
                                }
                                if (enemyData.ChaosResistance != 0)
                                {
                                    plusStr += " 混乱抵抗" + enemyData.ChaosResistance;
                                }
                                if (enemyData.ImmortalNum != 0)
                                {
                                    plusStr += " 复活次数" + enemyData.ImmortalNum;
                                }

                                if (plusStr != "")
                                {
                                    Debug.Log("    +" + enemyData.Name + "的加成:" + plusStr);
                                }
                            }
                            RoleData teamData;
                            for (int i = 0, len = BattleLogic.Instance.TeamsData.Count; i < len; i++)
                            {
                                teamData = BattleLogic.Instance.TeamsData[i];
                                string bookNames = "";
                                for (int j = 0, len2 =teamData.Books.Count; j < len2; j++)
                                {
                                    bookNames += ("," + teamData.Books[j].Name);
                                }
                                Debug.Log(string.Format("{0}[{1}{2}]{3},{4},{5}", "(" + teamData.Id + ")" + teamData.Name, teamData.Weapon.Name + "(LV" + teamData.Weapon.LV + ")", bookNames, teamData.Dodge, teamData.MagicDefense, teamData.PhysicsAttack));
                                plusStr = "";
                                for (int j = 0, len2 = teamData.Weapon.Buffs.Count; j < len2; j++)
                                {
                                    plusStr += " " + teamData.Weapon.Buffs[j].Type + "," + teamData.Weapon.Buffs[j].Rate + "," + teamData.Weapon.Buffs[j].FloatValue0 + "," + teamData.Weapon.Buffs[j].FloatValue1 + "," + teamData.Weapon.Buffs[j].Timeout;
                                }
                                if (teamData.AttackSpeedPlus != 0)
                                {
                                    plusStr += " 攻速增量" + teamData.AttackSpeedPlus;
                                }
                                if (teamData.DamageRatePlus != 0)
                                {
                                    plusStr += " 伤害比例增量" + teamData.DamageRatePlus;
                                }
                                if (teamData.DodgePlus != 0)
                                {
                                    plusStr += " 轻功增量" + teamData.DodgePlus;
                                }
                                if (teamData.FixedDamagePlus != 0)
                                {
                                    plusStr += " 固定伤害增量" + teamData.FixedDamagePlus;
                                }
                                if (teamData.HurtCutRatePlus != 0)
                                {
                                    plusStr += " 减伤比例增量" + teamData.HurtCutRatePlus;
                                }
                                if (teamData.MagicAttackPlus != 0)
                                {
                                    plusStr += " 内功增量" + teamData.MagicAttackPlus;
                                }
                                if (teamData.MagicDefensePlus != 0)
                                {
                                    plusStr += " 内防增量" + teamData.MagicDefensePlus;
                                }
                                if (teamData.MaxHPPlus != 0)
                                {
                                    plusStr += " 最大气血增量" + teamData.MaxHPPlus;
                                }
                                if (teamData.PhysicsAttackPlus != 0)
                                {
                                    plusStr += " 外功增量" + teamData.PhysicsAttackPlus;
                                }
                                if (teamData.PhysicsDefensePlus != 0)
                                {
                                    plusStr += " 外防增量" + teamData.PhysicsDefensePlus;
                                }
                                if (teamData.DrugResistance != 0)
                                {
                                    plusStr += " 中毒抵抗" + teamData.DrugResistance;
                                }
                                if (teamData.DisarmResistance != 0)
                                {
                                    plusStr += " 缴械抵抗" + teamData.DisarmResistance;
                                }
                                if (teamData.VertigoResistance != 0)
                                {
                                    plusStr += " 眩晕抵抗" + teamData.VertigoResistance;
                                }
                                if (teamData.CanNotMoveResistance != 0)
                                {
                                    plusStr += " 定身抵抗" + teamData.CanNotMoveResistance;
                                }
                                if (teamData.SlowResistance != 0)
                                {
                                    plusStr += " 迟缓抵抗" + teamData.SlowResistance;
                                }
                                if (teamData.ChaosResistance != 0)
                                {
                                    plusStr += " 混乱抵抗" + teamData.ChaosResistance;
                                }
                                if (teamData.ImmortalNum != 0)
                                {
                                    plusStr += " 复活次数" + teamData.ImmortalNum;
                                }
                                if (plusStr != "")
                                {
                                    Debug.Log("    +" + teamData.Name + "的加成:" + plusStr);
                                }
                            }
                            string teamRolePlus = "";
                            if (BattleLogic.Instance.CurrentTeamRole.DrugResistance != 0)
                            {
                                teamRolePlus += " 中毒抵抗" + BattleLogic.Instance.CurrentTeamRole.DrugResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.DisarmResistance != 0)
                            {
                                teamRolePlus += " 缴械抵抗" + BattleLogic.Instance.CurrentTeamRole.DisarmResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.VertigoResistance != 0)
                            {
                                teamRolePlus += " 眩晕抵抗" + BattleLogic.Instance.CurrentTeamRole.VertigoResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.CanNotMoveResistance != 0)
                            {
                                teamRolePlus += " 定身抵抗" + BattleLogic.Instance.CurrentTeamRole.CanNotMoveResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.SlowResistance != 0)
                            {
                                teamRolePlus += " 迟缓抵抗" + BattleLogic.Instance.CurrentTeamRole.SlowResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.ChaosResistance != 0)
                            {
                                teamRolePlus += " 混乱抵抗" + BattleLogic.Instance.CurrentTeamRole.ChaosResistance;
                            }
                            if (BattleLogic.Instance.CurrentTeamRole.ImmortalNum != 0)
                            {
                                teamRolePlus += " 复活次数" + BattleLogic.Instance.CurrentTeamRole.ImmortalNum;
                            }
                            if (teamRolePlus != "")
                            {
                                Debug.Log("本方总加成: " + teamRolePlus);
                            }

                            if (BattleLogic.Instance.IsWin()) {
                                Debug.Log("<color=\"#00FF00\">胜利</color>");
                            }
                            if (BattleLogic.Instance.IsFail()) {
                                Debug.Log("<color=\"#FF0000\">失败</color>");
                            }
                            Debug.Log(string.Format("出手比: <color=\"#FF00FF\">{0}</color> : {1}", 
                                processes.FindAll(item => item.Type == BattleProcessType.Attack && item.IsTeam).Count, 
                                processes.FindAll(item => item.Type == BattleProcessType.Attack && !item.IsTeam).Count));
                            Debug.Log(string.Format("命中比: <color=\"#FFFF00\">{0}%</color> : {1}%", 
                                (int)((float)processes.FindAll(item => item.Type == BattleProcessType.Attack && item.IsTeam && !item.IsMissed).Count / (float)processes.FindAll(item => item.Type == BattleProcessType.Attack && item.IsTeam).Count * 100), 
                                (int)((float)processes.FindAll(item => item.Type == BattleProcessType.Attack && !item.IsTeam && !item.IsMissed).Count / (float)processes.FindAll(item => item.Type == BattleProcessType.Attack && !item.IsTeam).Count * 100)));
                            Debug.Log(string.Format("本方剩余气血{0}/{1}: <color=\"#00FFFF\">{2}%</color>", BattleLogic.Instance.CurrentTeamRole.HP, BattleLogic.Instance.CurrentTeamRole.MaxHP, (int)(BattleLogic.Instance.CurrentTeamRole.HPRate * 100)));
                        }
						testRoleIdIndex0 = EditorGUI.Popup(new Rect(255, 460, 90, 18), testRoleIdIndex0, roleNames.ToArray());
						testRoleIdIndex1 = EditorGUI.Popup(new Rect(360, 460, 90, 18), testRoleIdIndex1, roleNames.ToArray());
						testRoleIdIndex2 = EditorGUI.Popup(new Rect(465, 460, 90, 18), testRoleIdIndex2, roleNames.ToArray());
                        testRoleIdIndex3 = EditorGUI.Popup(new Rect(255, 480, 90, 18), testRoleIdIndex3, roleNames.ToArray());
                        testRoleIdIndex4 = EditorGUI.Popup(new Rect(360, 480, 90, 18), testRoleIdIndex4, roleNames.ToArray());
                        testRoleIdIndex5 = EditorGUI.Popup(new Rect(465, 480, 90, 18), testRoleIdIndex5, roleNames.ToArray());
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