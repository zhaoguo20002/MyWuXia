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
                    ConfirmCtrl.Show("丐帮乃中原第一大帮\n兵器:缠手 镇派绝学:降龙十八掌\n特点:高外攻 缴械\n是否加入？", () => {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation0");
                        Back();
                    });
                    break;
                case "EnterOccupation1":
                    if (DbManager.Instance.HostData.Gender == GenderType.Male) {
                        ConfirmCtrl.Show("天下武功出少林\n兵器:棍 镇派绝学:袈裟伏魔功\n特点:高外防 眩晕\n是否加入？", () => {
                            Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation1");
                            Back();
                        });
                    } else {
                        AlertCtrl.Show("少林乃佛门清净之地,不接纳女弟子");
                        return;
                    }
                    break;
                case "EnterOccupation2":
                    ConfirmCtrl.Show("全真乃玄门正宗\n兵器:剑 镇派绝学:北斗七星剑法\n特点:高内攻 清空负面状态\n是否加入？", () => {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation2");
                        Back();
                    });
                    break;
                case "EnterOccupation3":
                    ConfirmCtrl.Show("大理段氏以武立国\n兵器:缠手 镇派绝学:六脉神剑\n特点:超强内攻 定身 混乱 眩晕\n是否加入？", () => {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation3");
                        Back();
                    });
                    break;
                case "EnterOccupation4":
                    ConfirmCtrl.Show("轻灵飘逸闲雅清隽任逍遥\n兵器:笛子 镇派绝学:北冥神功\n特点:高内攻 瞬间大量回血\n是否加入？", () => {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation4");
                        Back();
                    });
                    break;
                case "EnterOccupation5":
                    ConfirmCtrl.Show("撼山易，撼岳家军难\n兵器:枪 镇派绝学:镇军穿云枪法\n特点:超高外攻\n是否加入？", () => {
                        Messenger.Broadcast<string>(NotifyTypes.AddANewTask, "task_occupation5");
                        Back();
                    });
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
