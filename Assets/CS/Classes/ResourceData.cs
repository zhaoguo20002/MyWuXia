using System;

namespace Game {
	/// <summary>
	/// 生产资源实体类(所有需要用到生产资源制作的东西都统一用此类)
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
		public int WorkersNum;
		public ResourceData (ResourceType type, double num, int workersNum = 0) {
			Type = type;
			Num = num;
			WorkersNum = workersNum;
		}
	}
}

