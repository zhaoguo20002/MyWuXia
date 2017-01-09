using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Game;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskBtnPanelCtrl : WindowCore<TaskBtnPanelCtrl, JArray> {
		Button btn;
		Image mark;
		Image newFlag;
		bool showList = false;
        float date;
		protected override void Init () {
			btn = GetChildButton("Btn");
			EventTriggerListener.Get(btn.gameObject).onClick = onClick;
			mark = GetChildImage("Mark");
			newFlag = GetChildImage("NewFlag");
            date = 0;
		}
		
		void onClick(GameObject e) {
            if (Time.fixedTime - date < 1) {
                return;
            }
            date = Time.fixedTime;
			if (showList) {
				CloseList();
			}
			else {
				showList = true;
				Messenger.Broadcast(NotifyTypes.GetTaskListData);
				mark.transform.DORotate(new Vector3(0, 0, 180), 0.25f);
			}
			PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "NewTask", ""); //关闭提醒
			newFlag.gameObject.SetActive(false);
		}
		
		public void CloseList() {
			showList = false;
			Messenger.Broadcast(NotifyTypes.HideTaskListPanel);
			mark.transform.DORotate(new Vector3(0, 0, 0), 0.25f);
		}
		
		public override void RefreshView () {
			CloseList();
			//判断是否有新任务
			newFlag.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "NewTask")));
		}
		
		public static void Show() {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Task/TaskBtnPanelView", "TaskBtnPanelCtrl", 56, 0, UIModel.FrameCanvas.transform);
				Ctrl.MoveHorizontal(-(56 + 2));
			}
			Ctrl.RefreshView();
		}
		
		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}

		public static void MakeMoveOut() {
			if (Ctrl != null) {
				Ctrl.MoveHorizontal(56 + 2, () => {
					Ctrl.CloseList();
					Ctrl.Close();
				});
			}
		}
		
		public static void MakeTaskListHide() {
			if (Ctrl != null) {
				Ctrl.CloseList();
			}
		}
	}
}
