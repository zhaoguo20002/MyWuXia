using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

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
		/// 进入场景
		/// </summary>
		public static string EnterCityScene;
		/// <summary>
		/// 请求场景内的任务列表
		/// </summary>
		public static string CallTasksInCity;
		/// <summary>
		/// 请求场景内的任务列表回调
		/// </summary>
		public static string CallTasksInCityEcho;
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
						Messenger.Broadcast<string, int, int>(NotifyTypes.UpdateUserDataAreaInfo, areaName, x, y);
						Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, (userData) => {
							Messenger.Broadcast<string>(NotifyTypes.GoToScene, userData.CurrentAreaSceneName);
						});
					}
					break;
				case SceneEventType.Battle:
					Messenger.Broadcast<string>(NotifyTypes.CreateBattle, eventData.EventId);
					break;
				case SceneEventType.EnterCity:
					Messenger.Broadcast<string>(NotifyTypes.EnterCityScene, eventData.EventId);
					break;
				default:
					break;
				}
			});

			Messenger.AddListener<string>(NotifyTypes.EnterCityScene, (cityId) => {
				SceneData scene = JsonManager.GetInstance().GetMapping<SceneData>("Scenes", cityId);
				scene.MakeJsonToModel();
				CityScenePanelCtrl.Show(scene);
			});

			Messenger.AddListener<string>(NotifyTypes.CallTasksInCity, (cityId) => {
				
			});

		}
	}
}