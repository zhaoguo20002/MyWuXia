using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class TeamInBattleItemContainer : ComponentCore {
        public Text BookNameText;
        public Image BookIconImage;
        public Image CDProgress;
        public Image Block;
        public Image Disable;

        RoleData roleData;
        BookData bookData;
        SkillData skillData;
        float date;
        protected override void Init() {
            EventTriggerListener.Get(Block.gameObject).onClick = onClick;
        }

        void onClick(GameObject e) {
            if (BattleLogic.Instance.CurrentTeamRole.CanUseSkill && skillData.IsCDTimeout(BattleLogic.Instance.Frame)) {
                BattleLogic.Instance.PushSkill(roleData);
            }
        }

        void Update() {
            CDProgress.fillAmount = skillData.GetCDProgress(BattleLogic.Instance.Frame);
            if (Time.fixedTime - date < 0.5f) {
                return;
            }
            date = Time.fixedTime;
            Disable.gameObject.SetActive(!BattleLogic.Instance.CurrentTeamRole.CanUseSkill);
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
