using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
    public class RoleUpgradeLvPanelCtrl : WindowCore<RoleUpgradeLvPanelCtrl, JArray> {
        Image bg;
        Image block;
        Image iconImage;
        Text hpFromText;
        Text hpToText;
        Text physicsAttackFromText;
        Text physicsAttackToText;
        Text physicsDefenseFromText;
        Text physicsDefenseToText;
        Text magicAttackFromText;
        Text magicAttackToText;
        Text magicDefenseFromText;
        Text magicDefenseToText;
        Text dodgeFromText;
        Text dodgeToText;

        RoleData fromRoleData;
        RoleData toRoleData;
        float date;

        protected override void Init()
        {
            bg = GetChildImage("Bg");
            block = GetChildImage("Block");
            EventTriggerListener.Get(block.gameObject).onClick = onClick;
            iconImage = GetChildImage("iconImage");
            hpFromText = GetChildText("hpFromText");
            hpToText = GetChildText("hpToText");
            physicsAttackFromText = GetChildText("physicsAttackFromText");
            physicsAttackToText = GetChildText("physicsAttackToText");
            physicsDefenseFromText = GetChildText("physicsDefenseFromText");
            physicsDefenseToText = GetChildText("physicsDefenseToText");
            magicAttackFromText = GetChildText("magicAttackFromText");
            magicAttackToText = GetChildText("magicAttackToText");
            magicDefenseFromText = GetChildText("magicDefenseFromText");
            magicDefenseToText = GetChildText("magicDefenseToText");
            dodgeFromText = GetChildText("dodgeFromText");
            dodgeToText = GetChildText("dodgeToText");
            date = Time.fixedTime;
        }

        void onClick(GameObject e) {
            if (Time.fixedTime - date > 1)
            {
                Back();
            }
        }

        public void UpdateData(RoleData fromData, RoleData toData) {
            fromRoleData = fromData;
            toRoleData = toData;
        }

        public override void RefreshView()
        {
            iconImage.sprite = Statics.GetIconSprite(fromRoleData.IconId);
            hpFromText.text = fromRoleData.MaxHP.ToString();
            hpToText.text = toRoleData.MaxHP.ToString();
            physicsAttackFromText.text = fromRoleData.PhysicsAttack.ToString();
            physicsAttackToText.text = toRoleData.PhysicsAttack.ToString();
            physicsDefenseFromText.text = fromRoleData.PhysicsDefense.ToString();
            physicsDefenseToText.text = toRoleData.PhysicsDefense.ToString();
            magicAttackFromText.text = fromRoleData.MagicAttack.ToString();
            magicAttackToText.text = toRoleData.MagicAttack.ToString();
            magicDefenseFromText.text = fromRoleData.MagicDefense.ToString();
            magicDefenseToText.text = toRoleData.MagicDefense.ToString();
            dodgeFromText.text = fromRoleData.Dodge.ToString();
            dodgeToText.text = toRoleData.Dodge.ToString();
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

        public static void Show(RoleData fromData, RoleData toData) {
            if (Ctrl == null) {
                InstantiateView("Prefabs/UI/Role/RoleUpgradeLvPanelView", "RoleUpgradeLvPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
                Ctrl.Pop();
            }
            Ctrl.UpdateData(fromData, toData);
            Ctrl.RefreshView();
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Back();
            }
        }
    }
}
