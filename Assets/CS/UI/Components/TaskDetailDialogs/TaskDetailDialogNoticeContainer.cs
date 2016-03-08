using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogNoticeContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Text Msg;

		CanvasGroup alphaGroup;
		string msgStr;

		public void UpdateData(string id, TaskDialogData data, bool willDuring = false, TaskDialogStatusType status = TaskDialogStatusType.HoldOn) {
			msgStr = status == TaskDialogStatusType.HoldOn ? data.TalkMsg : (status == TaskDialogStatusType.ReadNo ? data.NoMsg : data.YesMsg);
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
			Msg.text = msgStr;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
