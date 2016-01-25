using UnityEngine;
using System.Collections;
namespace Game {
	public class SceneManagerController {
		private string _sceneName;
		private static SceneManagerController _instance;

		//惰性获取静态实例
		public static SceneManagerController GetInstance () {
			if (null == _instance) {
				_instance = new SceneManagerController();
			}
			return _instance;
		}

		public SceneManagerController () {
			_sceneName = ""; //default
		}

		//切换场景
		public void ChangeScene (string sceneName) {
			_sceneName = sceneName;
			Application.LoadLevel("Loading");
		}

		//场景名[只读]
		public string SceneName {
			get { return _sceneName; }
		}
	}
}