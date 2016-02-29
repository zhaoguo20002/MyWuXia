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
		}
	}
}