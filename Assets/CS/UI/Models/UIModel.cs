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
		/// 窗口集合
		/// </summary>
		public static Dictionary<string, GameObject> Windows = null;
	}

	public class UIModel<T> {
		/// <summary>
		/// 窗口控制器集合
		/// </summary>
		public static Dictionary<string, T> Ctrls;
	}
}
