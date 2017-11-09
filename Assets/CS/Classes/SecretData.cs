using UnityEngine;
using System.Collections;

namespace Game {
    public class SecretData {
        /// <summary>
        /// 数据主键id
        /// </summary>
        public int PrimaryKeyId;
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型
        /// </summary>
        public SecretType Type;
        /// <summary>
        /// Icon Id
        /// </summary>
        public string IconId;
        /// <summary>
        /// 品质
        /// </summary>
        public QualityType Quality;
        /// <summary>
        /// 归属秘籍id
        /// </summary>
        public string BelongToBookId;
        /// <summary>
        /// 整型参数
        /// </summary>
        public int IntValue;
        /// <summary>
        /// 浮点型参数
        /// </summary>
        public float FloatValue;

        public SecretData() {
            
        }

        /// <summary>
        /// 返回诀要描述
        /// </summary>
        /// <returns>The desc.</returns>
        public string GetDesc() {
            switch(Type) {
                case SecretType.IncreaseMaxHP:
                    return string.Format("气血上限点数+{0}", IntValue);
                case SecretType.IncreaseMaxHPRate:
                    return string.Format("基础气血上限比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreasePhysicsAttack:
                    return string.Format("外功点数+{0}", IntValue);
                case SecretType.IncreasePhysicsAttackRate:
                    return string.Format("基础外功比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreasePhysicsDefense:
                    return string.Format("外防点数+{0}", IntValue);
                case SecretType.IncreasePhysicsDefenseRate:
                    return string.Format("基础外防比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreaseMagicAttack:
                    return string.Format("内功点数+{0}", IntValue);
                case SecretType.IncreaseMagicAttackRate:
                    return string.Format("基础内功比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreaseMagicDefense:
                    return string.Format("内防点数+{0}", IntValue);
                case SecretType.IncreaseMagicDefenseRate:
                    return string.Format("基础内防比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreaseFixedDamage:
                    return string.Format("固定伤害+{0}", IntValue);
                case SecretType.IncreaseDamageRate:
                    return string.Format("伤害比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreaseHurtCutRate:
                    return string.Format("减伤比例+{0}%", (FloatValue * 100d) / 100);
                case SecretType.IncreaseDodge:
                    return string.Format("轻功+{0}", IntValue);
                case SecretType.DrugResistance:
                    return string.Format("中毒抵抗+{0}", IntValue);
                case SecretType.DisarmResistance:
                    return string.Format("缴械抵抗+{0}", IntValue);
                case SecretType.VertigoResistance:
                    return string.Format("眩晕抵抗+{0}", IntValue);
                case SecretType.CanNotMoveResistance:
                    return string.Format("定身抵抗+{0}", IntValue);
                case SecretType.SlowResistance:
                    return string.Format("迟缓抵抗+{0}", IntValue);
                case SecretType.ChaosResistance:
                    return string.Format("混乱抵抗+{0}", IntValue);
                case SecretType.AlarmedResistance:
                    return string.Format("惊慌抵抗+{0}", IntValue);
                case SecretType.CutCD:
                    return string.Format("减少武功招式CD时间{0}秒", FloatValue);
                case SecretType.Immortal:
                    return string.Format("抵御{0}次阵亡效果", IntValue);
                case SecretType.Killed:
                    return string.Format("{0}%概率秒杀敌方(对Boss无效)", (FloatValue * 100d) / 100);
                case SecretType.MakeAFortune:
                    return string.Format("掉落概率+{0}%", (FloatValue * 100d) / 100);
                default:
                    return "未知属性";
            }
        }
    }
}
