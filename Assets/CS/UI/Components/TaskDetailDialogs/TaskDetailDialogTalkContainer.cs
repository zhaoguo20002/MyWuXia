using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace Game {
	public class TaskDetailDialogTalkContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Image Icon;
		public Text Msg;

		CanvasGroup alphaGroup;
		string iconId;
		string msgStr;
		
		public void UpdateData(string id, JArray data, bool willDuring) {
			iconId = data[4].ToString();
			msgStr = data[2].ToString();
            msgStr = msgStr.Replace("<n>", DbManager.Instance.HostData.Name);
            msgStr = msgStr.Replace("<o>", Statics.GetOccupationName(DbManager.Instance.HostData.Occupation));
            msgStr = msgStr.Replace("<s>", Statics.GetGenderDesc(DbManager.Instance.HostData.Gender));
            msgStr = msgStr.Replace("<ss>", DbManager.Instance.HostData.Gender == GenderType.Male ? "哥哥" : "姐姐");
            msgStr = msgStr.Replace("<sss>", DbManager.Instance.HostData.Gender == GenderType.Male ? "公子" : "小姐");
			if (willDuring) {
				alphaGroup = gameObject.AddComponent<CanvasGroup>();
				alphaGroup.alpha = 0;
				alphaGroup.DOFade(1, 0.5f).OnComplete(() => {
					if (alphaGroup != null) {
						Destroy(alphaGroup);
					}
				});
			}
		}

		public void RefreshView() {
			//如果icon为｛0｝则表示为主角说话，这时候需要动态显示当家角色icon
			Icon.sprite = Statics.GetIconSprite(iconId == "{0}" ? DbManager.Instance.HostData.IconId : iconId);
			Msg.text = msgStr;
		}
	}
}
