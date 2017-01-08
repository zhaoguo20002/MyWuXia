using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class TeamInBattleItemContainer : ComponentCore {
        public Text BookNameText;
        public Image BookIconImage;
        public Image CDProgress;
        public Image Block;

        RoleData roleData;
        BookData bookData;
        SkillData skillData;
        protected override void Init() {
            EventTriggerListener.Get(Block.gameObject).onClick = onClick;
        }

        void onClick(GameObject e) {
            if (skillData.IsCDTimeout(BattleLogic.Instance.Frame)) {
                BattleLogic.Instance.PushSkill(roleData);
            }
        }

        void Update() {
            CDProgress.fillAmount = skillData.GetCDProgress(BattleLogic.Instance.Frame);
        }

        public void UpdateData(RoleData role) {
            roleData = role;
            bookData = roleData.GetCurrentBook();
            skillData = bookData.GetCurrentSkill();
        }

        public override void RefreshView() {
            BookNameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
            BookIconImage.sprite = Statics.GetIconSprite(bookData.IconId);
        }
    }
}
