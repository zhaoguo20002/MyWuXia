using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BaseWindowCtrl : WindowCore<BaseWindowCtrl, JArray> {
		protected Text Title;
		protected Button CloseButton;
		protected Image Block;
		RectTransform blockRectTrans;
		protected Image Parent;

		IWindowInterface childInterFace;
		string msg;

		protected override void Init () {
			Title = GetChildText("Title");
			CloseButton = GetChildButton("CloseButton");
			Block = GetComponent<Image>();
			blockRectTrans = GetComponent<RectTransform>();
			Parent = GetChildImage("Parent");
			EventTriggerListener.Get(CloseButton.gameObject).onClick += onClick;
		}

		void onClick(GameObject e) {
			Debug.LogWarning(e.name + "," + Id);
			MoveOut();

		}

		public override void SetChildPath(string path) {
			if (path != "" && childInterFace == null) {
				childInterFace = CreateUIPrefab(Parent.transform, path).GetComponent<IWindowInterface>();
			}
		}

		public override void UpdateData(object obj) {
			JArray data = (JArray)obj;
			msg = data[0].ToString();
			if (childInterFace != null) {
				childInterFace.UpdateData(obj);
				childInterFace.RefreshView();
			}
		}

		public override void RefreshView() {
			Title.text = msg;
		}

		/// <summary>
		/// Displaies the block.
		/// </summary>
		/// <param name="block">If set to <c>true</c> block.</param>
		public void DisplayBlock(bool block) {
			blockRectTrans.sizeDelta = new Vector3(Screen.width, Screen.height);
			Block.enabled = block;
		}

		/// <summary>
		/// Show the specified data, block, childPrefabPath and id.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="block">If set to <c>true</c> block.</param>
		/// <param name="childPrefabPath">Child prefab path.</param>
		/// <param name="id">Identifier.</param>
		public static void Show(JArray data, bool block = true, string childPrefabPath = "", string id = "") {
			InstantiateView("Prefabs/UI/Windows/BaseWindow", id, Screen.width);
			var ctrl = id == "" ? Ctrl : Ctrls[id];
			ctrl.SetChildPath(childPrefabPath);
			ctrl.UpdateData(data);
			ctrl.RefreshView();
			ctrl.DisplayBlock(block);
			ctrl.MoveIn();
		}

		/// <summary>
		/// Hide the specified id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void Hide(string id = "") {
			var ctrl = id == "" ? Ctrl : Ctrls[id];
			ctrl.MoveOut();
		}
	}
	

}