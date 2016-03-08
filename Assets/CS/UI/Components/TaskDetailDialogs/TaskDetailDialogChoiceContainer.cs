using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogChoiceContainer : ComponentCore, ITaskDetailDialogInterface {
		public Text Msg;
		public Button SureBtn;
		public Button CancelBtn;

		CanvasGroup alphaGroup;
		string taskId;
		string msgStr;
		TaskDialogStatusType dialogStatus;

		void Start() {
			EventTriggerListener.Get(SureBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CancelBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button> ().enabled) {
				return;
			}
			if (dialogStatus == TaskDialogStatusType.HoldOn) {
				Messenger.Broadcast<string, bool, bool>(NotifyTypes.CheckTaskDialog, taskId, false, e.name == "CancelBtn");
			}
			dialogStatus = e.name == SureBtn.name ? TaskDialogStatusType.ReadYes : TaskDialogStatusType.ReadNo;
		}

		public void UpdateData(string id, TaskDialogData data, bool willDuring = false, TaskDialogStatusType status = TaskDialogStatusType.HoldOn) {
			taskId = id;
			msgStr = data.TalkMsg;
			dialogStatus = status;
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

		public override void RefreshView() {
			Msg.text = msgStr;
			if (dialogStatus == TaskDialogStatusType.ReadNo) {
				MakeButtonEnable(CancelBtn, false);
			} else if (dialogStatus == TaskDialogStatusType.ReadYes) {
				MakeButtonEnable(SureBtn, false);
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
