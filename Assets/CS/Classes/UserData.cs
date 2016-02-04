using System;

namespace Game {
	/// <summary>
	/// 用户相关数据保存在此
	/// </summary>
	public class UserData {
		/// <summary>
		/// 用户主键Id
		/// </summary>
		public string Id;

		/// <summary>
		/// 拥有金钱
		/// </summary>
		public int Money;

		/// <summary>
		/// 大地图上行动需要的体力道具
		/// </summary>
		public ItemData AreaFood;

		/// <summary>
		/// 当前所处位置
		/// </summary>
		public UserPositionStatusType PositionStatu;

		/// <summary>
		/// 当前所处场景Id
		/// </summary>
		public string CurrentCitySceneId;

		/// <summary>
		/// 当前所处大地图场景文件名
		/// </summary>
		public string CurrentAreaSceneName;

		/// <summary>
		/// 当前所处的大地图X坐标
		/// </summary>
		public int CurrentAreaX;

		/// <summary>
		/// 当前所处的大地图Y坐标
		/// </summary>
		public int CurrentAreaY;

		public UserData () {
			
		}
	}
}

