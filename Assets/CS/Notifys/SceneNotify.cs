using UnityEngine;
using System.Collections;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// The go to scene.
		/// </summary>
		public static string GoToScene;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void SceneNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GoToScene, (sceneName) => {
				SceneManagerController.GetInstance().ChangeScene(sceneName);
			});

		}
	}
}