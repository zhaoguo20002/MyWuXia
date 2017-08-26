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
                        addHP = 175;
                        break;
                    case 2:
                        addHP = 300;
                        break;
                    case 3:
                        addHP = 450;
                        break;
                    case 4:
                        addHP = 650;
                        break;
                    case 5:
                        addHP = 900;
                        break;
                    case 6:
                        addHP = 1200;
                        break;
                    case 7:
                        addHP = 1500;
                        break;
                    case 8:
                        addHP = 2000;
                        break;
                    case 9:
                        addHP = 3000;
                        break;
                    case 10:
                        addHP = 5000;
                        break;
                }
                BattleLogic.Instance.PushDrug(addHP);
                RefreshView();
                SendMessageUpwards("StartDrugCD");
                //扣除道具数量
                DbManager.Instance.CostItemFromBag(drugData.Id, 1);
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
                CDProgress.DOKill();
                CDProgress.fillAmount = 1;
                CDProgress.DOFillAmount(0, drugData.Lv * 0.5f).SetEase(Ease.Linear);
            }
        }
    }
}
