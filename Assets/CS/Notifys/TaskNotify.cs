using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 请求场景内的任务列表
		/// </summary>
		public static string GetTaskListDataInCityScene;
		/// <summary>
		/// 请求场景内的任务列表回调
		/// </summary>
		public static string GetTaskListDataInCitySceneEcho;
		/// <summary>
		/// 获取当前任务列表数据
		/// </summary>
		public static string GetTaskListData;
		/// <summary>
		/// 获取当前任务列表数据回调
		/// </summary>
		public static string GetTaskListDataEcho;
		/// <summary>
		/// 关闭任务列表界面
		/// </summary>
		public static string HideTaskListPanel;
		/// <summary>
		/// 打开任务跟踪按钮界面
		/// </summary>
		public static string ShowTaskBtnPanel;
		/// <summary>
		/// 关闭任务跟踪按钮界面
		/// </summary>
		public static string HideTaskBtnPanel;
		/// <summary>
		/// 从任务跟踪按钮界面关闭任务列表界面
		/// </summary>
		public static string MakeTaskListHide;
		/// <summary>
		/// 获取任务详细数据
		/// </summary>
		public static string GetTaslDetailInfoData;
		/// <summary>
		/// 打开任务详细信息界面
		/// </summary>
		public static string ShowTaskDetailInfoPanel;
		/// <summary>
		/// 关闭任务详细信息界面
		/// </summary>
		public static string HideTaskDetailInfoPanel;
		/// <summary>
		/// 检测任务对话状态
		/// </summary>
		public static string CheckTaskDialog;
		/// <summary>
		/// 检测任务对话状态回调
		/// </summary>
		public static string CheckTaskDialogEcho;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Tasks the notify init.
		/// </summary>
		public static void TaskNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GetTaskListDataInCityScene, (cityId) => {
				DbManager.Instance.GetTaskListDataInCityScene(cityId);
			});
			
			Messenger.AddListener<List<TaskData>>(NotifyTypes.GetTaskListDataInCitySceneEcho, (list) => {
				CityScenePanelCtrl.ShowTask(list);
			});

			Messenger.AddListener(NotifyTypes.GetTaskListData, () => {
				DbManager.Instance.GetTaskListData();
			});

			Messenger.AddListener<List<TaskData>>(NotifyTypes.GetTaskListDataEcho, (list) => {
				TaskListPanelCtrl.Show(list);
			});

			Messenger.AddListener(NotifyTypes.HideTaskListPanel, () => {
				TaskListPanelCtrl.Hide();
			});

			Messenger.AddListener(NotifyTypes.ShowTaskBtnPanel, () => {
				TaskBtnPanelCtrl.Show();
			});

			Messenger.AddListener(NotifyTypes.HideTaskBtnPanel, () => {
				TaskBtnPanelCtrl.Hide();
			});

			Messenger.AddListener(NotifyTypes.MakeTaskListHide, () => {
				TaskBtnPanelCtrl.MakeTaskListHide();
			});
			
			Messenger.AddListener<string>(NotifyTypes.GetTaslDetailInfoData, (taskId) => {
				DbManager.Instance.GetTaskDetailInfoData(taskId);
			});

			Messenger.AddListener<JArray>(NotifyTypes.ShowTaskDetailInfoPanel, (data) => {
				TaskDetailInfoPanelCtrl.Show(data);
				Messenger.Broadcast(NotifyTypes.MakeTaskListHide);
			});

			Messenger.AddListener(NotifyTypes.HideTaskDetailInfoPanel, () => {
				TaskDetailInfoPanelCtrl.Hide();
			});

			Messenger.AddListener<string, bool, bool>(NotifyTypes.CheckTaskDialog, (taskId, auto, selectedNo) => {
				DbManager.Instance.CheckTaskDialog(taskId, auto, selectedNo);
			});

			Messenger.AddListener<JArray>(NotifyTypes.CheckTaskDialogEcho, (data) => {
				TaskDetailInfoPanelCtrl.PopDialogToList(data);
			});
		}
	}
}