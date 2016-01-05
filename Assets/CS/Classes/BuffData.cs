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

		public BuffData() {
			Rate = 100;
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
		/// 判断是否触发概率
		/// </summary>
		/// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
		public bool IsTrigger() {
			return UnityEngine.Random.Range(0f, 100f) <= Rate;
		}
	}
}
