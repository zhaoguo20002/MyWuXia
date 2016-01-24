using UnityEngine;
using System.Collections;

namespace Game {
	public class SoundData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 声音描述名字
		/// </summary>
		public string Name;
		/// <summary>
		/// 声音路径
		/// </summary>
		public string Src;
		/// <summary>
		/// 音频速率[0.1-3]
		/// </summary>
		public float Pitch;
		/// <summary>
		/// 音量[0-1]
		/// </summary>
		public float Volume;

		public SoundData() {
			Pitch = 1;
			Volume = 1;
		}
	}
}
