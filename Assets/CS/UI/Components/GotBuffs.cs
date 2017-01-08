using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class GotBuffs : MonoBehaviour {
		public Image[] BuffIconImages;
        List<Text> texts;

        List<BuffData> buffDatas;
        float date = -1;
		// Use this for initialization
		void Awake () {
            texts = new List<Text>();
            for (int i = 0; i < BuffIconImages.Length; i++) {
                texts.Add(BuffIconImages[i].GetComponentInChildren<Text>()); 
            }
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
            refresh();
            date = Time.fixedTime;
		}

        void refresh() {
            BuffData buff;
            for (int i = 0; i < BuffIconImages.Length; i++) {
                if (buffDatas.Count > i) {
                    buff = buffDatas[i];
                    BuffIconImages[i].gameObject.SetActive(true);
                    BuffIconImages[i].sprite = Statics.GetBuffSprite(((int)buff.Type).ToString());
                    texts[i].text = ((int)(buff.GetProgress(BattleLogic.Instance.Frame) * buff.Timeout)).ToString();;
                }
                else {
                    BuffIconImages[i].gameObject.SetActive(false);
                }
            }
        }

        void Update() {
            if (date >= 0 && Time.fixedTime - date > 1) {
                date = Time.fixedTime;
                refresh();
            }
        }
	}
}
