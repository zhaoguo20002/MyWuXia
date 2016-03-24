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
		/// <summary>
		/// 显示当前的大地图坐标
		/// </summary>
		public static string SetAreaPosition;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void AreaNotifyInit() {
			Messenger.AddListener<AreaTarget, AreaMain>(NotifyTypes.AreaInit, (target, main) => {
				AreaModel.CurrentTarget = target;
				AreaModel.AreaMainScript = main;
				//打开大地图UI交互界面
				Messenger.Broadcast(NotifyTypes.CallAreaMainPanelData);
				//如果当前所处的位置是城镇,则进入城镇
				if (UserModel.CurrentUserData.PositionStatu == UserPositionStatusType.InCity) {
					Messenger.Broadcast<string>(NotifyTypes.EnterCityScene, UserModel.CurrentUserData.CurrentCitySceneId);
				}
			});

			Messenger.AddListener(NotifyTypes.AreaDestroyed, () => {
				if(AreaModel.CurrentTarget != null && AreaModel.CurrentTarget.gameObject != null) {
					MonoBehaviour.Destroy(AreaModel.CurrentTarget.gameObject);
					AreaModel.CurrentTarget = null;
				}
				Messenger.Broadcast(NotifyTypes.HideAreaMainPanel);
			});

			Messenger.AddListener(NotifyTypes.CallAreaMainPanelData, () => {
				Messenger.Broadcast<JArray>(NotifyTypes.CallAreaMainPanelDataEcho, new JArray(UserModel.CurrentUserData.AreaFood.IconId, UserModel.CurrentUserData.AreaFood.Num, UserModel.CurrentUserData.AreaFood.MaxNum));
				Vector2 pos = new Vector2(UserModel.CurrentUserData.CurrentAreaX, UserModel.CurrentUserData.CurrentAreaY);
				Messenger.Broadcast<Vector2, bool>(NotifyTypes.SetAreaPosition, pos, false);
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
				Vector2 pos = AreaModel.CurrentTarget.Move(direction);
				AreaMainPanelCtrl.MakeSetPosition(pos);
			});

			Messenger.AddListener<Vector2, bool>(NotifyTypes.SetAreaPosition, (pos, doEvent) => {
				AreaMainPanelCtrl.MakeSetPosition(pos);
				AreaModel.AreaMainScript.SetPosition(pos, doEvent);
			});
		}
	}
}