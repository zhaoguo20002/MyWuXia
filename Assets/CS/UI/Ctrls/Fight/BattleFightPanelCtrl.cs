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
        Text autoFightLabel;
        Toggle upSpeedToggle;
        Text upSpeedLabel;
        GridLayoutGroup drugsGrid;
        List<DrugInBattleItemContainer> drugInBattleItemContainers;
        GridLayoutGroup teamsGrid;
        List<TeamInBattleItemContainer> teamInBattleItemContainers;
        TeamInBattleItemContainer teamInBattleLostKnowledgeContainer;
        CanvasGroup alphaGroup;
        Image failSprite;
        Image winSprite;
        GotBuffs enemyGotBuffsScript;
        GotBuffs teamGotBuffsScript;
        RectTransform enemySkillPos;
        RectTransform teamSkillPos;
        RectTransform enemyPoplPos;
        RectTransform teamPoplPos;
        Image limePowderBg;
        Image limePowderTimerImage;
        Image limePowderBlockImage;
        Text limePowderNumText;

        FightData fightData;
        List<RoleData> teamsData;
        List<RoleData> enemysData;
        int averageEnemyLv; //敌人平均等级
        List<BookData> teamBooks; //记录本方秘籍集合用于升级秘籍
        List<ItemData> drugsData;
        Object drugPrefab;
        Object teamPrefab;
        PropData propLimePowderData;

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
                autoFightLabel.text = check ? "手动\n施展" : "自动\n施展";
            });
            autoFightLabel = GetChildText("autoFightLabel");
            upSpeedToggle = GetChildToggle("upSpeedToggle");
            upSpeedToggle.onValueChanged.AddListener((check) => {
                PlayerPrefs.SetString("BattleUpSpeed", check ? "true" : "");
                BattleLogic.Instance.UpSpeed = !string.IsNullOrEmpty(PlayerPrefs.GetString("BattleUpSpeed"));
                upSpeedLabel.text = check ? "一倍\n速度" : "二倍\n速度";
            });
            upSpeedLabel = GetChildText("upSpeedLabel");
            drugsGrid = GetChildGridLayoutGroup("drugsGrid");
            drugInBattleItemContainers = new List<DrugInBattleItemContainer>();
            teamsGrid = GetChildGridLayoutGroup("teamsGrid");
            teamInBattleItemContainers = new List<TeamInBattleItemContainer>();
            teamInBattleLostKnowledgeContainer = GetChildComponent<TeamInBattleItemContainer>(gameObject, "teamInBattleLostKnowledgeContainer");
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

            limePowderBg = GetChildImage("limePowderBg");
            limePowderTimerImage = GetChildImage("limePowderTimerImage");
            limePowderTimerImage.fillAmount = 0;
            limePowderBlockImage = GetChildImage("limePowderBlockImage");
            EventTriggerListener.Get(limePowderBlockImage.gameObject).onClick = onClick;
            limePowderNumText = GetChildText("limePowderNumText");

            state = waiting;
        }

        void onClick(GameObject e) {
            switch (e.name)
            {
                case "limePowderBlockImage":
                    if (propLimePowderData == null)
                    {
                        break;
                    }
                    if (limePowderTimerImage.fillAmount <= 0)
                    {
                        limePowderTimerImage.DOKill();
                        limePowderTimerImage.fillAmount = 1;
                        limePowderTimerImage.DOFillAmount(0, 5).SetEase(Ease.Linear);
                        propLimePowderData.Num--;
                        refreshLimePowder();
                        Messenger.Broadcast<PropType, int>(NotifyTypes.UseProp, PropType.LimePowder, 1);
                        Messenger.Broadcast(NotifyTypes.MakeUpdateProps);
                        if (Random.Range(0, 100) >= 50)
                        {
                            Hide();
                            //任务详情界面打开时不呼出角色信息板
                            if (TaskDetailInfoPanelCtrl.Ctrl == null) {
                                Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
                            }
                            Messenger.Broadcast(NotifyTypes.PlayBgm);
                            Statics.CreatePopMsg(Vector3.zero, "抓了一把石灰粉洒向敌人，本方全身而退！", Color.yellow, 30);
                            PlayerPrefs.SetString("BattleIsGoingOn_FightFlag_For_" + DbManager.Instance.HostData.Id, "");
                        }
                        else
                        {
                            Statics.CreatePopMsg(Vector3.zero, "抓了一把石灰粉洒向敌人，被对方躲开！", Color.red, 30);
                        }
                    }
                    break;
                default:
                    break;
            }
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
                    if (process.IsTeam)
                    {
                        refreshTeamBlood();
                        dealSkillEffectAndSound("Enemy", process.Skill);
                    }
                    else
                    {
                        refreshEnemyBlood();
                    }
                    if (process.HurtedHP != 0)
                    {
                        Statics.CreatePopMsg(
                            process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.3f : enemyPoplPos.transform.position + Vector3.up * 0.3f, 
                            process.HurtedHP > 0 ? ("+" + process.HurtedHP + "(恢复)") : process.HurtedHP.ToString() + "(流血)", 
                            process.HurtedHP > 0 ? Color.green : Color.red, 40, 0.2f);
                    }
                    break;
                case BattleProcessType.Drug:
                    refreshTeamBlood();
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.3f : enemyPoplPos.transform.position + Vector3.up * 0.3f, 
                        "+" + process.HurtedHP + "(药)", 
                        Color.green, 40, 0.2f);
                    break;
                case BattleProcessType.ReboundInjury:
                    if (process.IsTeam) {
                        refreshTeamBlood();
                    } else {
                        refreshEnemyBlood();
                    }
                    Statics.CreatePopMsg(
                        process.IsTeam ? teamPoplPos.transform.position + Vector3.up * 0.4f : enemyPoplPos.transform.position + Vector3.up * 0.4f, 
                        process.HurtedHP + "(反伤)", 
                        Color.red, 40, 0.2f);
                    break;
                case BattleProcessType.AccidentalInjury:
                    if (process.IsTeam) {
                        refreshTeamBlood();
                    } else {
                        refreshEnemyBlood();
                    }
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
            Messenger.Broadcast<JArray, List<BookData>>(NotifyTypes.SendFightResult, new JArray(isWin, fightData.Id, isWin ? 1 : 0, usedSkillIdData, new JArray(), averageEnemyLv, Mathf.Clamp(BattleLogic.Instance.CurrentTeamRole.MakeAFortuneRate, 0, 0.3f)), teamBooks); //目前没有考虑战斗评级系统，所以默认所有战斗都是1星
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
                    alphaGroup.DOFade(1, 1).SetDelay(1).OnComplete(endSend);
                    SoundManager.GetInstance().StopBGM();
                    refreshTeamBlood();
                    refreshEnemyBlood();
                    break;
                case fail:
                    state = waiting;
                    isWin = false;
                    alphaGroup.gameObject.SetActive(true);
                    failSprite.gameObject.SetActive(true);
                    alphaGroup.DOFade(1, 1).SetDelay(1).OnComplete(endSend);
                    SoundManager.GetInstance().StopBGM();
                    refreshTeamBlood();
                    refreshEnemyBlood();
                    break;
                case waiting:
                    break;
            }
        }

        public void UpdateData(FightData fight, List<RoleData> teams, List<List<SecretData>> secrets, List<RoleData> enemys, List<ItemData> drugs, PropData limePowderData) {
            fightData = fight;
            teamsData = teams;
            enemysData = enemys;
            drugsData = drugs;
            propLimePowderData = limePowderData;
            BattleLogic.Instance.AutoFight = string.IsNullOrEmpty(PlayerPrefs.GetString("BattleNotAutoFight"));
            autoFightToggle.isOn = BattleLogic.Instance.AutoFight;
            autoFightLabel.text = autoFightToggle.isOn ? "手动\n施展" : "自动\n施展";
            BattleLogic.Instance.UpSpeed = !string.IsNullOrEmpty(PlayerPrefs.GetString("BattleUpSpeed"));
            upSpeedToggle.isOn = BattleLogic.Instance.UpSpeed;
            upSpeedLabel.text = upSpeedToggle.isOn ? "一倍\n速度" : "二倍\n速度";
            BattleLogic.Instance.Init(teamsData, secrets, enemysData);
            BattleLogic.Instance.PopEnemy();
            enemyGotBuffsScript.SetBuffDatas(BattleLogic.Instance.EnemyBuffsData);
            teamGotBuffsScript.SetBuffDatas(BattleLogic.Instance.TeamBuffsData);
            state = ready;
            readyDate = Time.fixedTime;
            //计算敌人平均等级
            averageEnemyLv = 0;
            for (int i = 0, len = enemysData.Count; i < len; i++) {
                averageEnemyLv += enemysData[i].Lv;
            }
            averageEnemyLv /= enemysData.Count;
            //记录本方的秘籍（不包括心法）
            teamBooks = new List<BookData>();
            for (int i = 0, len = teamsData.Count; i < len; i++)
            {
                for (int j = 0, len2 = teamsData[i].Books.Count; j < len2; j++)
                {
                    if (!teamsData[i].Books[j].IsMindBook)
                    {
                        teamBooks.Add(teamsData[i].Books[j]);
                    }
                }
            }
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

        void refreshLimePowder() {
            if (propLimePowderData != null && propLimePowderData.Num > 0)
            {
                limePowderBg.gameObject.SetActive(true);
                limePowderNumText.text = propLimePowderData.Num.ToString();
            }
            else
            {
                limePowderBg.gameObject.SetActive(false);
            }
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
            refreshLimePowder();
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
            //绝学视图
            if (teamsData.Count > 0) {
                teamInBattleLostKnowledgeContainer.UpdateData(teamsData[0], true);
                teamInBattleLostKnowledgeContainer.RefreshView();
            }
            SoundManager.GetInstance().PlayBGM("bgm0004");
        }  

        public void StartDrugCD() {
            for (int i = 0, len = drugInBattleItemContainers.Count; i < len; i++) {
                drugInBattleItemContainers[i].StartCD();
            }
        }

        public static void Show(FightData fight, List<RoleData> teams, List<List<SecretData>> secrets, List<RoleData> enemys, List<ItemData> drugs, PropData limePowderData) {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Fight/BattleFightPanelViewNew", "BattleFightPanelCtrl");
            }
            Ctrl.UpdateData(fight, teams, secrets, enemys, drugs, limePowderData);
            Ctrl.RefreshView();
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Close();
            }
        }
    }
}
