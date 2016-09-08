#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel;
using System;

namespace GameEditor {
	public class Base {
		/// <summary>
		/// Reads the names.
		/// </summary>
		/// <returns>The names.</returns>
		public static List<string> ReadNames() {
			List<string> temp = new List<string>();
			foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
				if (S.enabled) {
					string name = S.path.Substring(S.path.LastIndexOf('/')+1);
					name = name.Substring(0,name.Length-6);
					temp.Add(name);
					Debug.LogWarning(S.path);
				}
			}
			return temp;
		}

		/// <summary>
		/// Tops the view.
		/// </summary>
		public static void TopView() {
			SetView(new Vector3(90, 0, 0), Vector3.zero);
		}

		/// <summary>
		/// Sets the view.
		/// </summary>
		/// <param name="rota">Rota.</param>
		public static void SetView(Vector3 rota, Vector3 pos, float size = 10, bool orthographic = true) {
			SceneView.lastActiveSceneView.rotation = Quaternion.Euler(rota.x, rota.y, rota.z);
			SceneView.lastActiveSceneView.pivot = pos;
			SceneView.lastActiveSceneView.size = size; // unit
			SceneView.lastActiveSceneView.orthographic = orthographic; // or false
		}

		/// <summary>
		/// Creates the file.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="fileName">File name.</param>
		/// <param name="text">Text.</param>
		public static void CreateFile(string path, string fileName, string text) {
			StreamWriter sw;
			FileInfo file = new FileInfo(path + "//" + fileName);
//			if (!file.Exists) {
			sw = file.CreateText();
//			}
//			else {
//				sw = file.AppendText(); 
//			}
			sw.Write(text);
			sw.Close();
			sw.Dispose();
			AssetDatabase.Refresh();
		}

		public static Dictionary<string, Texture> IconTextureMappings;
		public static List<string> IconNames;
		public static Dictionary<string, int> IconIdIndexs;
		public static List<ResourceSrcData> Icons;

//		public static Dictionary<string, Texture> HalfBodyTextureMappings;
//		public static List<string> HalfBodyNames;
//		public static Dictionary<string, int> HalfBodyIdIndexs;
//		public static List<ResourceSrcData> HalfBodys;
//
//		public static List<string> BookNames;
//		public static Dictionary<string, int> BookIdIndexs;
//		public static List<BookData> Books;
//
//		public static List<string> WeaponNames;
//		public static Dictionary<string, int> WeaponIdIndexs;
//		public static List<WeaponData> Weapons;

		public static List<OccupationType> OccupationTypeEnums;
		public static List<string> OccupationTypeStrs;
		public static Dictionary<OccupationType, int> OccupationTypeIndexMapping;

		public static List<GenderType> GenderTypeEnums;
		public static List<string> GenderTypeStrs;
		public static Dictionary<GenderType, int> GenderTypeIndexMapping;

		public static List<ResourceType> ResourceTypeEnums;
		public static List<string> ResourceTypeStrs;
		public static Dictionary<ResourceType, int> ResourceTypeIndexMapping;

		public static List<NpcType> NpcTypeEnums;
		public static List<string> NpcTypeStrs;
		public static Dictionary<NpcType, int> NpcTypeIndexMapping;

		public static List<string> SoundNames;
		public static Dictionary<string, int> SoundIdIndexs;
		public static List<SoundData> Sounds;

		public static List<string> ItemDataNames;
		public static Dictionary<string, int> ItemDataIdIndexs;
		public static List<ItemData> ItemDatas;

		public static List<string> TimeNames;

		public static List<string> AllAreaSceneNames;
		public static Dictionary<string, int> AllAreaSceneNameIndexesMapping;

		public static List<string> AllCitySceneNames;
		public static Dictionary<string, int> AllCitySceneIdIndexs;
		public static List<SceneData> AllCityScenes;

