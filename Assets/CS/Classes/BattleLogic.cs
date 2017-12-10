using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Game {
    public enum BattleProcessType {
        /// <summary>
        /// 普通行为
        /// </summary>
        [Description("普通行为")]
        Normal = 0,
        /// <summary>
        /// 攻击行为
        /// </summary>
        [Description("攻击行为")]
        Attack = 1,
        /// <summary>
        /// 对己方增益
        /// </summary>
        [Description("对己方增益")]
        Plus = 2,
        /// <summary>
        /// Buff/Debuff增减益行为
        /// </summary>
        [Description("Buff/Debuff增减益行为")]
        Increase = 3,
        /// <summary>
        /// 敌人出现
        /// </summary>
        [Description("敌人出现")]
        EnemyPop = 4,
        /// <summary>
        /// 吃药
        /// </summary>
        [Description("吃药")]
        Drug = 5,
        /// <summary>
        /// 被反伤
        /// </summary>
        [Description("被反伤")]
        ReboundInjury = 6,
        /// <summary>
        /// 被误伤
        /// </summary>
        [Description("被误伤")]
        AccidentalInjury = 7
    }
    /// <summary>
    /// 战斗过程类
    /// </summary>
    public class BattleProcess {
        /// <summary>
        /// 是否主队
        /// </summary>
        public bool IsTeam;
        /// <summary>
        /// 行为类型
        /// </summary>
        public BattleProcessType Type;
        /// <summary>
        /// 发招角色id
        /// </summary>
        public string RoleId;
        /// <summary>
        /// 易造成的伤害值
        /// </summary>
        public int HurtedHP;
        /// <summary>
        /// 是否闪避
        /// </summary>
        public bool IsMissed;
        /// <summary>
        /// 战报结果
        /// </summary>
        public string Result;
        /// <summary>
        /// 技能id
        /// </summary>
        public SkillData Skill;
        /// <summary>
        /// 是否无视伤害
        /// </summary>
        public bool IsIgnoreAttack;
        public BattleProcess(bool isTeam, BattleProcessType type, string roleId, int hurtedHP, bool isMissed, string result, SkillData skill = null, bool isIgnoreAttack = false) {
            IsTeam = isTeam;
            Type = type;
            RoleId = roleId;
            HurtedHP = hurtedHP;
            IsMissed = isMissed;
            Result = result;
            Skill = skill;
            IsIgnoreAttack = isIgnoreAttack;
        }
    }
    /// <summary>
    /// 战斗buff持续类
    /// </summary>
    public class BattleBuff {
        /// <summary>
        /// buff类型
        /// </summary>
        public BuffType Type;
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Timeout;
        public BattleBuff(BuffType type, float timeout) {
            Type = type;
            Timeout = timeout;
        }
    }
    /// <summary>
    /// 战斗核心逻辑
    /// </summary>
    public class BattleLogic {
        static BattleLogic _instance = null;
        public static BattleLogic Instance {
            get { 
                if (_instance == null) {
                    _instance = new BattleLogic();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 根据帧数换算成秒数
        /// </summary>
        /// <returns>The second.</returns>
        /// <param name="frame">Frame.</param>
        public static float GetSecond(long frame) {
            return (float)Statics.ClearError((double)frame * (double)Global.FrameCost, 10);
        }

        /// <summary>
        /// 是否自动战斗
        /// </summary>
        public bool AutoFight = true;
        /// <summary>
        /// 是否加速
        /// </summary>
        public bool UpSpeed = false;

        public long Frame;
        bool paused = true;

        public List<RoleData> TeamsData;
        public List<BuffData> TeamBuffsData;
        public RoleData CurrentTeamRole;
        public List<RoleData> EnemysData;
        public List<BuffData> EnemyBuffsData;
        public RoleData CurrentEnemyRole;
        public int CurrentEnemy;
        Queue<BattleProcess> battleProcessQueue;
        Queue<List<BattleBuff>> teamBuffsResultQueue; //主角buff总结果显示队列
        Queue<List<BattleBuff>> enemyBuffsResultQueue; //敌人buff总结果显示队列
        public BattleLogic() {
            battleProcessQueue = new Queue<BattleProcess>();
            teamBuffsResultQueue = new Queue<List<BattleBuff>>();
            enemyBuffsResultQueue = new Queue<List<BattleBuff>>();
        }

        /// <summary>
        /// 初始化战场
        /// </summary>
        /// <param name="teams">Teams.</param>
        /// <param name="enemys">Enemys.</param>
        public void Init(List<RoleData> teams, List<List<SecretData>> secrets, List<RoleData> enemys) {
            TeamsData = teams;
            TeamBuffsData = new List<BuffData>();
            CurrentEnemyRole = null;
            EnemysData = enemys;
            EnemyBuffsData = new List<BuffData>();
            battleProcessQueue.Clear();
            teamBuffsResultQueue.Clear();
            enemyBuffsResultQueue.Clear();
            Frame = 0;
            //合并角色
            CurrentTeamRole = new RoleData();
            CurrentTeamRole.TeamName = "Team";
            CurrentTeamRole.Name = "本方队伍";
            CurrentTeamRole.MaxHP = 0;
            CurrentTeamRole.Init();
            CurrentTeamRole.ImmortalNum = 0;
            CurrentTeamRole.MakeAFortuneRate = 0;
            RoleData bindRole;
            BookData book;
            SkillData skill;
            for (int i = 0, len = TeamsData.Count; i < len; i++) {
                bindRole = TeamsData[i];
                bindRole.MakeJsonToModel();
                bindRole.Init();
                bindRole.PlusSecretsToRole(secrets[i]);
                bindRole.TeamName = "Team";
                CurrentTeamRole.MaxHP += bindRole.MaxHP;
                CurrentTeamRole.HP += bindRole.HP;
                CurrentTeamRole.MagicDefense += bindRole.MagicDefense;
                CurrentTeamRole.PhysicsDefense += bindRole.PhysicsDefense;
                CurrentTeamRole.Dodge += bindRole.Dodge;
                //处理抗性,取最大值
                CurrentTeamRole.DrugResistance = Mathf.Max(CurrentTeamRole.DrugResistance, bindRole.DrugResistance);
                CurrentTeamRole.DisarmResistance = Mathf.Max(CurrentTeamRole.DisarmResistance, bindRole.DisarmResistance);
                CurrentTeamRole.VertigoResistance = Mathf.Max(CurrentTeamRole.VertigoResistance, bindRole.VertigoResistance);
                CurrentTeamRole.CanNotMoveResistance = Mathf.Max(CurrentTeamRole.CanNotMoveResistance, bindRole.CanNotMoveResistance);
                CurrentTeamRole.SlowResistance = Mathf.Max(CurrentTeamRole.SlowResistance, bindRole.SlowResistance);
                CurrentTeamRole.ChaosResistance = Mathf.Max(CurrentTeamRole.ChaosResistance, bindRole.ChaosResistance);
                //处理复活次数
                if (bindRole.ImmortalNum > CurrentTeamRole.ImmortalNum) {
                    CurrentTeamRole.ImmortalNum = bindRole.ImmortalNum;
                }
                CurrentTeamRole.MakeAFortuneRate += bindRole.MakeAFortuneRate;
                //初始化普通秘籍
                book = bindRole.GetCurrentBook();
                if (book != null) {
                    skill = book.GetCurrentSkill();
                    if (skill != null)
                    {
                        //处理秘籍cd时间判定
                        if (bindRole.SkillCutCD > 0) {
                            skill.UpdateCDTime(skill.CDTime - bindRole.SkillCutCD);
                        }
                        skill.StartCD(Frame);
                    }
                }
                //初始化绝学
                book = bindRole.GetLostKnowledge();
                if (book != null) {
                    skill = book.GetCurrentSkill();
                    if (skill != null)
                    {
                        //处理绝学cd时间判定
                        if (bindRole.SkillCutCD > 0) {
                            skill.UpdateCDTime(skill.CDTime - bindRole.SkillCutCD);
                        }
                        skill.StartCD(Frame);
                    }
                }
            }
            for (int i = 0, len = EnemysData.Count; i < len; i++) {
                EnemysData[i].MakeJsonToModel();
                EnemysData[i].TeamName = "Enemy";
                EnemysData[i].Init();
            }
            CurrentEnemy = 0;
            paused = false;
        }

        /// <summary>
        /// 判断是否胜利
        /// </summary>
        /// <returns><c>true</c> if this instance is window; otherwise, <c>false</c>.</returns>
        public bool IsWin() {
            int index = EnemysData.FindIndex(item => item.HP > 0);
            return index < 0;
        }

        /// <summary>
        /// 判断是否失败
        /// </summary>
        /// <returns><c>true</c> if this instance is fail; otherwise, <c>false</c>.</returns>
        public bool IsFail() {
            return CurrentTeamRole.HP <= 0;
        }

        /// <summary>
        /// 发招队列
        /// </summary>
        /// <param name="role">Role.</param>
        public void PushSkill(RoleData role, bool isLostKnowledge = false) {
            if (IsFail() || IsWin() || !role.CanUseSkill) {
                return;
            }
            if (role.TeamName == "Team") {
                if (CurrentEnemyRole.HP > 0) {
                    if (!isLostKnowledge)
                    {
                        role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    }
                    else
                    {
                        role.GetLostKnowledge().GetCurrentSkill().StartCD(Frame);
                    }
                    doSkill(role, CurrentEnemyRole, isLostKnowledge);
                }
            } else {
                if (CurrentTeamRole.HP > 0) {
                    if (!isLostKnowledge)
                    {
                        role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    }
                    else
                    {
                        role.GetLostKnowledge().GetCurrentSkill().StartCD(Frame);
                    }
                    doSkill(role, CurrentTeamRole, isLostKnowledge);
                }
            }
        }

        /// <summary>
        /// 吃药回血
        /// </summary>
        /// <param name="addHP">Add H.</param>
        public void PushDrug(int addHP) {
            if (IsFail() || IsWin() || !CurrentTeamRole.CanUseTool) {
                return;
            }
            if (addHP > 0 && CurrentTeamRole.HP > 0) {
                dealHP(CurrentTeamRole, addHP);
                battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.Drug, CurrentTeamRole.Id, addHP, false, string.Format("第{0}秒:服用药物,{1}气血", GetSecond(Frame), addHP)));
            }
        }

        /// <summary>
        /// 检测战斗指令
        /// </summary>
        /// <returns>The process.</returns>
        public BattleProcess PopProcess() {
            return battleProcessQueue.Count > 0 ? battleProcessQueue.Dequeue() : null;
        }

        /// <summary>
        /// 返回战斗指令长度
        /// </summary>
        /// <returns>The process count.</returns>
        public int GetProcessCount() {
            return battleProcessQueue.Count;
        }

        /// <summary>
        /// 检测主角buff/debuff追加结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopTeamBattleBuffResults() {
            return teamBuffsResultQueue.Count > 0 ? teamBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 检测敌人buff/debuff追加结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopEnemyBattleBuffResults() {
            return enemyBuffsResultQueue.Count > 0 ? enemyBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 生成本方buff追加返回结构
        /// </summary>
        void createTeamBattleBuffResult() {
            List<BattleBuff> buffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = TeamBuffsData.Count; i < len; i++) {
                buff = TeamBuffsData[i];
                buffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
            }
            teamBuffsResultQueue.Enqueue(buffResult);
        }

        /// <summary>
        /// 生成敌方debuff追加返回结构
        /// </summary>
        void createEnemyBattleBuffResult() {
            List<BattleBuff> deBuffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = EnemyBuffsData.Count; i < len; i++) {
                buff = EnemyBuffsData[i];
                deBuffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
            }
            enemyBuffsResultQueue.Enqueue(deBuffResult);
        }

        /// <summary>
        /// 主队监听器
        /// </summary>
        void teamsAction() {
            if (CurrentTeamRole != null && CurrentTeamRole.HP > 0) {
                //清空旧的buff和debuff
                CurrentTeamRole.ClearPluses();
                for (int j = 0, len = TeamsData.Count; j < len; j++) {
                    TeamsData[j].ClearPluses();
                }
                BuffData curBuff;
                for (int i = TeamBuffsData.Count - 1; i >= 0; i--) {
                    curBuff = TeamBuffsData[i];
                    appendBuffParams(CurrentTeamRole, curBuff);
                    for (int j = 0, len = TeamsData.Count; j < len; j++) {
                        appendBuffParams(TeamsData[j], curBuff);
                    }
                    if (curBuff.IsTimeout(Frame)) {
                        TeamBuffsData.RemoveAt(i);
                    }
                }
            }
            //自动战斗检测
            if (AutoFight) {
                RoleData teamData;
                BookData book;
                SkillData skill;
                for (int i = 0, len = TeamsData.Count; i < len; i++) {
                    teamData = TeamsData[i];
                    if (teamData.HP > 0) {
                        //普通秘籍
                        book = teamData.GetCurrentBook();
                        if (book != null)
                        {
                            skill = book.GetCurrentSkill();
                            if (skill != null)
                            {
                                if (!teamData.CanUseSkill) {
                                    skill.ExtendOneFrameCD();
                                }
                                if (skill.IsCDTimeout(Frame))
                                {
                                    PushSkill(teamData);
                                }
                            }
                        }
                        //绝学
                        book = teamData.GetLostKnowledge();
                        if (book != null)
                        {
                            skill = book.GetCurrentSkill();
                            if (skill != null)
                            {
                                if (!teamData.CanUseSkill) {
                                    skill.ExtendOneFrameCD();
                                }
                                if (skill.IsCDTimeout(Frame))
                                {
                                    PushSkill(teamData, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PopEnemy() {
            if (EnemysData.Count > CurrentEnemy) {
                CurrentEnemyRole = EnemysData[CurrentEnemy++];
                if (CurrentEnemyRole.GetCurrentBook() != null) {
                    CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                }
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.EnemyPop, CurrentEnemyRole.Id, 0, false, string.Format("第{0}秒:{1}现身", GetSecond(Frame), CurrentEnemyRole.Name)));
                EnemyBuffsData.Clear(); //清掉原有的debuff
                createEnemyBattleBuffResult();
            }
        }

        /// <summary>
        /// 敌人监听器
        /// </summary>
        void enemysAction() {
            if (CurrentEnemyRole == null || CurrentEnemyRole.HP <= 0) {
                PopEnemy();
                return;
            }
            if (CurrentEnemyRole != null && CurrentEnemyRole.HP > 0) {
                //清空旧的buff和debuff
                CurrentEnemyRole.ClearPluses();
                BuffData curBuff;
                for (int i = EnemyBuffsData.Count - 1; i >= 0; i--) {
                    curBuff = EnemyBuffsData[i];
                    appendBuffParams(CurrentEnemyRole, curBuff);
                    if (curBuff.IsTimeout(Frame)) {
                        EnemyBuffsData.RemoveAt(i);
                    }
                }
                if (CurrentEnemyRole.HP > 0 && CurrentEnemyRole.GetCurrentBook() != null) {
                    if (!CurrentEnemyRole.CanUseSkill) {
                        CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().ExtendOneFrameCD();
                    }
                    if (CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().IsCDTimeout(Frame))
                    {
                        PushSkill(CurrentEnemyRole);
                    }
                }
            }
        }

        string getBuffDesc(BuffData buff, string head = "自身") {
//            string rateStr = buff.Rate >= 100 ? "" : "<color=\"#A64DFF\">" + buff.Rate + "%</color>概率";
            string rateStr = "";
            string firstEffectStr = buff.FirstEffect ? "" : "下招起";
            string roundRumberStr;
            string roundRumberStr2;
            if (!buff.FirstEffect && buff.RoundNumber <= 0) {
                roundRumberStr = "<color=\"#B20000\">无效</color>";
                roundRumberStr2 = "<color=\"#B20000\">无效</color>";
            } 
            else {
                roundRumberStr = buff.Timeout <= 0 ? "" : (buff.Timeout + "秒");
                roundRumberStr2 = buff.Timeout <= 0 ? "" : "持续" + (buff.Timeout + "秒");
            }
            switch(buff.Type) {
                case BuffType.CanNotMove:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">定身</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Chaos:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">混乱</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Disarm:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">缴械</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Drug:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">中毒</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Fast:
                    return string.Format("{0}{1}{2}触发<color=\"#FF9326\">疾走(轻功+{3}%)</color>持续{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
                case BuffType.Slow:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">迟缓(轻功削弱{3}%)</color>{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
                case BuffType.Vertigo:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">眩晕</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Alarmed:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">惊慌</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.CanNotMoveResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫定身</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.ChaosResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫混乱</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.DisarmResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫缴械</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.DrugResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫中毒</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.SlowResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫迟缓</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.VertigoResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫眩晕</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.ReboundInjury:
                    return string.Format("{0}<color=\"#FF9326\">{3}获得反伤效果(将受到伤害的{2}％反弹给对方)</color>持续{1}", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.MustMiss:
                    return string.Format("{0}<color=\"#FF9326\">{2}获得必闪效果(闪避开一切招式)</color>持续{1}", rateStr, roundRumberStr, head);
                case BuffType.SuckHP:
                    return string.Format("{0}<color=\"#FF9326\">{3}将输出伤害的{2}%转化为自身气血</color>持续{1}", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.ForgotMe:
                    return string.Format("{0}<color=\"#FF9326\">{2}获得无我状态(无视一切负面debuff)</color>持续{1}", rateStr, roundRumberStr, head);
                case BuffType.SolveDrug:
                    return string.Format("{0}<color=\"#FF9326\">{2}获得解毒状态(将损失气血转化为增益气血)</color>持续{1}", rateStr, roundRumberStr, head);
                case BuffType.Blindness:
                    return string.Format("{0}<color=\"#FF9326\">{3}致盲(武功cd时间增加{2}%)</color>持续{1}", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.MakeDebuffStrong:
                    return string.Format("{0}<color=\"#FF9326\">{3}所中的debuff时间增加{2}秒</color>持续{1}", rateStr, roundRumberStr, buff.Value, head);
                case BuffType.CDTimeout:
                    return string.Format("{0}<color=\"#FF9326\">{2}所有侠客的武功CD时间瞬间清零</color>{1}内只能生效一次", rateStr, roundRumberStr, head);
                case BuffType.UpSpeedCDTime:
                    return string.Format("{0}<color=\"#FF9326\">{3}所有侠客的武功CD时间减少{2}%</color>持续{1}", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.AddRateMaxHP:
                    return string.Format("{0}<color=\"#FF9326\">{3}获得回天效果(瞬间恢复{2}％气血)</color>{1}内只能生效一次", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.ClearDebuffs:
                    return string.Format("{0}<color=\"#FF9326\">{2}获得洁净效果(清空负面状态)</color>{1}内只能生效一次", rateStr, roundRumberStr, head);
                case BuffType.IncreaseDamageRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">最终伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseFixedDamage:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">固定伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseHP:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseHurtCutRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">所受伤害</color>" + (buff.Value > 0 ? "-" : "+") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMagicAttack:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#2693FF\">内功点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMagicAttackRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#2693FF\">内功比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMagicDefense:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#73B9FF\">内防点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMagicDefenseRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#73B9FF\">内防比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMaxHP:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMaxHPRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreasePhysicsAttack:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF0000\">外功点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreasePhysicsAttackRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF0000\">外功比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreasePhysicsDefense:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF7373\">外防点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreasePhysicsDefenseRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF7373\">外防比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.Normal:
                    return "无";
                default:
                    return "";
            }
        }

        void dealHP(RoleData role, int increaseHP) {
            role.HP += increaseHP;
        }

        void checkDie(RoleData role) {
            if (role.HP <= 0) {
                if (role.TeamName == "Team") {
                    if (role.HP <= 0 && role.ImmortalNum > 0)
                    {
                        role.ImmortalNum--;
                        role.HP = 1; //复活后血量为1
                        battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.Plus, role.Id, 1, false, string.Format("第{0}秒:<color=\"#FFFF00\">不死金刚诀要生效，垂死后挣扎站起！</color>", GetSecond(Frame))));
                    }
                    else
                    {
                        battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒:技不如人,全体侠客集体阵亡", GetSecond(Frame))));
                    }
                } else {
                    battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒:{1}被击毙", GetSecond(Frame), role.Name)));
                }
            }
        }

        /// <summary>
        /// 处理buff和debuff的属性叠加
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="buff">Buff.</param>
        void appendBuffParams(RoleData role, BuffData buff) {
            switch (buff.Type) {
                case BuffType.Slow: //迟缓
                    role.DodgePlus -= role.Dodge * buff.Value;
                    break;
                case BuffType.Fast: //疾走
                    role.DodgePlus += role.Dodge * buff.Value;
                    break;
                case BuffType.Drug: //中毒
                    if (buff.IsSkipTimeout(Frame)) {
                        List<BuffData> buffs = role.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
                        //判断是否存在解毒状态
                        BuffData solveDrugBuff = buffs.Find(item => item.Type == BuffType.SolveDrug);
                        if (solveDrugBuff == null)
                        {
                            int cutHP = -(int)((float)role.HP * 0.1f);
                            dealHP(role, cutHP);
                            battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, cutHP, false, string.Format("第{0}秒:{1}中毒,损耗<color=\"#FF0000\">{2}</color>点气血", GetSecond(Frame), role.Name, cutHP)));
                            checkDie(role);
                        }
                        else
                        {
                            int gotHP = (int)((float)role.HP * 0.1f);
                            dealHP(role, gotHP);
                            battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, gotHP, false, string.Format("第{0}秒:{1}<color=\"#FFFF00\">触发解毒状态</color>,恢复<color=\"#00FF00\">{2}</color>点气血", GetSecond(Frame), role.Name, gotHP)));
                        }
                    }
                    break;
                case BuffType.CanNotMove: //定身
                    role.CanChangeRole = false;
                    role.CanMiss = false;
                    break;
                case BuffType.Chaos: //混乱
                    role.CanNotMakeMistake = false;
                    break;
                case BuffType.Disarm: //缴械
                    role.CanUseSkill = false;
                    break;
                case BuffType.Vertigo: //眩晕
                    role.CanUseSkill = false;
                    role.CanChangeRole = false;
                    role.CanUseTool = false;
                    role.CanMiss = false;
                    break;
                case BuffType.Alarmed:
                    role.CanUseTool = false;
                    break;
                case BuffType.IncreaseDamageRate: //增减益伤害比例
                    role.DamageRatePlus += (int)((float)role.DamageRate * buff.Value);
                    break;
                case BuffType.IncreaseFixedDamage: //增减益固定伤害
                    role.FixedDamagePlus += (int)buff.Value;
                    break;
                case BuffType.IncreaseHP: //增减益气血
                    if (buff.IsSkipTimeout(Frame)) {
                        int addHP = (int)buff.Value;
                        dealHP(role, addHP);
                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, addHP, false, string.Format("第{0}秒:{1}{2}{3}点气血", GetSecond(Frame), role.Name, addHP > 0 ? "恢复" : "损耗", "<color=\"" + (addHP > 0 ? "#00FF00" : "#FF0000") + "\">" + addHP + "</color>")));
                        checkDie(role);
                    }
                    break;
                case BuffType.IncreaseMaxHP: //增减益气血上限
                    role.MaxHPPlus += (int)buff.Value;
                    break;
                case BuffType.IncreaseMaxHPRate: //增减益气血上限比例
                    role.MaxHPPlus += (int)((float)role.MaxHP * buff.Value);
                    break;
                case BuffType.IncreaseHurtCutRate: //增减减伤比例
                    role.HurtCutRatePlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicAttack: //增减益内功点数
                    role.MagicAttackPlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicAttackRate: //增减益内功比例
                    role.MagicAttackPlus += (role.MagicAttack * buff.Value);
                    break;
                case BuffType.IncreaseMagicDefense: //增减益内防点数
                    role.MagicDefensePlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicDefenseRate: //增减益内防比例
                    role.MagicDefensePlus += (role.MagicDefense * buff.Value);
                    break;
                case BuffType.IncreasePhysicsAttack: //增减益外功点数
                    role.PhysicsAttackPlus += buff.Value;
                    break;
                case BuffType.IncreasePhysicsAttackRate: //增减益外功比例
                    role.PhysicsAttackPlus += (role.PhysicsAttack * buff.Value);
                    break;
                case BuffType.IncreasePhysicsDefense: //增减益外防点数
                    role.PhysicsDefensePlus += buff.Value;
                    break;
                case BuffType.IncreasePhysicsDefenseRate: //增减益外防比例
                    role.PhysicsDefensePlus += (role.PhysicsDefense * buff.Value);
                    break;
                case BuffType.AddRateMaxHP: //一次性恢复气血上限百分比的气血
                    if (!buff.TookEffect) {
                        buff.TookEffect = true;
                        int addHP = (int)(role.MaxHP * buff.Value);
                        dealHP(role, addHP);
                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, addHP, false, string.Format("第{0}秒:{1}{2}{3}点气血", GetSecond(Frame), role.Name, addHP > 0 ? "恢复" : "损耗", "<color=\"" + (addHP > 0 ? "#00FF00" : "#FF0000") + "\">" + addHP + "</color>")));
                    }
                    break;
                case BuffType.ClearDebuffs: //清除身上所有的debuf
                    if (!buff.TookEffect)
                    {
                        buff.TookEffect = true;
                        role.CanUseSkill = true;
                        role.CanChangeRole = true;
                        role.CanUseTool = true;
                        role.CanMiss = true;
                        List<BuffData> buffs = role.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
                        for (int i = buffs.Count - 1; i >= 0; i--)
                        {
                            switch (buffs[i].Type)
                            {
                                case BuffType.Alarmed:
                                case BuffType.CanNotMove:
                                case BuffType.Chaos:
                                case BuffType.Disarm:
                                case BuffType.Drug:
                                case BuffType.Slow:
                                case BuffType.Vertigo:
                                    buffs.RemoveAt(i);
                                    break;
                            }
                        }
//                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, 0, false, string.Format("第{0}秒:{1}{2}{3}状态", GetSecond(Frame), role.Name, "清除掉自身一个", "<color=\"#FF0000\">" + addHP + "</color>")));
                    }
                    break;
                case BuffType.Blindness: //致盲（所有侠客武功cd时间增加）
                case BuffType.UpSpeedCDTime: //武功CD时间减少n%
                    if (role.TeamName == "Team")
                    {
                        for (int i = 0, len = TeamsData.Count; i < len; i++)
                        {
                            TeamsData[i].GetCurrentBook().GetCurrentSkill().AddCDTimePlusScale(buff.Value);
                        }
                    }
                    else
                    {
                        if (CurrentEnemyRole != null)
                        {
                            CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().AddCDTimePlusScale(buff.Value);
                        }
                    }
                    break;
                case BuffType.CDTimeout: //武功CD时间瞬间清零
                    if (!buff.TookEffect)
                    {
                        buff.TookEffect = true;
                        if (role.TeamName == "Team")
                        {
                            for (int i = 0, len = TeamsData.Count; i < len; i++)
                            {
                                TeamsData[i].GetCurrentBook().GetCurrentSkill().Reset();
                            }
                        }
                        else
                        {
                            if (CurrentEnemyRole != null)
                            {
                                CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().Reset();
                            }
                        }
                    }
                    break;
                case BuffType.MakeDebuffStrong://增加debuff时间
                    if (!buff.TookEffect)
                    {
                        buff.TookEffect = true;
                        List<BuffData> buffs = role.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
                        for (int i = buffs.Count - 1; i >= 0; i--)
                        {
                            switch (buffs[i].Type)
                            {
                                case BuffType.Alarmed:
                                case BuffType.CanNotMove:
                                case BuffType.Chaos:
                                case BuffType.Disarm:
                                case BuffType.Drug:
                                case BuffType.Slow:
                                case BuffType.Vertigo:
                                    buffs[i].AddTime(buff.Value);
                                    break;
                            }
                        }
                        battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒:{1}的负面debuff时间增加了{2}秒", GetSecond(Frame), role.Name, buff.Value)));
                    }
                    break;
                default:
                    break;
            }
//            if (!role.CanUseSkill && role.GetCurrentBook() != null) {
//                role.GetCurrentBook().GetCurrentSkill().ExtendOneFrameCD();
//            }
        }

        /// <summary>
        /// 发招
        /// </summary>
        /// <param name="roleRole">Role role.</param>
        /// <param name="toRole">To role.</param>
        void doSkill(RoleData fromRole, RoleData toRole, bool isLostKnowledge = false) {
            BattleProcessType processType = BattleProcessType.Attack;
            int hurtedHP = 0;
            int killHurtedHP;
            bool isMissed = false;
            bool isIgnoreAttack = false;
            string result = "";
            string weaponBuffResult = "";
            BookData currentBook = !isLostKnowledge ? fromRole.GetCurrentBook() : fromRole.GetLostKnowledge();
            SkillData currentSkill = currentBook.GetCurrentSkill();

            if (currentSkill.Type == SkillType.Plus) {
                processType = BattleProcessType.Plus;
                result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name);
                //己方增益技能的技能施展通知要先于攻击类型技能,这样才能表现出先释放技能再回血的效果
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result, currentSkill)); //添加到战斗过程队列
            }

            //处理buff/debuff
            string buffDesc = "";
            List<BuffData> buffs = fromRole.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
            List<BuffData> deBuffs = fromRole.TeamName == "Team" ? EnemyBuffsData : TeamBuffsData;
            //判断敌人身上是否有必闪buff，有的话则敌人miss
            if (deBuffs.FindIndex(item => item.Type == BuffType.MustMiss) >= 0) {
                if (toRole.Weapon != null && toRole.Weapon.Buffs.Count > 0)
                {
                    //处理全真教闪金兵器 承影 效果 敌人闪避后增加基础内功200%，最高叠加至1000%，命中敌人后内功增益消失
                    WeaponBuffData qzWeapon0Buff0 = toRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.MAMultipleIncreaseWhenBeMissed);
                    if (qzWeapon0Buff0 != null && qzWeapon0Buff0.FloatIncrease < qzWeapon0Buff0.FloatValue1)
                    {
                        qzWeapon0Buff0.FloatIncrease += qzWeapon0Buff0.FloatValue0;
                        weaponBuffResult += string.Format("<color=\"#FFFF00\">承影爆发！基础内功增加{0}%(命中敌人后增益将清除)</color>", (int)((qzWeapon0Buff0.FloatIncrease * 100d + 0.005d) / 100));
                    }
                }
                isMissed = true;
                result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4} {5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手强力闪躲", weaponBuffResult);
                return;
            }
            //判断敌人身上是否有无敌buff，有的话攻击无效
            if (deBuffs.FindIndex(item => item.Type == BuffType.Invincible) >= 0)
            {
                isMissed = true;
                isIgnoreAttack = true;
                result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手的<color=\"#FF0000\">强力气墙</color>化解！");
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result, currentSkill, isIgnoreAttack)); //添加到战斗过程队列
                return;
            }
            BuffData buff;
            for (int i = 0, len = currentSkill.BuffDatas.Count; i < len; i++)
            {
                buff = currentSkill.BuffDatas[i].GetClone(Frame);
                //持续时间为0的buff表示可能是一次性使用的buff，如果一次性增加攻击等
                if (buff.IsTrigger() && (buff.Timeout == 0 || buffs.FindIndex(item => item.Type == buff.Type) < 0))
                {
                    if (buff.FirstEffect)
                    {
                        appendBuffParams(fromRole.TeamName == "Team" ? CurrentTeamRole : CurrentEnemyRole, buff);
                        for (int j = 0, len2 = TeamsData.Count; j < len2; j++)
                        {
                            appendBuffParams(TeamsData[j], buff);
                        }
                    }
                    else
                    {
                        buff.IsSkipTimeout(Frame + 1); //不是立即执行的buff强制是间隔计时器启动
                    }
                    if (buff.Timeout > 0)
                    {
                        buffs.Add(buff);
                        buffDesc += getBuffDesc(buff, "自身") + ",";
                    }
                }
            }
            //判断无我状态，无我状态下无视所有的debuff
            if (deBuffs.FindIndex(item => item.Type == BuffType.ForgotMe) < 0)
            {
                for (int i = 0, len = currentSkill.DeBuffDatas.Count; i < len; i++)
                {
                    buff = currentSkill.DeBuffDatas[i].GetClone(Frame);
                    if (buff.IsTrigger() && deBuffs.FindIndex(item => item.Type == buff.Type) < 0)
                    {
                        //处理硬直免疫
                        switch (buff.Type)
                        {
                            case BuffType.CanNotMove:
                                buff.Timeout -= toRole.CanNotMoveResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">定身被免疫</color>,";
                                }
                                break;
                            case BuffType.Chaos:
                                buff.Timeout -= toRole.ChaosResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">混乱被免疫</color>,";
                                }
                                break;
                            case BuffType.Disarm:
                                buff.Timeout -= toRole.DisarmResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">缴械被免疫</color>,";
                                }
                                break;
                            case BuffType.Drug:
                                buff.Timeout -= toRole.DrugResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">中毒被免疫</color>,";
                                }
                                break;
                            case BuffType.Slow:
                                buff.Timeout -= toRole.SlowResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">迟缓被免疫</color>,";
                                }
                                break;
                            case BuffType.Vertigo:
                                buff.Timeout -= toRole.VertigoResistance;
                                if (buff.Timeout <= 0)
                                {
                                    buffDesc += "<color=\"#FF0000\">眩晕被免疫</color>,";
                                }
                                break;
                            default:
                                break;
                        }
                        if (buff.Timeout > 0)
                        {
                            //更新buff时间(硬直抗性值直接对应硬直时间1代表减免1秒的硬直)
                            buff.UpdateTimeout(Frame);
                        }
                        else
                        {
                            //硬直时间小于等于0则硬直效果不生效
                            continue;
                        }
                        if (buff.FirstEffect)
                        {
                            appendBuffParams(fromRole.TeamName == "Team" ? CurrentEnemyRole : CurrentTeamRole, buff);
                        }
                        else
                        {
                            buff.IsSkipTimeout(Frame + 1); //不是立即执行的debuff强制是间隔计时器启动
                        }
                        if (buff.Timeout > 0)
                        {
                            deBuffs.Add(buff);
                        }
                        buffDesc += getBuffDesc(buff, "致敌") + ",";
                    }
                }
            }
            else
            {
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, toRole.Id, 0, false, string.Format("第{0}秒:{1}身上的无我状态抵消了一切负面debuff", GetSecond(Frame), toRole.Name)));
            }
            if (buffDesc.Length > 1)
            {
                buffDesc = buffDesc.Remove(buffDesc.Length - 1, 1);
            }
            //处理攻击伤害
            killHurtedHP = 0;
            switch (currentSkill.Type)
            {
                case SkillType.FixedDamage:
                    //处理秒杀
                    if (!toRole.IsBoss && fromRole.KilledRate > 0) {
                        if (UnityEngine.Random.Range(0.0f, 1.01f) < fromRole.KilledRate)
                        {
                            killHurtedHP = -toRole.HP;
                        }
                    }
                    hurtedHP = killHurtedHP == 0 ? -(fromRole.FixedDamage + fromRole.FixedDamagePlus) : killHurtedHP;
                    result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点固定伤害{5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP, killHurtedHP != 0 ? "<color=\"#FFFF00\">(一击必杀!)</color>" : "");
                    break;
                case SkillType.MagicAttack:
//                    if (!toRole.CanMiss || fromRole.IsHited(toRole))
//                    {
                        if (fromRole.Weapon != null && fromRole.Weapon.Buffs.Count > 0)
                        {
                            //判断承影基础内功增益
                            WeaponBuffData qzWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.MAMultipleIncreaseWhenBeMissed);
                            if (qzWeapon0Buff0 != null && qzWeapon0Buff0.FloatIncrease > 0)
                            {
                                fromRole.MagicAttackPlus += (fromRole.MagicAttack * qzWeapon0Buff0.FloatIncrease);
                                qzWeapon0Buff0.FloatIncrease = 0;
                            }

                            //处理逍遥派闪金兵器 清音 效果 x%概率触发攻击吸收气墙，气墙回血一次后消失，cd30秒
                            WeaponBuffData xyWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.AttackAbsorption);
                            if (xyWeapon0Buff0 != null && xyWeapon0Buff0.IsCDTimeout(Frame) &&  xyWeapon0Buff0.IsTrigger())
                            {
                                xyWeapon0Buff0.StartCD(Frame);
                                xyWeapon0Buff0.FloatIncrease = 1;
                                weaponBuffResult += string.Format("<color=\"#FFFF00\">清音爆发！形成转化伤害为气血的气场(吸收{0}次伤害后消失)</color>", xyWeapon0Buff0.FloatIncrease);
                            }
                        }

                        if ((toRole.MagicDefense - fromRole.MagicAttack) < 10000)
                        { 
                            //防御-攻击小于10000正常计算
                            //处理秒杀
                            if (!toRole.IsBoss && fromRole.KilledRate > 0)
                            {
                                if (UnityEngine.Random.Range(0.0f, 1.01f) < fromRole.KilledRate)
                                {
                                    killHurtedHP = -toRole.HP;
                                }
                            }
                            hurtedHP = killHurtedHP == 0 ? -fromRole.GetMagicDamage(toRole) : killHurtedHP;
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点内功伤害{5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP, killHurtedHP != 0 ? "<color=\"#FFFF00\">(一击必杀!)</color>" : "");
                        }
                        else
                        {
                            //处理大理闪金兵器 天谴 效果 对处于免疫攻击状态下的敌人造成大幅伤害（基础内功提高600%）
                            WeaponBuffData dlWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.BreachAttack);
                            if (dlWeapon0Buff0 == null)
                            {
                                //防御大于10000则免疫伤害
                                isMissed = true;
                                isIgnoreAttack = true;
                                result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手<color=\"#FF0000\">无视</color>");
                            }
                            else
                            {
                                fromRole.MagicAttackPlus += (fromRole.MagicAttack * dlWeapon0Buff0.FloatValue0);
                                hurtedHP = -fromRole.GetMagicDamage(toRole);
                                result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点内功伤害 <color=\"#FFFF00\">天谴爆发！破解对方不坏金身！</color>", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                            }
                        }
