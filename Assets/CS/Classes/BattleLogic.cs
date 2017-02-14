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
        public BattleProcess(bool isTeam, BattleProcessType type, string roleId, int hurtedHP, bool isMissed, string result, SkillData skill = null) {
            IsTeam = isTeam;
            Type = type;
            RoleId = roleId;
            HurtedHP = hurtedHP;
            IsMissed = isMissed;
            Result = result;
            Skill = skill;
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
        public void Init(List<RoleData> teams, List<RoleData> enemys) {
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
            RoleData bindRole;
            for (int i = 0, len = TeamsData.Count; i < len; i++) {
                bindRole = TeamsData[i];
                bindRole.MakeJsonToModel();
                bindRole.Init();
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
                //初始化技能
                if (bindRole.GetCurrentBook() != null) {
                    bindRole.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
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
        public void PushSkill(RoleData role) {
            if (IsFail() || IsWin() || !role.CanUseSkill) {
                return;
            }
            if (role.TeamName == "Team") {
                if (CurrentEnemyRole.HP > 0) {
                    role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    doSkill(role, CurrentEnemyRole);
                }
            } else {
                if (CurrentTeamRole.HP > 0) {
                    role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    doSkill(role, CurrentTeamRole);
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
                for (int i = 0, len = TeamsData.Count; i < len; i++) {
                    teamData = TeamsData[i];
                    if (teamData.HP > 0 && teamData.GetCurrentBook() != null) {
                        if (!teamData.CanUseSkill) {
                            teamData.GetCurrentBook().GetCurrentSkill().ExtendOneFrameCD();
                        }
                        if (teamData.GetCurrentBook().GetCurrentSkill().IsCDTimeout(Frame))
                        {
                            PushSkill(teamData);
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
                    return string.Format("{0}{1}{2}触发<color=\"#FF9326\">疾走(加速{3}%)</color>持续{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
                case BuffType.Slow:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">迟缓(减速{3}%)</color>{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
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
                    battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒:技不如人,全体侠客集体阵亡", GetSecond(Frame))));
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
                        int cutHP = -(int)((float)role.HP * 0.1f);
                        dealHP(role, cutHP);
                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, cutHP, false, string.Format("第{0}秒:{1}中毒,损耗<color=\"#FF0000\">{2}</color>点气血", GetSecond(Frame), role.Name, cutHP)));
                        checkDie(role);
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
        void doSkill(RoleData fromRole, RoleData toRole) {
            BattleProcessType processType = BattleProcessType.Attack;
            int hurtedHP = 0;
            bool isMissed = false;
            string result = "";
            BookData currentBook = fromRole.GetCurrentBook();
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
            for (int i = 0, len = currentSkill.DeBuffDatas.Count; i < len; i++)
            {
                buff = currentSkill.DeBuffDatas[i].GetClone(Frame);
                if (buff.IsTrigger() && deBuffs.FindIndex(item => item.Type == buff.Type) < 0)
                {
                    //处理硬直免疫
                    switch(buff.Type) {
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
            if (buffDesc.Length > 1)
            {
                buffDesc = buffDesc.Remove(buffDesc.Length - 1, 1);
            }
            //处理攻击伤害
            switch (currentSkill.Type) {
                case SkillType.FixedDamage:
                    hurtedHP = -fromRole.FixedDamage;
                    result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点固定伤害", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                    break;
                case SkillType.MagicAttack:
//                    if (!toRole.CanMiss || fromRole.IsHited(toRole)) {
                        if (toRole.MagicDefense < 10000)
                        { 
                            //防御小于10000正常计算
                            hurtedHP = -fromRole.GetMagicDamage(toRole);
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点内功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                        }
                        else
                        {
                            //防御大于10000则免疫伤害
                            isMissed = true;
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手<color=\"#FF0000\">无视</color>");
                        }
//                    } else {
//                        isMissed = true;
//                        result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手闪躲");
//                    }
                    break;
                case SkillType.PhysicsAttack:
                    if (!toRole.CanMiss || fromRole.IsHited(toRole)) {
                        if (toRole.PhysicsDefense < 10000)
                        { 
                            //防御小于10000正常计算
                            hurtedHP = -fromRole.GetPhysicsDamage(toRole);
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,造成对手<color=\"#FF0000\">{4}</color>点外功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                        }
                        else
                        {
                            //防御大于10000则免疫伤害
                            isMissed = true;
                            result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手<color=\"#FF0000\">无视</color>");
                        }
                    } else {
                        isMissed = true;
                        result = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,{4}", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, "被对手闪躲");
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
                        dealHP(fromRole, hurtedHP);
                        string accidentalInjuryResult = string.Format("第{0}秒:{1}施展<color=\"{2}\">{3}</color>,混乱中造成自己<color=\"#FF0000\">{4}</color>点外功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, Statics.GetQualityColorString(currentBook.Quality), currentBook.Name, hurtedHP);
                        battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.AccidentalInjury, fromRole.Id, hurtedHP, false, accidentalInjuryResult, currentSkill));
                        checkDie(fromRole);
                        return;
                    }
                }
                dealHP(toRole, hurtedHP);
            }
            //攻击类型技能通知要放到最后面,这样才能保证基础属性增益或者建议的基础上计算伤害
            if (currentSkill.Type != SkillType.Plus) {
                result = string.Format("{0}{1}", result, buffDesc.Length > 0 ? ("," + buffDesc) : "");
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result, currentSkill)); //添加到战斗过程队列
            }
            checkDie(toRole);

            if (hurtedHP < 0 && toRole.HP > 0)
            {
                //处理反伤
                List<BuffData> findBuffs = toRole.TeamName == "Team" ? TeamBuffsData : EnemyBuffsData;
                BuffData reboundInjuryBuff = findBuffs.Find(item => item.Type == BuffType.ReboundInjury);
                if (reboundInjuryBuff != null)
                {
                    int reboundInjuryHP = -(int)(hurtedHP * reboundInjuryBuff.Value);
                    dealHP(fromRole, reboundInjuryHP);
                    string reboundInjuryResult = string.Format("第{0}秒:{1}受到<color=\"#FF0000\">{2}</color>点<color=\"#FF9326\">反震伤害</color>", BattleLogic.GetSecond(Frame), fromRole.Name, reboundInjuryHP);
                    battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", BattleProcessType.ReboundInjury, fromRole.Id, reboundInjuryHP, false, reboundInjuryResult, currentSkill));
                    checkDie(fromRole);
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
            //核心逻辑
            teamsAction();
            enemysAction();
            Frame++;
        }
    }   
    
}
