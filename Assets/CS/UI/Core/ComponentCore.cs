using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

namespace Game {
	public class ComponentCore : MonoBehaviour, IWindowInterface {
		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init() {
			
		}

		protected void Awake() {
			Init();
		}

		/// <summary>
		/// Sets the identifier.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public virtual void SetId(string id) {
			
		}

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <returns>The identifier.</returns>
		public virtual string GetId() {
			return "";
		}

		/// <summary>
		/// Updates the data.
		/// </summary>
		/// <param name="obj">Object.</param>
		public virtual void UpdateData(object obj) {
			
		}

		/// <summary>
		/// Refreshs the view.
		/// </summary>
		public virtual void RefreshView() {
			
		}

		/// <summary>
		/// Sets the child path.
		/// </summary>
		/// <param name="path">Path.</param>
		public virtual void SetChildPath(string path) {
			
		}

		/// <summary>
        /// 查找指定父对象下的后代对象
        /// </summary>
        /// <param name="go">父对象</param>
        /// <param name="name">要查找的后代对象的名称</param>
        /// <param name="includeSelf">查找范围是否包括父对象本身</param>
        /// <returns></returns>
        public static GameObject GetChild(GameObject go, string name, bool includeSelf = false)
        {
            if ((go != null) && !string.IsNullOrEmpty(name))
            {
                if (includeSelf && (go.name == name))
                {
                    return go;
                }
                Transform transform = go.transform;
                Transform transform2 = null;
                if (name.IndexOf('/') != -1)
                {
                    var arr = name.Split('/').ToList();
                    if (arr.Count > 0)
                    {
                         var ch = GetChild(go, arr[0], includeSelf);
                         arr.RemoveAt(0);
                         return GetChild(ch, string.Join("/", arr.ToArray()), includeSelf);
                    }
                    return null;
                }
                transform2 = transform.FindChild(name);
                if (transform2 != null)
                {
                    return transform2.gameObject;
                }
                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject obj = GetChild(transform.GetChild(i).gameObject, name, false);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

		/// <summary>
		/// 查找指定父对象下的后代对象
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="name">Name.</param>
		public GameObject GetChild(string name) {
			return GetChild(gameObject, name);
		}

		/// <summary>
        /// 获取子对象的组件
        /// </summary>
        /// <typeparam name="T">组件类<peparam>
        /// <param name="go">父对象</param>
        /// <param name="name">子对象名称</param>
        /// <returns></returns>
        public static T GetChildComponent<T>(GameObject go, string name) where T : Component
        {
            if ((go != null) && !string.IsNullOrEmpty(name))
            {
                GameObject obj = GetChild(go, name, false);
                if (obj != null)
                {
                    return obj.GetComponent<T>();
                }
            }
            return null;
        }
		
		/// <summary>
		/// Gets the child text.
		/// </summary>
		/// <returns>The child text.</returns>
		/// <param name="name">Name.</param>
		public Text GetChildText(string name) {
			return GetChildComponent<Text>(gameObject, name);
		}
		
		/// <summary>
		/// Gets the child button.
		/// </summary>
		/// <returns>The child button.</returns>
		/// <param name="name">Name.</param>
		public Button GetChildButton(string name) {
			return GetChildComponent<Button>(gameObject, name);
		}
		
		/// <summary>
		/// Gets the child image.
		/// </summary>
		/// <returns>The child image.</returns>
		/// <param name="name">Name.</param>
		public Image GetChildImage(string name) {
			return GetChildComponent<Image>(gameObject, name);
		}

		/// <summary>
		/// Gets the child canvas group.
		/// </summary>
		/// <returns>The child canvas group.</returns>
		/// <param name="name">Name.</param>
		public CanvasGroup GetChildCanvasGroup(string name) {
			return GetChildComponent<CanvasGroup>(gameObject, name);
		}

		/// <summary>
		/// Gets the child grid layout group.
		/// </summary>
		/// <returns>The child grid layout group.</returns>
		/// <param name="name">Name.</param>
		public GridLayoutGroup GetChildGridLayoutGroup(string name) {
			return GetChildComponent<GridLayoutGroup>(gameObject, name);
		}

		/// <summary>
		/// 控制按钮是否可点击
		/// </summary>
		/// <param name="btn">Button.</param>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		public void MakeButtonEnable(Button btn, bool enabled) {
			btn.enabled = enabled;
			ColorBlock cb = btn.colors;
			if (btn.enabled) {
				cb.normalColor = new Color(1, 1, 1, 1);
				cb.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1);
				cb.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1);
				cb.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1);
			}
			else {
				cb.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1);
				cb.normalColor = cb.disabledColor;
				cb.highlightedColor = cb.disabledColor;
				cb.pressedColor = cb.disabledColor;
			}
			btn.colors = cb;
		}

		/// <summary>
		/// 改变按钮颜色
		/// </summary>
		/// <param name="btn">Button.</param>
		/// <param name="color">Color.</param>
		public void ChangeButtonColor(Button btn, Color color) {
			if (btn.enabled) {
				ColorBlock cb = btn.colors;
				cb.normalColor = color;
				cb.highlightedColor = color;
				cb.pressedColor = color;
				cb.disabledColor = color;
				btn.colors = cb;
			}
		}

		/// <summary>
		/// 将按钮颜色恢复默认
		/// </summary>
		/// <param name="btn">Button.</param>
		public void ChangeButtonColorToDefault(Button btn) {
			if (btn.enabled) {
				ColorBlock cb = btn.colors;
				cb.normalColor = new Color(1, 1, 1, 1);
				cb.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1);
				cb.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1);
				cb.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1);
				btn.colors = cb;
			}
		}

		/// <summary>
		/// 将子控件添加到对应的父控件上
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		public void MakeToParent(Transform parent, Transform child) {
			child.SetParent(parent);
			child.localScale = Vector3.one;
		}
	}
}
