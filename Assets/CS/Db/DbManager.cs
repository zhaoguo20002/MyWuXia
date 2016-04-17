using UnityEngine;
using System.Collections;

namespace Game {
	/// <summary>
	/// 数据库管理类
	/// </summary>
	public partial class DbManager {
		static DbManager _instance;
		public static DbManager Instance {
			get {
				if (_instance == null) {
					_instance = new DbManager("MyWuXiaDB.db");
				}
				return _instance;
			}
		}

		string dbConnectionString;
		DbAccess db;
		string currentRoleId;
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		public DbManager(string connectionString) {
			dbConnectionString = connectionString + ";Pooling=True;";
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("CurrentRoleId"))) {
				PlayerPrefs.SetString("CurrentRoleId", "role_0");
			}
			SetCurrentRoleId(PlayerPrefs.GetString("CurrentRoleId"));
		}

		/// <summary>
		/// 打开数据库对象
		/// </summary>
		/// <returns>The db.</returns>
		public DbAccess OpenDb() {
			return new DbAccess(dbConnectionString);
		}


		/// <summary>
		/// 改变当前主角id
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void SetCurrentRoleId(string id) {
			currentRoleId = id;
		}
	}
}
