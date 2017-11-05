using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class DropItemContainer : MonoBehaviour {
		public Image IconImage;
		public Text NameText;
		public Text NumText;

		DropData dropData;

		public void UpdateData(DropData drop) {
			dropData = drop;
		}

		public void RefreshView() {
			IconImage.sprite = Statics.GetIconSprite(dropData.Item.IconId);
			NameText.text = string.Format("<color=\"#1ABDE6\">{0}</color>", dropData.Item.Name);
			NumText.text = string.Format("<color=\"#00FF00\">+ {0}</color>", dropData.Item.Num);
		}
	}
}
