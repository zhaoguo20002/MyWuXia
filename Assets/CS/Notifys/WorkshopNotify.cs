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
		}
	}
}