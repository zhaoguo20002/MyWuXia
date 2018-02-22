using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace Game {
	public class TaskDetailDialogFightContainer : ComponentCore, ITaskDetailDialogInterface {
		public Text Msg;
		public Button SureBtn;

		CanvasGroup alphaGroup;
		string taskId;
		string msgStr;
		TaskDialogStatusType dialogStatus;
		string fightId;
        FightData fightData;

		void Start() {
			EventTriggerListener.Get(SureBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			if (dialogStatus == TaskDialogStatusType.HoldOn) {
				Messenger.Broadcast<string>(NotifyTypes.CreateBattle, fightId);
			}
		}

		public void UpdateData(string id, JArray data, bool willDuring) {
			taskId = id;
			msgStr = data[2].ToString();
			dialogStatus = (TaskDialogStatusType)((short)data[3]);
			fightId = data[5].ToString();
            fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
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
			if (dialogStatus == TaskDialogStatusType.ReadYes) {
				MakeButtonEnable(SureBtn, false);
			}
            SureBtn.gameObject.SetActive(fightData.Type == FightType.Task);
		}

		public void DisableBtn() {
			dialogStatus = TaskDialogStatusType.ReadYes;
			RefreshView();
		}
	}
}
