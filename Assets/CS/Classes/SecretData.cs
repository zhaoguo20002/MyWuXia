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
        /// 计算最终整型值
        /// </summary>
        /// <returns>The real int value.</returns>
        public int GetRealIntValue() {
            switch(Type) {
                case SecretType.IncreaseMaxHP:
                    return IntValue + (int)(Mathf.Pow((float)Quality + 1, 1.6f) * IntValue);
                case SecretType.IncreasePhysicsAttack:
                case SecretType.IncreaseMagicAttack:
                    return IntValue + (int)(Mathf.Pow((float)Quality + 1, 1.8f) * IntValue);
                case SecretType.IncreasePhysicsDefense:
                case SecretType.IncreaseMagicDefense:
                    return IntValue + (int)(Mathf.Pow((float)Quality + 1, 1.6f) * IntValue);
                case SecretType.IncreaseFixedDamage:
                    return IntValue + (int)(Mathf.Pow((float)Quality + 1, 1.7f) * IntValue);
                case SecretType.DrugResistance:
                case SecretType.DisarmResistance:
                case SecretType.VertigoResistance:
                case SecretType.CanNotMoveResistance:
                case SecretType.SlowResistance:
                case SecretType.ChaosResistance:
                case SecretType.AlarmedResistance:
//                    return Mathf.Clamp((IntValue + (int)Quality * IntValue) - 5, 0, 5);
                    return IntValue + (int)Quality;
                case SecretType.Immortal:
                    return Mathf.Clamp((IntValue + (int)Quality * IntValue) - 7, 0, 3);
                case SecretType.PlusIncreaseHP:
                    return IntValue + (int)(Mathf.Pow((float)Quality + 1, 2) * IntValue);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 计算最终浮点数型值
        /// </summary>
        /// <returns>The real float value.</returns>
        public float GetRealFloatValue() {
            switch(Type) {
                case SecretType.IncreaseMaxHPRate:
                    return FloatValue + (Mathf.Pow((float)Quality + 1, 1.2f) * FloatValue);
                case SecretType.IncreasePhysicsAttackRate:
                case SecretType.IncreaseMagicAttackRate:
                    return FloatValue + (Mathf.Pow((float)Quality + 1, 1.7f) * FloatValue);
                case SecretType.IncreasePhysicsDefenseRate:
                case SecretType.IncreaseMagicDefenseRate:
                    return FloatValue + (Mathf.Pow((float)Quality + 1, 1.3f) * FloatValue);
                case SecretType.IncreaseDodge:
                    return (float)(int)(FloatValue + (float)Quality * FloatValue);
                case SecretType.IncreaseDamageRate:
                    return FloatValue + (Mathf.Pow((float)Quality + 1, 1.1f) * FloatValue);
                case SecretType.IncreaseHurtCutRate:
                    return FloatValue + (Mathf.Pow((float)Quality + 1, 0.5f) * FloatValue);
                case SecretType.CutCD:
                    return Mathf.Clamp(FloatValue + ((int)Quality - 5) * 0.1f, 0, 1);
                case SecretType.Killed:
                    return Mathf.Clamp((FloatValue + ((float)Quality - 6) * FloatValue), 0, 0.3f);
                case SecretType.MakeAFortune:
                    return Mathf.Clamp((FloatValue + ((float)Quality - 3) * FloatValue), 0, 0.2f);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 返回诀要描述
        /// </summary>
        /// <returns>The desc.</returns>
        public string GetDesc() {
            switch(Type) {
                case SecretType.IncreaseMaxHP:
                    return string.Format("气血上限点数+{0}", GetRealIntValue());
                case SecretType.IncreaseMaxHPRate:
                    return string.Format("基础气血上限比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreasePhysicsAttack:
                    return string.Format("外功点数+{0}", GetRealIntValue());
                case SecretType.IncreasePhysicsAttackRate:
                    return string.Format("基础外功比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreasePhysicsDefense:
                    return string.Format("外防点数+{0}", GetRealIntValue());
                case SecretType.IncreasePhysicsDefenseRate:
                    return string.Format("基础外防比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreaseMagicAttack:
                    return string.Format("内功点数+{0}", GetRealIntValue());
                case SecretType.IncreaseMagicAttackRate:
                    return string.Format("基础内功比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreaseMagicDefense:
                    return string.Format("内防点数+{0}", GetRealIntValue());
                case SecretType.IncreaseMagicDefenseRate:
                    return string.Format("基础内防比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreaseFixedDamage:
                    return string.Format("固定伤害+{0}", GetRealIntValue());
                case SecretType.IncreaseDamageRate:
                    return string.Format("伤害比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreaseHurtCutRate:
                    return string.Format("减伤比例+{0}%", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.IncreaseDodge:
                    return string.Format("轻功+{0}", (int)GetRealFloatValue());
                case SecretType.DrugResistance:
                    return string.Format("中毒抵抗+{0}(取最大值)", GetRealIntValue());
                case SecretType.DisarmResistance:
                    return string.Format("缴械抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.VertigoResistance:
                    return string.Format("眩晕抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.CanNotMoveResistance:
                    return string.Format("定身抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.SlowResistance:
                    return string.Format("迟缓抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.ChaosResistance:
                    return string.Format("混乱抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.AlarmedResistance:
                    return string.Format("惊慌抵抗+{0}(最大值诀要为准)", GetRealIntValue());
                case SecretType.CutCD:
                    return string.Format("减少武功招式CD时间{0}秒", GetRealFloatValue().ToString("0.000"));
                case SecretType.Immortal:
                    return string.Format("抵御{0}次阵亡效果(最高次数诀要为准)", GetRealIntValue());
                case SecretType.Killed:
                    return string.Format("{0}%概率秒杀敌方(对Boss无效)", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.MakeAFortune:
                    return string.Format("掉落概率+{0}%(上限30%)", ((GetRealFloatValue() * 10000d + 0.005d) / 100).ToString("0.0"));
                case SecretType.PlusIncreaseHP:
                    return string.Format("气血恢复点数+{0}", GetRealIntValue());
                default:
                    return "未知属性";
            }
        }
    }
}
