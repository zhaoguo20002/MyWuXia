using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class TaskDetailDialogChoiceContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Text Msg;
		public Button SureBtn;
		public Button CancelBtn;

		public void UpdateData(TaskData data) {
			
		}

		public void RefreshView() {

		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
