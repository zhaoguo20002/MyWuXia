using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Game {
	public class FramePanelCtrl : ComponentCore {
		Image sunAndMoonImage;
		Text timeText;
		Button settingBtn;
		Button viewTimesButton;
        Button enemyInfoBtn;
        Button backToCityBtn;
        Image enemyInfoRedPointImage;
        static FramePanelCtrl instance;

		static string[] timeNames;
		static int _currentTimeIndex;
		static int lastTimeIndex = -1;
		static float currentAngle;
		static float oldAngle;
		static DateTime angleRotateDate;
		float angleRotateTimeout;
		static bool canGo;
		/// <summary>
		/// The index of the current time.
		/// </summary>
		public static int CurrentTimeIndex {
			get {
				return _currentTimeIndex;
			}
		}

		/// <summary>
		/// Gets the name of the current time.
		/// </summary>
		/// <value>The name of the current time.</value>
		public static string CurrentTimeName {
			get {
				return timeNames[CurrentTimeIndex];
			}	
		}

		/// <summary>
		/// Gets the name of the time.
		/// </summary>
		/// <returns>The time name.</returns>
		/// <param name="index">Index.</param>
		public static string GetTimeName(int index) {
			index = index < 0 ? 0 : index;
			index %= timeNames.Length;
			return timeNames[index];
		}

		protected override void Init () {
			canGo = false;
			sunAndMoonImage = GetChildImage("sunAndMoonImage");
			timeText = GetChildText("timeText");
			settingBtn = GetChildButton("SettingBtn");
			EventTriggerListener.Get(settingBtn.gameObject).onClick = onClick;
			viewTimesButton = GetChildButton("viewTimesButton");
			EventTriggerListener.Get(viewTimesButton.gameObject).onClick = onClick;
            enemyInfoBtn = GetChildButton("EnemyInfoBtn");
            EventTriggerListener.Get(enemyInfoBtn.gameObject).onClick = onClick;
            backToCityBtn = GetChildButton("BackToCityBtn");
            EventTriggerListener.Get(backToCityBtn.gameObject).onClick = onClick;
            enemyInfoRedPointImage = GetChildImage("EnemyInfoRedPointImage");
			timeNames = Statics.GetTimeNames();
			_currentTimeIndex = -1;
			lastTimeIndex = _currentTimeIndex;
			currentAngle = 0;
			oldAngle = -1;
			angleRotateDate = DateTime.Now;
			angleRotateTimeout = 1f; //20秒旋转1度
//			resetTimeIndex();
            instance = this;
            refreshRedPointView();
		}

        void Start() {
            DontDestroyOnLoad(gameObject);
        }

		void resetTimeIndex() {
			_currentTimeIndex = (int)Mathf.Floor(currentAngle / 30);
			_currentTimeIndex %= timeNames.Length;
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "SettingBtn":
                    if (BattleFightPanelCtrl.Ctrl != null)
                    {
                        AlertCtrl.Show("请专心战斗");
                        break;
                    }
                    Messenger.Broadcast<bool>(NotifyTypes.ShowSettingPanel, true);
                    break;
                case "viewTimesButton":
                    AlertCtrl.Show("时辰顺序: \n午时, 未时, 申时, 酉时, 戌时, 亥时  子时, 丑时, 寅时, 卯时, 辰时, 巳时", null, "关闭");
                    break;
                case "EnemyInfoBtn":
                    EnemysInfoPanelCtrl.Show();
                    SetEnemyInfoRedPointFlag(false);
                    break;
                case "BackToCityBtn":
                    if (BattleFightPanelCtrl.Ctrl != null)
                    {
                        AlertCtrl.Show("请专心战斗");
                        break;
                    }
                    ConfirmCtrl.Show("现在马上回城？", () => {
                        Messenger.Broadcast(NotifyTypes.BackToCity);
                    });
                    break;
                default:
                    break;
            }
		}
		
		// Update is called once per frame
		void Update () {
			if (!canGo) {
				return;
			}
			DateTime newDate = DateTime.Now;
			double passSeconds = (newDate - angleRotateDate).TotalSeconds;
			if (passSeconds >= angleRotateTimeout) {
				angleRotateDate = newDate;
				currentAngle += Mathf.Floor((float)(passSeconds / angleRotateTimeout));
				currentAngle %= 360;
			}
			if (currentAngle != oldAngle) {
                sunAndMoonImage.transform.localEulerAngles = Vector3.Lerp(sunAndMoonImage.transform.localEulerAngles, new Vector3(0, 0, currentAngle), Time.deltaTime * 0.1f);
				oldAngle = currentAngle;
				resetTimeIndex();
				timeText.text = String.Format("当前: {0}", CurrentTimeName);
			}
			if (_currentTimeIndex != lastTimeIndex) {
				lastTimeIndex = _currentTimeIndex;
				Messenger.Broadcast<int, float>(NotifyTypes.TimeIndexChanged, _currentTimeIndex, currentAngle);
			}
		}

        void refreshRedPointView() {
            enemyInfoRedPointImage.gameObject.SetActive(!string.IsNullOrEmpty(PlayerPrefs.GetString("EnemyInfoRedPointFlag")));
        }

        public void SetEnemyInfoRedPointFlag(bool isShow) {
            PlayerPrefs.SetString("EnemyInfoRedPointFlag", isShow ? "true" : "");
            refreshRedPointView();
        }

		/// <summary>
		/// 设置时间戳
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <param name="ticks">Ticks.</param>
		public static void TimePlay(float angle, long ticks) {
			currentAngle = angle;
			oldAngle = -1;
//			resetTimeIndex();
//			lastTimeIndex = _currentTimeIndex;
			angleRotateDate = new DateTime(ticks);
			canGo = true;
		}

        public static void MakeSetEnemyInfoRedPointFlag(bool isShow) {
            if (instance != null)
            {
                instance.SetEnemyInfoRedPointFlag(isShow);
            }
        }
	}
}
