﻿using UnityEngine;
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

        //初始帧
        long initFrame;
        //当前技能冷却累加帧数
        long cDAddFrame;
        //当前技能冷却结束帧
        long cDEndFrame;

        public WeaponBuffData(string id, WeaponBuffType type, float rate = 100, float floatValue0 = 0, float floatValue1 = 0, float timeout = 0, float cDTime = 0) {
            Id = id;
            Type = type;
            Rate = rate;
            FloatValue0 = floatValue0;
            FloatValue1 = floatValue1;
            Timeout = timeout;
            CDTime = cDTime;
            Init();
        }

        /// <summary>
        /// 初始化兵器buff
        /// </summary>
        public void Init() {
            FloatIncrease = 0;
            initFrame = 0;
            cDEndFrame = 0;
            cDAddFrame = (long)Statics.ClearError((double)CDTime / (double)Global.FrameCost);
        }

        /// <summary>
        /// 判断是否触发概率
        /// </summary>
        /// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
        public bool IsTrigger() {
            return UnityEngine.Random.Range(0f, 100f) <= Rate;
        }

        /// <summary>
        /// 开始CD计时
        /// </summary>
        /// <param name="frame">Frame.</param>
        public void StartCD(long frame) {
            cDEndFrame = frame + cDAddFrame;
        }

        /// <summary>
        /// CD时间是否过期
        /// </summary>
        /// <returns><c>true</c> if this instance is CD timeout the specified frame; otherwise, <c>false</c>.</returns>
        /// <param name="frame">Frame.</param>
        public bool IsCDTimeout(long frame) {
            return frame > cDEndFrame;
        }
    }
}
