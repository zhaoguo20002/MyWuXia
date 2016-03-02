using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class TaskDetailDialogLoadingContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Image Point0;
		public Image Point1;
		public Image Point2;

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

		public void UpdateData(TaskData data) {
			
		}
		
		public void RefreshView() {
			
		}

		// Update is called once per frame
		void Update () {
			
		}

		void OnDestroy() {
			Desposed();
		}
	}
}
