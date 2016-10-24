using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
    public class OccupationPanelCtrl : WindowCore<OccupationPanelCtrl, UIModel> {
        Image bg;
        Image block;
        Button sureBtn;
        List<Button> occupationBtns;

        float date;
        float timeout = 0.6f;
        protected override void Init () {
            bg = GetChildImage("Bg");
            block = GetChildImage("Block");
            EventTriggerListener.Get(block.gameObject).onClick = onClick;
            sureBtn = GetChildButton("SureBtn");
            EventTriggerListener.Get(sureBtn.gameObject).onClick = onClick;
            date = Time.fixedTime;
            occupationBtns = new List<Button>() { 
                GetChildButton("EnterOccupation0"),
                GetChildButton("EnterOccupation1"),
                GetChildButton("EnterOccupation2"),
                GetChildButton("EnterOccupation3"),
                GetChildButton("EnterOccupation4"),
                GetChildButton("EnterOccupation5")
            };
            for (int i = 0, len = occupationBtns.Count; i < len; i++) {
                EventTriggerListener.Get(occupationBtns[i].gameObject).onClick = onClick;
            }
        }

        void onClick(GameObject e) {
            if (Time.fixedTime - date <= timeout) {
                return;
            }
            switch (e.name) {
                case "EnterOccupation0":
                    Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation0");
                    break;
                case "EnterOccupation1":
                    if (DbManager.Instance.HostData.Gender == GenderType.Male) {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation1");
                    } else {
                        AlertCtrl.Show("少林乃佛门清净之地,不接纳女弟子");
                        return;
                    }
                    break;
                case "EnterOccupation2":
                    Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation2");
                    break;
                case "EnterOccupation3":
                    Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation3");
                    break;
                case "EnterOccupation4":
                    Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation4");
                    break;
                case "EnterOccupation5":
                    Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation5");
                    break;
                default:
                    break;
            }
            Back();
            AlertCtrl.Show("去吧去吧,老夫已为你做了引荐");
        }

        public void Pop() {
            bg.transform.DOScale(0, 0);
            bg.transform.DOScale(1, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack);
        }

        public void Back() {
            Close();
        }

        public void UpdateData() {
        }

        public override void RefreshView () {
            
        }

        public static void Show() {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/OccupationPanelView", "OccupationPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
                Ctrl.Pop();
            }
            Ctrl.UpdateData();
            Ctrl.RefreshView();
        }
    }
}
