using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
    public class EnemysInfoPanelCtrl : WindowCore<EnemysInfoPanelCtrl, JArray> {
		Image bg;
		Button block;
		Button closeBtn;

        List<EnemyInfoContainer> containers;
        List<string> enemyIds;
        List<RoleData> enemysData;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
            containers = new List<EnemyInfoContainer>() {
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer0"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer1"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer2"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer3"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer4"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer5"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer6"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer7"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer8"),
                GetChildComponent<EnemyInfoContainer>(gameObject, "EnemyInfoContainer9")
            };
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
            switch (e.name)
            {
                case "Block":
                case "CloseBtn":
                    Back();
                    break;
                default:
                    break;
            }
		}

        public void UpdateData() {
            enemyIds = JsonManager.GetInstance().GetMapping<List<string>>("AreaEnemyIds", UserModel.CurrentUserData.CurrentAreaSceneName);
            enemysData = new List<RoleData>();
            RoleData enemyData;
            for (int i = 0, len = enemyIds.Count; i < len; i++)
            {
                enemyData = JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", enemyIds[i]);
                enemysData.Add(enemyData);
            }
            enemysData.Sort((a, b) => b.IsBoss.CompareTo(a.IsBoss));
        }

        public override void RefreshView()
        {
            for (int i = 0, len = containers.Count; i < len; i++)
            {
                if (enemyIds.Count > i)
                {
                    containers[i].gameObject.SetActive(true);
                    containers[i].UpdateData(enemysData[i]);
                    containers[i].RefreshView();
                }
                else
                {
                    containers[i].gameObject.SetActive(false);
                }
            }
        }

		public void Pop() {
			bg.transform.DOScale(0, 0);
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

		public static void Show() {
			if (Ctrl == null) {
                InstantiateView("Prefabs/UI/MainTool/EnemysInfoPanelView", "EnemysInfoPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData();
            Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
