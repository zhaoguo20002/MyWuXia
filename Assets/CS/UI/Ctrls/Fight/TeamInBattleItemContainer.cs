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
        bool _isLostKnowledge;
        float date;
        protected override void Init() {
            EventTriggerListener.Get(Block.gameObject).onClick = onClick;
        }

        void onClick(GameObject e) {
            if (skillData == null)
            {
                return;
            }
            if (BattleLogic.Instance.CurrentTeamRole.CanUseSkill && skillData.IsCDTimeout(BattleLogic.Instance.Frame)) {
                BattleLogic.Instance.PushSkill(roleData, _isLostKnowledge);
            }
        }

        void Update() {
            if (skillData == null)
            {
                return;
            }
            CDProgress.fillAmount = skillData.GetCDProgress(BattleLogic.Instance.Frame);
            if (Time.fixedTime - date < 0.5f) {
                return;
            }
            date = Time.fixedTime;
            Disable.gameObject.SetActive(!BattleLogic.Instance.CurrentTeamRole.CanUseSkill);
        }

        public void UpdateData(RoleData role, bool isLostKnowledge = false) {
            roleData = role;
            _isLostKnowledge = isLostKnowledge;
            bookData = !_isLostKnowledge ? roleData.GetCurrentBook() : roleData.GetLostKnowledge();
            skillData = bookData != null ? bookData.GetCurrentSkill() : null;
        }

        public override void RefreshView() {
            if (bookData != null)
            {
                BookNameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
                BookIconImage.transform.parent.gameObject.SetActive(true);
                BookIconImage.sprite = Statics.GetIconSprite(bookData.IconId);
            }
            else
            {
                BookNameText.text = "无";
                BookIconImage.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
