using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DG.Tweening;

namespace Game {
	public class AreaMainPanelCtrl : WindowCore<AreaMainPanelCtrl, JArray> {
		string foodIcondId;
		int foodsNum;
		int foodsMax;

		Image foodIcon;
		Text foodProcessText;
		Button moveBtn;
		Image point01;
		Image upArrow;
		Image downArrow;
		Image leftArrow;
		Image rightArrow;
		Text positionText;

		protected override void Init () {
			foodIcon = GetChildImage("foodIcon");
			foodProcessText = GetChildText("foodProcessText");
			moveBtn = GetChildButton("moveBtn");
			EventTriggerListener.Get(moveBtn.gameObject).onClick += onClick;
			point01 = GetChildImage("point01");
			upArrow = GetChildImage("upArrow");
			downArrow = GetChildImage("downArrow");
			leftArrow = GetChildImage("leftArrow");
			rightArrow = GetChildImage("rightArrow");
			positionText = GetChildText("positionText");
			point01.gameObject.SetActive(false);
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
			case "moveBtn":
				float centerX = Screen.width * 0.5f;
				float centerY = Screen.height * 0.5f;
				float angle = Statics.GetAngle(Input.mousePosition.x, Input.mousePosition.y, centerX, centerY);
				if (angle < 45 || angle >= 315) {
					Messenger.Broadcast<string>(NotifyTypes.MoveOnArea, AreaTarget.Down);
				}
				else if (angle >= 45 && angle < 135) {
					Messenger.Broadcast<string>(NotifyTypes.MoveOnArea, AreaTarget.Left);
				}
				else if (angle >= 135 && angle < 225) {
					Messenger.Broadcast<string>(NotifyTypes.MoveOnArea, AreaTarget.Up);
				}
				else if (angle >= 225 && angle < 315) {
					Messenger.Broadcast<string>(NotifyTypes.MoveOnArea, AreaTarget.Right);
				}
				break;
			default:
				break;
			}
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
			foodIcondId = data[0].ToString();
			foodsNum = (int)data[1];
			foodsMax = (int)data[2];
		}

		void refreshFoodProcess() {
			foodProcessText.text = string.Format("{0}/{1}", foodsNum, foodsMax);
		}

		public override void RefreshView () {
			foodIcon.sprite = Statics.GetIconSprite(foodIcondId);
			refreshFoodProcess();
		}

		/// <summary>
		/// 显示箭头动画
		/// </summary>
		/// <param name="direction">Direction.</param>
		/// <param name="foodsnum">Foodsnum.</param>
		public void ArrowShow(string direction, int foodsnum) {
			foodsNum = foodsnum;
			doKill();
			point01.gameObject.SetActive(true);
			point01.DOFade(0, 0);
			upArrow.DOFade(0, 0);
			downArrow.DOFade(0, 0);
			leftArrow.DOFade(0, 0);
			rightArrow.DOFade(0, 0);

			point01.DOFade(1, 0.5f).OnComplete(() => {
				point01.DOFade(0, 0.5f).OnComplete(() => {
					point01.gameObject.SetActive(false);
				});
			});
			switch (direction) {
			case "up":
				upArrow.DOFade(1, 0.5f).OnComplete(() => {
					upArrow.DOFade(0, 0.5f);
				});
				break;
			case "down":
				downArrow.DOFade(1, 0.5f).OnComplete(() => {
					downArrow.DOFade(0, 0.5f);
				});
				break;
			case "left":
				leftArrow.DOFade(1, 0.5f).OnComplete(() => {
					leftArrow.DOFade(0, 0.5f);
				});
				break;
			case "right":
				rightArrow.DOFade(1, 0.5f).OnComplete(() => {
					rightArrow.DOFade(0, 0.5f);
				});
				break;
			default:
				break;
			}
			refreshFoodProcess();
		}

		/// <summary>
		/// 设置显示坐标
		/// </summary>
		/// <param name="pos">Position.</param>
		public void SetPosition(Vector2 pos) {
			positionText.text = string.Format("x: {0} - y: {1}", pos.x, pos.y);
		}

		void doKill() {
			point01.DOKill(true);
			upArrow.DOKill();
			downArrow.DOKill();
			leftArrow.DOKill();
			rightArrow.DOKill();
		}

		void OnDestroy() {
			doKill();
		}

		public static void Show(JArray data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/AreaMainPanelView", "AreaMainPanelCtrl");
			}
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}

		/// <summary>
		/// 控制箭头动画显示
		/// </summary>
		/// <param name="direction">Direction.</param>
		/// <param name="foodsnum">Foodsnum.</param>
		public static void MakeArrowShow(string direction, int foodsnum) {
			if(Ctrl != null) {
				Ctrl.ArrowShow(direction, foodsnum);
			}
		}

		/// <summary>
		/// 设置显示坐标
		/// </summary>
		/// <param name="pos">Position.</param>
		public static void MakeSetPosition(Vector2 pos) {
			if (Ctrl != null) {
				Ctrl.SetPosition(pos);
			}
		}
	}
}
