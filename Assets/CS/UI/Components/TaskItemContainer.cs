using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;

public class TaskItemContainer : MonoBehaviour {
	public Text title;
	public Text desc;

	TaskData taskData;
	// Use this for initialization
	void Start () {
		if (title == null || desc == null) {
			enabled = false;
		}
	}

	public void UpdateData(TaskData data) {
		taskData = data;
	}

	public void RefreshView() {
		title.text = string.Format("<color=\"{0}\">{1}</color>", "#FF0000", taskData.Name);

	}
}
