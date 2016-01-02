using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

public class PopMsg : MonoBehaviour {
	/// <summary>
	/// 文本信息
	/// </summary>
	public string Msg = "None";
	/// <summary>
	/// 字体颜色
	/// </summary>
	public Color Color = Color.white;
	/// <summary>
	/// 字体大小
	/// </summary>
	public int FontSize = 30;
	/// <summary>
	/// 飘字强度
	/// </summary>
	public float Strength = 1;
	Text text;
	void Awake() {
		text = GetComponent<Text>();
		text.DOFade(0, 0);
	}

	// Use this for initialization
	void Start () {
		if (!string.IsNullOrEmpty(Msg)) {
			text.text = Msg;
			text.color = Color;
			text.fontSize = FontSize;
			text.DOKill();
			Sequence sq = DOTween.Sequence();
			Tweener alpha0 = text.DOFade(1, 0.5f);
			sq.Join(alpha0);
			Tweener scale0 = text.transform.DOShakeScale(0.5f, Strength);
			sq.Join(scale0);
			Tweener move0 = text.rectTransform.DOLocalMoveY(text.rectTransform.anchoredPosition.y + 100, 2f);
			sq.Join(move0);
			Tweener alpha1 = text.DOFade(0, 0.2f);
			sq.Append(alpha1);
			sq.OnComplete(() => {
				Destroy(gameObject);
			});
		}
	}
}
