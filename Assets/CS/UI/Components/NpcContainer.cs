using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;
using DG.Tweening;

namespace Game {
	public class NpcContainer : MonoBehaviour {
		public Text Name;
		public Image State;
		public Button ClickButton;
		public Image FightFlagImage;
		Image icon;
		NpcData npcData;
		RectTransform trans;
		float timeout = 3;
		float date;
		// Use this for initialization
		void Awake () {
			icon = GetComponent<Image>();
			Name.text = "";
			State.gameObject.SetActive(false);
			EventTriggerListener.Get(ClickButton.gameObject).onClick += onClick;
			trans = GetComponent<RectTransform>();
			date = Time.fixedTime - timeout;
		}
		
		void onClick(GameObject e) {
            if (npcData.Type == NpcType.Fight) {
                if (Time.fixedTime - date >= timeout) {
                    date = Time.fixedTime;
                    if (npcData.DefaultDialogMsg != "") {
                        Statics.CreateDialogMsgPop(new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z), npcData.DefaultDialogMsg, Color.black);
                    }
                    Invoke("doFight", 1);
                }
            } else if (npcData.CurrentTask != null) {
                Messenger.Broadcast<string>(NotifyTypes.GetTaslDetailInfoData, npcData.CurrentTask.Id);
            } else if (npcData.Type == NpcType.AfterTask) {
                switch (npcData.Id) {
                    case "05002001": //江湖百晓生
                        Messenger.Broadcast<string>(NotifyTypes.NpcsEventHandler, npcData.Id);
                        break;
                    default:
                        break;
                }
            }
			else {
				if (npcData.DefaultDialogMsg != "") {
					Statics.CreateDialogMsgPop(new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z), npcData.DefaultDialogMsg, Color.black);
				}
			}
		}

		void doFight() {
			Messenger.Broadcast<string>(NotifyTypes.CreateBattle, npcData.CurrentFightId);
		}
		
		public void UpdateData(NpcData data) {
			npcData = data;
		}
		
		public void RefreshView() {
			Name.text = npcData.Name;
			icon.sprite = Statics.GetIconSprite(npcData.IconId);
			FightFlagImage.gameObject.SetActive(npcData.Type == NpcType.Fight);
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
            State.DOKill();
            switch (npcData.CurrentTask.State)
            {
                case TaskStateType.Accepted:
                    State.gameObject.SetActive(true);
                    State.sprite = Statics.GetSprite("TaskState2");
                    break;
                case TaskStateType.CanAccept:
                    State.gameObject.SetActive(true);
                    State.sprite = Statics.GetSprite("TaskState1");
                    State.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
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
