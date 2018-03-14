using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DG.Tweening;
using System;

namespace Game {
	public class AreaMainPanelCtrl : WindowCore<AreaMainPanelCtrl, JArray> {
		string foodIcondId;
		int foodsNum;
		int foodsMax;
        string areaId;
		string areaName;

		Image foodIcon;
		Text foodProcessText;
		Button moveBtn;
		Image point01;
		Image upArrow;
		Image downArrow;
		Image leftArrow;
		Image rightArrow;
		Text positionText;
        Text areaNameText;
        List<Text> propsNumText;
        Image prop1BlockImage;
        Image prop2BlockImage;
        Image prop3BlockImage;
        Image prop4BlockImage;
        Text difficultyText;

		float date;
		float moveTimeout;

        List<PropData> propsData;

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
			areaNameText = GetChildText("areaNameText");
            propsNumText = new List<Text>() { 
                GetChildText("prop1NumText"),
                GetChildText("prop2NumText"),
                GetChildText("prop3NumText"),
                GetChildText("prop4NumText")
            };
            prop1BlockImage = GetChildImage("prop1BlockImage");
            EventTriggerListener.Get(prop1BlockImage.gameObject).onClick = onClick;
            prop2BlockImage = GetChildImage("prop2BlockImage");
            EventTriggerListener.Get(prop2BlockImage.gameObject).onClick = onClick;
            prop3BlockImage = GetChildImage("prop3BlockImage");
            EventTriggerListener.Get(prop3BlockImage.gameObject).onClick = onClick;
            prop4BlockImage = GetChildImage("prop4BlockImage");
            EventTriggerListener.Get(prop4BlockImage.gameObject).onClick = onClick;
			point01.gameObject.SetActive(false);
			date = Time.fixedTime;
			moveTimeout = 0.3f;

            propsData = new List<PropData>(){
                new PropData(PropType.NocturnalClothing, 0),
                new PropData(PropType.Bodyguard, 0),
                new PropData(PropType.LimePowder, 0),
                new PropData(PropType.Scout, 0)
            };

