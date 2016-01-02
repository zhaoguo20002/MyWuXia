using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;

public class SkillNameShow : MonoBehaviour {
	Image bgImage;
	Text text;
	// Use this for initialization
	void Awake () {
		bgImage = GetComponent<Image>();
		text = GetComponentInChildren<Text>();

		bgImage.DOFade(0, 0);
		text.DOFade(0, 0);
		text.rectTransform.DOLocalMoveX(0, 0);
	}

	/// <summary>
	/// 播放飘字动画
	/// </summary>
	/// <param name="msg">Message.</param>
	public void StartPlay(string msg) {
		text.text = msg;
		bgImage.DOKill();
		text.DOKill();
		Sequence sq = DOTween.Sequence();
		Tweener alpha0 = bgImage.DOFade(1, 0.1f);
		sq.Append(alpha0);
		Tweener alpha1 = text.DOFade(1, 0.1f);
		sq.Append(alpha1);
		Tweener move0 = text.rectTransform.DOLocalMoveX(-55, 0.1f);
		sq.Append(move0);
		sq.AppendInterval(1);
		Tweener move1 = text.rectTransform.DOLocalMoveX(0, 0.1f);
		sq.Append(move1);
		Tweener alpha3 = text.DOFade(0, 0.1f);
		sq.Join(alpha3);
		Tweener alpha2 = bgImage.DOFade(0, 0.1f);
		sq.Join(alpha2);
	}
}
