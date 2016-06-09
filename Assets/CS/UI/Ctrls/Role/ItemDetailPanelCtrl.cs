using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;
using UnityEngine.UI;

namespace Game {
	public class ItemDetailPanelCtrl : WindowCore<ItemDetailPanelCtrl, JArray> {
		Image bg;
		Button block;
		Image icon;
		Text nameText;
		Text typeText;
		Text lvText;
		Text numText;
		Text sellFlagText;
		Image silverImage;
		Text sellPriceText;
		Text discardFlagText;
		Text descText;
		Button destroyBtn;
		Button useBtn;

		ItemData itemData;
		bool _fromBag;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			icon = GetChildImage("Icon");
			nameText = GetChildText("NameText");
			typeText = GetChildText("TypeText");
			lvText = GetChildText("LvText");
			numText = GetChildText("NumText");
			sellFlagText = GetChildText("SellFlagText");
			silverImage = GetChildImage("SilverImage");
			sellPriceText = GetChildText("SellPriceText");
			discardFlagText = GetChildText("DiscardFlagText");
			descText = GetChildText("DescText");
			destroyBtn = GetChildButton("DestroyBtn");
			EventTriggerListener.Get(destroyBtn.gameObject).onClick = onClick;
			useBtn = GetChildButton("UseBtn");
			EventTriggerListener.Get(useBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "DestroyBtn":
				ConfirmCtrl.Show(string.Format("<color=\"#1ABDE6\">{0}</color>将<color=\"#FF0000\">永久消失</color>, 确定丢弃?", itemData.Name), () => {
					Messenger.Broadcast<int>(NotifyTypes.DiscardItem, itemData.PrimaryKeyId);
					Back();
				});
				break;
			case "UseBtn":
				Messenger.Broadcast<int>(NotifyTypes.UseItem, itemData.PrimaryKeyId);
				Back();
				break;
			default:
				Back();
				break;
			}
		}

		public void UpdateData (ItemData item, bool fromBag = false) {
			itemData = item;
			_fromBag = fromBag;
		}

		public override void RefreshView () {
			icon.sprite = Statics.GetIconSprite(itemData.IconId);
			nameText.text = string.Format("<color=\"#1ABDE6\">{0}</color>", itemData.Name);
			typeText.text = string.Format("类型:{0}", Statics.GetItemTypeName(itemData.Type));
			lvText.text = string.Format("等级:{0}级", itemData.Lv);
			numText.text = string.Format("数量:{0}/{1}", itemData.Num, itemData.MaxNum);
			if (itemData.SellPrice >= 0) {
				sellFlagText.text = "回收:";
				silverImage.gameObject.SetActive(true);
				sellPriceText.text = itemData.SellPrice.ToString();
			}
			else {
				sellFlagText.text = "<color=\"#FF0000\">不可回收</color>";
				silverImage.gameObject.SetActive(false);
			}
			discardFlagText.text = itemData.CanDiscard ? "<color=\"#00FF00\">可以丢弃</color>" : "<color=\"#FF0000\">不可丢弃</color>";
			bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, 510);
			destroyBtn.gameObject.SetActive(false);
			useBtn.gameObject.SetActive(false);
			if (_fromBag && itemData.CanDiscard) {
				bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, 550);
				destroyBtn.gameObject.SetActive(true);
			}
			switch (itemData.Type) {
			case ItemType.Food: //干粮可以直接吃补充区域大地图体力
				bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, 550);
				useBtn.gameObject.SetActive(true);
				useBtn.GetComponentInChildren<Text>().text = "吃";
				break;
			case ItemType.Weapon:
				bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, 550);
				useBtn.gameObject.SetActive(true);
				useBtn.GetComponentInChildren<Text>().text = "打开";
				break;
			default:
				break;
			}
			descText.text = string.Format("描述\n{0}", itemData.Desc);

		}

		public void Pop() {
			bg.transform.DOScale(0, 0);
			bg.transform.DOScale(1, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

		public static void Show(ItemData item, bool fromBag = false) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/ItemDetailPanelView", "ItemDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(item, fromBag);
			Ctrl.RefreshView();
		}
	}
}
