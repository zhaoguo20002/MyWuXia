using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Game {
	public class JsonManager {
		static Dictionary<string, JObject> mapping;
        public static string DecderKey = "zyzkmqzg";

		public JsonManager() {
			mapping = new Dictionary<string, JObject>();
		}

		/// <summary>
		/// 根据Json文件名获取到Json文件解析后的JObject对象[固定的目录]
		/// </summary>
		/// <returns>The json.</returns>
		/// <param name="jsonFileName">Json file name.</param>
		/// <param name="fromCache">If set to <c>true</c> from cache.</param>
		public JObject GetJson(string jsonFileName, bool fromCache = true) {
			if (fromCache) {
				if (!mapping.ContainsKey(jsonFileName)) {
					TextAsset jsonText = Resources.Load<TextAsset>("Data/Json/" + jsonFileName);
					if (jsonText != null) {
                        string str = jsonText.text.IndexOf("{") == 0 || jsonText.text.IndexOf("[") == 0 ? jsonText.text : DESStatics.StringDecder(jsonText.text, DecderKey);
						JObject jObj = JObject.Parse(str);
						mapping.Add(jsonFileName, jObj);
						jsonText = null;
					}
				}
				return mapping[jsonFileName];
			}
			else {
                TextAsset jsonText = Resources.Load<TextAsset>("Data/Json/" + jsonFileName);
                string str = jsonText.text.IndexOf("{") == 0 || jsonText.text.IndexOf("[") == 0 ? jsonText.text : DESStatics.StringDecder(jsonText.text, DecderKey);
                return JObject.Parse(str);
			}
		}

		/// <summary>
		/// 根据Json文件名和键命获取Json数据的值
		/// </summary>
		/// <returns>The mapping.</returns>
		/// <param name="jsonFileName">Json file name.</param>
		/// <param name="key">Key.</param>
		public T GetMapping<T>(string jsonFileName, string key) {
			JObject jObj = GetJson(jsonFileName);
			key = key == null ? "" : key;
			if (jObj[key] != null) {
				return jObj[key].ToObject<T>();
			}
			else {
				return jObj["0"].ToObject<T>();
			}
		}

		/// <summary>
		/// 递归处理Json里的Vector
		/// </summary>
		/// <returns>The vector.</returns>
		/// <param name="obj">Object.</param>
		JToken dealVector(JToken obj) {
			//过滤Vector
			if (obj.Type == JTokenType.Object) {
				JObject _obj = (JObject)obj;
				if (obj["normalized"] != null || obj["magnitude"] != null || obj["sqrMagnitude"] != null) {
					JToken x = null, y = null, z = null;
					if (obj["x"] != null) {
						x = obj["x"];
					}
					if (obj["y"] != null) {
						y = obj["y"];
					}
					if (obj["z"] != null) {
						z = obj["z"];
					}
					_obj.ClearItems();
					if (x != null) {
						_obj["x"] = x;
					}
					if (y != null) {
						_obj["y"] = y;
					}
					if (z != null) {
						_obj["z"] = z;
					}
					return (JToken)_obj;
				}
				else {
					foreach (var child in _obj) {
						_obj[child.Key] = dealVector(_obj[child.Key]);
					}
				}
			}
			else if (obj.Type == JTokenType.Array) {
				JArray arr = (JArray)obj;
				//递归数组
				for(int i = 0; i < arr.Count; i++) {
					arr[i] = dealVector((JToken)arr[i]);
				}
				return (JToken)arr;
			}
			return obj;
		}
		/// <summary>
		/// 序列化实体类
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="value">Value.</param>
		public string SerializeObject(object value) {
			return JsonConvert.SerializeObject(value, new StringEnumConverter());
		}

		/// <summary>
		/// 序列化实体类[处理Vector后的json格式使其更小]
		/// </summary>
		/// <returns>The object deal vector.</returns>
		/// <param name="value">Value.</param>
		public string SerializeObjectDealVector(object value) {
			JObject getJson = JObject.Parse(SerializeObject(value));
			foreach (var obj in getJson) {
				getJson[obj.Key] = dealVector(getJson[obj.Key]);
			}
			return getJson.ToString();
		}

		/// <summary>
		/// 反序列化Json到实体类
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T DeserializeObject<T>(string value) {
			return JsonConvert.DeserializeObject<T>(value, new StringEnumConverter());
		}

		/// <summary>
		/// 清空全部缓存数据
		/// </summary>
		public void Clear() {
			mapping.Clear();
		}

		static JsonManager _instance;
		/// <summary>
		/// 惰性加载JsonManager
		/// </summary>
		/// <returns>The instance.</returns>
		public static JsonManager GetInstance() {
			if (_instance == null) {
				_instance = new JsonManager();
			}
			return _instance;
		}
	}
}

