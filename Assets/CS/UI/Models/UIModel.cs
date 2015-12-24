using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		/// 全屏旋转控制脚本
		/// </summary>
		public static CameraVortex CameraVortexScript = null;
	}
}
