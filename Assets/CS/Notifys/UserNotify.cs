using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
		/// 更新当前城镇状态信息
		/// </summary>
		public static string UpdateUserDataCityInfo;
		/// <summary>
		/// 更新当前大地图的坐标
		/// </summary>
		public static string UpdateUserDataAreaPos;
		/// <summary>
		/// 播放背景音乐
		/// </summary>
		public static string PlayBgm;
		/// <summary>
		/// 进入游戏消息
		/// </summary>
		public static string EnterGame;
		/// <summary>
		/// 打开创建角色界面
		/// </summary>
		public static string ShowCreateHostRolePanel;
		/// <summary>
		/// 创建角色
		/// </summary>
		public static string CreateHostRole;
		/// <summary>
		/// 查询游戏记录数据
		/// </summary>
		public static string GetRecordListData;
		/// <summary>
		/// 查询游戏记录数据回调
		/// </summary>
		public static string GetRecordListDataEcho;
		/// <summary>
		/// 打开主界面
		/// </summary>
		public static string ShowMainPanel;
		/// <summary>
		/// 打开设置界面
		/// </summary>
		public static string ShowSettingPanel;
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
					UserModel.CurrentUserData.AreaFood.Num = (int)data[2];
					UserModel.CurrentUserData.TimeAngle = (float)data[3];
					UserModel.CurrentUserData.TimeTicks = (long)data[4];
					FramePanelCtrl.TimePlay(UserModel.CurrentUserData.TimeAngle, UserModel.CurrentUserData.TimeTicks); //初始化时辰时间戳
					callUserDataCallback(UserModel.CurrentUserData);
					callUserDataCallback = null;
				}
			});

			Messenger.AddListener<string, Vector2, System.Action<UserData>>(NotifyTypes.UpdateUserDataAreaInfo, (areaName, pos, callback) => {
				if (UserModel.CurrentUserData != null) {
					UserModel.CurrentUserData.PositionStatu = UserPositionStatusType.InArea;
					UserModel.CurrentUserData.CurrentAreaSceneName = areaName;
					UserModel.CurrentUserData.CurrentAreaX = (int)pos.x;
					UserModel.CurrentUserData.CurrentAreaY = (int)pos.y;
					Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, callback);
				}
			});

			Messenger.AddListener<string>(NotifyTypes.UpdateUserDataCityInfo, (cityId) => {
				if (UserModel.CurrentUserData != null) {
					UserModel.CurrentUserData.PositionStatu = UserPositionStatusType.InCity;
					UserModel.CurrentUserData.CurrentCitySceneId = cityId;
				}
			});

			Messenger.AddListener<string, Vector2, System.Action<UserData>>(NotifyTypes.UpdateUserDataAreaPos, (areaName, pos, callback) => {
				if (UserModel.CurrentUserData != null) {
					UserModel.CurrentUserData.CurrentAreaSceneName = areaName;
					UserModel.CurrentUserData.CurrentAreaX = (int)pos.x;
					UserModel.CurrentUserData.CurrentAreaY = (int)pos.y;
					Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, callback);
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
						CityScenePanelCtrl.MakePlayBgm();
						break;
					default:
						break;
					}
				}
			});

			Messenger.AddListener(NotifyTypes.EnterGame, () => {
				DbManager.Instance.ResetTasks(); //初始化任务
				if (DbManager.Instance.GetRecordNum() > 0) {
//					MainPanelCtrl.Hide();
//					RecordListPanelCtrl.Hide();
//					SettingPanelCtrl.Hide();
					UIModel.CloseAllWindows();
					Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
					Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.CallUserData, (userData) => {
						Messenger.Broadcast<string>(NotifyTypes.GoToScene, userData.CurrentAreaSceneName);
					});
				}
				else {
					Messenger.Broadcast<string>(NotifyTypes.ShowCreateHostRolePanel, "role_0"); //第一个角色创建
				}
			});

			Messenger.AddListener<string>(NotifyTypes.ShowCreateHostRolePanel, (id) => {
//				MainPanelCtrl.Hide();
//				RecordListPanelCtrl.Hide();
//				SettingPanelCtrl.Hide();
				UIModel.CloseAllWindows();
				CreateHostRolePanelCtrl.Show(id);
			});

			Messenger.AddListener<RoleData>(NotifyTypes.CreateHostRole, (role) => {
				PlayerPrefs.SetString("CurrentRoleId", role.Id); //记录当前角色存档id
				DbManager.Instance.SetCurrentRoleId(role.Id);
				DbManager.Instance.AddNewRecord(role.Id, role.Name, "{}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

				//创建用户数据
				UserData userData = new UserData();
				userData.AreaFood = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "1");
				userData.AreaFood.Num = 0;
				userData.AreaFood.MaxNum = 100;
				userData.PositionStatu = UserPositionStatusType.InCity;
				userData.CurrentAreaSceneName = "Area0";
				userData.CurrentCitySceneId = "1";
				userData.CurrentAreaX = 9;
				userData.CurrentAreaY = 8;
				DbManager.Instance.AddNewUserData(JsonManager.GetInstance().SerializeObjectDealVector(userData), userData.AreaFood.Num, role.Id, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
	
				//创建角色数据
				role.ResourceBookDataIds.Clear();
				if (DbManager.Instance.AddNewRole(role.Id, JsonManager.GetInstance().SerializeObjectDealVector(role), (int)RoleStateType.InTeam, 0, role.HometownCityId, role.Id, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) {
//					DbManager.Instance.AddNewWeapon(role.ResourceWeaponDataId, role.Id);
//					DbManager.Instance.AddNewWeapon("1");
//					DbManager.Instance.AddNewWeapon("2");
//					DbManager.Instance.AddNewWeapon("3");
					CreateHostRolePanelCtrl.MakeStoryContinue(role.Name);
				}
			});

			Messenger.AddListener(NotifyTypes.GetRecordListData, () => {
				DbManager.Instance.GetRecordListData();
			});

			Messenger.AddListener<List<JArray>>(NotifyTypes.GetRecordListDataEcho, (data) => {
				RecordListPanelCtrl.Show(data);
			});

			Messenger.AddListener(NotifyTypes.ShowMainPanel, () => {
//				MainPanelCtrl.Hide();
//				RecordListPanelCtrl.Hide();
//				SettingPanelCtrl.Hide();
				UIModel.CloseAllWindows();
				MainPanelCtrl.Show();
			});

			Messenger.AddListener<bool>(NotifyTypes.ShowSettingPanel, (flag) => {
				//战斗时不能打开设置界面
				if (BattleMainPanelCtrl.Ctrl != null) {
					return;
				}
				SettingPanelCtrl.Show(flag);
			});
		}
	}
}