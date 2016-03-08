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
		string iconId;
		string msgStr;
		
		public void UpdateData(string id, TaskDialogData data, bool willDuring = false, TaskDialogStatusType status = TaskDialogStatusType.HoldOn) {
			iconId = data.IconId;
			msgStr = data.TalkMsg;
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
			//如果icon为｛0｝则表示为主角说话，这时候需要动态显示当家角色icon
			Icon.sprite = Statics.GetIconSprite(iconId == "{0}" ? "0" : iconId);
			Msg.text = msgStr;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
