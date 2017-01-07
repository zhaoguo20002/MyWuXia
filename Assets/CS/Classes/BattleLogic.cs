using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Game {
    public enum BattleProcessType {
        /// <summary>
        /// 默认行为
        /// </summary>
        [Description("默认行为")]
        None = 0,
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
        Increase = 3
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
        /// 增益效果
        /// </summary>
        public List<BattleBuff> Buffs;
        /// <summary>
        /// 减益效果
        /// </summary>
        public List<BattleBuff> DeBuffs;
        public BattleProcess(bool isTeam, BattleProcessType type, string roleId, int hurtedHP, bool isMissed, string result, List<BattleBuff> buffs, List<BattleBuff> deBuffs) {
            IsTeam = isTeam;
            Type = type;
            RoleId = roleId;
            HurtedHP = hurtedHP;
            IsMissed = isMissed;
            Result = result;
            Buffs = buffs;
            DeBuffs = deBuffs;
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
        /// 剩余回合数
        /// </summary>
        public int Round;
        public BattleBuff(BuffType type, int round) {
            Type = type;
            Round = round;
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
            return (float)Statics.ClearError((double)frame * (double)Global.FrameCost);
        }

        /// <summary>
        /// 是否自动战斗
        /// </summary>
        public bool AutoFight = false;

        const short waiting = 0;
        const short ready = 1;
        const short fight = 2;
        const short win = 3;
        const short skillDoing = 4;
        const short fail = 5;
        const short end = 6;
        short state = -1;
        long frame;
        float doSkillDate;
        float doSkillTimeout = 0.2f;
        bool paused = true;

        List<RoleData> teamsData;
        List<BuffData> teamBuffsData;
        RoleData teamRole;
        List<RoleData> enemysData;
        List<BuffData> enemyBuffsData;
        RoleData currentEnemyRole;
        int currentEnemy;
        Queue<BattleProcess> battleProcessQueue;
        Queue<List<BattleBuff>> teamBuffsResultQueue; //主角buff总结果显示队列
        Queue<List<BattleBuff>> enemyBuffsResultQueue; //敌人buff总结果显示队列
        public BattleLogic() {
            state = waiting;
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
            teamsData = teams;
            teamBuffsData = new List<BuffData>();
            enemysData = enemys;
            enemyBuffsData = new List<BuffData>();
            frame = 0;
            //合并角色
            teamRole = new RoleData();
            RoleData bindRole;
            for (int i = 0, len = teamsData.Count; i < len; i++) {
                bindRole = teamsData[i];
                bindRole.MakeJsonToModel();
                bindRole.TeamName = "Team";
                teamRole.MaxHP += bindRole.MaxHP;
                teamRole.HP += bindRole.HP;
                teamRole.MagicDefense += bindRole.MagicDefense;
                teamRole.PhysicsDefense += bindRole.PhysicsDefense;
                teamRole.Dodge += bindRole.Dodge;
                //初始化技能
                bindRole.GetCurrentBook().GetCurrentSkill().StartCD(frame);
            }
            for (int i = 0, len = enemysData.Count; i < len; i++) {
                enemysData[i].MakeJsonToModel();
                enemysData[i].TeamName = "Enemy";
            }
            currentEnemy = 0;
            state = ready;
            paused = false;
        }

        /// <summary>
        /// 判断是否胜利
        /// </summary>
        /// <returns><c>true</c> if this instance is window; otherwise, <c>false</c>.</returns>
        public bool IsWin() {
            int index = enemysData.FindIndex(item => item.HP > 0);
            return index < 0;
        }

        /// <summary>
        /// 判断是否失败
        /// </summary>
        /// <returns><c>true</c> if this instance is fail; otherwise, <c>false</c>.</returns>
        public bool IsFail() {
            return teamRole.HP <= 0;
        }

        /// <summary>
        /// 发招队列
        /// </summary>
        /// <param name="role">Role.</param>
        public void PushSkill(RoleData role) {
            if (IsFail() || IsWin()) {
                return;
            }
            role.GetCurrentBook().GetCurrentSkill().StartCD(frame);
            if (role.TeamName == "Team") {
                doSkill(role, currentEnemyRole);
                if (currentEnemyRole.HP <= 0) {
                    battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.None, currentEnemyRole.Id, 0, false, string.Format("第{0}秒: {1}被击毙", GetSecond(frame), currentEnemyRole.Name), null, null));
                }
            } else {
                doSkill(role, teamRole);
                if (teamRole.HP <= 0) {
                    battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.None, teamRole.Id, 0, false, string.Format("第{0}秒: 技不如人, 全体侠客集体阵亡", GetSecond(frame)), null, null));
                }
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
        /// 检测主角buff/debuff总结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopTeamBattleBuffResults() {
            return teamBuffsResultQueue.Count > 0 ? teamBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 检测敌人buff/debuff总结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopEnemyBattleBuffResults() {
            return enemyBuffsResultQueue.Count > 0 ? enemyBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 生成本方buff返回结构
        /// </summary>
        void createTeamBattleBuffResult() {
            List<BattleBuff> buffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = teamBuffsData.Count; i < len; i++) {
                buff = teamBuffsData[i];
                buffResult.Add(new BattleBuff(buff.Type, buff.RoundNumber));
            }
            teamBuffsResultQueue.Enqueue(buffResult);
        }

        /// <summary>
        /// 生成敌方debuff返回结构
        /// </summary>
        void createEnemyBattleBuffResult() {
            List<BattleBuff> deBuffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = enemyBuffsData.Count; i < len; i++) {
                buff = enemyBuffsData[i];
                deBuffResult.Add(new BattleBuff(buff.Type, buff.RoundNumber));
            }
            enemyBuffsResultQueue.Enqueue(deBuffResult);
        }

        /// <summary>
        /// buff/debuff添加判定逻辑
        /// </summary>
        /// <param name="buffs">Buffs.</param>
        /// <param name="toRole">To role.</param>
        void dealBuff(List<BuffData> buffs, RoleData toRole) {
            BuffData buff;
            for (int i = 0, len = buffs.Count; i < len; i++) {
                buff = buffs[i];
                if (buff.IsTrigger()) {

                }
            }
        }

        /// <summary>
        /// 主队监听器
        /// </summary>
        void teamsAction() {
            //自动战斗检测
            if (AutoFight) {
                RoleData teamData;
                for (int i = 0, len = teamsData.Count; i < len; i++) {
                    teamData = teamsData[i];
                    if (teamData.HP > 0 && teamData.GetCurrentBook().GetCurrentSkill().IsCDTimeout(frame)) {
                        PushSkill(teamData);
                    }
                }
            }
        }

        void popEnemy() {
            if (enemysData.Count > currentEnemy) {
                currentEnemyRole = enemysData[currentEnemy++];
                currentEnemyRole.GetCurrentBook().GetCurrentSkill().StartCD(frame);
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.None, currentEnemyRole.Id, 0, false, string.Format("第{0}秒: {1}现身", GetSecond(frame), currentEnemyRole.Name), null, null));
            } else {
                currentEnemyRole = null;
            }
        }

        /// <summary>
        /// 敌人监听器
        /// </summary>
        void enemysAction() {
            if (currentEnemyRole == null || currentEnemyRole.HP <= 0) {
                popEnemy();
                return;
            }
            if (currentEnemyRole != null && currentEnemyRole.HP > 0) {
                if (currentEnemyRole.GetCurrentBook().GetCurrentSkill().IsCDTimeout(frame)) {
                    PushSkill(currentEnemyRole);
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
                roundRumberStr = buff.RoundNumber <= 0 ? "1招" : (buff.RoundNumber + "招");
                roundRumberStr2 = buff.RoundNumber <= 1 ? "" : "持续" + (buff.RoundNumber + "招");
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
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">所受伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
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
            //处理攻击伤害
            BookData currentBook = fromRole.GetCurrentBook();
            SkillData currentSkill = currentBook.GetCurrentSkill();
            switch (currentSkill.Type) {
                case SkillType.FixedDamage:
                    hurtedHP = -fromRole.FixedDamage;
                    result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点伤害", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name, hurtedHP);
                    break;
                case SkillType.MagicAttack:
                    if (fromRole.IsHited(toRole)) {
                        hurtedHP = -fromRole.GetMagicDamage(toRole);
                        result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点伤害", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name, hurtedHP);
                    } else {
                        isMissed = true;
                        result = string.Format("第{0}秒: {1}施展{2}, {3}", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name, "被对手闪躲");
                    }
                    break;
                case SkillType.PhysicsAttack:
                    if (fromRole.IsHited(toRole)) {
                        hurtedHP = -fromRole.GetPhysicsDamage(toRole);
                        result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点伤害", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name, hurtedHP);
                    } else {
                        isMissed = true;
                        result = string.Format("第{0}秒: {1}施展{2}, {3}", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name, "被对手闪躲");
                    }
                    break;
                case SkillType.Plus:
                default:
                    processType = BattleProcessType.Plus;
                    result = string.Format("第{0}秒: {1}施展{2}", BattleLogic.GetSecond(frame), fromRole.Name, currentBook.Name);
                    break;
            }
            //处理扣血
            if (hurtedHP < 0) {
                toRole.DealHP(hurtedHP);
            }
            //处理buff/debuff
            string buffDesc = "";
            List<BattleBuff> addBuffs = null;
            List<BattleBuff> addDeBuffs = null;
            if (!isMissed) {
                addBuffs = new List<BattleBuff>();
                addDeBuffs = new List<BattleBuff>();
                buffDesc = ", ";
                bool hasNewBuff = false;
                BuffData buff;
                for(int i = 0, len = currentSkill.BuffDatas.Count; i < len; i++) {
                    buff = currentSkill.BuffDatas[i];
                    if (buff.IsTrigger()) {
                        teamBuffsData.Add(buff);
                        addBuffs.Add(new BattleBuff(buff.Type, buff.RoundNumber));
                        buffDesc += " " + getBuffDesc(buff, "自身") + ",";
                        hasNewBuff = true;
                    }
                }
                for(int i = 0, len = currentSkill.DeBuffDatas.Count; i < len; i++) {
                    buff = currentSkill.DeBuffDatas[i];
                    if (buff.IsTrigger()) {
                        enemyBuffsData.Add(buff);
                        addDeBuffs.Add(new BattleBuff(buff.Type, buff.RoundNumber));
                        buffDesc += " " + getBuffDesc(buff, "致敌") + ",";
                        hasNewBuff = true;
                    }
                }
                if (buffDesc.Length > 1) {
                    buffDesc = buffDesc.Remove(buffDesc.Length - 1, 1);
                }
                if (addBuffs.Count > 0) {
                    createTeamBattleBuffResult(); //通知本方buff变更
                }
                if (addDeBuffs.Count > 0) {
                    createEnemyBattleBuffResult(); //通知地方debuff变更
                }
            }
            result = string.Format("{0}{1}", result, buffDesc);
            BattleProcess process = new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result, addBuffs, addDeBuffs);
            battleProcessQueue.Enqueue(process); //添加到战斗过程队列
        }

        /// <summary>
        /// 逻辑监听器
        /// </summary>
        public void Action() {
//            switch (state) {
//                case ready:
//                    state = fight;
//                    break;
//                case fight:
//                    if (IsFail()) { //判负
//                        state = fail;
//                        break;
//                    }
//                    if (IsWin()) { //判胜
//                        state = win;
//                        break;
//                    }
//
//                    break;
//                case skillDoing:
//                    if (Time.fixedTime - doSkillDate > doSkillTimeout) {
//                        state = fight;
//                    }
//                    break;
//                case win:
//
//                    break;
//                case fail:
//
//                    break;
//                case waiting:
//                    break;
//            }
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
            if (GetSecond(frame) >= 180) {
                teamRole.HP = 0;
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.None, currentEnemyRole.Id, 0, false, string.Format("第{0}秒: 时间结束", GetSecond(frame)), null, null));
                return;
            }
            //核心逻辑
            teamsAction();
            enemysAction();
            frame++;
        }
    }   
    
}
