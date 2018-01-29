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
	public class RoleDatasEditorWindow : EditorWindow {

		static RoleDatasEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Role Datas Editor")]
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
				window = (RoleDatasEditorWindow)EditorWindow.GetWindowWithRect(typeof(RoleDatasEditorWindow), size, true, "角色数据编辑器");
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

		static Dictionary<string, Texture> halfBodyTextureMappings;
		static List<string> halfBodyNames;
		static Dictionary<string, int> halfBodyIdIndexs;
		static List<ResourceSrcData> halfBodys;

		static List<string> bookNames;
		static Dictionary<string, int> bookIdIndexs;
		static List<BookData> books;

		static List<string> weaponNames;
		static Dictionary<string, int> weaponIdIndexs;
		static List<WeaponData> weapons;

		static List<OccupationType> occupationTypeEnums;
		static List<string> occupationTypeStrs;
		static Dictionary<OccupationType, int> occupationTypeIndexMapping;

		static List<GenderType> genderTypeEnums;
		static List<string> genderTypeStrs;
		static Dictionary<GenderType, int> genderTypeIndexMapping;

		static List<string> soundNames;
		static Dictionary<string, int> soundIdIndexs;
		static List<SoundData> sounds;

		static List<string> allCitySceneNames;
		static Dictionary<string, int> allCitySceneIdIndexs;
		static List<SceneData> allCityScenes;

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
					if (iconData.Name.IndexOf("头像-") < 0) {
						continue;
					}
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
					iconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(iconPrefab);
                    iconNames.Add(iconData.Name + "[" + iconData.Id + "]");
					iconIdIndexs.Add(iconData.Id, index);
					icons.Add(iconData);
					index++;
				}
			}
			//获取全部的半身像对象
			halfBodyTextureMappings = new Dictionary<string, Texture>();
			halfBodyNames = new List<string>();
			halfBodyIdIndexs = new Dictionary<string, int>();
			halfBodys = new List<ResourceSrcData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("HalfBodys", false);
			ResourceSrcData halfBodyData;
			GameObject halfBodyPrefab;
			foreach(var item in obj) {
				if (item.Key != "0") {
					halfBodyData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
					halfBodyPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("HalfBodys", halfBodyData.Id).Src);
					halfBodyTextureMappings.Add(halfBodyData.Id, halfBodyPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(halfBodyPrefab);
					halfBodyNames.Add(halfBodyData.Name);
					halfBodyIdIndexs.Add(halfBodyData.Id, index);
					halfBodys.Add(halfBodyData);
					index++;
				}
			}

			bookNames = new List<string>() { "无" };
			bookIdIndexs = new Dictionary<string, int>();
			bookIdIndexs.Add("", 0);
			books = new List<BookData>() { null };
			index = 1;
			obj = JsonManager.GetInstance().GetJson("Books", false);
			BookData bookData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					bookData = JsonManager.GetInstance().DeserializeObject<BookData>(item.Value.ToString());
					bookNames.Add(bookData.Name);
					bookIdIndexs.Add(bookData.Id, index);
					books.Add(bookData);
					index++;
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

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的OccupationType枚举类型
			occupationTypeEnums = new List<OccupationType>();
			occupationTypeStrs = new List<string>();
			occupationTypeIndexMapping = new Dictionary<OccupationType, int>();
			index = 0;
			foreach(OccupationType type in Enum.GetValues(typeof(OccupationType))) {
				occupationTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				occupationTypeStrs.Add(attrib.Description);
				occupationTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的GenderType枚举类型
			genderTypeEnums = new List<GenderType>();
			genderTypeStrs = new List<string>();
			genderTypeIndexMapping = new Dictionary<GenderType, int>();
			index = 0;
			foreach(GenderType type in Enum.GetValues(typeof(GenderType))) {
				genderTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				genderTypeStrs.Add(attrib.Description);
				genderTypeIndexMapping.Add(type, index);
				index++;
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
					if (soundData.Name.IndexOf("阵亡-") < 0) {
						continue;
					}
					soundNames.Add(soundData.Name);
					soundIdIndexs.Add(soundData.Id, index);
					sounds.Add(soundData);
					index++;
				}
			}

			allCitySceneIdIndexs = new Dictionary<string, int>();
			allCitySceneNames = new List<string>();
			allCityScenes = new List<SceneData>();
			obj = JsonManager.GetInstance().GetJson("Scenes", false);
			SceneData sceneData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					sceneData = JsonManager.GetInstance().DeserializeObject<SceneData>(item.Value.ToString());
					allCitySceneNames.Add(sceneData.Name);
					allCitySceneIdIndexs.Add(sceneData.Id, index);
					allCityScenes.Add(sceneData);
					index++;
				}
			}
		}

		static Dictionary<string, RoleData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, RoleData>();
			JObject obj = JsonManager.GetInstance().GetJson("RoleDatas", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<RoleData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<RoleData> showListData;
		static List<string> listNames;
		static List<string> allDataNames;
		static List<RoleData> allRoleDatas;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<RoleData>();
			allDataNames = new List<string>(){ "无额外招式" };
			allRoleDatas = new List<RoleData>() { null };
			foreach(RoleData data in dataMapping.Values) {
				allDataNames.Add(data.Name);
				allRoleDatas.Add(data);
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
			List<RoleData> datas = new List<RoleData>();
			foreach(RoleData data in dataMapping.Values) {
				datas.Add(data);
			}
			datas.Sort((a, b) => a.Id.CompareTo(b.Id));
			JObject rolesOfWinShopData = new JObject(); //酒馆侠客静态json数据
			foreach(RoleData data in datas) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				if (data.IsStatic) {
					if (rolesOfWinShopData[data.HometownCityId] == null) {
						rolesOfWinShopData[data.HometownCityId] = new JArray(data.Id);
					}
					else {
						((JArray)rolesOfWinShopData[data.HometownCityId]).Add(data.Id);
					}
					
				}
			}
            Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "RoleDatas.json", DESStatics.StringEncoder(JsonManager.GetInstance().SerializeObject(writeJson)));
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "RoleIdsOfWinShopDatas.json", JsonManager.GetInstance().SerializeObject(rolesOfWinShopData));
		}

		RoleData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string roleName = "";
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int genderTypeIndex = 0;
		int occupationTypeIndex = 0;
		int halfBodyIdIndex = 0;
		int oldHalfBodyIdIndex = -1;
		Texture halfBodyTexture = null;
		string roleDesc = "";
		int hp = 0;
		int maxHp = 0;
		float physicsAttack = 0;
		float physicsDefense = 0;
		float magicAttack = 0;
		float magicDefense = 0;
		float attackSpeed = 1;
		float dodge = 0;
        int lv = 1;
        int difLv4HP = 0;
        int difLv4PhysicsAttack = 0;
        int difLv4PhysicsDefense = 0;
        int difLv4MagicAttack = 0;
        int difLv4MagicDefense = 0;
        int difLv4Dodge = 0;
		List<int> bookDataIdIndexes;
		int weaponDataIdIndex = 0;
		int effectSoundIdIndex = 0;
        bool isImmuneMaxHPReduce;
		bool isStatic;
        bool isKnight;
        bool isBoss;
		int homedownCityIdIndex = 0;

		short toolState; //0正常 1增加 2删除
		string addId = "";
		string addRoleName = "";
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

            GUILayout.BeginArea(new Rect(205, 5, 200, 20));
            if (GUI.Button(new Rect(0, 0, 80, 18), "生成对应简表")) {
                //生成excel
                Excel outputXls = new Excel();
                ExcelTable outputTable = new ExcelTable();
                outputTable.TableName = "战斗角色简表";
                string outputPath = ExcelEditor.DocsPath + "/战斗角色简表.xlsx";
                outputXls.Tables.Add(outputTable);

                outputXls.Tables[0].SetValue(1, 1, "角色id");
                outputXls.Tables[0].SetValue(1, 2, "名称");
                outputXls.Tables[0].SetValue(1, 3, "兵器");
                outputXls.Tables[0].SetValue(1, 4, "秘籍");

                int rowIndex = 2;
                RoleData role;
                string booksStr;
                for (int i = 0, len = allRoleDatas.Count; i < len; i++) {
                    role = allRoleDatas[i];
                    if (role == null) {
                        continue;
                    }
                    outputXls.Tables[0].SetValue(rowIndex, 1, role.Id);
                    outputXls.Tables[0].SetValue(rowIndex, 2, role.Name);
                    outputXls.Tables[0].SetValue(rowIndex, 3, role.ResourceWeaponDataId);
                    booksStr = "";
                    for (int j = 0, len2 = role.ResourceBookDataIds.Count; j < len2; j++) {
                        if (j > 0) {
                            booksStr += "|";
                        }
                        booksStr += role.ResourceBookDataIds[j];
                    }
                    outputXls.Tables[0].SetValue(rowIndex, 4, booksStr);
                    rowIndex++;
                }
                ExcelHelper.SaveExcel(outputXls, outputPath); //生成excel

                Debug.Log("对应简表创建完毕");
            }

            if (GUI.Button(new Rect(85, 0, 80, 18), "加载角色数据")) {
                string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
                Excel xls =  ExcelHelper.LoadExcel(path);
                ExcelTable table = xls.Tables[0];
                List<string> areaNames = new List<string>() { };
                string areaName;
                RoleData friend;
                RoleData enemy;
                for (int i = 1; i <= table.NumberOfRows; i++) {
                    areaName = table.GetValue(i, 1).ToString();
                    if (!string.IsNullOrEmpty(areaName)) {
                        areaNames.Add(areaName);
                    } else {
                        if (!string.IsNullOrEmpty(table.GetValue(i, 2).ToString())) {
                            if (dataMapping.ContainsKey(table.GetValue(i, 2).ToString())) {
                                friend = dataMapping[table.GetValue(i, 2).ToString()];
                                friend.IsKnight = true;
                                friend.IsBoss = false;
                                friend.Id = table.GetValue(i, 2).ToString();
                                friend.Name = table.GetValue(i, 3).ToString();
                                friend.Lv = int.Parse(table.GetValue(i, 4).ToString());
                                friend.DifLv4HP = int.Parse(table.GetValue(i, 7).ToString());
                                friend.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 9).ToString());
                                friend.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 11).ToString());
                                friend.DifLv4MagicAttack = int.Parse(table.GetValue(i, 13).ToString());
                                friend.DifLv4MagicDefense = int.Parse(table.GetValue(i, 15).ToString());
                                friend.DifLv4Dodge = int.Parse(table.GetValue(i, 17).ToString());
                                friend.Desc = table.GetValue(i, 20).ToString(); //记录武功类型 0为外功 1为内功
                                //处理兵器秘籍
//                                if (!string.IsNullOrEmpty(table.GetValue(i, 18).ToString())) {
//                                    friend.ResourceWeaponDataId = table.GetValue(i, 18).ToString();
//                                }
//                                if (!string.IsNullOrEmpty(table.GetValue(i, 19).ToString())) {
//                                    string[] fen = table.GetValue(i, 19).ToString().Split(new char[] { '|' });
//                                    friend.ResourceBookDataIds.Clear();
//                                    foreach (string f in fen) {
//                                        friend.ResourceBookDataIds.Add(f);
//                                    }
//                                }
                                friend.InitAttribute();
                                Debug.Log(friend.Id + "," + friend.Name);
                            }
                        }
                        if (!string.IsNullOrEmpty(table.GetValue(i, 21).ToString())) {
                            if (dataMapping.ContainsKey(table.GetValue(i, 21).ToString())) {
                                enemy = dataMapping[table.GetValue(i, 21).ToString()];
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
//                                //处理兵器秘籍
//                                if (!string.IsNullOrEmpty(table.GetValue(i, 38).ToString())) {
//                                    enemy.ResourceWeaponDataId = table.GetValue(i, 38).ToString();
//                                }
//                                if (!string.IsNullOrEmpty(table.GetValue(i, 39).ToString())) {
//                                    string[] fen = table.GetValue(i, 39).ToString().Split(new char[] { '|' });
//                                    enemy.ResourceBookDataIds.Clear();
//                                    foreach (string f in fen) {
//                                        enemy.ResourceBookDataIds.Add(f);
//                                    }
//                                }
                                enemy.InitAttribute();
                                Debug.Log(enemy.Id + "," + enemy.Name);
                            }
                        }
                    }
                }

                writeDataToJson();
                oldSelGridInt = -1;
                getData();
                fetchData(searchKeyword);
                this.ShowNotification(new GUIContent("数据加载成功"));
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
					roleName = data.Name;
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
					occupationTypeIndex = occupationTypeIndexMapping[data.Occupation];
					genderTypeIndex = genderTypeIndexMapping[data.Gender];
					if (halfBodyIdIndexs.ContainsKey(data.HalfBodyId)) {
						halfBodyIdIndex = halfBodyIdIndexs[data.HalfBodyId];
					}
					else {
						halfBodyIdIndex = 0;
					}
					if (halfBodyTextureMappings.ContainsKey(data.HalfBodyId)) {
						halfBodyTexture = halfBodyTextureMappings[data.HalfBodyId];	
					}
					else {
						halfBodyTexture = null;
					}	
