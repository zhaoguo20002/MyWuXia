using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskListPanelCtrl : WindowCore<TaskListPanelCtrl, JArray> {
		Button block;
		Image bg;
		GridLayoutGroup grid;
		
		List<TaskData> taskList;
		List<TaskItemContainer> taskContainers;
		
		Object prefabObj;
		
		protected override void Init () {
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;;
			bg = GetChildImage("Bg");
			bg.rectTransform.anchoredPosition = new Vector2(480, 0);
			grid = GetChildGridLayoutGroup("Grid");
			taskContainers = new List<TaskItemContainer>();
		}

		void onClick(GameObject e) {
			Messenger.Broadcast(NotifyTypes.MakeTaskListHide);
		}

		public void UpdateData (List<TaskData> list) {
			taskList = list;
		}

		public void In() {
			bg.rectTransform.DOAnchorPos(new Vector2(-35, 0), 0.5f);
		}

		public void Out() {
			bg.rectTransform.DOAnchorPos(new Vector2(480, 0), 0.5f).OnComplete(() => {
				Close();
			});
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
				InstantiateView("Prefabs/UI/Task/TaskListPanelView", "TaskListPanelCtrl");
				Ctrl.In();
			}
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}
		
		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Out();
			}
		}
	}
}
