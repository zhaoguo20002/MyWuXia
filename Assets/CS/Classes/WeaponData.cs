using UnityEngine;
using System.Collections;

namespace Game {
	public class WeaponData {
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// The name.
		/// </summary>
		public string Name;
		/// <summary>
		/// The desc.
		/// </summary>
		public string Desc;
		/// <summary>
		/// Icon图标id
		/// </summary>
		public string IconId;
		/// <summary>
		/// 品质
		/// </summary>
		public QualityType Quality;
		/// <summary>
		/// 武器宽度
		/// </summary>
		public float Width;
		/// <summary>
		/// 武器威力增量倍率集合
		/// </summary>
		public float[] Rates;

//		public WeaponData(string id, string name, string desc, QualityType quality, float width, float[] rates) {
//			Id = id;
//			Name = name;
//			Desc = desc;
//			Quality = quality;
//			Width = width;
//			Rates = rates;
//		}
	}
}