            difficultyText = GetChildText("difficultyText");
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "moveBtn":
                    if (!moveBtn.enabled)
                    {
                        return;
                    }
                    float newDate = Time.fixedTime;
                    if (newDate - date < moveTimeout)
                    {
                        return;
                    }
                    date = newDate;
                    float centerX = Screen.width * 0.5f;
                    float centerY = Screen.height * 0.5f;
                    float angle = Statics.GetAngle(Input.mousePosition.x, Input.mousePosition.y, centerX, centerY);
                    if (angle < 45 || angle >= 315)
                    {
                        Messenger.Broadcast<string, bool>(NotifyTypes.MoveOnArea, AreaTarget.Down, true);
                    }
                    else if (angle >= 45 && angle < 135)
                    {
                        Messenger.Broadcast<string, bool>(NotifyTypes.MoveOnArea, AreaTarget.Left, true);
                    }
                    else if (angle >= 135 && angle < 225)
                    {
                        Messenger.Broadcast<string, bool>(NotifyTypes.MoveOnArea, AreaTarget.Up, true);
                    }
                    else if (angle >= 225 && angle < 315)
                    {
                        Messenger.Broadcast<string, bool>(NotifyTypes.MoveOnArea, AreaTarget.Right, true);
                    }
                    break;
                case "prop1BlockImage":
//                    ConfirmCtrl.Show("一件夜行衣能避免一场野外战斗\n(观看一段视频可以获得1-2件)", () => {
//                        if (propsData[0].Num >= propsData[0].Max)
//                        {
//                            AlertCtrl.Show(string.Format("最多只能携带{0}件夜行衣", propsData[0].Max));
//                            return;;
//                        }
//                        int randomNum = UnityEngine.Random.Range(1, 3);
//                        Messenger.Broadcast<PropType, int>(NotifyTypes.AddProp, PropType.NocturnalClothing, randomNum);
//                        AlertCtrl.Show(string.Format("获得了{0}件夜行衣", randomNum));
//                    }, null, "观看", "不了");
                    PropsMallPanelCtrl.Show(propsData[0]);
                    break;
                case "prop2BlockImage":
//                    ConfirmCtrl.Show("一位镖师能够抵消一位侠客受伤\n(观看一段视频可以雇佣1-2位)", () => {
//                        if (propsData[1].Num >= propsData[1].Max)
//                        {
//                            AlertCtrl.Show(string.Format("最多只能雇佣{0}个镖师", propsData[1].Max));
//                            return;
//                        }
//                        int randomNum = UnityEngine.Random.Range(1, 3);
//                        Messenger.Broadcast<PropType, int>(NotifyTypes.AddProp, PropType.Bodyguard, randomNum);
//                        AlertCtrl.Show(string.Format("雇佣了{0}个镖师", randomNum));
//                    }, null, "观看", "不了");
                    PropsMallPanelCtrl.Show(propsData[1]);
                    break;
                case "prop3BlockImage":
//                    ConfirmCtrl.Show("石灰粉有50%概率能脱离战斗\n(观看一段视频可以获得1-2包)", () => {
//                        if (propsData[2].Num >= propsData[2].Max)
//                        {
//                            AlertCtrl.Show(string.Format("最多只能携带{0}包石灰粉", propsData[2].Max));
//                            return;
//                        }
//                        int randomNum = UnityEngine.Random.Range(1, 3);
//                        Messenger.Broadcast<PropType, int>(NotifyTypes.AddProp, PropType.LimePowder, randomNum);
//                        AlertCtrl.Show(string.Format("获得了{0}包石灰粉", randomNum));
//                    }, null, "观看", "不了");
                    PropsMallPanelCtrl.Show(propsData[2]);
                    break;
                case "prop4BlockImage":
//                    ConfirmCtrl.Show("探子可以追踪未知的任务目标\n(观看一段视频可以获得1-2个)", () => {
//                        if (propsData[3].Num >= propsData[3].Max)
//                        {
//                            AlertCtrl.Show(string.Format("最多只能拥有{0}个探子", propsData[3].Max));
//                            return;
//                        }
//                        int randomNum = UnityEngine.Random.Range(1, 3);
//                        Messenger.Broadcast<PropType, int>(NotifyTypes.AddProp, PropType.Scout, randomNum);
//                        AlertCtrl.Show(string.Format("获得了{0}个探子", randomNum));
//                    }, null, "观看", "不了");
                    PropsMallPanelCtrl.Show(propsData[3]);
                    break;
                default:
                    break;
            }
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
//			foodIcondId = data[0].ToString();
            foodIcondId = "600000";
			foodsNum = (int)data[1];
			foodsMax = (int)data[2];
            areaId = data[3].ToString();
            areaName = JsonManager.GetInstance().GetMapping<JObject>("AreaNames", areaId)["Name"].ToString();
			date = Time.fixedTime;
		}

        public void UpdateData(List<PropData> props) {
            if (props == null)
            {
                return;
            }
            PropData propData, findProps;
            for (int i = 0, len = propsData.Count; i < len; i++)
            {
                propData = propsData[i];
                findProps = props.Find(item => item.Type == propData.Type);
                if (findProps != null)
                {
                    propData.Num = findProps.Num;
                }
                else
                {
                    propData.Num = 0;
                }
            }
        }

        public void RefreshPropsView() {
            PropData propData;
            for (int i = 0, len = propsNumText.Count; i < len; i++)
            {
                if (propsData.Count > i)
                {
                    propsNumText[i].gameObject.SetActive(true);
                    propData = propsData[i];
                    propsNumText[i].text = string.Format("{0}/{1}", propData.Num, propData.Max);
                }
                else
                {
                    propsNumText[i].gameObject.SetActive(false);
                }
            }
        }

        public bool CostNocturnalClothing() {
            if (propsData[0].Num > 0)
            {
                propsData[0].Num--;
                DbManager.Instance.UseProp(PropType.NocturnalClothing, 1);
                RefreshPropsView();
                return true;
            }
            return false;
        }

		void refreshFoodProcess(bool isDuring = true) {
			foodProcessText.text = string.Format("{0}/{1}", foodsNum, foodsMax);
			if (isDuring) {
				foodProcessText.transform.localScale = Vector3.one;
				foodProcessText.transform.DOKill();
				foodProcessText.transform.DOPunchScale(Vector3.one, 0.2f, 1, 0.5f);
			}
		}

		public override void RefreshView () {
			foodIcon.sprite = Statics.GetIconSprite(foodIcondId);
			refreshFoodProcess(false);
			areaNameText.text = areaName;
            RefreshDifficultyView();
		}

        public void RefreshDifficultyView() {
            if (areaId == "Area31")
            {
                difficultyText.gameObject.SetActive(true);
                int difficulty = PlayerPrefs.GetInt("TowerDifficulty");
                switch (difficulty)
                {
                    case 0:
                        difficultyText.text = "量子强度：普通";
                        difficultyText.color = Color.white;
                        break;
                    case 1:
                        difficultyText.text = "量子强度：噩梦";
                        difficultyText.color = new Color(0.93f, 1, 0.33f);
                        break;
                    case 2:
                        difficultyText.text = "量子强度：绝望";
                        difficultyText.color = new Color(0.98f, 0.26f, 0.26f);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                difficultyText.gameObject.SetActive(false);
            }
        }

		public void UpdateFoods(int foodsnum) {
			foodsNum = foodsnum;
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
				InstantiateView("Prefabs/UI/MainTool/AreaMainPanelView", "AreaMainPanelCtrl");
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

		/// <summary>
		/// 刷新区域大地图干粮
		/// </summary>
		/// <param name="foodsnum">Foodsnum.</param>
		public static void MakeUpdateFoods(int foodsnum) {
			if (Ctrl != null) {
				Ctrl.UpdateFoods(foodsnum);
			}
		}

        public static void MakeUpdateProps(List<PropData> props) {
            if (Ctrl != null)
            {
                Ctrl.UpdateData(props);
                Ctrl.RefreshPropsView();
            }
        }

        /// <summary>
        /// 消耗夜行衣
        /// </summary>
        /// <returns><c>true</c>, if cost nocturnal clothing was made, <c>false</c> otherwise.</returns>
        public static bool MakeCostNocturnalClothing() {
            if (Ctrl != null)
            {
                return Ctrl.CostNocturnalClothing();
            }
            return false;
        }

        /// <summary>
        /// 刷新通天塔强度
        /// </summary>
        public static void MakeRefreshDifficultyView() {
            if (Ctrl != null)
            {
                Ctrl.RefreshDifficultyView();
            }
        }
	}
}
