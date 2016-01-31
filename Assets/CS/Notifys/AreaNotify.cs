using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 大地图相关数据初始化消息
		/// </summary>
		public static string AreaInit;
		/// <summary>
		///  大地图数据回收
		/// </summary>
		public static string AreaDestroyed;
		/// <summary>
		/// 请求大地图主界面数据
		/// </summary>
		public static string CallAreaMainPanelData;
		/// <summary>
		/// 请求大地图主界面数据回调
		/// </summary>
		public static string CallAreaMainPanelDataEcho;
		/// <summary>
		/// 关闭大地图主界面
		/// </summary>
		public static string HideAreaMainPanel;
		/// <summary>
		/// 在大地图上移动前需要先判定下体力够不够
		/// </summary>
		public static string MoveOnArea;
		/// <summary>
		/// 在大地图上移动回调
		/// </summary>
		public static string MoveOnAreaEcho;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void AreaNotifyInit() {
			Messenger.AddListener<AreaTarget>(NotifyTypes.AreaInit, (target) => {
				
				AreaModel.CurrentTarget = target;
				//打开大地图UI交互界面
				Messenger.Broadcast(NotifyTypes.CallAreaMainPanelData);
			});

			Messenger.AddListener(NotifyTypes.AreaDestroyed, () => {
				if(AreaModel.CurrentTarget != null && AreaModel.CurrentTarget.gameObject != null) {
					MonoBehaviour.Destroy(AreaModel.CurrentTarget.gameObject);
					AreaModel.CurrentTarget = null;
				}
				Messenger.Broadcast(NotifyTypes.HideAreaMainPanel);
			});

			Messenger.AddListener(NotifyTypes.CallAreaMainPanelData, () => {
				Messenger.Broadcast<JArray>(NotifyTypes.CallAreaMainPanelDataEcho, new JArray("500000", 966, 999));
			});

			Messenger.AddListener<JArray>(NotifyTypes.CallAreaMainPanelDataEcho, (data) => {
				AreaMainPanelCtrl.Show(data);
			});

			Messenger.AddListener(NotifyTypes.HideAreaMainPanel, () => {
				AreaMainPanelCtrl.Hide();
			});

			Messenger.AddListener<string>(NotifyTypes.MoveOnArea, (direction) => {
				//判定体力是否足够移动
				Messenger.Broadcast<string, int>(NotifyTypes.MoveOnAreaEcho, direction, 666);
			});

			Messenger.AddListener<string, int>(NotifyTypes.MoveOnAreaEcho, (direction, foodsNum) => {
				AreaMainPanelCtrl.MakeArrowShow(direction, foodsNum);
				AreaModel.CurrentTarget.Move(direction);
			});
		}
	}
}