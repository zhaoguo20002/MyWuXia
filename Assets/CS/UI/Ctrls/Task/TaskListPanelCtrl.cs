using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game {
	public class TaskListPanelCtrl : WindowCore<TaskListPanelCtrl, JArray> {
		GridLayoutGroup grid;
		
		List<TaskData> taskList;
		List<TaskItemContainer> taskContainers;
		
		Object prefabObj;
		
		protected override void Init () {
			grid = GetChildGridLayoutGroup("Grid");
			taskContainers = new List<TaskItemContainer>();
		}
		
		public void UpdateData (List<TaskData> list) {
			taskList = list;
		}
		
		public override void RefreshView () {
			for (int i = 0; i < taskContainers.Count; i++) {
				Destroy(taskContainers[i].gameObject);
			}
			taskContainers.Clear();
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskItemContainer");
			}
			GameObject itemPrefab;
			TaskData task;
			TaskItemContainer container;
			for (int i = 0; i < taskList.Count; i++) {
				task = taskList[i];
				itemPrefab = Statics.GetPrefabClone(prefabObj);
				itemPrefab.name = i.ToString();
				MakeToParent(grid.transform, itemPrefab.transform);
				container = itemPrefab.GetComponent<TaskItemContainer>();
				container.UpdateData(task);
				container.RefreshView();
				taskContainers.Add(container);
			}
			grid.GetComponent<RectTransform>().sizeDelta = new Vector2(grid.cellSize.x, (grid.cellSize.y + grid.spacing.y) * taskContainers.Count - grid.spacing.y);
		}
		
		public static void Show(List<TaskData> data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Task/TaskListPanelView", "TaskListPanelCtrl", 480, 0);
				Ctrl.MoveHorizontal(-(480 + 36));
			}
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}
		
		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.MoveHorizontal(480 + 36, () => {
					Ctrl.Close();
				});
			}
		}
	}
}
