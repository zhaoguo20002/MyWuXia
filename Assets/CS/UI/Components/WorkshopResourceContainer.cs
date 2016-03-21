using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class WorkshopResourceContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Text Num;
		public Text Cost;
		public Text Output;
		public Text WorkerNum;
		public Button LeftBtn;
		public Button RightBtn;

		ResourceData resourceData;

		// Use this for initialization
		void Start () {

		}

		void onClick(GameObject e) {

		}

		public void UpdateData(ResourceData resource) {
			resourceData = resource;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetResourceSprite(resourceData.Type);
			Name.text = Statics.GetResourceName(resourceData.Type);
			Num.text = resourceData.Num.ToString();
			WorkerNum.text = resourceData.WorkersNum.ToString();
			ResourceRelationshipData relationship = WorkshopModel.Relationships.Find(item => item.Type == resourceData.Type);
			if (relationship != null) {
				Output.text = string.Format("{0}+{1}", Statics.GetResourceName(relationship.Type), relationship.YieldNum);
				string costStr = "";
				ResourceData need;
				for (int i = 0; i < relationship.Needs.Count; i++) {
					need = relationship.Needs[i];
					costStr += string.Format("{0}-{1}", Statics.GetResourceName(need.Type), need.Num);
				}
				Cost.text = costStr;
			}
		}

	}
}