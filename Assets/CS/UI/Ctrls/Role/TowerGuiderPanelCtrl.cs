using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
    public class TowerGuiderPanelCtrl : WindowCore<TowerGuiderPanelCtrl, UIModel> {
        Image bg;
        Image block;
        Button sureBtn;
        List<Button> difficultyBtns;
        List<Image> selectImages;

        float date;
        float timeout = 0.6f;
        int difficulty;
        protected override void Init () {
            bg = GetChildImage("Bg");
            block = GetChildImage("Block");
            EventTriggerListener.Get(block.gameObject).onClick = onClick;
            sureBtn = GetChildButton("SureBtn");
            EventTriggerListener.Get(sureBtn.gameObject).onClick = onClick;
            difficultyBtns = new List<Button>() { 
                GetChildButton("difficultyBtn0"),
                GetChildButton("difficultyBtn1"),
                GetChildButton("difficultyBtn2")
            };
            for (int i = 0, len = difficultyBtns.Count; i < len; i++)
            {
                EventTriggerListener.Get(difficultyBtns[i].gameObject).onClick = onClick;
            }
            selectImages = new List<Image> { 
                GetChildImage("selectImage0"),
                GetChildImage("selectImage1"),
                GetChildImage("selectImage2")
            };
            date = Time.fixedTime;
        }

        void onClick(GameObject e) {
            if (Time.fixedTime - date <= timeout) {
                return;
            }
            switch (e.name) {
                case "SureBtn":
                case "Block":
                    Back();
                    break;
                case "difficultyBtn0":
                    if (difficultyBtns[0].enabled)
                    {
                        ConfirmCtrl.Show("是否激活普通强度？", () => {
                            if (DbManager.Instance.CostSilver(10000)) {
                                PlayerPrefs.SetInt("TowerDifficulty", 0);
                                Messenger.Broadcast(NotifyTypes.OpenTowerGuiderPanel);
                                Statics.CreatePopMsg(Vector3.zero, "普通的量子强度被激活", Color.white, 30);
                            }
                            else {
                                AlertCtrl.Show("银子不足！");
                            }
                        });
                    }
                    break;
                case "difficultyBtn1":
                    if (difficultyBtns[1].enabled)
                    {
                        ConfirmCtrl.Show("是否激活噩梦强度？", () => {
                            if (DbManager.Instance.CostSilver(180000)) {
                                PlayerPrefs.SetInt("TowerDifficulty", 1);
                                Messenger.Broadcast(NotifyTypes.OpenTowerGuiderPanel);
                                Statics.CreatePopMsg(Vector3.zero, "噩梦的量子强度被激活", new Color(0.93f, 1, 0.33f), 30);
                            }
                            else {
                                AlertCtrl.Show("银子不足！");
                            }
                        });
                    }
                    break;
                case "difficultyBtn2":
                    if (difficultyBtns[2].enabled)
                    {
                        ConfirmCtrl.Show("是否激活绝望强度？", () => {
                            if (DbManager.Instance.CostSilver(980000)) {
                                PlayerPrefs.SetInt("TowerDifficulty", 2);
                                Messenger.Broadcast(NotifyTypes.OpenTowerGuiderPanel);
                                Statics.CreatePopMsg(Vector3.zero, "绝望的量子强度被激活", new Color(0.98f, 0.26f, 0.26f), 30);
                            }
                            else {
                                AlertCtrl.Show("银子不足！");
                            }
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public void Pop() {
            bg.transform.DOScale(0, 0);
            bg.transform.DOScale(1, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack);
        }

        public void Back() {
            Close();
        }

        public void UpdateData() {
            difficulty = PlayerPrefs.GetInt("TowerDifficulty");
        }

        public override void RefreshView () {
            for (int i = 0, len = difficultyBtns.Count; i < len; i++)
            {
                selectImages[i].gameObject.SetActive(difficulty == i);
                MakeButtonEnable(difficultyBtns[i], difficulty != i);
            }
        }

        public static void Show() {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/TowerGuiderPanelView", "TowerGuiderPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
                Ctrl.Pop();
            }
            Ctrl.UpdateData();
            Ctrl.RefreshView();
        }
    }
}
