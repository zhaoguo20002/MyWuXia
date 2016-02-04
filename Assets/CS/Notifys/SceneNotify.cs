using UnityEngine;
using System.Collections;

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
				default:
					break;
				}
			});

		}
	}
}