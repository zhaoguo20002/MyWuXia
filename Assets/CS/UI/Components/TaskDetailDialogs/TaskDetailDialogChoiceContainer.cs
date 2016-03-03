using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogChoiceContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Text Msg;
		public Button SureBtn;
		public Button CancelBtn;

		CanvasGroup alphaGroup;
		string taskId;

		void Start() {
			EventTriggerListener.Get(SureBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CancelBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			Debug.LogWarning(e.name);
			Messenger.Broadcast<string, bool, bool>(NotifyTypes.CheckTaskDialog, taskId, false, e.name == "CancelBtn");
		}

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