		public static void InitParams(string iconKey = "头像-") { 
			JsonManager.GetInstance().Clear();
			//加载全部的icon对象
			IconTextureMappings = new Dictionary<string, Texture>();
			IconNames = new List<string>();
			IconIdIndexs = new Dictionary<string, int>();
			Icons = new List<ResourceSrcData>();
			int index = 0;
			JObject obj = JsonManager.GetInstance().GetJson("Icons", false);
			ResourceSrcData iconData;
			GameObject iconPrefab;
			foreach(var item in obj) {
				if (item.Key != "0") {
					iconData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
					if (iconKey != "" && iconData.Name.IndexOf(iconKey) < 0) {
						continue;
					}
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
					IconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
					MonoBehaviour.DestroyImmediate(iconPrefab);
                    IconNames.Add(iconData.Name + "[" + iconData.Id + "]");
					IconIdIndexs.Add(iconData.Id, index);
					Icons.Add(iconData);
					index++;
				}
			}
//			//获取全部的半身像对象
//			HalfBodyTextureMappings = new Dictionary<string, Texture>();
//			HalfBodyNames = new List<string>();
//			HalfBodyIdIndexs = new Dictionary<string, int>();
//			HalfBodys = new List<ResourceSrcData>();
//			index = 0;
//			obj = JsonManager.GetInstance().GetJson("HalfBodys", false);
//			ResourceSrcData HalfBodyData;
//			GameObject HalfBodyPrefab;
//			foreach(var item in obj) {
//				if (item.Key != "0") {
//					HalfBodyData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
//					HalfBodyPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("HalfBodys", HalfBodyData.Id).Src);
//					HalfBodyTextureMappings.Add(HalfBodyData.Id, HalfBodyPrefab.GetComponent<Image>().sprite.texture);
//					MonoBehaviour.DestroyImmediate(HalfBodyPrefab);
//					HalfBodyNames.Add(HalfBodyData.Name);
//					HalfBodyIdIndexs.Add(HalfBodyData.Id, index);
//					HalfBodys.Add(HalfBodyData);
//					index++;
//				}
//			}
//
//			BookNames = new List<string>() { "无" };
//			BookIdIndexs = new Dictionary<string, int>();
//			BookIdIndexs.Add("", 0);
//			Books = new List<BookData>() { null };
//			index = 1;
//			obj = JsonManager.GetInstance().GetJson("Books", false);
//			BookData BookData;
//			foreach(var item in obj) {
//				if (item.Key != "0") {
//					BookData = JsonManager.GetInstance().DeserializeObject<BookData>(item.Value.ToString());
//					BookNames.Add(BookData.Name);
//					BookIdIndexs.Add(BookData.Id, index);
//					Books.Add(BookData);
//					index++;
//				}
//			}
//
//			WeaponNames = new List<string>();
//			WeaponIdIndexs = new Dictionary<string, int>();
//			Weapons = new List<WeaponData>();
//			index = 0;
//			obj = JsonManager.GetInstance().GetJson("Weapons", false);
//			WeaponData WeaponData;
//			foreach(var item in obj) {
//				if (item.Key != "0") {
//					WeaponData = JsonManager.GetInstance().DeserializeObject<WeaponData>(item.Value.ToString());
//					WeaponNames.Add(WeaponData.Name);
//					WeaponIdIndexs.Add(WeaponData.Id, index);
//					Weapons.Add(WeaponData);
//					index++;
//				}
//			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的OccupationType枚举类型
			 OccupationTypeEnums = new List<OccupationType>();
			 OccupationTypeStrs = new List<string>();
			 OccupationTypeIndexMapping = new Dictionary<OccupationType, int>();
			index = 0;
			foreach(OccupationType type in Enum.GetValues(typeof(OccupationType))) {
				 OccupationTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				 OccupationTypeStrs.Add(attrib.Description);
				 OccupationTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的GenderType枚举类型
			 GenderTypeEnums = new List<GenderType>();
			 GenderTypeStrs = new List<string>();
			 GenderTypeIndexMapping = new Dictionary<GenderType, int>();
			index = 0;
			foreach(GenderType type in Enum.GetValues(typeof(GenderType))) {
				 GenderTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				 GenderTypeStrs.Add(attrib.Description);
				 GenderTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的GenderType枚举类型
			ResourceTypeEnums = new List<ResourceType>();
			ResourceTypeStrs = new List<string>();
			ResourceTypeIndexMapping = new Dictionary<ResourceType, int>();
			index = 0;
			foreach(ResourceType type in Enum.GetValues(typeof(ResourceType))) {
				ResourceTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				ResourceTypeStrs.Add(attrib.Description);
				ResourceTypeIndexMapping.Add(type, index);
				index++;
			}

			NpcTypeEnums = new List<NpcType>();
			NpcTypeStrs = new List<string>();
			NpcTypeIndexMapping = new Dictionary<NpcType, int>();
			index = 0;
			foreach(NpcType type in Enum.GetValues(typeof(NpcType))) {
				NpcTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				NpcTypeStrs.Add(attrib.Description);
				NpcTypeIndexMapping.Add(type, index);
				index++;
			}

			SoundNames = new List<string>();
			SoundIdIndexs = new Dictionary<string, int>();
			Sounds = new List<SoundData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("Sounds", false);
			SoundData SoundData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					SoundData = JsonManager.GetInstance().DeserializeObject<SoundData>(item.Value.ToString());
					SoundNames.Add(SoundData.Name);
					SoundIdIndexs.Add(SoundData.Id, index);
					Sounds.Add(SoundData);
					index++;
				}
			}

			ItemDataNames = new List<string>();
			ItemDataIdIndexs = new Dictionary<string, int>();
			ItemDatas = new List<ItemData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("ItemDatas", false);
			ItemData ItemData;
			foreach(var item in obj) {
				if (item.Key != "0") {
					ItemData = JsonManager.GetInstance().DeserializeObject<ItemData>(item.Value.ToString());
					ItemDataNames.Add(ItemData.Name);
					ItemDataIdIndexs.Add(ItemData.Id, index);
					ItemDatas.Add(ItemData);
					index++;
				}
			}

			AllCitySceneIdIndexs = new Dictionary<string, int>();
			AllCitySceneNames = new List<string>();
			AllCityScenes = new List<SceneData>();
			obj = JsonManager.GetInstance().GetJson("Scenes", false);
			SceneData sceneData;
			index = 0;
			foreach(var item in obj) {
				if (item.Key != "0") {
					sceneData = JsonManager.GetInstance().DeserializeObject<SceneData>(item.Value.ToString());
					AllCitySceneNames.Add(sceneData.Name);
					AllCitySceneIdIndexs.Add(sceneData.Id, index);
					AllCityScenes.Add(sceneData);
					index++;
				}
			}

			TimeNames = new List<string>() { "午时", "未时", "申时", "酉时", "戌时", "亥时", "子时", "丑时", "寅时", "卯时", "辰时", "巳时" };

			AllAreaSceneNames = new List<string>();
			AllAreaSceneNameIndexesMapping = new Dictionary<string, int>();
			string[] fen;
			string sceneName;
			index = 0;
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
				fen = scene.path.Split(new char[] { '/' });
				if (fen.Length >= 3) {
					sceneName = fen[2];
					if (sceneName.IndexOf("Area") == 0) {
						sceneName = sceneName.Replace(".unity", "");
						AllAreaSceneNames.Add(sceneName);
						AllAreaSceneNameIndexesMapping.Add(sceneName, index);
						index++;
					}
				}
			}
		}

		public static void DestroyParams() {
			IconTextureMappings.Clear();
			IconNames.Clear();
			IconIdIndexs.Clear();
			Icons.Clear();

//			HalfBodyTextureMappings.Clear();
//			HalfBodyNames.Clear();
//			HalfBodyIdIndexs.Clear();
//			HalfBodys.Clear();
//
//			BookNames.Clear();
//			BookIdIndexs.Clear();
//			Books.Clear();
//
//			WeaponNames.Clear();
//			WeaponIdIndexs.Clear();
//			Weapons.Clear();

			OccupationTypeEnums.Clear();
			OccupationTypeStrs.Clear();
			OccupationTypeIndexMapping.Clear();

			GenderTypeEnums.Clear();
			GenderTypeStrs.Clear();
			GenderTypeIndexMapping.Clear();

			SoundNames.Clear();
			SoundIdIndexs.Clear();
			Sounds.Clear();

			ItemDataNames.Clear();
			ItemDataIdIndexs.Clear();
			ItemDatas.Clear();

			TimeNames.Clear();

			AllAreaSceneNameIndexesMapping.Clear();
			AllAreaSceneNames.Clear();
		}
	}
}
#endif