using UnityEngine;
using System.Collections;

namespace Game {
	/// <summary>
	/// 江湖阅历实体类
	/// </summary>
	public class ExperienceData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 阅历名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 阅历类型
		/// </summary>
		public ExperienceType Type;
		/// <summary>
		/// 所属区域名
		/// </summary>
		public string BelongToAreaName;
		/// <summary>
		/// 字符串类型值
		/// </summary>
		public string StringValue;
		/// <summary>
		/// 整型值
		/// </summary>
		public int IntValue;
		/// <summary>
		/// 浮点数型值
		/// </summary>
		public float FloatValue;
		/// <summary>
		/// 整型值区间最小值
		/// </summary>
		public int MinIntValue;
		/// <summary>
		/// 整型值区间最大值
		/// </summary>
		public int MaxIntValue;

		public ExperienceData() {
			Name = "默认成就名";
		}

		/// <summary>
		/// 判断是否达成条件,将成就Id和判断结果存入数据库中,以便持久化
		/// </summary>
		public bool Check() {
			return false;
		}
	}
}
