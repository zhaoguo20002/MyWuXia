﻿using UnityEngine;
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
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void SceneNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GoToScene, (sceneName) => {
				SceneManagerController.GetInstance().ChangeScene(sceneName);
			});

			Messenger.AddListener<EventData>(NotifyTypes.DealSceneEvent, (eventData) => {
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
				SceneData scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
				scene.MakeJsonToModel();
				CityScenePanelCtrl.Show(scene);
				Messenger.Broadcast<string>(NotifyTypes.GetTaskListDataInCityScene, cityId);
				Messenger.Broadcast(NotifyTypes.MakeTaskListHide);
			});
		}
	}
}