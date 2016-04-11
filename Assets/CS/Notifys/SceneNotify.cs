using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 切换场景
		/// </summary>
		public static string GoToScene;
		/// <summary>
		/// 场景中事件集中处理消息
		/// </summary>
		public static string DealSceneEvent;
		/// <summary>
		/// 关闭城镇界面从城镇返回大地图
		/// </summary>
		public static string FromCitySceneBackToArea;
		/// <summary>
		/// 进入场景
		/// </summary>
		public static string EnterCityScene;
		/// <summary>
		/// 获取城镇内任务列表
		/// </summary>
		public static string GetTasksInCityScene;
		/// <summary>
		/// 关闭城镇主界面
		/// </summary>
		public static string HideCityScenePanel;
		/// <summary>
		/// 获取杂货铺商品数据
		/// </summary>
		public static string GetStorePanelData;
		/// <summary>
		/// 获取杂货铺商品数据回调
		/// </summary>
		public static string GetStorePanelDataEcho;
		/// <summary>
		/// 购买杂货铺物品
		/// </summary>
		public static string BuyItem;
		/// <summary>
		/// 购买杂货铺物品回调
		/// </summary>
		public static string BuyItemEcho;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void SceneNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GoToScene, (sceneName) => {
				SceneManagerController.GetInstance().ChangeScene(sceneName);
			});

			Messenger.AddListener<string>(NotifyTypes.DealSceneEvent, (eventId) => {
//				EventData eventData = JsonManager.GetInstance().GetMapping<EventData>("AreaEventDatas", eventId);
				EventData eventData = null;
				if (AreaMain.StaticAreaEventsMapping.ContainsKey(eventId)) {
					eventData = AreaMain.StaticAreaEventsMapping[eventId];
				}
				else if (AreaMain.ActiveAreaEventsMapping.ContainsKey(eventId)) {
					eventData = AreaMain.ActiveAreaEventsMapping[eventId];
				}
				switch (eventData.Type) {
				case SceneEventType.EnterArea:
					string[] fen = eventData.EventId.Split(new char[] { '_' });
					if (fen.Length >= 3) {
						string areaName = fen[0];
						int x = int.Parse(fen[1]);
						int y = int.Parse(fen[2]);
						Messenger.Broadcast<string, Vector2, System.Action<UserData>>(NotifyTypes.UpdateUserDataAreaInfo, areaName, new Vector2(x, y), (userData) => {
							Messenger.Broadcast<string>(NotifyTypes.GoToScene, userData.CurrentAreaSceneName);
						});
					}
					break;
				case SceneEventType.Battle:
					Messenger.Broadcast<string>(NotifyTypes.CreateBattle, eventData.EventId);
					break;
				case SceneEventType.EnterCity:
					Messenger.Broadcast<string>(NotifyTypes.UpdateUserDataCityInfo, eventData.EventId);
					Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, (userData) => {
						Messenger.Broadcast<string>(NotifyTypes.EnterCityScene, userData.CurrentCitySceneId);
					});
					break;
				default:
					break;
				}
			});

			Messenger.AddListener(NotifyTypes.FromCitySceneBackToArea, () => {
				Messenger.Broadcast<string, Vector2, System.Action<UserData>>(NotifyTypes.UpdateUserDataAreaInfo,
					UserModel.CurrentUserData.CurrentAreaSceneName, 
					new Vector2(UserModel.CurrentUserData.CurrentAreaX, UserModel.CurrentUserData.CurrentAreaY), 
					(userData) => {
						//播放大地图背景音乐
						Messenger.Broadcast(NotifyTypes.PlayBgm);
					}
				);
			});

			Messenger.AddListener<string>(NotifyTypes.EnterCityScene, (cityId) => {
				DbManager.Instance.CheckEnterCity(cityId);
				DbManager.Instance.GetCitySceneMenuData(cityId);
				Messenger.Broadcast(NotifyTypes.GetTasksInCityScene);
				Messenger.Broadcast(NotifyTypes.MakeTaskListHide);
			});

			Messenger.AddListener(NotifyTypes.GetTasksInCityScene, () => {
				CityScenePanelCtrl.GetTasksInCityScene();
			});

			Messenger.AddListener(NotifyTypes.HideCityScenePanel, () => {
				CityScenePanelCtrl.Hide();
			});

			Messenger.AddListener<string>(NotifyTypes.GetStorePanelData, (cityId) => {
				DbManager.Instance.GetStorePanelData(cityId);
			});

			Messenger.AddListener<List<ItemData>, double>(NotifyTypes.GetStorePanelDataEcho, (items, silver) => {
				StorePanelCtrl.Show(items, silver);
			});

			Messenger.AddListener<string>(NotifyTypes.BuyItem, (itemId) => {
				DbManager.Instance.BuyItem(itemId);
			});

			Messenger.AddListener<string, double>(NotifyTypes.BuyItemEcho, (msg, silver) => {
				StorePanelCtrl.MakeBuyItemEcho(msg, silver);
			});
		}
	}
}