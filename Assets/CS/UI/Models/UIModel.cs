using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using System;
using System.Reflection;

namespace Game {
	public class UIModel {
		/// <summary>
		/// The font canvas.
		/// </summary>
		public static GameObject FontCanvas = null;
		/// <summary>
		/// The frame canvas.
		/// </summary>
		public static GameObject FrameCanvas = null;
		/// <summary>
		/// The user interface canvas.
		/// </summary>
		public static GameObject UICanvas = null;
		/// <summary>
		/// UI摄像机对象
		/// </summary>
		public static GameObject UICamera = null;
		/// <summary>
		/// 窗口集合
		/// </summary>
		public static Dictionary<string, GameObject> Windows = null;
		/// <summary>
		/// 窗口类型集合
		/// </summary>
		public static Dictionary<string, string> AllWindowTypeMapping = null;
		/// <summary>
		/// 全屏旋转控制脚本
		/// </summary>
		public static CameraVortex CameraVortexScript = null;
		/// <summary>
		/// 景深脚本
		/// </summary>
		public static DepthOfField CameraDepthOfFieldScript = null;
		/// <summary>
		/// 说话气泡脚本
		/// </summary>
		public static DialogMsgPop DialogMsgPopScript = null;

		/// <summary>
		/// 关闭所有窗口
		/// </summary>
		public static void CloseAllWindows() {
			if (AllWindowTypeMapping == null) {
				return;
			}
			List<MethodInfo> wins = new List<MethodInfo>();
			Type t;
			MethodInfo method;
			string ctrlType;
			foreach(string id in AllWindowTypeMapping.Keys) {
				ctrlType = AllWindowTypeMapping[id];
				t = Type.GetType(ctrlType);
				if (t != null) {
					method = t.GetMethod("Hide");
					if (method != null) {
						wins.Add(method);
					}
				}
			}
			for (int i = 0; i < wins.Count; i++) {
				wins[i].Invoke(null, null);
			}
			wins.Clear();
			AllWindowTypeMapping.Clear();
		}
	}
}
