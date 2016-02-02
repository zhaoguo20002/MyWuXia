using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Game {
	public class FramePanelCtrl : ComponentCore {
		Image sunAndMoonImage;
		Text timeText;
		Button testButton;
		Button testButton1;

		static string[] timeNames;
		static int _currentTimeIndex;
		float currentAngle;
		float oldAngle;
		DateTime angleRotateDate;
		float angleRotateTimeout;
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
			DontDestroyOnLoad(gameObject);
			sunAndMoonImage = GetChildImage("sunAndMoonImage");
			timeText = GetChildText("timeText");
			timeNames = new string[] { "午时", "未时", "申时", "酉时", "戌时", "亥时", "子时", "丑时", "寅时", "卯时", "辰时", "巳时" };
			_currentTimeIndex = 0;
			currentAngle = 0;
			oldAngle = -1;
			angleRotateDate = DateTime.Now;
			angleRotateTimeout = 20f; //20秒旋转1度

			testButton = GetChildButton("TestButton");
			EventTriggerListener.Get(testButton.gameObject).onClick += onClick;
			testButton1 = GetChildButton("TestButton1");
			EventTriggerListener.Get(testButton1.gameObject).onClick += onClick;
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "TestButton":
				Debug.LogWarning("战斗");
				Messenger.Broadcast<string>(NotifyTypes.CreateBattle, "");
				break;
			case "TestButton1":
				Debug.LogWarning("切换场景");
				Messenger.Broadcast<string>(NotifyTypes.GoToScene, "Area2");
				break;
			default:
				break;
			}
		}
		
		// Update is called once per frame
		void Update () {
			DateTime newDate = DateTime.Now;
			if ((newDate - angleRotateDate).TotalSeconds >= angleRotateTimeout) {
				angleRotateDate = newDate;
				currentAngle++;
				currentAngle %= 360;
			}
			if (currentAngle != oldAngle) {
				sunAndMoonImage.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
				oldAngle = currentAngle;
				_currentTimeIndex = (int)Mathf.Floor(currentAngle / 30);
				timeText.text = String.Format("当前: {0}", CurrentTimeName);
			}
		}
	}
}
