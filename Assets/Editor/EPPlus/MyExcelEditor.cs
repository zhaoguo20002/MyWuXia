#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using Game;


public class MyExcelEditor : Editor
{

    [MenuItem("ExcelEditor/test")] 
    static void test()
    {
        Excel xls = new Excel();
        ExcelTable table = new ExcelTable();
        table.TableName = "test";
        string outputPath = ExcelEditor.DocsPath + "/Test2.xlsx";
        xls.Tables.Add(table);
        xls.Tables[0].SetValue(1, 1, "1");
        xls.Tables[0].SetValue(1, 2, "2");
        xls.Tables[0].SetValue(2, 1, "3");
        xls.Tables[0].SetValue(2, 2, "4");
        xls.ShowLog();
        ExcelHelper.SaveExcel(xls, outputPath);
    }

    [MenuItem("ExcelEditor/LoadXls")] 
    static void LoadXls()
    {
//        string path = Application.dataPath + "/Test/Test4.xlsx";
        string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
        Excel xls =  ExcelHelper.LoadExcel(path);
        xls.ShowLog();
        Debug.Log(xls.Tables[0].TableName);
    }

    [MenuItem("ExcelEditor/WriteXls")] 
    static void WriteXls()
    {
        Excel xls = new Excel();
        ExcelTable table = new ExcelTable();
        table.TableName = "test";
//        string outputPath = Application.dataPath + "/Test/Test2.xlsx";
        string outputPath = ExcelEditor.DocsPath + "/Test.xlsx";
        xls.Tables.Add(table);
        xls.Tables[0].SetValue(1, 1, "字段1");
        xls.Tables[0].SetValue(1, 2, "字段2");
        xls.Tables[0].SetValue(1, 3, "字段3");
        for (int i = 2; i <= 11; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                xls.Tables[0].SetValue(i, j, Random.Range(1000,100000).ToString());
            }
        }
        xls.ShowLog();
        ExcelTable table2 = new ExcelTable();
        table2.TableName = "标签2";
        xls.Tables.Add(table2);
        xls.Tables[1].SetValue(1, 1, Random.Range(1000,100000).ToString());
        xls.Tables[1].SetValue(1, 2, "测试");
        xls.Tables[1].SetValue(1, 3, "我勒个去我勒个去我勒个去我勒个去");
        xls.Tables[1].SetValue(4, 3, "我在这里");
        xls.ShowLog();
        ExcelHelper.SaveExcel(xls, outputPath);
    }



    static void appendBuffParams(RoleData role, BuffData buff) {
        switch (buff.Type) {
            case BuffType.Slow: //迟缓
                role.AttackSpeedPlus -= role.AttackSpeed * buff.Value;
                break;
            case BuffType.Fast: //疾走
                role.AttackSpeedPlus += role.AttackSpeed * buff.Value;
                break;
            case BuffType.Drug: //中毒
                int cutHP = -(int)((float)role.HP * 0.1f);
                role.HP += cutHP;
                break;
            case BuffType.CanNotMove: //定身
                role.CanChangeRole = false;
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
                break;
            case BuffType.IncreaseDamageRate: //增减益伤害比例
                role.DamageRatePlus += (int)((float)role.DamageRate * buff.Value);
                break;
            case BuffType.IncreaseFixedDamage: //增减益固定伤害
                role.FixedDamagePlus += (int)buff.Value;
                break;
            case BuffType.IncreaseHP: //增减益气血
                int addHP = (int)buff.Value;
                role.HP += addHP;
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
    }

    static void buffsAction(RoleData role, List<BuffData> buffs) {
        BuffData curBuff;
        for (int i = buffs.Count - 1; i >= 0; i--) {
            curBuff = buffs[i];
            if (curBuff.RoundNumber-- > 0) {
                appendBuffParams(role, curBuff);
            }
            if (curBuff.RoundNumber <= 0) {
                buffs.RemoveAt(i);
            }
        }
    }

    static void dealInjury(RoleData role1, RoleData role2, List<BuffData> buffs1, List<BuffData> buffs2) {
        int _costHP = 0;
        float powerMult = 0.5f;
        SkillData currentSkill = JsonManager.GetInstance().GetMapping<SkillData>("Skills", "00000001");
        float rangeLeft = 584.0f * 0.3f;
        float rangeRight = 584.0f * 0.9f;
        WeaponData weapon = role1.Weapon != null ? role1.Weapon : JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", "1");
        //判断是否出招
        if (Random.Range(rangeLeft, rangeRight) <= weapon.Width) {
            if (role1.GetCurrentBook() != null) {
                currentSkill = role1.GetCurrentBook().GetCurrentSkill();
                role1.GetCurrentBook().NextSkill();
            }
            //处理暴击
            float weaponPlusRate = Random.Range(0.0f, 1.0f);
            float weaponStepRate3 = weapon.Rates[3];
            float weaponStepRate2 = weapon.Rates[2];
            float weaponStepRate1 = weapon.Rates[1];
            if (weaponPlusRate <= weaponStepRate3) {
                powerMult = 2.0f;
            } else if (weaponPlusRate <= weaponStepRate2) {
                powerMult = 1.5f;
            } else if (weaponPlusRate <= weaponStepRate3) {
                powerMult = 1.25f;
            } else {
                powerMult = 1;
            }
        } else {
            if (role1.GetCurrentBook() != null) {
                role1.GetCurrentBook().Restart(); //出招失误后秘籍招式必须重置
            }
        }
        //计算buff和debuff, 这里需要buff对象的克隆
        BuffData buff;
        BuffData searchBuff;
        for (int i = 0; i < currentSkill.BuffDatas.Count; i++) {
            buff = currentSkill.BuffDatas[i];
            if (buff.IsTrigger()) {
                //相同的buff不能重复添加
                searchBuff = buffs1.Find((item) => { return item.Type == buff.Type; });
                if (searchBuff == null) {
                    if (buff.FirstEffect) {
                        appendBuffParams(role1, buff);
                    }
                    buffs1.Add(buff.GetClone());
                }
            }
        }
        bool willBeDebuff;
        for (int i = 0; i < currentSkill.DeBuffDatas.Count; i++) {
            buff = currentSkill.DeBuffDatas[i];
            if (buff.IsTrigger()) {
                //相同的debuff不能重复添加
                searchBuff = buffs2.Find((item) => { return item.Type == buff.Type; });
                if (searchBuff == null) {
                    willBeDebuff = true;
                    //处理抵抗
                    switch (buff.Type) {
                        case BuffType.Drug:
                            //判断敌方是否有中毒抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.DrugResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        case BuffType.Disarm:
                            //判断敌方是否有缴械抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.DisarmResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        case BuffType.Vertigo:
                            //判断敌方是否有眩晕抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.VertigoResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        case BuffType.CanNotMove:
                            //判断敌方是否有抵抗抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.CanNotMoveResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        case BuffType.Slow:
                            //判断敌方是否有迟缓抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.SlowResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        case BuffType.Chaos:
                            //判断敌方是否有混乱抵抗
                            if (buffs2.FindIndex((item) => { return item.Type == BuffType.ChaosResistance; }) >= 0) {
                                willBeDebuff = false;
                            }
                            break;
                        default:
                            break;
                    }
                    if (willBeDebuff) {
                        if (buff.FirstEffect) {
                            appendBuffParams(role2, buff);
                        }
                        buffs2.Add(buff.GetClone());
                    }
                }
            }
        }
        switch (currentSkill.Type) {
            case SkillType.Plus:
            default:
                break;
            case SkillType.FixedDamage:
                _costHP = -role1.FixedDamage;
                break;
            case SkillType.PhysicsAttack:
                _costHP = -role1.GetPhysicsDamage(role2);
                break;
            case SkillType.MagicAttack:
                _costHP = -role1.GetMagicDamage(role2);
                break;
        }
        if (_costHP < 0) {
            int hurtHP = (int)((float)_costHP * powerMult);
            if (role1.CanNotMakeMistake) {
                role2.DealHP(hurtHP);
            }
            else {
                //处理混乱误伤
                if (Random.Range(1, 100) <= 50) {
                    role2.DealHP(hurtHP);
                }
                else {
                    role1.DealHP(hurtHP);
                }
            }
            //处理反伤，被攻击者没被打死再计算反伤
            if (role2.HP > 0) {
                BuffData reboundBuff = buffs2.Find((item) => { return item.Type == BuffType.ReboundInjury; });
                if (reboundBuff != null) {
                    int reboundHurtHp = (int)((float)hurtHP * reboundBuff.Value);
                    role1.DealHP(reboundHurtHp);
                }
            }
        }
    }

    [MenuItem("ExcelEditor/MathBattles")]
    static void MathBattles() {
        string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
        Excel xls =  ExcelHelper.LoadExcel(path);
        ExcelTable table = xls.Tables[0];
        Debug.Log(table.TableName + "计算开始。。。");
        Debug.Log(table.NumberOfColumns + "," + table.NumberOfRows);
        List<string> areaNames = new List<string>() { };
        List<List<RoleData>> friends = new List<List<RoleData>>();
        List<List<RoleData>> enemys = new List<List<RoleData>>();
        string areaName;
        RoleData friend;
        RoleData enemy;
        for (int i = 1; i < table.NumberOfRows; i++) {
            areaName = table.GetValue(i, 1).ToString();
            if (!string.IsNullOrEmpty(areaName)) {
                areaNames.Add(areaName);
                friends.Add(new List<RoleData>());
                enemys.Add(new List<RoleData>());
            } else {
                if (!string.IsNullOrEmpty(table.GetValue(i, 2).ToString())) {
                    friend = new RoleData();
                    friend.IsKnight = true;
                    friend.Id = table.GetValue(i, 2).ToString();
                    friend.Name = table.GetValue(i, 3).ToString();
                    friend.Lv = int.Parse(table.GetValue(i, 4).ToString());
                    friend.DifLv4HP = int.Parse(table.GetValue(i, 7).ToString());
                    friend.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 9).ToString());
                    friend.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 11).ToString());
                    friend.DifLv4MagicAttack = int.Parse(table.GetValue(i, 13).ToString());
                    friend.DifLv4MagicDefense = int.Parse(table.GetValue(i, 15).ToString());
                    friend.DifLv4Dodge = int.Parse(table.GetValue(i, 17).ToString());
                    //处理兵器秘籍
                    if (!string.IsNullOrEmpty(table.GetValue(i, 18).ToString())) {
                        friend.ResourceWeaponDataId = table.GetValue(i, 18).ToString();
                    }
                    if (!string.IsNullOrEmpty(table.GetValue(i, 19).ToString())) {
                        string[] fen = table.GetValue(i, 19).ToString().Split(new char[] { '|' });
                        foreach (string f in fen) {
                            friend.ResourceBookDataIds.Add(f);
                        }
                    }
                    friend.Desc = table.GetValue(i, 20).ToString(); //记录武功类型 0为外功 1为内功
                    friend.MakeJsonToModel();
                    friend.Init();
                    friends[friends.Count - 1].Add(friend);
//                    Debug.Log(JsonManager.GetInstance().SerializeObject(friend));
                }
                if (!string.IsNullOrEmpty(table.GetValue(i, 21).ToString())) {
                    enemy = new RoleData();
                    enemy.IsKnight = false;
                    enemy.Id = table.GetValue(i, 21).ToString();
                    enemy.Name = table.GetValue(i, 22).ToString();
                    enemy.Lv = int.Parse(table.GetValue(i, 23).ToString());
                    enemy.DifLv4HP = int.Parse(table.GetValue(i, 26).ToString());
                    enemy.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 28).ToString());
                    enemy.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 30).ToString());
                    enemy.DifLv4MagicAttack = int.Parse(table.GetValue(i, 32).ToString());
                    enemy.DifLv4MagicDefense = int.Parse(table.GetValue(i, 34).ToString());
                    enemy.DifLv4Dodge = int.Parse(table.GetValue(i, 36).ToString());
                    enemy.Desc = table.GetValue(i, 37).ToString(); //记录武功类型 0为外功 1为内功
                    //处理兵器秘籍
                    if (!string.IsNullOrEmpty(table.GetValue(i, 38).ToString())) {
                        enemy.ResourceWeaponDataId = table.GetValue(i, 38).ToString();
                    }
                    if (!string.IsNullOrEmpty(table.GetValue(i, 39).ToString())) {
                        string[] fen = table.GetValue(i, 39).ToString().Split(new char[] { '|' });
                        foreach (string f in fen) {
                            enemy.ResourceBookDataIds.Add(f);
                        }
                    }
                    enemy.MakeJsonToModel();
                    enemy.Init();
