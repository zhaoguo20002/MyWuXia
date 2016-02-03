using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class SceneData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 场景名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 商店Id
		/// </summary>
		public string ResourceStoreId;
		/// <summary>
		/// 商店数据
		/// </summary>
		public StoreData Store;
		/// <summary>
		/// 场景内Npc的Id列表
		/// </summary>
		public List<string> ResourceNpcDataIds;
		/// <summary>
		/// 场景内的Npc列表
		/// </summary>
		public List<NpcData> Npcs;

		public SceneData() {
			ResourceStoreId = "";
			ResourceNpcDataIds = new List<string>();
			Npcs = new List<NpcData>();
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Store = JsonManager.GetInstance().GetMapping<StoreData>("Stores", ResourceStoreId);
			Npcs.Clear();
			for (int i= 0; i< ResourceNpcDataIds.Count; i++) {
				Npcs.Add(JsonManager.GetInstance().GetMapping<NpcData>("Npcs", ResourceNpcDataIds[i]));
			}
		}
	}
}
