using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game;
using System.Collections.Generic;

public class GotBuffs : MonoBehaviour {
	public Image[] BuffIconImages;
	List<BuffData> buffDatas;
	// Use this for initialization
	void Awake () {
		Clear();
	}

	public void Clear() {
		if (BuffIconImages == null) {
			return;
		}
		for (int i = 0; i < BuffIconImages.Length; i++) {
			BuffIconImages[i].gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 设置buff属性
	/// </summary>
	/// <param name="buffs">Buffs.</param>
	public void SetBuffDatas(List<BuffData> buffs) {
		if (BuffIconImages == null) {
			return;
		}
		buffDatas = buffs;
		BuffData buff;
		for (int i = 0; i < BuffIconImages.Length; i++) {
			if (buffDatas.Count > i) {
				buff = buffDatas[i];
				BuffIconImages[i].gameObject.SetActive(true);
				BuffIconImages[i].sprite = Statics.GetBuffSprite(((int)buff.Type).ToString());
				BuffIconImages[i].GetComponentInChildren<Text>().text = buff.RoundNumber.ToString();;
			}
			else {
				BuffIconImages[i].gameObject.SetActive(false);
			}
		}
	}

}
