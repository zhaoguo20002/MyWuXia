using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 请求队伍信息面板数据
		/// </summary>
		public static string CallRoleInfoPanelData;
		/// <summary>
		/// 请求队伍信息面板数据回调
		/// </summary>
		public static string CallRoleInfoPanelDataEcho;
		/// <summary>
		/// 关闭队伍信息面板
		/// </summary>
		public static string HideRoleInfoPanel;
		/// <summary>
		/// 控制角色属性面板是否可以切换角色
		/// </summary>
		public static string MakeChangeRoleEnable;
		/// <summary>
		/// 控制角色属性面板是否可以切书
		/// </summary>
		public static string MakeChangeBookEnable;

	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void RoleNotifyInit() {

			Messenger.AddListener(NotifyTypes.CallRoleInfoPanelData, () => {
				DbManager.Instance.CallRoleInfoPanelData();
			});

			Messenger.AddListener<JObject>(NotifyTypes.CallRoleInfoPanelDataEcho, (obj) => {
				RoleInfoPanelCtrl.Show((JArray)obj["data"]);
			});

			Messenger.AddListener(NotifyTypes.HideRoleInfoPanel, () => {
				RoleInfoPanelCtrl.Hide();
			});

			Messenger.AddListener<bool>(NotifyTypes.MakeChangeRoleEnable, (enable) => {
				RoleInfoPanelCtrl.MakeChangeRoleEnable(enable);
			});

			Messenger.AddListener<bool>(NotifyTypes.MakeChangeBookEnable, (enable) => {
				RoleInfoPanelCtrl.MakeChangeBookEnable(enable);
			});
		}
	}
}
