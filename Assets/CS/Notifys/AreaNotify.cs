using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
		/// <summary>
		/// 获取区域动态事件列表
		/// </summary>
		public static string GetActiveEventsInArea;
		/// <summary>
		/// 获取区域动态事件列表回调
		/// </summary>
		public static string GetActiveEventsInAreaEcho;
		/// <summary>
		/// 缓存将要禁用的事件
		/// </summary>
		public static string HandleDisableEvent;
		/// <summary>
		/// 根据战斗胜负处理是否添加临时禁用事件
		/// </summary>
		public static string ReleaseDisableEvent;
		/// <summary>
		/// 添加禁用事件
		/// </summary>
		public static string PushDisableEvent;
		/// <summary>
		/// 清空临时禁用事件
		/// </summary>
		public static string ClearDisableEventIdMapping;
		/// <summary>
		/// 吃干粮
		/// </summary>
		public static string EatFood;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void AreaNotifyInit() {
			Messenger.AddListener<AreaTarget, AreaMain>(NotifyTypes.AreaInit, (target, main) => {
				AreaModel.CurrentTarget = target;
				AreaModel.AreaMainScript = main;
				//加载动态事件列表
				Messenger.Broadcast<string>(NotifyTypes.GetActiveEventsInArea, UserModel.CurrentUserData.CurrentAreaSceneName);
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
				Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.CallUserData, (userData) => {
					Messenger.Broadcast<JArray>(NotifyTypes.CallAreaMainPanelDataEcho, new JArray(userData.AreaFood.IconId, userData.AreaFood.Num, userData.AreaFood.MaxNum, userData.CurrentAreaSceneName));
					Vector2 pos = new Vector2(userData.CurrentAreaX, userData.CurrentAreaY);
					Messenger.Broadcast<Vector2, bool>(NotifyTypes.SetAreaPosition, pos, false);
				});
			});

			Messenger.AddListener<JArray>(NotifyTypes.CallAreaMainPanelDataEcho, (data) => {
				AreaMainPanelCtrl.Show(data);
			});

			Messenger.AddListener(NotifyTypes.HideAreaMainPanel, () => {
				AreaMainPanelCtrl.Hide();
			});

			Messenger.AddListener<string, bool>(NotifyTypes.MoveOnArea, (direction, duringMove) => {
				//移动前先判断移动目的地是否有战斗
				Vector2 nextMovePosition = AreaModel.CurrentTarget.GetNextMovePosition(direction);
				//判断前方是否是障碍
				tk2dRuntime.TileMap.TileInfo groundTile = AreaModel.CurrentTarget.GetTileInfo((int)nextMovePosition.x, (int)nextMovePosition.y, 0);
				//判断禁止通过的碰撞区域
				if (groundTile == null || groundTile.stringVal == "obstacle") {
					return;
				}
				string fightEventId = string.Format("{0}_{1}_{2}", UserModel.CurrentUserData.CurrentAreaSceneName, (int)nextMovePosition.x, (int)nextMovePosition.y);
				EventData data;

				if (AreaMain.ActiveAreaEventsMapping.ContainsKey(fightEventId)) {
					data = AreaMain.ActiveAreaEventsMapping[fightEventId];
					if (data.Type == SceneEventType.Battle) {
						ConfirmCtrl.Show("前方将有恶战，是否继续？", () => {
							Messenger.Broadcast<string>(NotifyTypes.CreateBattle, data.EventId);
						}, null, "动手", "撤退");
						return;
					}
				}
				else if (AreaMain.StaticAreaEventsMapping.ContainsKey(fightEventId)) {
					//判断静态事件是否禁用
					if (!AreaMain.DisableEventIdMapping.ContainsKey(fightEventId)) {
						data = AreaMain.StaticAreaEventsMapping[fightEventId];
						if (data.Type == SceneEventType.Battle) {
							ConfirmCtrl.Show("前方将有恶战，是否继续？", () => {
								Messenger.Broadcast<string>(NotifyTypes.CreateBattle, data.EventId);
								//处理静态事件的预禁用操作
								EventData disableEvent = new EventData();
								disableEvent.Id = fightEventId;
								disableEvent.Type = SceneEventType.DisableEvent;
								disableEvent.X = (int)nextMovePosition.x;
								disableEvent.Y = (int)nextMovePosition.y;
								Messenger.Broadcast<EventData>(NotifyTypes.HandleDisableEvent, disableEvent);
							}, null, "动手", "撤退");
							return;
						}
						else if (data.OpenType != SceneEventOpenType.None) {
							//静态事件有一个开启判定类型
							switch(data.OpenType) {
							case SceneEventOpenType.FightWined:
								if (!DbManager.Instance.IsFightWined(data.OpenKey)) {
									ConfirmCtrl.Show(string.Format("前方有强敌守卫，是否硬闯?{0}", data.Notice), () => {
										Messenger.Broadcast<string>(NotifyTypes.CreateBattle, data.OpenKey);
									}, null, "动手", "撤退");
									return;
								}
								break;
							case SceneEventOpenType.NeedItem:
								if (DbManager.Instance.GetUsedItemNumByItemId(data.OpenKey) <= 0) {
									ItemData item = JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", data.OpenKey);
									if (DbManager.Instance.GetItemNumByItemId(data.OpenKey) > 0) {
										ConfirmCtrl.Show(string.Format("需要交出<color=\"#1ABDE6\">{0}</color>才能通过", item.Name), () => {
											if (DbManager.Instance.CostItemFromBag(data.OpenKey, 1)) {
												DbManager.Instance.UpdateUsedItemRecords(data.OpenKey, 1);
											}
										}, null, "给", "不给");
									}
									else {
										AlertCtrl.Show(string.Format("行囊里没有<color=\"#1ABDE6\">{0}</color>，不能过去！{1}", item.Name, data.Notice));
									}
									return;
								}
								break;
							default:
								break;
							}
						}
					}
				}
				//判定体力是否足够移动	
				DbManager.Instance.MoveOnArea(direction, duringMove);
//				Messenger.Broadcast<string, int, bool>(NotifyTypes.MoveOnAreaEcho, direction, 666, duringMove);
			});

			Messenger.AddListener<string, int, bool>(NotifyTypes.MoveOnAreaEcho, (direction, foodsNum, duringMove) => {
				AreaMainPanelCtrl.MakeArrowShow(direction, foodsNum);
				Vector2 pos = AreaModel.CurrentTarget.Move(direction, foodsNum > 0, duringMove);
				AreaMainPanelCtrl.MakeSetPosition(pos);
				if (foodsNum <= 0) {
					AlertCtrl.Show("干粮耗尽, 先回城镇休整", () => {
						Messenger.Broadcast(NotifyTypes.BackToCity);
					});
				}
			});

			Messenger.AddListener<Vector2, bool>(NotifyTypes.SetAreaPosition, (pos, doEvent) => {
				AreaMainPanelCtrl.MakeSetPosition(pos);
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.SetPosition(pos, doEvent);
				}
			});

			Messenger.AddListener<string>(NotifyTypes.GetActiveEventsInArea, (sceneId) => {
				DbManager.Instance.GetActiveEventsInArea(sceneId);
			});

			Messenger.AddListener<List<EventData>>(NotifyTypes.GetActiveEventsInAreaEcho, (events) => {
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.UpdateActiveAreaEventsData(events);
					AreaModel.AreaMainScript.RefreshActiveAreaEventsView();
				}
			});

			Messenger.AddListener<EventData>(NotifyTypes.HandleDisableEvent, (ev) => {
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.HandleDisableEvent(ev);
				}
			});

			Messenger.AddListener<bool>(NotifyTypes.ReleaseDisableEvent, (win) => {
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.ReleaseDisableEvent(win);
				}
			});

			Messenger.AddListener<string, EventData>(NotifyTypes.PushDisableEvent, (eventId, eventData) => {
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.PushDisableEvent(eventId, eventData);
				}
			});

			Messenger.AddListener(NotifyTypes.ClearDisableEventIdMapping, () => {
				if (AreaModel.AreaMainScript != null) {
					AreaModel.AreaMainScript.ClearDisableEventIdMapping();
				}
			});

			Messenger.AddListener<int>(NotifyTypes.EatFood, (num) => {
				int eatNum = DbManager.Instance.EatFood(num);
				AlertCtrl.Show(string.Format("打开包袱发现<color=\"#00FF00\">{0}个干粮</color>{1}", num, eatNum != num ? "\n你的行囊只装得下<color=\"#FF0000\">" + eatNum + "个</color>" : ""));
			});
		}
	}
}