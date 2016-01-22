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
		/// <summary>
		/// 禁用/启用队伍信息面板交互
		/// </summary>
		public static string MakeRoleInfoPanelDisable;

	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void RoleNotifyInit() {

			Messenger.AddListener<bool>(NotifyTypes.CallRoleInfoPanelData, (isfighting) => {
				DbManager.Instance.CallRoleInfoPanelData(isfighting);
			});

			Messenger.AddListener<JObject, bool>(NotifyTypes.CallRoleInfoPanelDataEcho, (obj, isfighting) => {
				RoleInfoPanelCtrl.Show((JArray)obj["data"], isfighting);
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

			Messenger.AddListener<bool>(NotifyTypes.MakeRoleInfoPanelDisable, (dis) => {
				RoleInfoPanelCtrl.MakeDisable(dis);
			});
		}
	}
}
