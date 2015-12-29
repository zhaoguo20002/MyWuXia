using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class FightData {
		/// <summary>
		/// 战斗Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 战斗类型
		/// </summary>
		public FightType Type;
		/// <summary>
		/// 敌人索引Id
		/// </summary>
		public List<string> ResourceEnemyIds;
		/// <summary>
		/// 敌人列表
		/// </summary>
		public List<RoleData> Enemys;
		/// <summary>
		/// 凋落物索引Id
		/// </summary>
		public List<string> ResourceDropIds;

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {

		}
	}
}
