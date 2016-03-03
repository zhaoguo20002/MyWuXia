using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;

namespace Game {
	public class NpcContainer : MonoBehaviour {
		public Text Name;
		public Image State;
		public Button ClickButton;
		Image icon;
		NpcData npcData;
		// Use this for initialization
		void Awake () {
			icon = GetComponent<Image>();
			Name.text = "";
			State.gameObject.SetActive(false);
			EventTriggerListener.Get(ClickButton.gameObject).onClick += onClick;
		}
		
		void onClick(GameObject e) {
			Debug.LogWarning(npcData.Name);
			if (npcData.CurrentTask != null) {
				Debug.LogWarning(npcData.CurrentTask.Name);
				Messenger.Broadcast<string>(NotifyTypes.GetTaslDetailInfoData, npcData.CurrentTask.Id);
			}
		}
		
		public void UpdateData(NpcData data) {
			npcData = data;
		}
		
		public void RefreshView() {
			Name.text = npcData.Name;
			icon.sprite = Statics.GetIconSprite(npcData.IconId);
		}
		
		public void SetNpcData(NpcData data) {
			UpdateData(data);
			RefreshView();
		}
		
		public void UpdateTaskData(string taskId, TaskStateType state) {
			npcData.CurrentResourceTaskDataId = taskId;
			npcData.MakeJsonToModel();
			npcData.CurrentTask.State = state;
		}
		
		public void RefreshTaskView() {
			switch(npcData.CurrentTask.State) {
			case TaskStateType.Accepted:
				State.gameObject.SetActive(true);
				State.sprite = Statics.GetSprite("TaskState2");
				break;
			case TaskStateType.CanAccept:
				State.gameObject.SetActive(true);
				State.sprite = Statics.GetSprite("TaskState1");
				break;
			case TaskStateType.CanNotAccept:
				State.gameObject.SetActive(true);
				State.sprite = Statics.GetSprite("TaskState0");
				break;
			case TaskStateType.Ready:
				State.gameObject.SetActive(true);
				State.sprite = Statics.GetSprite("TaskState3");
				break;
			default:
				State.gameObject.SetActive(false);
				break;
			}
		}
	}
}
