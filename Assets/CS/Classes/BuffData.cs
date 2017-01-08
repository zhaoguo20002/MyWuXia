using UnityEngine;
using System.Collections;
using System;

namespace Game {
	public class BuffData : ICloneable {
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// 类型
		/// </summary>
		public BuffType Type;
		/// <summary>
		/// 生效概率[0-100]
		/// </summary>
		public float Rate;
		/// <summary>
		/// 持续回合数
		/// </summary>
		public int RoundNumber;
		/// <summary>
		/// buff/debuff增减益的值
		/// (根据buff/debuff的类型不同,该值的范围不同,百分比的类型该值都为0-1)
		/// </summary>
		public float Value;
		/// <summary>
		/// 是否首回合生效
		/// </summary>
        public bool FirstEffect;
        /// <summary>
        /// 持续时间 (单位:秒)
        /// </summary>
        public float Timeout;

        //初始帧
        long initFrame;
        //持续时间累加帧数
        long timeoutAddFrame;
        //持续时间结束帧
        long timeoutEndFrame;
        //间隔累加帧数
        long skipAddFrame;
        //间隔结束帧
        long skipEndFrame;

		public BuffData() {
			Rate = 100;
            initFrame = 0;
		}

		/// <summary>
		///  浅克隆(只能完全克隆值类型属性)
		/// </summary>
		public object Clone() {
			return this.MemberwiseClone();
		}
        /// <summary>
        /// 返回克隆之后的BuffData实体
        /// </summary>
        /// <returns>The clone.</returns>
        public BuffData GetClone() {
            return (BuffData)Clone();
        }

		/// <summary>
		/// 返回克隆之后的BuffData实体
		/// </summary>
		/// <returns>The clone.</returns>
        public BuffData GetClone(long frame) {
            timeoutAddFrame = (long)Statics.ClearError(((double)Timeout + 0.1d) / (double)Global.FrameCost);
            timeoutEndFrame = frame + timeoutAddFrame;
            skipAddFrame = (long)Statics.ClearError(1.0d / (double)Global.FrameCost);
            skipEndFrame = frame;
            return GetClone();
		}

		/// <summary>
		/// 判断是否触发概率
		/// </summary>
		/// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
		public bool IsTrigger() {
			return UnityEngine.Random.Range(0f, 100f) <= Rate;
		}

        /// <summary>
        /// buff时间是否过期
        /// </summary>
        /// <returns><c>true</c> if this instance is CD timeout the specified frame; otherwise, <c>false</c>.</returns>
        /// <param name="frame">Frame.</param>
        public bool IsTimeout(long frame) {
            return frame > timeoutEndFrame;
        }

        /// <summary>
        /// 获取当前Buff时间进度
        /// </summary>
        /// <returns>The progress.</returns>
        /// <param name="frame">Frame.</param>
        public float GetProgress(long frame) {
            return Mathf.Clamp01((float)(timeoutEndFrame - frame) / (float)timeoutAddFrame);
        }

        /// <summary>
        /// buff间隔时间是否过期
        /// </summary>
        /// <returns><c>true</c> if this instance is CD timeout the specified frame; otherwise, <c>false</c>.</returns>
        /// <param name="frame">Frame.</param>
        public bool IsSkipTimeout(long frame) {
            bool result = frame > skipEndFrame;
            if (result) {
                skipEndFrame = frame + skipAddFrame;
            }
            return result;
        }
	}
}
