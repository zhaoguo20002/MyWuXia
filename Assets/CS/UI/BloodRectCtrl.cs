using UnityEngine;
using System.Collections;
using Game;

public class BloodRectCtrl : BindUIToObjectCore {
	public RectTransform HPTransform;
	public RectTransform MaxHPTransform;
	public RectTransform MPTransform;
	public RectTransform MaxMPTransform;
	float maxHPWidth;
	float maxHPHeight;
	float maxMPWidth;
	float maxMPHeight;

	void Awake() {
		maxHPWidth = MaxHPTransform.sizeDelta.x;
		maxHPHeight = MaxHPTransform.sizeDelta.y;
		maxMPWidth = MaxMPTransform.sizeDelta.x;
		maxMPHeight = MaxMPTransform.sizeDelta.y;
	}

	/// <summary>
	/// 设置HP进度条 (设置一个0-1之间的浮点数)
	/// </summary>
	/// <param name="rat">Rat.</param>
	public void SetHPRat(float rat) {
		HPTransform.sizeDelta = new Vector2(rat * maxHPWidth, maxHPHeight);
	}

	/// <summary>
	/// 设置MP进度条 (设置一个0-1之间的浮点数)
	/// </summary>
	/// <param name="rat">Rat.</param>
	public void SetMPRat(float rat) {
		MPTransform.sizeDelta = new Vector2(rat * maxMPWidth, maxMPHeight);
	}
}
