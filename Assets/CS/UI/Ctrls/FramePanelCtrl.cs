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
			DontDestroyOnLoad(gameObject);
			sunAndMoonImage = GetChildImage("sunAndMoonImage");
			timeText = GetChildText("timeText");
			settingBtn = GetChildButton("SettingBtn");
			EventTriggerListener.Get(settingBtn.gameObject).onClick = onClick;
			timeNames = Statics.GetTimeNames();
			_currentTimeIndex = -1;
			lastTimeIndex = _currentTimeIndex;
			currentAngle = 0;
			oldAngle = -1;
			angleRotateDate = DateTime.Now;
			angleRotateTimeout = 1f; //20秒旋转1度
//			resetTimeIndex();
		}

		void resetTimeIndex() {
			_currentTimeIndex = (int)Mathf.Floor(currentAngle / 30);
			_currentTimeIndex %= timeNames.Length;
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "SettingBtn":
				Messenger.Broadcast<bool>(NotifyTypes.ShowSettingPanel, true);
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
				sunAndMoonImage.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
				oldAngle = currentAngle;
				resetTimeIndex();
				timeText.text = String.Format("当前: {0}", CurrentTimeName);
			}
			if (_currentTimeIndex != lastTimeIndex) {
				lastTimeIndex = _currentTimeIndex;
				Messenger.Broadcast<int, float>(NotifyTypes.TimeIndexChanged, _currentTimeIndex, currentAngle);
			}
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
	}
}
