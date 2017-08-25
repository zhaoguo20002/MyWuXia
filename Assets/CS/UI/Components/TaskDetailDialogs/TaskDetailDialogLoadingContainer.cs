using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogLoadingContainer : MonoBehaviour {
		public Image Point0;
		public Image Point1;
		public Image Point2;
		System.Action _callback;
		float date = -1;
		float timeout = Random.Range(1.5f, 2.5f);

		void Start() {
			Vector2 anchorPos0 = Point0.rectTransform.anchoredPosition;
			Point0.rectTransform.DOAnchorPos(new Vector2(anchorPos0.x, anchorPos0.y + 15), 0.3f).SetEase(Ease.InExpo).SetLoops(-1, LoopType.Yoyo);
			Vector2 anchorPos1 = Point1.rectTransform.anchoredPosition;
			Point1.rectTransform.DOAnchorPos(new Vector2(anchorPos1.x, anchorPos1.y + 15), 0.3f).SetEase(Ease.InExpo).SetLoops(-1, LoopType.Yoyo).SetDelay(0.1f);
			Vector2 anchorPos2 = Point2.rectTransform.anchoredPosition;
			Point2.rectTransform.DOAnchorPos(new Vector2(anchorPos2.x, anchorPos2.y + 15), 0.3f).SetEase(Ease.InExpo).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
		}

		void Desposed() {
			Point0.rectTransform.DOKill();
			Point1.rectTransform.DOKill();
			Point2.rectTransform.DOKill();
		}

		public void UpdateData(System.Action callback) {
			date = Time.fixedTime;
			_callback = callback;
		}

        public void MakeEnd() {
            date -= timeout;
        }

		// Update is called once per frame
		void Update () {
			if (date >= 0 && Time.fixedTime - date >= timeout) {
				Destroy(gameObject);
			}
		}

		void OnDestroy() {
			Desposed();
			if (_callback != null) {
				_callback();
				_callback = null;
			}
		}
	}
}
