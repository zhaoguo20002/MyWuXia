using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace Game {
	public class DialogMsgPop : MonoBehaviour {
		CanvasGroup alpha;
		RectTransform trans;
		public Text MsgText;
		public float Timeout = 3;
		float date;
		void Awake() {
			alpha = GetComponent<CanvasGroup>();
			alpha.DOFade(0, 0);
			trans = GetComponent<RectTransform>();
		}
		// Use this for initialization
		void Start () {
			date = Time.fixedTime;
			trans.localScale = Vector3.one;
			alpha.DOFade(1, 0.3f);
		}

		public void SetData(Vector3 pos, string msg, Color color, int fontSize = 22) {
			transform.position = pos;
			MsgText.text = msg;
			MsgText.color = color;
			MsgText.fontSize = fontSize;
		}
		
		// Update is called once per frame
		void Update () {
			if (Time.fixedTime - date >= Timeout) {
				alpha.DOFade(0, 0.3f).OnComplete(() => {
					Disposed();
				});
			}
		}

		public void Disposed() {
			alpha.DOKill();
			Destroy(gameObject);
		}
	}
	
}
