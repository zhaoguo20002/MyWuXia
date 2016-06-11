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
		public Image NewFlag;
		public Text Name;
		public Text Num;
		public Text Cost;
		public Text Output;
		public Text WorkerNum;
		public Button LeftBtn;
		public Button RightBtn;

		ResourceData resourceData;

		float date;
		float timeout = 0.1f; //选择数量需要有间隔时间

		float leftDownDate = -1;
		float rightDownDate = -1;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(LeftBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(LeftBtn.gameObject).onDown = onPointerDown;
			EventTriggerListener.Get(LeftBtn.gameObject).onUp = onPointerUp;
			EventTriggerListener.Get(RightBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(RightBtn.gameObject).onDown = onPointerDown;
			EventTriggerListener.Get(RightBtn.gameObject).onUp = onPointerUp;
			date = Time.fixedTime;
		}

		void LateUpdate() {
			float newDate = Time.fixedTime;
			if (leftDownDate >= 0) {
				if (newDate - leftDownDate >= timeout) {
					leftDownDate = newDate;
					Messenger.Broadcast<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, Type, -1);
				}
			}
			else if (rightDownDate >= 0) {
				if (newDate - rightDownDate >= timeout) {
					rightDownDate = newDate;
					Messenger.Broadcast<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, Type, 1);
				}
			}
		}

		void onClick(GameObject e) {
			float newDate = Time.fixedTime;
			if (newDate - date >= timeout) {
				date = newDate;
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
				viewedNewFlag();
			}
		}

		void viewedNewFlag() {
			if (NewFlag.gameObject.activeSelf) {
				PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "ResourceTypeStrOfWorkShopNewFlagIsHide_" + resourceData.Type.ToString(), "true"); //让新增提示消失
				NewFlag.gameObject.SetActive(false);
			}
		}

		void onPointerDown(GameObject e) {
			switch(e.name) {
			case "LeftBtn":
				leftDownDate = Time.fixedTime + 0.3f;
				break;
			case "RightBtn":
				rightDownDate = Time.fixedTime + 0.3f;
				break;
			default:
				break;
			}
		}

		void onPointerUp(GameObject e) {
			leftDownDate = -1;
			rightDownDate = -1;
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
				string costStr = relationship.Needs.Count > 0 ? "" : "无";
				ResourceData need;
				for (int i = 0; i < relationship.Needs.Count; i++) {
					need = relationship.Needs[i];
					costStr += string.Format("{0}-{1}\n", Statics.GetResourceName(need.Type), need.Num);
				}
				Cost.text = costStr;
			}
			//判断是否为新增资源，控制新增标记显示隐藏
			NewFlag.gameObject.SetActive(string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "ResourceTypeStrOfWorkShopNewFlagIsHide_" + resourceData.Type.ToString())));
		}

		public void UpdateNum(double addNum) {
			resourceData.Num += addNum;
			resourceData.Num = resourceData.Num < 0 ? 0 : resourceData.Num;
			Num.text = resourceData.Num.ToString();
			Num.transform.DOScale(new Vector3(3, 3, 3), 0.5f).SetEase(Ease.InExpo).SetLoops(2, LoopType.Yoyo);
		}

	}
}