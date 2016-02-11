using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 请求用户基础信息
		/// </summary>
		public static string CallUserData;
		/// <summary>
		/// 更新用户基础信息
		/// </summary>
		public static string UpdateUserData;
		/// <summary>
		/// 请求用户基础信息回调
		/// </summary>
		public static string CallUserDataEcho;
		/// <summary>
		/// 更改大地图的状态信息
		/// </summary>
		public static string UpdateUserDataAreaInfo;
		/// <summary>
		/// 播放背景音乐
		/// </summary>
		public static string PlayBgm;
	}
	public partial class NotifyRegister {
		static System.Action<UserData> callUserDataCallback = null;
		/// <summary>
		/// 用户相关消息
		/// </summary>
		public static void UserNotifyInit() {
			Messenger.AddListener<System.Action<UserData>>(NotifyTypes.CallUserData, (callback) => {
				callUserDataCallback = callback;
				DbManager.Instance.CallUserData();
			});

			Messenger.AddListener<System.Action<UserData>>(NotifyTypes.UpdateUserData, (callback) => {
				if (UserModel.CurrentUserData != null) {
					callUserDataCallback = callback;
					DbManager.Instance.UpdateUserData(JsonManager.GetInstance().SerializeObjectDealVector(UserModel.CurrentUserData));	
				}
			});

			Messenger.AddListener<JObject>(NotifyTypes.CallUserDataEcho, (obj) => {
				if (callUserDataCallback != null) {
					JArray data = (JArray)obj["data"];
					UserModel.CurrentUserData = JsonManager.GetInstance().DeserializeObject<UserData>(data[1].ToString());
					UserModel.CurrentUserData.Id = data[0].ToString();
					callUserDataCallback(UserModel.CurrentUserData);
					callUserDataCallback = null;
				}
			});

			Messenger.AddListener<string, int, int>(NotifyTypes.UpdateUserDataAreaInfo, (areaName, x, y) => {
				if (UserModel.CurrentUserData != null) {
					UserModel.CurrentUserData.PositionStatu = UserPositionStatusType.InArea;
					UserModel.CurrentUserData.CurrentAreaSceneName = areaName;
					UserModel.CurrentUserData.CurrentAreaX = x;
					UserModel.CurrentUserData.CurrentAreaY = y;
				}
			});

			Messenger.AddListener(NotifyTypes.PlayBgm, () => {
				if (UserModel.CurrentUserData != null) {
					switch (UserModel.CurrentUserData.PositionStatu) {
					case UserPositionStatusType.InArea:
						if (AreaModel.AreaMainScript) {
							AreaModel.AreaMainScript.PlayBgm();
						}
						break;
					case UserPositionStatusType.InCity:

						break;
					default:
						break;
					}
				}
			});
		}
	}
}