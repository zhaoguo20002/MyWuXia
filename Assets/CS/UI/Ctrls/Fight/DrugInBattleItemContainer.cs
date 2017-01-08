using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Game {
    public class DrugInBattleItemContainer : ComponentCore {
        public Image DrugIconImage;
        public Text CountText;
        public Image CDProgress;
        public Image Block;

        ItemData drugData;
        protected override void Init() {
            EventTriggerListener.Get(Block.gameObject).onClick = onClick;
            Block.fillAmount = 0;
        }

        void onClick(GameObject e) {
            if (CDProgress.fillAmount > 0) {
                return;
            }
            if (drugData.Num > 0) {
                drugData.Num--;
                //还有剩余才会走cd
                if (drugData.Num > 0) {
                    CDProgress.fillAmount = 1;
                    CDProgress.DOFillAmount(0, 10).SetEase(Ease.Linear);
                }
                RefreshView();
            }
        }

        public void UpdateData(ItemData data) {
            drugData = data;
        }

        public override void RefreshView() {
            DrugIconImage.sprite = Statics.GetIconSprite(drugData.IconId);
            if (drugData.Num > 0) {
                MakeImageDefault(DrugIconImage);
            } 
            else {
                MakeImageGrey(DrugIconImage);
            }
            CountText.text = drugData.Num > 1 ? drugData.Num.ToString() : "";
        }
    }
}
