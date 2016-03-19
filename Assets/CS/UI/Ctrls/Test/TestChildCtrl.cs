using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using Mono.Data.Sqlite;

namespace Game {
	public class TestChildCtrl : ComponentCore {
		Text MsgText;
		Button CreateDBBtn;
		Button AddBtn;
		Button UpdateBtn;
		Button DeleteBtn;
		string msg;
		protected override void Init () {
			MsgText = GetChildText("MsgText");
			CreateDBBtn = GetChildButton("CreateDBBtn");
			AddBtn = GetChildButton("AddBtn");
			UpdateBtn = GetChildButton("UpdateBtn");
			DeleteBtn = GetChildButton("DeleteBtn");

			EventTriggerListener.Get(CreateDBBtn.gameObject).onClick += onClick;
			EventTriggerListener.Get(AddBtn.gameObject).onClick += onClick;
			EventTriggerListener.Get(UpdateBtn.gameObject).onClick += onClick;
			EventTriggerListener.Get(DeleteBtn.gameObject).onClick += onClick;

			DbAccess db = new DbAccess("LocalData.db");
			db.CreateTable("test",new string[]{"name","qq","email","blog"}, new string[]{"text","text","text","text"});
			db.CloseSqlConnection();
		}

		void onClick(GameObject e) {
			DbAccess db = new DbAccess("LocalData.db");
			switch (e.name) {
			case "CreateDBBtn":
				//创建数据库名称为xuanyusong.db
				//请注意 插入字符串是 已经要加上'宣雨松' 不然会报错
				db.CreateTable("test",new string[]{"name","qq","email","blog"}, new string[]{"text","text","text","text"});
				//我在数据库中连续插入三条数据
				db.InsertInto("test", new string[]{ "'宣雨松'","'289187120'","'xuanyusong@gmail.com'","'www.xuanyusong.com'"   });
				db.InsertInto("test", new string[]{ "'雨松MOMO'","'289187120'","'000@gmail.com'","'www.xuanyusong.com'"   });
				db.InsertInto("test", new string[]{ "'哇咔咔'","'289187120'","'111@gmail.com'","'www.xuanyusong.com'"   });

				db.UpdateInto("test", new string[]{ "name", "qq" }, new string[] { "'赵果Start'", "'631251345'" }, "email", "'111@gmail.com'");
		 
				//然后在删掉两条数据
				db.Delete("test",new string[]{"email","email"}, new string[]{"'xuanyusong@gmail.com'","'000@gmail.com'"}  );
		 
				break;
			case "AddBtn":
				db.InsertInto("test", new string[]{ "'赵果'","'631251345'","'zhaoguo2004@126.com'","'www.nookjoy.com'"  });
				break;
			case "UpdateBtn":
				db.UpdateInto("test", new string[]{ "name", "qq" }, new string[] { "'赵果Change'", "'631251345'" }, "email", "'zhaoguo2004@126.com'");
				break;
			case "DeleteBtn":
				db.DeleteContents("test");
				break;
			default:
				break;
			}

			//注解1
			SqliteDataReader sqReader = db.SelectWhere("test",new string[]{"name","qq","email","blog"},new string[]{"qq"},new string[]{"="},new string[]{"631251345"});
			msg = "";
			while (sqReader.Read()) {
				msg += sqReader.GetString(sqReader.GetOrdinal("name")) + "," + sqReader.GetString(sqReader.GetOrdinal("qq")) + "," + sqReader.GetString(sqReader.GetOrdinal("email")) + "," + sqReader.GetString(sqReader.GetOrdinal("blog")) + "\n";
	    	} 
			db.CloseSqlConnection();
			MsgText.text = msg;
		}

		/// <summary>
		/// Updates the data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="obj">Object.</param>
		public override void UpdateData(object obj) {
			JArray data = (JArray)obj;
			Debug.LogWarning("TestChildCtrl UpdateData data = " + data);
			DbAccess db = new DbAccess("LocalData.db");
			SqliteDataReader sqReader = db.SelectWhere("test",new string[]{"name","qq","email","blog"},new string[]{"qq"},new string[]{"="},new string[]{"631251345"});
			msg = "";
			while (sqReader.Read()) {
				msg += sqReader.GetString(sqReader.GetOrdinal("name")) + "," + sqReader.GetString(sqReader.GetOrdinal("qq")) + "," + sqReader.GetString(sqReader.GetOrdinal("email")) + "," + sqReader.GetString(sqReader.GetOrdinal("blog")) + "\n";
	    	} 
			db.CloseSqlConnection();
		}

		/// <summary>
		/// Refreshs the view.
		/// </summary>
		public override void RefreshView() {
			MsgText.text = msg;
		}
	}
}