using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class BookData {
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
		/// 武功招式集合
		/// </summary>
		public List<SkillData> Skills;
		/// <summary>
		/// Icon Id
		/// </summary>
		public string IconId;

		int currentSkillIndex;
	}
}