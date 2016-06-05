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
		/// 场景内Npc的Id列表
		/// </summary>
		public List<string> ResourceNpcDataIds;
		/// <summary>
		/// 场景内的Npc列表
		/// </summary>
		public List<NpcData> Npcs;
		/// <summary>
		/// 所属场景(一个场景只能属于一个区域大地图)
		/// </summary>
		public string BelongToAreaName;

		/// <summary>
		/// 城镇背景音乐
		/// </summary>
		public string BgmSoundId;

		/// <summary>
		/// 佛洛依德算法需要用到的索引值
		/// </summary>
		public int FloydIndex;

		/// <summary>
		/// 标记是否隐藏传送功能
		/// </summary>
		public bool IsInnDisplay;

		/// <summary>
		/// 标记是否隐藏衙门功能
		/// </summary>
		public bool IsYamenDisplay;

		/// <summary>
		/// 标记是否隐藏秘境功能
		/// </summary>
		public bool IsForbiddenAreaDisplay;

		/// <summary>
		/// 标记是否隐藏结识功能
		/// </summary>
		public bool IsWinshopDisplay;

		public SceneData() {
			ResourceStoreId = "";
			ResourceNpcDataIds = new List<string>();
			Npcs = new List<NpcData>();
			BgmSoundId = "";
			BelongToAreaName = "";
			IsInnDisplay = false;
			IsYamenDisplay = true;
			IsForbiddenAreaDisplay = false;
			IsWinshopDisplay = true;
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Npcs.Clear();
			for (int i= 0; i< ResourceNpcDataIds.Count; i++) {
				Npcs.Add(JsonManager.GetInstance().GetMapping<NpcData>("Npcs", ResourceNpcDataIds[i]));
			}
		}
	}
}
