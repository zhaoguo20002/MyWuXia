using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class EnemyInfoContainer : ComponentCore {
        public Button Btn;
        public Text NameText;

        RoleData enemyData;
        protected override void Init()
        {
            EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
        }

        void onClick(GameObject e) {
            if (enemyData != null)
            {
                RoleDetailPanelCtrl.Show(enemyData);
            }
        }

        public void UpdateData(RoleData data) {
            enemyData = data;
            enemyData.MakeJsonToModel();
        }

        public override void RefreshView()
        {
            NameText.text = enemyData.Name;
            NameText.color = enemyData.IsBoss ? Color.yellow : Color.white;
        }

        public void MakeNone() {
            enemyData = null;
            NameText.text = "？？？";
            NameText.color = Color.white;
        }
    }
}
