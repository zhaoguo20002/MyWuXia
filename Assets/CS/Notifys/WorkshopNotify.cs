using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 获取工坊主界面数据
		/// </summary>
		public static string GetWorkshopPanelData;
		/// <summary>
		/// 获取工坊主界面数据回调
		/// </summary>
		public static string GetWorkshopPanelDataEcho;
		/// <summary>
		/// 增减资源的工作家丁数
		/// </summary>
		public static string ChangeResourceWorkerNum;
		/// <summary>
		/// 增减资源的工作家丁数回调
		/// </summary>
		public static string ChangeResourceWorkerNumEcho;
		/// <summary>
		/// 刷新资源
		/// </summary>
		public static string ModifyResources;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// 工坊相关消息
		/// </summary>
		public static void WorkshopNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GetWorkshopPanelData, (cityId) => {
				DbManager.Instance.GetWorkshopPanelData(cityId);
			});

			Messenger.AddListener<JArray>(NotifyTypes.GetWorkshopPanelDataEcho, (data) => {
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				WorkshopPanelCtrl.Show(data);
			});

			Messenger.AddListener<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, (type, num) => {
				DbManager.Instance.ChangeResourceWorkerNum(type, num);
			});

			Messenger.AddListener<JArray>(NotifyTypes.ChangeResourceWorkerNumEcho, (data) => {
				WorkshopPanelCtrl.MakeChangeResourceWorkerNumEcho(data);
			});

			Messenger.AddListener(NotifyTypes.ModifyResources, () => {
				DbManager.Instance.ModifyResources();
			});
		}
	}
}