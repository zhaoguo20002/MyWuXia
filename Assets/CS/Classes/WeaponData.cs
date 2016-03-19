using UnityEngine;
using System.Collections;

namespace Game {
	public class WeaponData {
		/// <summary>
		/// 数据主键id
		/// </summary>
		public int PrimaryKeyId;
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

		/// <summary>
		/// 正在使用该兵器的角色id
		/// </summary>
		public string BeUsingByRoleId;

		public WeaponData() {
			Desc = "";
			IconId = "";
			Width = 100;
			Rates = new float[] { 1, 0, 0, 0 };
			BeUsingByRoleId = "";
		}
	}
}
