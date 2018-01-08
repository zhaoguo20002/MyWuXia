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
		/// 战斗描述名称
		/// </summary>
		public string Name;
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
		/// 掉落物列表
		/// </summary>
		public List<DropData> Drops;
        /// <summary>
        /// 是否为静态战斗
        /// 如果为静态战斗则作为永久存在的战斗不会被批量删除
        /// </summary>
        public bool IsStatic;

		public FightData() {
			ResourceEnemyIds = new List<string>();
			Enemys = new List<RoleData>();
			Drops = new List<DropData>();
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Enemys.Clear();
			RoleData enemy;
			for (int i = 0; i < ResourceEnemyIds.Count; i++) {
				enemy = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", ResourceEnemyIds[i]);
//				enemy.MakeJsonToModel();
				Enemys.Add(enemy);
			}
			for (int i = 0; i < Drops.Count; i++) {
				Drops[i].MakeJsonToModel();
			}
		}
	}
}
