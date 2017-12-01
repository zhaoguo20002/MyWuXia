using UnityEngine;
using System.Collections;

namespace Game {
    /// <summary>
    /// 兵器buff类
    /// </summary>
    public class WeaponBuffData {
        /// <summary>
        /// id
        /// </summary>
        public string Id;
        /// <summary>
        /// 兵器buff类型
        /// </summary>
        public WeaponBuffType Type;
        /// <summary>
        /// 生效概率[0-100]
        /// </summary>
        public float Rate;
        /// <summary>
        /// 浮点数数值0
        /// </summary>
        public float FloatValue0;
        /// <summary>
        /// 浮点数数值1
        /// </summary>
        public float FloatValue1;
        /// <summary>
        /// 浮点数增益数值
        /// </summary>
        public float FloatIncrease;
        /// <summary>
        /// 持续时间 (单位:秒)
        /// </summary>
        public float Timeout;
        /// <summary>
        /// 技能冷却时间 (单位:秒)
        /// </summary>
        public float CDTime;

        public WeaponBuffData() {
            
        }

        /// <summary>
        /// 初始化兵器buff
        /// </summary>
        public void Init() {
            FloatIncrease = 0;
        }

        /// <summary>
        /// 判断是否触发概率
        /// </summary>
        /// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
        public bool IsTrigger() {
            return UnityEngine.Random.Range(0f, 100f) <= Rate;
        }
    }
}