//                Debug.Log(JsonManager.GetInstance().SerializeObject(enemy));
                    enemys[enemys.Count - 1].Add(enemy);
                }
            }
        }

        //生成excel
        Excel outputXls = new Excel();
        ExcelTable outputTable= new ExcelTable();
        outputTable.TableName = "理想伤害统计";
        string outputPath = ExcelEditor.DocsPath + "/理想伤害统计表.xlsx";
        outputXls.Tables.Add(outputTable);

        outputXls.Tables[0].SetValue(1, 1, "战斗区域");
        outputXls.Tables[0].SetValue(1, 2, "攻方");
        outputXls.Tables[0].SetValue(1, 3, "守方");
        outputXls.Tables[0].SetValue(1, 4, "战斗次数");
        outputXls.Tables[0].SetValue(1, 5, "攻方胜利数");
        outputXls.Tables[0].SetValue(1, 6, "攻方失败数");
        outputXls.Tables[0].SetValue(1, 7, "攻方胜率");
        outputXls.Tables[0].SetValue(1, 8, "攻方命中率");
        outputXls.Tables[0].SetValue(1, 9, "攻方闪避率");
        outputXls.Tables[0].SetValue(1, 10, "攻方HP平均余量");
        outputXls.Tables[0].SetValue(1, 11, "平均持续招数");
        outputXls.Tables[0].SetValue(1, 12, "攻方兵器");
        outputXls.Tables[0].SetValue(1, 13, "攻方秘籍");
        outputXls.Tables[0].SetValue(1, 14, "守方兵器");
        outputXls.Tables[0].SetValue(1, 15, "守方秘籍");
        outputXls.Tables[0].SetValue(1, 16, "守方HP平均余量");

        int rowIndex = 1;

        //假设在单位战斗次数中没次攻击都100%成功
        int fightTimes = 20;
        int times;
        int winTimes;
        int failTimes; //出招次数
        int attackTimes;
        int hitedTimes;
        int missTimes;
        int freindLeftHP;
        int enemyLeftHP;
        int maxRounds = 300;
        int rounds;

        List<BuffData> teamBuffs = new List<BuffData>();
        List<BuffData> enemyBuffs = new List<BuffData>();
        for (int i = 0, len0 = friends.Count; i < len0; i++) {
            for (int j = 0, len1 = friends[i].Count; j < len1; j++) {
                friend = friends[i][j];
                for (int k = 0, len3 = enemys[i].Count; k < len3; k++) {
                    enemy = enemys[i][k];
                    times = fightTimes;
                    winTimes = 0;
                    failTimes = 0;
                    attackTimes = 0;
                    hitedTimes = 0;
                    missTimes = 0;
                    freindLeftHP = 0;
                    enemyLeftHP = 0;
                    while (times-- > 0) {
                        friend.HP = friend.MaxHP;
                        enemy.HP = enemy.MaxHP;
                        teamBuffs.Clear();
                        enemyBuffs.Clear();
                        rounds = maxRounds;
                        while (rounds > 0 && friend.HP > 0 && enemy.HP > 0) {
                            if (friend.AttackSpeed >= enemy.AttackSpeed) {
                                if (friend.HP > 0) {
                                    attackTimes++; //记录总的出手次数
                                    if (friend.IsHited(enemy)) {
//                                        enemy.HP -= friend.Desc == "0" ? friend.GetPhysicsDamage(enemy) : friend.GetMagicDamage(enemy);
                                        dealInjury(friend, enemy, teamBuffs, enemyBuffs);
                                        hitedTimes++; //记录命中次数
                                    }
                                }
                                if (enemy.HP > 0) {
                                    if (enemy.IsHited(friend)) {
//                                        friend.HP -= enemy.Desc == "0" ? enemy.GetPhysicsDamage(enemy) : enemy.GetMagicDamage(enemy);
                                        dealInjury(enemy, friend, enemyBuffs, teamBuffs);
                                    } else {
                                        missTimes++; //记录闪避次数
                                    }
                                }
                            } else {
                                if (enemy.HP > 0) {
                                    if (enemy.IsHited(friend)) {
//                                        friend.HP -= enemy.Desc == "0" ? enemy.GetPhysicsDamage(enemy) : enemy.GetMagicDamage(enemy);
                                        dealInjury(enemy, friend, enemyBuffs, teamBuffs);
                                    } else {
                                        missTimes++; //记录闪避次数
                                    }
                                }
                                if (friend.HP > 0) {
                                    attackTimes++; //记录总的出手次数
                                    if (friend.IsHited(enemy)) {
//                                        enemy.HP -= friend.Desc == "0" ? friend.GetPhysicsDamage(enemy) : friend.GetMagicDamage(enemy);
                                        dealInjury(friend, enemy, teamBuffs, enemyBuffs);
                                        hitedTimes++; //记录命中次数
                                    }
                                }
                            }
                            rounds--; //回合数
                            buffsAction(friend, teamBuffs);
                            buffsAction(enemy, enemyBuffs);
                        }
                        if (enemy.HP <= 0) {
                            winTimes++; //记录获胜次数
                            freindLeftHP += Mathf.Clamp(friend.HP, 0, friend.HP);
                        } else if (friend.HP <= 0) {
                            failTimes++; //记录失败次数
                            enemyLeftHP += Mathf.Clamp(enemy.HP, 0, enemy.HP);
                        }
                    }
//                    Debug.Log(string.Format("{0}与{1}战斗次数:{2}，胜利{3}次，失败{4}次， 胜率:{5}%，命中率:{6}%，闪避率:{7}%，平均气血剩余值:{8}，平均持续招数:{9}", 
//                        friend.Name, 
//                        enemy.Name, 
//                        fightTimes, 
//                        winTimes, 
//                        failTimes, 
//                        (int)((float)winTimes / (float)fightTimes * 100), 
//                        (int)((float)hitedTimes / (float)attackTimes * 100), 
//                        (int)((float)missTimes / (float)attackTimes * 100), 
//                        winTimes > 0 ?freindLeftHP / winTimes : 0, 
//                        (float)attackTimes / (float)fightTimes));
                    rowIndex++;
                    outputXls.Tables[0].SetValue(rowIndex, 1, areaNames[i]);
                    outputXls.Tables[0].SetValue(rowIndex, 2, friend.Name);
                    outputXls.Tables[0].SetValue(rowIndex, 3, enemy.Name);
                    outputXls.Tables[0].SetValue(rowIndex, 4, fightTimes.ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 5, winTimes.ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 6, failTimes.ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 7, ((int)((float)winTimes / (float)fightTimes * 100)).ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 8, ((int)((float)hitedTimes / (float)attackTimes * 100)).ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 9, ((int)((float)missTimes / (float)attackTimes * 100)).ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 10, (winTimes > 0 ? freindLeftHP / winTimes : 0).ToString() + (winTimes > 0 ? string.Format("/{0}({1}%)", friend.MaxHP, (int)((float)(freindLeftHP / winTimes) / (float)friend.MaxHP * 100)) : ""));
                    outputXls.Tables[0].SetValue(rowIndex, 11, ((float)attackTimes / (float)fightTimes).ToString());
                    outputXls.Tables[0].SetValue(rowIndex, 12, friend.Weapon != null ? friend.Weapon.Name : "");
                    string friendBookNames = "";
                    foreach (BookData book in friend.Books) {
                        friendBookNames += book.Name + "|";
                    }
                    if (friendBookNames.Length > 1) {
                        friendBookNames = friendBookNames.Remove(friendBookNames.Length - 1, 1);
                    }
                    outputXls.Tables[0].SetValue(rowIndex, 13, friendBookNames);
                    outputXls.Tables[0].SetValue(rowIndex, 14, enemy.Weapon != null ? enemy.Weapon.Name : "");
                    string enemyBookNames = "";
                    foreach (BookData book in enemy.Books) {
                        enemyBookNames += book.Name + "|";
                    }
                    if (enemyBookNames.Length > 1) {
                        enemyBookNames = enemyBookNames.Remove(enemyBookNames.Length - 1, 1);
                    }
                    outputXls.Tables[0].SetValue(rowIndex, 15, enemyBookNames);
                    outputXls.Tables[0].SetValue(rowIndex, 16, (failTimes > 0 ? enemyLeftHP / failTimes : 0).ToString() + (failTimes > 0 ? string.Format("/{0}({1}%)", enemy.MaxHP, (int)((float)(enemyLeftHP / failTimes) / (float)enemy.MaxHP * 100)) : ""));
                }
            }
            rowIndex++;
        }

        ExcelHelper.SaveExcel(outputXls, outputPath); //生成excel

        Debug.Log(table.TableName + "计算结束，请查看原表。");
    }
}
#endif
