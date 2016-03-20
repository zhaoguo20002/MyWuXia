using System;

namespace Game {
	/// <summary>
	/// 生产资源实体类
	/// </summary>
	public class ResourceData {
		/// <summary>
		/// 资源类型
		/// </summary>
		public ResourceType Type;
		/// <summary>
		/// 资源数量
		/// </summary>
		public double Num;
		/// <summary>
		/// 正在采集资源的家丁数
		/// </summary>
		public int WorksNum;
		public ResourceData (ResourceType type, double num, int worksNum = 0) {
			Type = type;
			Num = num;
			WorksNum = worksNum;
		}
	}
}

