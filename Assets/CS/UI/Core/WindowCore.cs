using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Game {

	public class WindowCore<T, W> : ComponentCore {
		string _id;
		/// <summary>
		/// 窗口Id
		/// </summary>
		/// <value>The identifier.</value>
		public string Id {
			get {
				return _id;
			}
		}

		float _width;
		/// <summary>
		/// 窗口的宽度
		/// </summary>
		/// <value>The width.</value>
		public float Width {
			get {
				return _width;
			}
		}

		float _height;
		/// <summary>
		/// 窗口的高度
		/// </summary>
		/// <value>The height.</value>
		public float Height {
			get {
				return _height;
			}
		}

		/// <summary>
		/// Sets the identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public override void SetId(string id) {
			_id = id;
			RectTransform rectTrans = GetComponent<RectTransform>();
			Vector2 sizeDelta = rectTrans.sizeDelta;
			_width = sizeDelta.x;
			_height = sizeDelta.y;
		}

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <returns>The identifier.</returns>
		public override string GetId() {
			return Id;
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void Close() {
			CloseWindow(Id);
		}

		/// <summary>
		/// Moves the out.
		/// </summary>
		public void MoveOut() {
			transform.DOLocalMoveX (Screen.width, 0.5f).OnComplete (() => {
				Close();
			});
		}

		/// <summary>
		/// Moves the in.
		/// </summary>
		public void MoveIn() {
			transform.DOLocalMoveX (0, 0.5f);
		}

		/// <summary>
		/// 垂直移动动画
		/// </summary>
		/// <param name="moveY">Move y.</param>
		public void MoveVertical(float moveY, TweenCallback callback = null) {
			Tweener tw = transform.DOLocalMoveY(transform.localPosition.y + moveY, 0.5f);
			if (callback != null) {
				tw.OnComplete(callback);
			}
		}

		/// <summary>
		/// 水平移动动画
		/// </summary>
		/// <param name="moveX">Move x.</param>
		public void MoveHorizontal(float moveX, TweenCallback callback = null) {
			Tweener tw = transform.DOLocalMoveX(transform.localPosition.x + moveX, 0.5f);
			if (callback != null) {
				tw.OnComplete(callback);
			}
		}

		static Dictionary<string, T> _ctrls;
		/// <summary>
		/// 控制器集合
		/// </summary>
		public static Dictionary<string, T> Ctrls {
			get {
				return _ctrls;
			}
		}

		static T _ctrl;
		/// <summary>
		/// 当前控制器
		/// </summary>
		public static T Ctrl {
			get {
				return _ctrl;
			}
		}

		/// <summary>
		/// 创建UI视图实例
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="offsetWidth">Offset width.</param>
		/// <param name="offsetHeight">Offset height.</param>
		/// <param name="index">Index.</param>
		protected static void InstantiateView(string path, string id = "", float offsetWidth = 0, float offsetHeight = 0, int index = 0) {
			if (UIModel.Windows == null) {
				UIModel.Windows = new Dictionary<string, GameObject>();
			}

			if (_ctrls == null) {
				_ctrls = new Dictionary<string, T>();
			}

			if (id == "") {
				id = typeof(T).ToString();	
			}
			if (!UIModel.Windows.ContainsKey(id)) {
				GameObject winObj = CreateUIPrefab(index == 0 ? UIModel.UICanvas.transform : UIModel.FrameCanvas.transform, path, offsetWidth, offsetHeight);
				if (winObj != null) {
					winObj.name = id;
					UIModel.Windows.Add(id, winObj);
					_ctrl = winObj.GetComponent<T>();
					_ctrls.Add(id, _ctrl);
					IWindowInterface iWindowInterface = (IWindowInterface)_ctrl;
					iWindowInterface.SetId(id);
				}
				Debug.LogWarning("InstantiateView - " + UIModel.Windows.Count + "," + _ctrls.Count + "," + (_ctrl));
			}
		}

		/// <summary>
		/// Destroies the view.
		/// </summary>
		/// <param name="id">Identifier.</param>
		private int timer = 1;//计时器

		protected static void DestroyView(string id) {
			if (UIModel.Windows != null && UIModel.Windows.ContainsKey(id)) {
				Destroy(UIModel.Windows[id]);
				UIModel.Windows.Remove(id);
				_ctrls.Remove(id);
				if (_ctrl != null && ((IWindowInterface)_ctrl).GetId() == id) {
					_ctrl = default(T);
				}
			}
			Debug.LogWarning("DestroyView - " + UIModel.Windows.Count + "," + _ctrls.Count + "," + (_ctrl));
		}

		/// <summary>
		/// Creates the user interface prefab.
		/// </summary>
		/// <returns>The user interface prefab.</returns>
		/// <param name="parent">Parent.</param>
		/// <param name="path">Path.</param>
		/// <param name="offsetWidth">Offset width.</param>
		/// <param name="offsetHeight">Offset height.</param>
		protected static GameObject CreateUIPrefab(Transform parent, string path, float offsetWidth = 0, float offsetHeight = 0) {
			GameObject winObj = Statics.GetPrefabClone(path);
			if (winObj != null) {
				RectTransform rectTrans = winObj.GetComponent<RectTransform>();
				Vector2 offsetMin;
				Vector2 offsetMax;
				Vector2 sizeDelta = rectTrans.sizeDelta;
				if (offsetWidth == 0 && offsetHeight == 0) {
					offsetMin = rectTrans.offsetMin;
					offsetMax = rectTrans.offsetMax;
				}
				else {
					offsetMin = new Vector2(offsetWidth, offsetHeight);
					offsetMax = new Vector2(offsetWidth, offsetHeight);
				}
				winObj.transform.SetParent(parent);
				rectTrans.localScale = Vector3.one;
				rectTrans.offsetMin = offsetMin;
				rectTrans.offsetMax = offsetMax;
				rectTrans.sizeDelta = sizeDelta;
				return winObj;
			}
			return null;
		}

		/// <summary>
		/// Closes the window.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void CloseWindow(string id = "") {
			DestroyView(id);
		}
	}

}
