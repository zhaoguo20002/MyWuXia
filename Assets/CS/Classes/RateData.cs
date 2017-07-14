using UnityEngine;
using System.Collections;

namespace Game {
	public class RateData {
		/// <summary>
		/// 掉落概率[0-100]
		/// </summary>
		public float Rate;
		/// <summary>
		/// 掉落数量
		/// </summary>
		public string Id;
		/// <summary>
		/// 编辑器中使用的id对索引的反向映射
		/// </summary>
		public int IdIndex;

		public RateData(float rate = 100, string id = "", int idIndex = 0) {
			Rate = rate;
			Id = id;
			IdIndex = idIndex;
		}

		/// <summary>
		/// 判断是否掉落
		/// </summary>
		/// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
		public bool IsTrigger() {
			return UnityEngine.Random.Range(0f, 200f) <= Rate;
		}
	}
}
