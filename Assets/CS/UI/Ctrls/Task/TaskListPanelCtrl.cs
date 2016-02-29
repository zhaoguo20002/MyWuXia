using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class TaskListPanelCtrl : WindowCore<TaskListPanelCtrl, JArray> {
	protected override void Init () {

	}

	public override void UpdateData (object obj) {

	}

	public override void RefreshView () {

	}

	public static void Show(List<TaskData> data) {
		if (Ctrl == null) {

		}
	}
}
