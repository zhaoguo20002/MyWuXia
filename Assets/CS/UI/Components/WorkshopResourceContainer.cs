using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;

namespace Game {
	public class WorkshopResourceContainer : MonoBehaviour {
		public ResourceType Type;
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
			EventTriggerListener.Get(LeftBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(RightBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "LeftBtn":
				Messenger.Broadcast<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, Type, -1);
				break;
			case "RightBtn":
				Messenger.Broadcast<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, Type, 1);
				break;
				default:
				break;
			}
		}

		public void UpdateData(ResourceData resource) {
			resourceData = resource;
			Type = resourceData.Type;
		}

		public void UpdateData(int workerNum) {
			resourceData.WorkersNum = workerNum;
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
					costStr += string.Format("{0}-{1}\n", Statics.GetResourceName(need.Type), need.Num);
				}
				Cost.text = costStr;
			}
		}

		public void UpdateNum(double addNum) {
			resourceData.Num += addNum;
			resourceData.Num = resourceData.Num < 0 ? 0 : resourceData.Num;
			Num.text = resourceData.Num.ToString();
			Num.transform.DOScale(new Vector3(3, 3, 3), 0.5f).SetEase(Ease.InExpo).SetLoops(2, LoopType.Yoyo);
		}

	}
}