//                    }
//                    else
//                    {
//                        isMissed = true;
//                        result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4} {5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手闪躲", weaponBuffResult);
//                    }
                    break;
                case SkillType.PhysicsAttack:
                    if (fromRole.Weapon != null && fromRole.Weapon.Buffs.Count > 0) {
                        //处理丐帮闪金兵器 啸天狂龙 效果0 气血每降低x%基础外功增加y%
                        WeaponBuffData gbWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.PAUpWhenHPDown);
                        if (gbWeapon0Buff0 != null)
                        {
                            fromRole.PhysicsAttackPlus += (fromRole.PhysicsAttack * (gbWeapon0Buff0.FloatValue1 / gbWeapon0Buff0.FloatValue0 * Mathf.Clamp01(1 - fromRole.HPRate)));
                        }

                        //处理岳家军闪金兵器 神威 效果 每次攻击x%概率基础外功增加100%，最高叠加至500%
                        WeaponBuffData yjjWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.PAMultipleIncrease);
                        if (yjjWeapon0Buff0 != null)
                        {
                            if (yjjWeapon0Buff0.FloatIncrease < 5 && yjjWeapon0Buff0.IsTrigger())
                            {
                                yjjWeapon0Buff0.FloatIncrease++;
                                weaponBuffResult += string.Format("<color=\"#FFFF00\">神威爆发！基础外功增加{0}%</color>", (int)((yjjWeapon0Buff0.FloatIncrease * 100d + 0.005d) / 100));
                            }
                            if (yjjWeapon0Buff0.FloatIncrease > 0)
                            {
                                fromRole.PhysicsAttackPlus += (fromRole.PhysicsAttack * yjjWeapon0Buff0.FloatIncrease);
                            }
                        }

                        //处理少林闪金兵器 伏虎 效果 x%概率触发无敌气墙，持续y秒，cd20秒
                        WeaponBuffData slWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.InvincibleWall);
                        if (slWeapon0Buff0 != null && yjjWeapon0Buff0.IsCDTimeout(Frame) && yjjWeapon0Buff0.IsTrigger())
                        {
                            slWeapon0Buff0.StartCD(Frame);
                            if (buffs.FindIndex(item => item.Type == BuffType.Invincible) < 0)
                            {
                                BuffData invincibleBuff = new BuffData();
                                invincibleBuff.Type = BuffType.Invincible;
                                invincibleBuff.Timeout = slWeapon0Buff0.Timeout;
                                invincibleBuff.UpdateTimeout(Frame);
                                buffs.Add(invincibleBuff);
                                weaponBuffResult += string.Format("<color=\"#FFFF00\">伏虎爆发！形成无敌气墙，持续{0}秒</color>", slWeapon0Buff0.FloatValue0);
                            }
                        }
                    }

                    if (!toRole.CanMiss || fromRole.IsHited(toRole))
                    {
                        if ((toRole.PhysicsDefense - fromRole.PhysicsAttack) < 10000)
                        { 
                            //防御-攻击小于10000正常计算
                            //处理秒杀
                            if (!toRole.IsBoss && fromRole.KilledRate > 0) {
                                if (UnityEngine.Random.Range(0.0f, 1.01f) < fromRole.KilledRate)
                                {
                                    killHurtedHP = -toRole.HP;
                                }
                            }
                            hurtedHP = killHurtedHP == 0 ? -fromRole.GetPhysicsDamage(toRole) : killHurtedHP;
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点外功伤害{5} {6}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP, killHurtedHP != 0 ? "<color=\"#FFFF00\">(一击必杀!)</color>" : "", weaponBuffResult);
                        }
                        else
                        {
                            //防御大于10000则免疫伤害
                            isMissed = true;
                            isIgnoreAttack = true;
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4} {5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手<color=\"#FF0000\">无视</color>", weaponBuffResult);
                        }
                    }
                    else
                    {
                        if (toRole.Weapon != null && toRole.Weapon.Buffs.Count > 0)
                        {
                            //处理全真教闪金兵器 承影 效果 敌人闪避后增加基础内功200%，最高叠加至1000%，命中敌人后内功增益消失
                            WeaponBuffData qzWeapon0Buff0 = toRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.MAMultipleIncreaseWhenBeMissed);
                            if (qzWeapon0Buff0 != null && qzWeapon0Buff0.FloatIncrease < qzWeapon0Buff0.FloatValue1)
                            {
                                qzWeapon0Buff0.FloatIncrease += qzWeapon0Buff0.FloatValue0;
                                weaponBuffResult += string.Format("<color=\"#FFFF00\">承影爆发！基础内功增加{0}%(命中敌人后增益将清除)</color>", (int)((qzWeapon0Buff0.FloatIncrease * 100d + 0.005d) / 100));
                            }
                        }
                        isMissed = true;
                        result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4} {5}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手闪躲", weaponBuffResult);
                    }
                    break;
                default:
                    
                    break;
            }
            //处理扣血
            if (hurtedHP < 0) {
                //处理混乱
                if (!fromRole.CanNotMakeMistake) {
                    if (UnityEngine.Random.Range(0, 100) <= 50)
                    {
                        dealHP(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole, hurtedHP);
                        string accidentalInjuryResult = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,混乱中造成自己<color=\"#FF0000\">{4}</color>点外功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                        battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.AccidentalInjury, fromRole.Id, hurtedHP, false, accidentalInjuryResult, currentSkill));
                        checkDie(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole);
                        return;
                    }
                }
                dealHP(toRole, hurtedHP);
            }
            //攻击类型技能通知要放到最后面,这样才能保证基础属性增益或者建议的基础上计算伤害
            if (currentSkill.Type != SkillType.Plus) {
                result = string.Format("{0}{1}", result, buffDesc.Length > 0 ? ("," + buffDesc) : "");
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result, currentSkill, isIgnoreAttack)); //添加到战斗过程队列
            }
            checkDie(toRole);

            //处理吸血
            if (hurtedHP < 0)
            {
                BuffData suckHPBuff = buffs.Find(item => item.Type == BuffType.SuckHP);
                if (suckHPBuff != null)
                {
                    int suckHP = (int)Mathf.Abs(hurtedHP * suckHPBuff.Value);
                    //通知回血
                    battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.Increase, fromRole.Id, suckHP, false, string.Format("第{0}秒:{1}将输出转换为<color=\"#00FF00\">{2}</color>气血", BattleLogic.GetSecond(Frame), fromRole.Name, suckHP), currentSkill)); //添加到战斗过程队列
                    dealHP(fromRole, suckHP);
                }
            }

            if (hurtedHP < 0 && toRole.HP > 0)
            {
                //处理反伤
                List<BuffData> findBuffs = toRole.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
                BuffData reboundInjuryBuff = findBuffs.Find(item => item.Type == BuffType.ReboundInjury);
                if (reboundInjuryBuff != null)
                {
                    int reboundInjuryHP = (int)(hurtedHP * reboundInjuryBuff.Value);
                    dealHP(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole, reboundInjuryHP);
                    string reboundInjuryResult = string.Format("第{0}秒:{1}受到<color=\"#FF0000\">{2}</color>点<color=\"#FF9326\">反震伤害</color>", BattleLogic.GetSecond(Frame), fromRole.Name, reboundInjuryHP);
                    battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.ReboundInjury, fromRole.Id, reboundInjuryHP, false, reboundInjuryResult, currentSkill));
                    checkDie(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole);
                }

                //处理丐帮闪金兵器 啸天狂龙 效果1 生命低于30%时附加反伤y%伤害效果
                if (toRole.Weapon != null && toRole.Weapon.Buffs.Count > 0 && toRole.HPRate < 0.3f) {
                    WeaponBuffData gbWeapon0Buff1 = toRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.ReboundInjuryWhenHPDown);
                    if (gbWeapon0Buff1 != null)
                    {
                        int reboundInjuryHP = (int)(hurtedHP * gbWeapon0Buff1.FloatValue0);
                        dealHP(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole, reboundInjuryHP);
                        string reboundInjuryResult = string.Format("第{0}秒:<color=\"#FFFF00\">啸天狂龙爆发！</color>{1}受到<color=\"#FF0000\">{2}</color>点<color=\"#FF9326\">反震伤害</color>", BattleLogic.GetSecond(Frame), fromRole.Name, reboundInjuryHP);
                        battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.ReboundInjury, fromRole.Id, reboundInjuryHP, false, reboundInjuryResult, currentSkill));
                        checkDie(fromRole.TeamName == "Team" ? CurrentTeamRole : fromRole);
                    }
                }

                //处理 清音吸血
                WeaponBuffData xyWeapon0Buff0 = fromRole.Weapon.Buffs.Find(item => item.Type == WeaponBuffType.AttackAbsorption);
                if (xyWeapon0Buff0 != null && xyWeapon0Buff0.FloatIncrease > 0)
                {
                    xyWeapon0Buff0.FloatIncrease--;
                    int absorptionHP = Mathf.Abs(hurtedHP);
                    dealHP(toRole, absorptionHP);
                    battleProcessQueue.Enqueue(new BattleProcess(toRole.TeamName == "Team", BattleProcessType.Increase, toRole.Id, absorptionHP, false, string.Format("第{0}秒:{1}的强力气场生效，将全部伤害转换为<color=\"#00FF00\">{2}</color>点气血", GetSecond(Frame), toRole.Name, absorptionHP)));
                }
            }
        }

        /// <summary>
        /// 逻辑监听器
        /// </summary>
        public void Action() {
            if (paused) {
                return;
            }
            if (IsFail()) {
                paused = true;
                return;
            }
            if (IsWin()) {
                paused = true;
                return;
            }
            //战斗超过3分钟强制主角输
            if (GetSecond(Frame) >= 180) {
                CurrentTeamRole.HP = 0;
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, CurrentEnemyRole.Id, 0, false, string.Format("第{0}秒:时间结束", GetSecond(Frame))));
                return;
            }
            int loopTimes = UpSpeed ? 2 : 1;
            while (loopTimes-- > 0)
            {
                //核心逻辑
                teamsAction();
                enemysAction();
                Frame++;
            }
        }
    }   
    
}
