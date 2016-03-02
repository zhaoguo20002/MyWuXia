using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailInfoPanelCtrl : WindowCore<TaskDetailInfoPanelCtrl, JArray> {
		Image bg;
		ScrollRect listScrollRect;
		GridLayoutGroup grid;
		RectTransform girdRectTrans;
		Button closeBtn;

		Object loadingContainerObj = null;
		GameObject loadingContainerClone = null;
		Object talkLeftContainerObj = null;
		Object talkRightContainerObj = null;
		Object choiceContainerObj = null;
		Object noticeContainerObj = null;

		TaskData taskData;
		protected override void Init () {
			bg = GetChildImage("Bg");
			listScrollRect = GetChildScrollRect("List");
			grid = GetChildGridLayoutGroup("Grid");
			girdRectTrans = GetChildComponent<RectTransform>(gameObject, "Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			listScrollRect.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(false);
		}

		void onClick(GameObject e) {
			Back();
		}

		void pushItemToGrid(Transform child) {
			MakeToParent(grid.transform, child);
			girdRectTrans.sizeDelta = new Vector2(grid.cellSize.x, (grid.cellSize.y + grid.spacing.y) * grid.transform.childCount - grid.spacing.y);
		}

		void popLoading() {
			if (loadingContainerObj == null) {
				loadingContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogLoadingContainer");
			}
			if (loadingContainerClone != null) {
				Destroy(loadingContainerClone);
				loadingContainerClone = null;
			}
			loadingContainerClone = Statics.GetPrefabClone(loadingContainerObj);
			loadingContainerClone.name = "loadingContainer";
			pushItemToGrid(loadingContainerClone.transform);
		}

		void popDialog() {
			if (taskData.CheckCompleted()) {
				return;
			}
		}

		public void UpdateData(TaskData data) {
			taskData = data;
		}

		public override void RefreshView () {
			listScrollRect.gameObject.SetActive(true);
			closeBtn.gameObject.SetActive(true);
			popLoading();
		}

		public void Open() {
			Vector2 initSize = bg.rectTransform.sizeDelta;
			bg.rectTransform.sizeDelta = new Vector2(-580, initSize.y);
			bg.rectTransform.DOSizeDelta(initSize, 0.3f).SetEase(Ease.InOutCirc).OnComplete(() => {
				RefreshView();
			});
		}

		public void Back() {
			listScrollRect.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(false);
			bg.rectTransform.DOSizeDelta(new Vector2(-580, bg.rectTransform.sizeDelta.y), 0.3f).SetEase(Ease.InOutCirc).OnComplete(() => {
				Close();
			});
		}

		public static void Show(TaskData data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Task/TaskDetailInfoPanelView", "TaskDetailInfoPanelCtrl");
				Ctrl.UpdateData(data);
				Ctrl.Open();
			}
			else {
				Ctrl.UpdateData(data);
				Ctrl.RefreshView();
			}
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}
	}
}
