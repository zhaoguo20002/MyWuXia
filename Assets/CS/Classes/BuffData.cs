using UnityEngine;
using System.Collections;

namespace Game {
	public class BuffData {
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// 类型
		/// </summary>
		public BuffType Type;
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
	}
}
