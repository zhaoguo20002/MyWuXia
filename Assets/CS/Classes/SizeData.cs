using UnityEngine;
using System.Collections;

namespace Game {
	public class SizeData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 区域大地图宽度
		/// </summary>
		public int Width;
		/// <summary>
		/// 区域大地图高度
		/// </summary>
		public int Height;

		public SizeData(string id, int width, int height) {
			Id = id;
			Width = width;
			Height = height;
		}
	}
}
