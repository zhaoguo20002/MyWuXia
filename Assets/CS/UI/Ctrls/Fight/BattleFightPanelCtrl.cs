using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
    public class BattleFightPanelCtrl : WindowCore<BattleFightPanelCtrl, UIModel> {
        Image enemyBody;
        Image enemyBloodProgress;
        Text enemyNameText;
        Image enemyBookIconImage;
        Text enemyBloodText;
        ScrollRect scrollView;
        FreeHeightGrid freeHeightGrid;
        Image teamBloodProgress;
        Text teamBloodText;
        Toggle autoFightToggle;
        GridLayoutGroup drugsGrid;
        List<DrugInBattleItemContainer> drugInBattleItemContainers;
        GridLayoutGroup teamsGrid;
        List<TeamInBattleItemContainer> teamInBattleItemContainers;

        List<RoleData> teamsData;
        List<RoleData> enemysData;
        List<ItemData> drugsData;
        Object drugPrefab;
        Object teamPrefab;

        const short waiting = 0;
        const short ready = 1;
        const short fight = 2;
        const short win = 3;
        const short skillDoing = 4;
        const short fail = 5;
        const short end = 6;
        short state = -1;
        float doSkillDate;
        float doSkillTimeout = 0.2f;
        float readyDate;
        protected override void Init() {
            enemyBody = GetChildImage("enemyBody");
            enemyBloodProgress = GetChildImage("enemyBloodProgress");
            enemyNameText = GetChildText("enemyNameText");
            enemyBookIconImage = GetChildImage("enemyBookIconImage");
            enemyBloodText = GetChildText("enemyBloodText");
            scrollView = GetChildScrollRect("scrollView");
            freeHeightGrid = GetChildComponent<FreeHeightGrid>(gameObject, "FreeHeightGrid");
            teamBloodProgress = GetChildImage("teamBloodProgress");
            teamBloodText = GetChildText("teamBloodText");
            autoFightToggle = GetChildToggle("autoFightToggle");
            drugsGrid = GetChildGridLayoutGroup("drugsGrid");
            drugInBattleItemContainers = new List<DrugInBattleItemContainer>();
            teamsGrid = GetChildGridLayoutGroup("teamsGrid");
            teamInBattleItemContainers = new List<TeamInBattleItemContainer>();
            drugPrefab = Statics.GetPrefab("Prefabs/UI/Fight/DrugInBattleItemContainer");
            teamPrefab = Statics.GetPrefab("Prefabs/UI/Fight/TeamInBattleItemContainer");
            state = waiting;
        }

        void Update() {
            switch (state) {
                case ready:
                    if (Time.fixedTime - readyDate > 1) {
                        state = fight;
                    }
                    break;
                case fight:
                    BattleLogic.Instance.Action();
                    BattleProcess process = BattleLogic.Instance.PopProcess();
                    if (process != null) {
                        Debug.Log(JsonManager.GetInstance().SerializeObject(process));
                    }
                    if (BattleLogic.Instance.GetProcessCount() == 0) {
                        if (BattleLogic.Instance.IsFail()) { //判负
                            state = fail;
                            break;
                        }
                        if (BattleLogic.Instance.IsWin()) { //判胜
                            state = win;
                            break;
                        }
                    }
                    break;
                case skillDoing:
                    if (Time.fixedTime - doSkillDate > doSkillTimeout) {
                        state = fight;
                    }
                    break;
                case win:

                    break;
                case fail:

                    break;
                case waiting:
                    break;
            }
        }

        public void UpdateData(List<RoleData> teams, List<RoleData> enemys, List<ItemData> drugs) {
            teamsData = teams;
            enemysData = enemys;
            drugsData = drugs;
            BattleLogic.Instance.AutoFight = true;
            BattleLogic.Instance.Init(teamsData, enemysData);
            BattleLogic.Instance.PopEnemy();
            state = ready;
            readyDate = Time.fixedTime;
        }

        void refreshEnemy(RoleData enemy) {
            enemyBody.sprite = Statics.GetHalfBodySprite(enemy.HalfBodyId);
            enemyBody.SetNativeSize();
            enemyBloodProgress.fillAmount = enemy.HPRate;
            enemyNameText.text = enemy.Name;
            if (enemy.GetCurrentBook() != null) {
                enemyBookIconImage.transform.parent.gameObject.SetActive(true);
                enemyBookIconImage.sprite = Statics.GetIconSprite(enemy.GetCurrentBook().IconId);
                enemyBookIconImage.SetNativeSize();
            } else {
                enemyBookIconImage.transform.parent.gameObject.SetActive(false);
            }
        }

        public override void RefreshView() {
            refreshEnemy(BattleLogic.Instance.CurrentEnemyRole);
            DrugInBattleItemContainer drugContainer;
            for (int i = 0, len = drugsData.Count; i < len; i++) {
                drugContainer = Statics.GetPrefabClone(drugPrefab).GetComponent<DrugInBattleItemContainer>();
                MakeToParent(drugsGrid.transform, drugContainer.transform);
                drugContainer.UpdateData(drugsData[i]);
                drugContainer.RefreshView();
                drugInBattleItemContainers.Add(drugContainer);
            }

            TeamInBattleItemContainer teamContainer;
            for (int i = 0, len = teamsData.Count; i < len; i++) {
                teamContainer = Statics.GetPrefabClone(teamPrefab).GetComponent<TeamInBattleItemContainer>();
                MakeToParent(teamsGrid.transform, teamContainer.transform);
                teamContainer.UpdateData(teamsData[i]);
                teamContainer.RefreshView();
                teamInBattleItemContainers.Add(teamContainer);
            }
        }  

        public static void Show(List<RoleData> teams, List<RoleData> enemys, List<ItemData> drugs) {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Fight/BattleFightPanelView", "BattleFightPanelCtrl");
            }
            Ctrl.UpdateData(teams, enemys, drugs);
            Ctrl.RefreshView();
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Close();
            }
        }
    }
}
