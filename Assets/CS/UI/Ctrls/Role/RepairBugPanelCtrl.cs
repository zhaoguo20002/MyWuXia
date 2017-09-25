using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
    public class RepairBugPanelCtrl : WindowCore<RepairBugPanelCtrl, UIModel> {
        Image bg;
        Image block;
        Button sureBtn;
        Button repairBugBtn0;
        Text msg2;

        float date;
        float timeout = 0.6f;
        bool isMindBookMissBugHappend;
        bool hasBug;
        protected override void Init () {
            bg = GetChildImage("Bg");
            block = GetChildImage("Block");
            EventTriggerListener.Get(block.gameObject).onClick = onClick;
            sureBtn = GetChildButton("SureBtn");
            EventTriggerListener.Get(sureBtn.gameObject).onClick = onClick;
            date = Time.fixedTime;
            repairBugBtn0 = GetChildButton("repairBugBtn0");
            EventTriggerListener.Get(repairBugBtn0.gameObject).onClick = onClick;
            msg2 = GetChildText("Msg2");
        }

        void onClick(GameObject e) {
            if (Time.fixedTime - date <= timeout) {
                return;
            }
            switch (e.name) {
                case "repairBugBtn0":
                    //任务掉落的物品没有背包限制
                    DropData drop = new DropData();
                    switch (DbManager.Instance.HostData.Occupation)
                    {
                        case OccupationType.GaiBang:
                            drop.ResourceItemDataId = "100109";
                            break;
                        case OccupationType.ShaoLin:
                            drop.ResourceItemDataId = "100110";
                            break;
                        case OccupationType.QuanZhen:
                            drop.ResourceItemDataId = "100111";
                            break;
                        case OccupationType.XiaoYao:
                            drop.ResourceItemDataId = "100114";
                            break;
                        case OccupationType.DaLi:
                            drop.ResourceItemDataId = "100112";
                            break;
                        case OccupationType.YueJiaJun:
                            drop.ResourceItemDataId = "100113";
                            break;
                        default:
                            break;
                    }
                    List<DropData> drops = DbManager.Instance.PushItemToBag(new List<DropData>(){ drop }, true);
                    if (drops.Count > 0)
                    {
                        Messenger.Broadcast<List<DropData>>(NotifyTypes.ShowDropsListPanel, drops);
                    }
                    Show();
                    break;
                case "SureBtn":
                case "Block":
                    Back();
                    break;
                default:
                    break;
            }
//            AlertCtrl.Show("去吧去吧,老夫已为你做了引荐");
        }

        public void Pop() {
            bg.transform.DOScale(0, 0);
            bg.transform.DOScale(1, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack);
        }

        public void Back() {
            Close();
        }

        public void UpdateData() {
            isMindBookMissBugHappend = DbManager.Instance.IsMindBookMissBugHappend();
            hasBug = isMindBookMissBugHappend;
        }

        public override void RefreshView () {
            repairBugBtn0.gameObject.SetActive(isMindBookMissBugHappend);
            msg2.gameObject.SetActive(!hasBug);
        }

        public static void Show() {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/RepairBugPanelView", "RepairBugPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
                Ctrl.Pop();
            }
            Ctrl.UpdateData();
            Ctrl.RefreshView();
        }
    }
}
