using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

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
		Dictionary<string, Component> containersMapping;

		string taskId;
		JArray readDialogDataList;
		Queue<JArray> queuePushDialogData;
		bool canPushDialog;
		bool canSrollToBottom;
		protected override void Init () {
			bg = GetChildImage("Bg");
			listScrollRect = GetChildScrollRect("List");
			grid = GetChildGridLayoutGroup("Grid");
			girdRectTrans = GetChildComponent<RectTransform>(gameObject, "Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			listScrollRect.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(false);
			containersMapping = new Dictionary<string, Component>();
			queuePushDialogData = new Queue<JArray>();
		}

		void onClick(GameObject e) {
			Back();
		}

		void Update() {
			checkLoading();
			if (canSrollToBottom) {
				listScrollRect.verticalNormalizedPosition = Mathf.Lerp(listScrollRect.verticalNormalizedPosition, 0, Time.deltaTime * 5);
				if (listScrollRect.verticalNormalizedPosition <= 0) {
					canSrollToBottom = false;
				}
			}
		}

		void nextDialog() {
			Messenger.Broadcast<string, bool, bool>(NotifyTypes.CheckTaskDialog, taskId, true, false);
		}

		void pushItemToGrid(Transform child) {
			MakeToParent(grid.transform, child);
			girdRectTrans.sizeDelta = new Vector2(grid.cellSize.x, (grid.cellSize.y + grid.spacing.y) * grid.transform.childCount - grid.spacing.y);
			canSrollToBottom = true;
		}

		public void PopLoading(JArray data) {
			canPushDialog = false;
			for (int i = 0; i < data.Count; i++) {
				queuePushDialogData.Enqueue((JArray)data[i]);
			}
			createLoading();
		}

		void checkLoading() {
			if (canPushDialog) {
				canPushDialog = false;
				if (queuePushDialogData.Count > 0) {
					popDialog(queuePushDialogData.Dequeue(), true);
					createLoading();
				}
				if (queuePushDialogData.Count == 0) {
					nextDialog();
				}
			}
		}

		void createLoading() {
			if (queuePushDialogData.Count == 0 || loadingContainerClone != null) {
				canSrollToBottom = false;
				return;
			}
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
				loadingContainerClone = null;
			});
		}

		void popDialog(JArray dialogData, bool willDuring = false) {
			if (dialogData == null) {
				return;
			}
			string dialogId = dialogData[0].ToString();
			TaskDialogType dialogType = (TaskDialogType)((short)dialogData[1]);
			switch(dialogType) {
			case TaskDialogType.Choice:
				TaskDetailDialogChoiceContainer choiceContainer;
				if (!containersMapping.ContainsKey(dialogId)) {
					if (choiceContainerObj == null) {
						choiceContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogChoiceContainer");
					}
					choiceContainer = Statics.GetPrefabClone(choiceContainerObj).GetComponent<TaskDetailDialogChoiceContainer> ();
					pushItemToGrid(choiceContainer.transform);
					containersMapping.Add(dialogId, choiceContainer);
				}
				choiceContainer = (TaskDetailDialogChoiceContainer)containersMapping[dialogId];
				choiceContainer.UpdateData(taskId, dialogData, willDuring);
				choiceContainer.RefreshView();
				break;
			case TaskDialogType.JustTalk:
				TaskDetailDialogTalkContainer talkContainer;
				if (!containersMapping.ContainsKey(dialogId)) {
					if (dialogData[4].ToString() == "{0}") {
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
					containersMapping.Add(dialogId, talkContainer);
				}
				talkContainer = (TaskDetailDialogTalkContainer)containersMapping[dialogId];
				talkContainer.UpdateData(taskId, dialogData, willDuring);
				talkContainer.RefreshView();
				break;
			default:
				TaskDetailDialogNoticeContainer noticeContainer;
				if (!containersMapping.ContainsKey(dialogId)) {
					if (noticeContainerObj == null) {
						noticeContainerObj = Statics.GetPrefab("Prefabs/UI/GridItems/TaskDetailDialogs/TaskDetailDialogNoticeContainer");
					}
					noticeContainer = Statics.GetPrefabClone(noticeContainerObj).GetComponent<TaskDetailDialogNoticeContainer>();
					pushItemToGrid(noticeContainer.transform);
					containersMapping.Add(dialogId, noticeContainer);
				}
				noticeContainer = (TaskDetailDialogNoticeContainer)containersMapping[dialogId];
				noticeContainer.UpdateData(taskId, dialogData, willDuring);
				noticeContainer.RefreshView();
				break;
			}
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
			taskId = data[0].ToString();
			readDialogDataList = (JArray)data[1];
		}

		public override void RefreshView () {
			listScrollRect.gameObject.SetActive(true);
			closeBtn.gameObject.SetActive(true);
			for (int i = 0; i < readDialogDataList.Count; i++) {
				popDialog((JArray)readDialogDataList[i]);
			}
			listScrollRect.verticalNormalizedPosition = 0;
			nextDialog();
		}

		public void Open() {
			Vector2 initSize = bg.rectTransform.sizeDelta;
			bg.rectTransform.sizeDelta = new Vector2(-580, initSize.y);
			bg.rectTransform.DOSizeDelta(initSize, 0.3f).SetEase(Ease.InOutCirc).OnComplete(() => {
				RefreshView();
			});
		}

		public void Back() {
			if (loadingContainerClone != null) {
				return;
			}
			listScrollRect.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(false);
			bg.rectTransform.DOSizeDelta(new Vector2(-580, bg.rectTransform.sizeDelta.y), 0.3f).SetEase(Ease.InOutCirc).OnComplete(() => {
				Close();
				Messenger.Broadcast(NotifyTypes.GetTasksInCityScene);
			});
		}

		public static void Show(JArray data) {
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

		public static void PopDialogToList(JArray data) {
			if (Ctrl != null) {
				Ctrl.PopLoading(data);
			}
		}
	}
}
