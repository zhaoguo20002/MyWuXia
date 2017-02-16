using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace Game {
    public class BattleFightPanelCtrl : WindowCore<BattleFightPanelCtrl, UIModel> {
        Image enemyBody;
        Text enemyNumText;
        Image enemyBloodProgress;
        Text enemyNameText;
        Image enemyBookIconImage;
        Text enemyBloodText;
        Image enemyCDProgress;
        FreeHeightGrid freeHeightGrid;
        Image teamBloodProgress;
        Text teamBloodText;
        Toggle autoFightToggle;
        GridLayoutGroup drugsGrid;
        List<DrugInBattleItemContainer> drugInBattleItemContainers;
        GridLayoutGroup teamsGrid;
        List<TeamInBattleItemContainer> teamInBattleItemContainers;
        CanvasGroup alphaGroup;
        Image failSprite;
        Image winSprite;
        GotBuffs enemyGotBuffsScript;
        GotBuffs teamGotBuffsScript;
        RectTransform enemySkillPos;
        RectTransform teamSkillPos;
        RectTransform enemyPoplPos;
        RectTransform teamPoplPos;

        FightData fightData;
        List<RoleData> teamsData;
        List<RoleData> enemysData;
        List<ItemData> drugsData;
        Object drugPrefab;
        Object teamPrefab;

        const short waiting = 0;
        const short ready = 1;
        const short fight = 2;
        const short win = 3;
        const short delayDoing = 4;
        const short fail = 5;
        const short end = 6;
        short state = -1;
        float delayDate;
        float delayTimeout = 0.3f;
        float readyDate;
        Dictionary<string, int> usedSkillIdMapping; //记录使用过的招式
        bool isWin;

        protected override void Init() {
            enemyBody = GetChildImage("enemyBody");
            enemyNumText = GetChildText("enemyNumText");
            enemyBloodProgress = GetChildImage("enemyBloodProgress");
            enemyNameText = GetChildText("enemyNameText");
            enemyBookIconImage = GetChildImage("enemyBookIconImage");
            enemyBloodText = GetChildText("enemyBloodText");
            enemyCDProgress = GetChildImage("enemyCDProgress");
            freeHeightGrid = GetChildComponent<FreeHeightGrid>(gameObject, "FreeHeightGrid");
            teamBloodProgress = GetChildImage("teamBloodProgress");
            teamBloodText = GetChildText("teamBloodText");
            autoFightToggle = GetChildToggle("autoFightToggle");
            autoFightToggle.onValueChanged.AddListener((check) => {
                PlayerPrefs.SetString("BattleNotAutoFight", check ? "" : "true");
                BattleLogic.Instance.AutoFight = string.IsNullOrEmpty(PlayerPrefs.GetString("BattleNotAutoFight"));
            });
            drugsGrid = GetChildGridLayoutGroup("drugsGrid");
            drugInBattleItemContainers = new List<DrugInBattleItemContainer>();
            teamsGrid = GetChildGridLayoutGroup("teamsGrid");
            teamInBattleItemContainers = new List<TeamInBattleItemContainer>();
            drugPrefab = Statics.GetPrefab("Prefabs/UI/Fight/DrugInBattleItemContainer");
            teamPrefab = Statics.GetPrefab("Prefabs/UI/Fight/TeamInBattleItemContainer");
            alphaGroup = GetChildCanvasGroup("alphaGroup");
            failSprite = GetChildImage("failSprite");
            winSprite = GetChildImage("winSprite");
            enemyGotBuffsScript = GetChildComponent<GotBuffs>(gameObject, "enemyGotBuffs");
            teamGotBuffsScript = GetChildComponent<GotBuffs>(gameObject, "teamGotBuffs");
            usedSkillIdMapping = new Dictionary<string, int>();
            enemySkillPos = GetChildComponent<RectTransform>(gameObject, "enemySkillPos");
            teamSkillPos = GetChildComponent<RectTransform>(gameObject, "teamSkillPos");
            enemyPoplPos = GetChildComponent<RectTransform>(gameObject, "enemyPoplPos");
            teamPoplPos = GetChildComponent<RectTransform>(gameObject, "teamPoplPos");

            state = waiting;
        }

        /// <summary>
        /// 处理技能特效和音效
        /// </summary>
        /// <param name="skill">Skill.</param>
        void dealSkillEffectAndSound(string teamName, SkillData skill) {
            if (skill == null) {
                return;
            }
            if (skill.EffectSrc != "") {
                GameObject effect = Statics.GetSkillEffectPrefabClone(skill.EffectSrc);
                if (effect != null) {
                    if (teamName == "Team") {
                        effect.transform.SetParent(enemySkillPos.transform);
                    }
                    else {
                        effect.transform.SetParent(teamSkillPos.transform);
                    }
                    
                    effect.transform.localPosition = Vector3.zero;
                }
            }
            if (skill.EffectSoundId != "") {
                SoundManager.GetInstance().PushSound(skill.EffectSoundId);
            }
        }

        void dealBattleProcess(BattleProcess process) {
            freeHeightGrid.PushContext(process.Result);
            switch (process.Type) {
                case BattleProcessType.Normal:
                    default:

                    break;
                case BattleProcessType.EnemyPop:
                    enemyBody.DOFade(0, 0);
                    refreshEnemy();
                    enemyBody.DOFade(1, 0.45f);
                    state = delayDoing;
                    delayDate = Time.fixedTime;
                    delayTimeout = 0.5f;
                    enemyNumText.text = string.Format("剩余敌人: {0}/{1}", BattleLogic.Instance.EnemysData.Count - BattleLogic.Instance.CurrentEnemy, BattleLogic.Instance.EnemysData.Count);
                    break;
                case BattleProcessType.Attack:
                    if (process.IsTeam) {
                        refreshEnemyBlood();
                        dealSkillEffectAndSound("Team", process.Skill);
                        //记录使用过的招式
                        if (!usedSkillIdMapping.ContainsKey(process.Skill.Id)) {
                            usedSkillIdMapping.Add(process.Skill.Id, 1);
                        } else {
                            usedSkillIdMapping[process.Skill.Id]++;
                        }
                        enemyBody.transform.DOShakePosition(0.25f, 20, 50, 90);
                    } else {
                        refreshTeamBlood();
                        dealSkillEffectAndSound("Enemy", process.Skill);
                        enemyBody.transform.DOScale(1.5f, 0.15f).SetLoops(2, LoopType.Yoyo);
                    }
                    if (!process.IsMissed) {
                        Statics.CreatePopMsg(
                            process.IsTeam ? enemyPoplPos.transform.position : teamPoplPos.transform.position, 
                            process.HurtedHP > 0 ? ("+" + process.HurtedHP) : process.HurtedHP.ToString(), 
                            process.HurtedHP > 0 ? Color.green : Color.red, 40, 0.2f);
                    } else {
                        if (!process.IsIgnoreAttack)
                        {
                            Statics.CreatePopMsg(
                                process.IsTeam ? enemyPoplPos.transform.position : teamPoplPos.transform.position, 
                                "闪避", 
                                Color.white, 40, 0.2f);
                        }
                        else
                        {
                            Statics.CreatePopMsg(
                                process.IsTeam ? enemyPoplPos.transform.position : teamPoplPos.transform.position, 
                                "无视伤害", 
                                Color.white, 40, 0.2f);
                        }
                    }
                    state = delayDoing;
                    delayDate = Time.fixedTime;
                    delayTimeout = 0.3f;
                    break;
                case BattleProcessType.Increase:
                case BattleProcessType.Plus:
                    if (process.IsTeam) {
                        refreshTeamBlood();
                    } else {
                        refreshEnemyBlood();
                    }
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.3f : enemyPoplPos.transform.position + Vector3.up * 0.3f, 
                        process.HurtedHP > 0 ? ("+" + process.HurtedHP + "(恢复)") : process.HurtedHP.ToString() + "(流血)", 
                        process.HurtedHP > 0 ? Color.green : Color.red, 40, 0.2f);
                    break;
                case BattleProcessType.Drug:
                    refreshTeamBlood();
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.3f : enemyPoplPos.transform.position + Vector3.up * 0.3f, 
                        "+" + process.HurtedHP + "(药)", 
                        Color.green, 40, 0.2f);
                    break;
                case BattleProcessType.ReboundInjury:
                    refreshTeamBlood();
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.4f : enemyPoplPos.transform.position + Vector3.up * 0.4f, 
                        process.HurtedHP + "(反伤)", 
                        Color.red, 40, 0.2f);
                    break;
                case BattleProcessType.AccidentalInjury:
                    refreshTeamBlood();
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.5f : enemyPoplPos.transform.position + Vector3.up * 0.5f, 
                        process.HurtedHP + "(误伤)", 
                        Color.red, 40, 0.2f);
                    break;
            }
        }

        void endSend() {
            //将使用过的招式和兵器暴击整理后传递入库
            JArray usedSkillIdData = new JArray();
            foreach(string key in usedSkillIdMapping.Keys) {
                usedSkillIdData.Add(new JArray(key, usedSkillIdMapping[key]));
            }
            Messenger.Broadcast<JArray>(NotifyTypes.SendFightResult, new JArray(isWin, fightData.Id, isWin ? 1 : 0, usedSkillIdData, new JArray())); //目前没有考虑战斗评级系统，所以默认所有战斗都是1星
        }

        void Update() {
            switch (state) {
                case ready:
                    if (Time.fixedTime - readyDate > 1) {
                        state = fight;
                    }
                    break;
                case fight:
                    if (BattleLogic.Instance.CurrentEnemyRole != null && BattleLogic.Instance.CurrentEnemyRole.GetCurrentBook() != null) {
                        enemyCDProgress.fillAmount = BattleLogic.Instance.CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().GetCDProgress(BattleLogic.Instance.Frame);
                    }
                    BattleLogic.Instance.Action();
                    BattleProcess process = BattleLogic.Instance.PopProcess();
                    if (process != null) {
                        dealBattleProcess(process);
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
                case delayDoing:
                    if (Time.fixedTime - delayDate > delayTimeout) {
                        state = fight;
                    }
                    break;
                case win:
                    state = waiting;
                    isWin = true;
                    alphaGroup.gameObject.SetActive(true);
                    winSprite.gameObject.SetActive(true);
                    alphaGroup.DOFade(1, 2).SetDelay(2).OnComplete(endSend);
                    SoundManager.GetInstance().StopBGM();
                    refreshTeamBlood();
                    refreshEnemyBlood();
                    break;
                case fail:
                    state = waiting;
                    isWin = false;
                    alphaGroup.gameObject.SetActive(true);
                    failSprite.gameObject.SetActive(true);
                    alphaGroup.DOFade(1, 2).SetDelay(2).OnComplete(endSend);
                    SoundManager.GetInstance().StopBGM();
                    refreshTeamBlood();
                    refreshEnemyBlood();
                    break;
                case waiting:
                    break;
            }
        }

        public void UpdateData(FightData fight, List<RoleData> teams, List<RoleData> enemys, List<ItemData> drugs) {
            fightData = fight;
            teamsData = teams;
            enemysData = enemys;
            drugsData = drugs;
            BattleLogic.Instance.AutoFight = string.IsNullOrEmpty(PlayerPrefs.GetString("BattleNotAutoFight"));
            autoFightToggle.isOn = BattleLogic.Instance.AutoFight;
            BattleLogic.Instance.Init(teamsData, enemysData);
            BattleLogic.Instance.PopEnemy();
            enemyGotBuffsScript.SetBuffDatas(BattleLogic.Instance.EnemyBuffsData);
            teamGotBuffsScript.SetBuffDatas(BattleLogic.Instance.TeamBuffsData);
            state = ready;
            readyDate = Time.fixedTime;
        }

        void refreshTeamBlood() {
            teamBloodProgress.DOKill();
            teamBloodProgress.DOFillAmount(BattleLogic.Instance.CurrentTeamRole.HPRate, 0.2f).SetEase(Ease.Linear);
            teamBloodText.text = string.Format("{0}/{1}", BattleLogic.Instance.CurrentTeamRole.HP, BattleLogic.Instance.CurrentTeamRole.MaxHP);
        }

        void refreshEnemyBlood() {
            enemyBloodProgress.DOKill();
            enemyBloodProgress.DOFillAmount(BattleLogic.Instance.CurrentEnemyRole.HPRate, 0.2f).SetEase(Ease.Linear);
            enemyBloodText.text = string.Format("{0}/{1}", BattleLogic.Instance.CurrentEnemyRole.HP, BattleLogic.Instance.CurrentEnemyRole.MaxHP);
        }

        void refreshEnemy() {
            RoleData enemy = BattleLogic.Instance.CurrentEnemyRole;
            enemyBody.sprite = Statics.GetHalfBodySprite(enemy.HalfBodyId);
            enemyBody.SetNativeSize();
            refreshEnemyBlood();
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
            refreshEnemy();
            refreshTeamBlood();
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
            SoundManager.GetInstance().PlayBGM("bgm0004");
        }  

        public void StartDrugCD() {
            for (int i = 0, len = drugInBattleItemContainers.Count; i < len; i++) {
                drugInBattleItemContainers[i].StartCD();
            }
        }

        public static void Show(FightData fight, List<RoleData> teams, List<RoleData> enemys, List<ItemData> drugs) {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Fight/BattleFightPanelView", "BattleFightPanelCtrl");
            }
            Ctrl.UpdateData(fight, teams, enemys, drugs);
            Ctrl.RefreshView();
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Close();
            }
        }
    }
}
