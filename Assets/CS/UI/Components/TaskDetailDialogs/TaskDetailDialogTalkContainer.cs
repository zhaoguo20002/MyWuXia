using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogTalkContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Image Icon;
		public Text Msg;
		
		CanvasGroup alphaGroup;
		string taskId;
		
		public void UpdateData(string id, TaskDialogData data, bool willDuring = false) {
			taskId = id;
			if (willDuring) {
				alphaGroup = gameObject.AddComponent<CanvasGroup>();
				alphaGroup.alpha = 0;
				alphaGroup.DOFade(1, 0.5f).OnComplete(() => {
					if (alphaGroup != null) {
						Destroy(alphaGroup);
					}
				});
			}
		}

		public void RefreshView() {

		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
