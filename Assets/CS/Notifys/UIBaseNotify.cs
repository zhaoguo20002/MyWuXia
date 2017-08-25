using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using System;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 初始化UI组件的根
		/// </summary>
		public static string InitUISystem;
		/// <summary>
		/// 清空所有文字UI
		/// </summary>
		public static string ClearAllFonts;
		/// <summary>
		/// 播放全屏旋转过场动画
		/// </summary>
		public static string PlayCameraVortex;
		/// <summary>
		/// 显示隐藏景深特效脚本
		/// </summary>
		public static string DisplayCameraDepthOfField;
		/// <summary>
		/// 时辰更新通知消息
		/// </summary>
		public static string TimeIndexChanged;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void UINotifyInit() {
			//订阅 初始化UI组件的根 消息
			Messenger.AddListener(NotifyTypes.InitUISystem, () => {
				if (UIModel.UICanvas == null) {
					UIModel.UICanvas = GameObject.Find("UICanvas");
				}
				if (UIModel.FrameCanvas == null) {
					UIModel.FrameCanvas = GameObject.Find("FrameCanvas");
				}
				if (UIModel.FontCanvas == null) {
					UIModel.FontCanvas = GameObject.Find("FontCanvas");
				}
				if (UIModel.UICamera == null) {
					UIModel.UICamera = GameObject.Find("UICamera");
					if (UIModel.UICamera != null) {
						UIModel.CameraVortexScript = UIModel.UICamera.GetComponent<CameraVortex>();
						UIModel.CameraDepthOfFieldScript = UIModel.UICamera.GetComponent<DepthOfField>();
						Messenger.Broadcast<bool>(NotifyTypes.DisplayCameraDepthOfField, false);
					}
				}
			});
			Messenger.Broadcast(NotifyTypes.InitUISystem);

			//订阅 清空所有文字UI 消息
			Messenger.AddListener(NotifyTypes.ClearAllFonts, () => {
				foreach (Transform child in UIModel.FontCanvas.transform) {
					MonoBehaviour.Destroy(child.gameObject);
				}
			});

			Messenger.AddListener<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, (halfCallback, endCallback) => {
                if (string.IsNullOrEmpty(PlayerPrefs.GetString("EffectSwitchOffFlag"))) {
                    if (UIModel.CameraVortexScript != null) {
                        UIModel.CameraVortexScript.StartPlay(halfCallback, endCallback);
                        
                    }
                    else if (endCallback != null) {
                        endCallback();
                    }
                }
                else {
                    if (halfCallback != null) {
                        halfCallback();
                    }
                    if (endCallback != null) {
                        endCallback();
                    }
                }
			});

			Messenger.AddListener<bool>(NotifyTypes.DisplayCameraDepthOfField, (display) => {
				if (UIModel.CameraDepthOfFieldScript != null) {
					UIModel.CameraDepthOfFieldScript.enabled = display;
				}
			});

			Messenger.AddListener<int, float>(NotifyTypes.TimeIndexChanged, (index, angle) => {
				DbManager.Instance.UpdateTimeTicks(angle, DateTime.Now.Ticks); //更新时辰时间戳
//				//时辰更新时需要检测城镇场景中的任务列表是否有更新
//				if (UserModel.CurrentUserData != null) {
//					Messenger.Broadcast<string>(NotifyTypes.GetTaskListDataInCityScene, UserModel.CurrentUserData.CurrentCitySceneId);
//					//当任务列表打开的时候每个时辰切换都要再刷新下任务列表，更新任务状态
//					if (TaskListPanelCtrl.Ctrl != null) {
//						Messenger.Broadcast(NotifyTypes.GetTaskListData);
//					}
//						
//				}
                Messenger.Broadcast(NotifyTypes.RefreshTaskInfos);
			});

		}
	}
}
