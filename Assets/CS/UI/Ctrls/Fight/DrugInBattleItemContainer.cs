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
        public Image Disable;

        ItemData drugData;
        float date;
        protected override void Init() {
            EventTriggerListener.Get(Block.gameObject).onClick = onClick;
            Block.fillAmount = 0;
        }

        void Update() {
            if (Time.fixedTime - date < 0.5f) {
                return;
            }
            date = Time.fixedTime;
            Disable.gameObject.SetActive(!BattleLogic.Instance.CurrentTeamRole.CanUseTool);
        }

        void onClick(GameObject e) {
            if (CDProgress.fillAmount > 0 || !BattleLogic.Instance.CurrentTeamRole.CanUseTool) {
                return;
            }
            if (drugData.Num > 0) {
                drugData.Num--;
                int addHP;
                switch (drugData.Lv) {
                    case 1:
                    default:
                        addHP = 75;
                        break;
                    case 2:
                        addHP = 150;
                        break;
                    case 3:
                        addHP = 300;
                        break;
                    case 4:
                        addHP = 500;
                        break;
                    case 5:
                        addHP = 750;
                        break;

                }
                BattleLogic.Instance.PushDrug(addHP);
                RefreshView();
                SendMessageUpwards("StartDrugCD");
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

        public void StartCD() {
            //还有剩余才会走cd
            if (drugData.Num > 0) {
                CDProgress.fillAmount = 1;
                CDProgress.DOFillAmount(0, 10).SetEase(Ease.Linear);
            }
        }
    }
}
