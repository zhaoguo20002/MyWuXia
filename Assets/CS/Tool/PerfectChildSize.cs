using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	/// <summary>
	/// 适配自对象的宽度
	/// </summary>
	public class PerfectChildSize : MonoBehaviour {
		public RectTransform ChildRect;
		public float PaddingLeft = 5;
		public float PaddingRight = 5;
		public float PaddingTop = 5;
		public float PaddingBottom = 5;
		RectTransform rect;

		// Use this for initialization
		void Start () {
			if (ChildRect == null) {
				enabled = false;
				return;
			}
			rect = GetComponent<RectTransform>();
		}

		void Update() {
			if (rect != null) {
				if (ChildRect.sizeDelta.x != 0 && ChildRect.sizeDelta.y != 0) {
					rect.sizeDelta = new Vector2(ChildRect.sizeDelta.x + PaddingLeft + PaddingRight, ChildRect.sizeDelta.y + PaddingTop + PaddingBottom);
					ChildRect.anchoredPosition = new Vector2(ChildRect.anchoredPosition.x + PaddingLeft, ChildRect.anchoredPosition.y - PaddingTop);
					Destroy(this);
					ContentSizeFitter fitter = ChildRect.gameObject.GetComponent<ContentSizeFitter>();
					if (fitter != null) {
						Destroy(fitter);
					}
				}
			}
		}
	}
}
