using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;

public class WeaponPowerPlus : MonoBehaviour {
	public RectTransform[] ItemRects;
	float[] _rates;

	bool valid;
	RectTransform rectTrans;
	// Use this for initialization
	void Awake () {
		valid = false; //验证合法性
		rectTrans = GetComponent<RectTransform>();
	}

	/// <summary>
	/// 设置武器威力增量倍率集合
	/// </summary>
	/// <param name="rates">Rates.</param>
	public void SetRates(float[] rates) {
		_rates = rates;
		if (ItemRects != null && ItemRects.Length == 4 && ItemRects.Length == _rates.Length) {
			valid = true;
			Vector2 size;
			for (int i = 0; i < ItemRects.Length; i++) {
				size = ItemRects[i].sizeDelta;
				ItemRects[i].sizeDelta = new Vector2(rectTrans.sizeDelta.x * _rates[i], size.y);
			}
		}
	}

	/// <summary>
	/// 检测碰撞后返回对应的增量倍率
	/// </summary>
	/// <returns>The power multiplying by collision.</returns>
	/// <param name="coller">Coller.</param>
	public float GetPowerMultiplyingByCollision(RectTransform coller) {
		//白色1 黄色1.25 橙色1.5 红色2 倍
		if (coller != null || !valid) {
			Vector2 size1 = rectTrans.sizeDelta;
			Vector2 size2 = coller.sizeDelta;
			float x1 = rectTrans.anchoredPosition.x - size1.x * 0.5f;
			float x2 = coller.anchoredPosition.x - size2.x * 0.5f;
			if (Statics.Collision2D(x1, 0, size1.x, size1.y, x2, 0, size2.x, size2.y)) {
				//红色
				Vector2 size3 = ItemRects[3].sizeDelta;
				float x3 = rectTrans.anchoredPosition.x + ItemRects[3].anchoredPosition.x - size3.x * 0.5f;
				if (Statics.Collision2D(x3, 0, size3.x, size3.y, x2, 0, size2.x, size2.y)) {
					return 2;
				}
				//橙色
				size3 = ItemRects[2].sizeDelta;
				x3 = rectTrans.anchoredPosition.x + ItemRects[2].anchoredPosition.x - size3.x * 0.5f;
				if (Statics.Collision2D(x3, 0, size3.x, size3.y, x2, 0, size2.x, size2.y)) {
					return 1.5f;
				}
				//黄色
				size3 = ItemRects[1].sizeDelta;
				x3 = rectTrans.anchoredPosition.x + ItemRects[1].anchoredPosition.x - size3.x * 0.5f;
				if (Statics.Collision2D(x3, 0, size3.x, size3.y, x2, 0, size2.x, size2.y)) {
					return 1.25f;
				}
				//白色
				return 1;
			}
			else {
				return 0;
			}
		}
		else {
			return 0;
		}
	}
}