//                    data.InitAttribute();
					roleDesc = data.Desc;
					hp = data.HP;
					maxHp = data.MaxHP;
					physicsAttack = data.PhysicsAttack;
					physicsDefense = data.PhysicsDefense;
					magicAttack = data.MagicAttack;
					magicDefense = data.MagicDefense;
					attackSpeed = data.AttackSpeed;
					dodge = data.Dodge;
                    lv = data.Lv;
                    difLv4HP = data.DifLv4HP;
                    difLv4PhysicsAttack = data.DifLv4PhysicsAttack;
                    difLv4PhysicsDefense = data.DifLv4PhysicsDefense;
                    difLv4MagicAttack = data.DifLv4MagicAttack;
                    difLv4MagicDefense = data.DifLv4MagicDefense;
                    difLv4Dodge = data.DifLv4Dodge;
					bookDataIdIndexes = new List<int>();
					string bookId;
					for(int i = 0; i < 3; i++) {
						bookId = data.ResourceBookDataIds.Count > i ? data.ResourceBookDataIds[i] : "";
						bookDataIdIndexes.Add(bookIdIndexs.ContainsKey(bookId) ? bookIdIndexs[bookId] : 0);
					}
					if (weaponIdIndexs.ContainsKey(data.ResourceWeaponDataId)) {
						weaponDataIdIndex = weaponIdIndexs[data.ResourceWeaponDataId];
					}
					else {
						weaponDataIdIndex = 0;
					}
					effectSoundIdIndex = soundIdIndexs.ContainsKey(data.DeadSoundId) ? soundIdIndexs[data.DeadSoundId] : 0;
                    isImmuneMaxHPReduce = data.IsImmuneMaxHPReduce;
                    isStatic = data.IsStatic;
                    isKnight = data.IsKnight;
                    isBoss = data.IsBoss;
                    showId = data.Id;
					data.HometownCityId = data.HometownCityId == null ? "" : data.HometownCityId;
					homedownCityIdIndex = allCitySceneIdIndexs.ContainsKey(data.HometownCityId) ? allCitySceneIdIndexs[data.HometownCityId] : 0;
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 555));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 50, 50), iconTexture);
					}
					
					GUI.Label(new Rect(55, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(100, 0, 100, 18), showId);
					GUI.Label(new Rect(205, 0, 40, 18), "姓名:");
					roleName = EditorGUI.TextField(new Rect(250, 0, 100, 18), roleName);
					GUI.Label(new Rect(355, 0, 40, 18), "性别:");
					genderTypeIndex = EditorGUI.Popup(new Rect(400, 0, 100, 18), genderTypeIndex, genderTypeStrs.ToArray());
					GUI.Label(new Rect(55, 20, 40, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(100, 20, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(205, 20, 40, 18), "门派:");
					occupationTypeIndex = EditorGUI.Popup(new Rect(250, 20, 100, 18), occupationTypeIndex, occupationTypeStrs.ToArray());
					GUI.Label(new Rect(355, 20, 40, 18), "半身像:");
					halfBodyIdIndex = EditorGUI.Popup(new Rect(400, 20, 100, 18), halfBodyIdIndex, halfBodyNames.ToArray());
					GUI.Label(new Rect(55, 40, 40, 18), "描述:");
					roleDesc = GUI.TextArea(new Rect(100, 40, 400, 60), roleDesc);
					GUI.Label(new Rect(55, 105, 50, 18), "气血:");
					EditorGUI.Slider(new Rect(100, 105, 165, 18), hp, 1, 1000000);
					GUI.Label(new Rect(270, 105, 50, 18), "气血上限:");
					EditorGUI.Slider(new Rect(335, 105, 165, 18), maxHp, 1, 1000000);
					GUI.Label(new Rect(55, 125, 50, 18), "外功:");
					EditorGUI.Slider(new Rect(100, 125, 165, 18), physicsAttack, 0, 100000);
					GUI.Label(new Rect(270, 125, 50, 18), "外防:");
					EditorGUI.Slider(new Rect(335, 125, 165, 18), physicsDefense, 0, 1000000);
					GUI.Label(new Rect(55, 145, 50, 18), "内功:");
					EditorGUI.Slider(new Rect(100, 145, 165, 18), magicAttack, 0, 100000);
					GUI.Label(new Rect(270, 145, 50, 18), "内防:");
					EditorGUI.Slider(new Rect(335, 145, 165, 18), magicDefense, 0, 1000000);
					GUI.Label(new Rect(55, 165, 50, 18), "攻速:");
					attackSpeed = EditorGUI.Slider(new Rect(100, 165, 165, 18), attackSpeed, 0, 50);
					GUI.Label(new Rect(270, 165, 50, 18), "轻功:");
					EditorGUI.Slider(new Rect(335, 165, 165, 18), dodge, 0, 200);
					GUI.Label(new Rect(55, 185, 50, 18), "秘籍:");
					bookDataIdIndexes[0] = EditorGUI.Popup(new Rect(110, 185, 100, 18), bookDataIdIndexes[0], bookNames.ToArray());
					bookDataIdIndexes[1] = EditorGUI.Popup(new Rect(215, 185, 100, 18), bookDataIdIndexes[1], bookNames.ToArray());
					bookDataIdIndexes[2] = EditorGUI.Popup(new Rect(320, 185, 100, 18), bookDataIdIndexes[2], bookNames.ToArray());
					GUI.Label(new Rect(55, 205, 50, 18), "兵器:");
					weaponDataIdIndex = EditorGUI.Popup(new Rect(110, 205, 100, 18), weaponDataIdIndex, weaponNames.ToArray());
					GUI.Label(new Rect(215, 205, 50, 18), "音效:");
					effectSoundIdIndex = EditorGUI.Popup(new Rect(270, 205, 100, 18), effectSoundIdIndex, soundNames.ToArray());
                    GUI.Label(new Rect(215, 225, 100, 18), "免疫气血上限衰减:");
                    isImmuneMaxHPReduce = EditorGUI.Toggle(new Rect(320, 225, 20, 18), isImmuneMaxHPReduce);
                    GUI.Label(new Rect(375, 205, 50, 18), "静态:");
                    isStatic = EditorGUI.Toggle(new Rect(405, 205, 20, 18), isStatic);
                    GUI.Label(new Rect(440, 205, 50, 18), "侠客:");
                    isKnight = EditorGUI.Toggle(new Rect(470, 205, 20, 18), isKnight);
                    GUI.Label(new Rect(375, 225, 50, 18), "Boss:");
                    isBoss = EditorGUI.Toggle(new Rect(405, 225, 20, 18), isBoss);
					GUI.Label(new Rect(55, 225, 50, 18), "故乡:");
					homedownCityIdIndex = EditorGUI.Popup(new Rect(110, 225, 100, 18), homedownCityIdIndex, allCitySceneNames.ToArray());
					if (halfBodyTexture != null) {
						GUI.DrawTexture(new Rect(505, 0, 325, 260), halfBodyTexture);
					}
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
                    GUI.Label(new Rect(55, 245, 50, 18), "等级:");
                    try {
                        lv = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(110, 245, 40, 18), lv.ToString())), 1, 120);
                    }
                    catch(Exception e) {
                        lv = 1;
                    }
                    GUI.Label(new Rect(155, 245, 50, 18), "气血差量:");
                    try {
                        difLv4HP = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(205, 245, 40, 18), difLv4HP.ToString())), -1000, 1000);
                    }
                    catch(Exception e) {
                        difLv4HP = 0;
                    }
                    GUI.Label(new Rect(250, 245, 50, 18), "外功差量:");
                    try {
                        difLv4PhysicsAttack = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(305, 245, 40, 18), difLv4PhysicsAttack.ToString())), -1000, 1000);
                    }
                    catch(Exception e) {
                        difLv4PhysicsAttack = 0;
                    }
                    GUI.Label(new Rect(350, 245, 50, 18), "外防差量:");
                    try {
                        difLv4PhysicsDefense = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(405, 245, 40, 18), difLv4PhysicsDefense.ToString())), -5000, 5000);
                    }
                    catch(Exception e) {
                        difLv4PhysicsDefense = 0;
                    }

                    GUI.Label(new Rect(155, 265, 50, 18), "轻功差量:");
                    try {
                        difLv4Dodge = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(205, 265, 40, 18), difLv4Dodge.ToString())), -200, 200);
                    }
                    catch(Exception e) {
                        difLv4Dodge = 0;
                    }
                    GUI.Label(new Rect(250, 265, 50, 18), "内功差量:");
                    try {
                        difLv4MagicAttack = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(305, 265, 40, 18), difLv4MagicAttack.ToString())), -1000, 1000);
                    }
                    catch(Exception e) {
                        difLv4MagicAttack = 0;
                    }
                    GUI.Label(new Rect(350, 265, 50, 18), "内防差量:");
                    try {
                        difLv4MagicDefense = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(405, 265, 40, 18), difLv4MagicDefense.ToString())), -5000, 5000);
                    }
                    catch(Exception e) {
                        difLv4MagicDefense = 0;
                    }
					if (GUI.Button(new Rect(0, 295, 80, 18), "修改基础属性")) {
						if (roleName == "") {
							this.ShowNotification(new GUIContent("招式名不能为空!"));
							return;
						}
                        data.Id = showId;
						data.Name = roleName;
						data.IconId = icons[iconIndex].Id;
						data.Occupation = occupationTypeEnums[occupationTypeIndex];
						data.Gender = genderTypeEnums[genderTypeIndex];
						data.HalfBodyId = halfBodys[halfBodyIdIndex].Id;
						data.Desc = roleDesc;
						data.HP = hp;
						data.MaxHP = maxHp;
						data.PhysicsAttack = physicsAttack;
						data.PhysicsDefense = physicsDefense;
						data.MagicAttack = magicAttack;
						data.MagicDefense = magicDefense;
                        data.Dodge = dodge;
                        data.AttackSpeed = attackSpeed;
                        data.Lv = lv;
                        data.DifLv4HP = difLv4HP;
                        data.DifLv4PhysicsAttack = difLv4PhysicsAttack;
                        data.DifLv4PhysicsDefense = difLv4PhysicsDefense;
                        data.DifLv4MagicAttack = difLv4MagicAttack;
                        data.DifLv4MagicDefense = difLv4MagicDefense;
                        data.DifLv4Dodge = difLv4Dodge;
						data.HometownCityId = allCityScenes[homedownCityIdIndex].Id;
						data.ResourceBookDataIds.Clear();
						foreach(int bookIdIndex in bookDataIdIndexes) {
							if (bookIdIndex > 0) {
								if (books[bookIdIndex].Occupation == OccupationType.None || books[bookIdIndex].Occupation == data.Occupation) {
                                    if (books[bookIdIndex].LimitWeaponType == WeaponType.None || weapons[weaponDataIdIndex].Type == WeaponType.None || books[bookIdIndex].LimitWeaponType == weapons[weaponDataIdIndex].Type) {
                                        data.ResourceBookDataIds.Add(books[bookIdIndex].Id);
                                    } else {
                                        this.ShowNotification(new GUIContent(string.Format("装备{0}后不能再习练{1}，兵器类型不符!", weapons[weaponDataIdIndex].Name, books[bookIdIndex].Name)));
                                        return;
                                    }
								}
								else {
									this.ShowNotification(new GUIContent(string.Format("秘籍{0}无法装备到{1}身上，门派不符!", books[bookIdIndex].Name, data.Name)));
									return;
								}
							}
						}
						if (weapons[weaponDataIdIndex].Occupation == OccupationType.None || weapons[weaponDataIdIndex].Occupation == data.Occupation) {
							data.ResourceWeaponDataId = weapons[weaponDataIdIndex].Id;
						}
						else {
							this.ShowNotification(new GUIContent(string.Format("兵器{0}无法装备到{1}身上，门派不符!", weapons[weaponDataIdIndex].Name, data.Name)));
							return;
						}
						data.DeadSoundId = sounds[effectSoundIdIndex].Id;
                        data.IsImmuneMaxHPReduce = isImmuneMaxHPReduce;
                        data.IsStatic = isStatic;
                        data.IsKnight = isKnight;
                        data.IsBoss = isBoss;
                        data.InitAttribute();
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
				
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 320, 300, 60));
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
				GUI.Label(new Rect(140, 0, 60, 18), "角色名:");
				addRoleName = EditorGUI.TextField(new Rect(205, 0, 100, 18), addRoleName);
				if (GUI.Button(new Rect(0, 20, 60, 18), "确定添加")) {
					toolState = 0;
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addRoleName == "") {
						this.ShowNotification(new GUIContent("角色姓名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					RoleData addRoleData = new RoleData();
					addRoleData.Id = addId;
					addRoleData.Name = addRoleName;	
                    ResourceSrcData findIcon = icons.Find(item => item.Name.IndexOf(addRoleData.Name) >= 0);
                    if (findIcon != null) {
                        addRoleData.IconId = findIcon.Id;
                    }
                    ResourceSrcData findHalfBodyIcon = halfBodys.Find(item => item.Name.IndexOf(addRoleData.Name) >= 0);
                    if (findHalfBodyIcon != null) {
                        addRoleData.HalfBodyId = findHalfBodyIcon.Id;
                    }
					dataMapping.Add(addId, addRoleData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
//					addId = "";
					addRoleName = "";
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