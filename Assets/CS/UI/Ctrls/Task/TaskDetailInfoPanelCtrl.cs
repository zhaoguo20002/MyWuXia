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
		bool canPushDialog;
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

		void Update() {
			if (taskData == null) {
				return;
			}
			if (canPushDialog) {
				canPushDialog = false;
				popDialog(taskData.GetCurrentDialog(), true);
				nextDialog();
			}
		}

		void nextDialog() {
			if (!taskData.CheckCompleted() && taskData.GetCurrentDialog().Type != TaskDialogType.Choice) {
				Messenger.Broadcast<string, bool, bool>(NotifyTypes.CheckTaskDialog, taskData.Id, true, false);
			}
		}

		void pushItemToGrid(Transform child) {
			MakeToParent(grid.transform, child);
			girdRectTrans.sizeDelta = new Vector2(grid.cellSize.x, (grid.cellSize.y + grid.spacing.y) * grid.transform.childCount - grid.spacing.y);
			listScrollRect.verticalNormalizedPosition = 0;
		}

		public void PopLoading() {
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
			loadingContainerClone.GetComponent<TaskDetailDialogLoadingContainer>().UpdateData(() => {
				canPushDialog = true;
			});
		}

		void popDialog(TaskDialogData dialog, bool willDuring = false) {
			switch(dialog.Type) {
			case TaskDialogType.Choice:
				if (choiceContainerObj == null) {
					choiceContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogChoiceContainer");
				}
				TaskDetailDialogChoiceContainer choiceContainer = Statics.GetPrefabClone(choiceContainerObj).GetComponent<TaskDetailDialogChoiceContainer>();
				pushItemToGrid(choiceContainer.transform);
				choiceContainer.UpdateData(taskData.Id, dialog, willDuring);
				choiceContainer.RefreshView();
				break;
			case TaskDialogType.JustTalk:
				TaskDetailDialogTalkContainer talkContainer;
				if (dialog.IconId == "{0}") {
					if (talkRightContainerObj == null) {
						talkRightContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogTalkRightContainer");
					}
					talkContainer = Statics.GetPrefabClone(talkRightContainerObj).GetComponent<TaskDetailDialogTalkContainer>();
				}
				else {
					if (talkLeftContainerObj == null) {
						talkLeftContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogTalkLeftContainer");
					}
					talkContainer = Statics.GetPrefabClone(talkLeftContainerObj).GetComponent<TaskDetailDialogTalkContainer>();
				}
				pushItemToGrid(talkContainer.transform);
				talkContainer.UpdateData(taskData.Id, dialog, willDuring);
				talkContainer.RefreshView();
				break;
			default:
				if (noticeContainerObj == null) {
					noticeContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogNoticeContainer");
				}
				TaskDetailDialogNoticeContainer noticeContainer = Statics.GetPrefabClone(noticeContainerObj).GetComponent<TaskDetailDialogNoticeContainer>();
				pushItemToGrid(noticeContainer.transform);
				noticeContainer.UpdateData(taskData.Id, dialog, willDuring);
				noticeContainer.RefreshView();
				break;
			}
		}

		public void UpdateData(TaskData data) {
			taskData = data;
			canPushDialog = false;
		}

		public override void RefreshView () {
			listScrollRect.gameObject.SetActive(true);
			closeBtn.gameObject.SetActive(true);
			TaskDialogData dialog;
			for (int i = 0; i < taskData.Dialogs.Count; i++) {
				if (taskData.ProgressData.Count > i) {
					dialog = taskData.Dialogs[i];
					Debug.LogWarning(dialog.Type);
					popDialog(dialog);
				}
				else {
					nextDialog();
					break;
				}
			}
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

		public static void PopDialogToList(TaskData data) {
			if (Ctrl != null) {
				Ctrl.UpdateData(data);
				Ctrl.PopLoading();
			}
		}
	}
}
