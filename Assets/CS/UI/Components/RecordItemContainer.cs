using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RecordItemContainer : MonoBehaviour {
		public Text DescText;
		public Button EnterBtn;
		public Button DeleteBtn;
		public Button CreateBtn;

		JArray data;
		string currentRoleId;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(EnterBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(DeleteBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CreateBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "EnterBtn":
				PlayerPrefs.SetString("CurrentRoleId", currentRoleId);
				DbManager.Instance.SetCurrentRoleId(currentRoleId);
				Messenger.Broadcast(NotifyTypes.EnterGame);
				break;
			case "DeleteBtn":
				ConfirmCtrl.Show("删除存档后无法恢复数据，确定删除？", () => {
					PlayerPrefs.SetString("CurrentRoleId", "");
					DbManager.Instance.DeleteRecord((int)data[0]);
				});
				break;
			case "CreateBtn":
				PlayerPrefs.SetString("CurrentRoleId", currentRoleId);
				DbManager.Instance.SetCurrentRoleId(currentRoleId);
				Messenger.Broadcast<string>(NotifyTypes.ShowCreateHostRolePanel, currentRoleId);
				break;
			default:
				break;
			}
		}

		public void UpdateData(JArray d) {
			data = d;
			currentRoleId = data[1].ToString();
		}

		public void RefreshView() {
			if (data.Count >= 5) {
				DescText.text = string.Format("当家:{0}\n建档时间:{1}", data[2].ToString(), data[4].ToString());
				EnterBtn.gameObject.SetActive(true);
				DeleteBtn.gameObject.SetActive(true);
				CreateBtn.gameObject.SetActive(false);
			}
			else {
				DescText.text = "无记录";
				EnterBtn.gameObject.SetActive(false);
				DeleteBtn.gameObject.SetActive(false);
				CreateBtn.gameObject.SetActive(true);
			}
		}
			
	}
}
