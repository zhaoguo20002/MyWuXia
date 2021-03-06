﻿using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class RoleDetailPanelCtrl : WindowCore<RoleDetailPanelCtrl, JArray> {
		Image bg;
		Button block;
		Image roleIconImage;
		Text descText;
		Text infoText;
		Image weaponIconImage;
        Image weaponFlashImage;
		Image bookIconImage0;
		Image bookIconImage1;
        Image bookIconImage2;
        Image bookFlashImage0;
        Image bookFlashImage1;
        Image bookFlashImage2;
		Button viewWeaponBtn;
		Button viewBookBtn0;
		Button viewBookBtn1;
		Button viewBookBtn2;
        Button brokenDefBtn;

		RoleData roleData;
        List<SecretData> secretsData;
        RoleData currentHostData;
		string desc;
		string info;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			roleIconImage = GetChildImage("RoleIconImage");
			descText = GetChildText("DescText");
			infoText = GetChildText("InfoText");
			weaponIconImage = GetChildImage("WeaponIconImage");
            weaponFlashImage = GetChildImage("weaponFlashImage");
			bookIconImage0 = GetChildImage("BookIconImage0");
			bookIconImage1 = GetChildImage("BookIconImage1");
			bookIconImage2 = GetChildImage("BookIconImage2");
            bookFlashImage0 = GetChildImage("bookFlashImage0");
            bookFlashImage1 = GetChildImage("bookFlashImage1");
            bookFlashImage2 = GetChildImage("bookFlashImage2");
			viewWeaponBtn = GetChildButton("ViewWeaponBtn");
			EventTriggerListener.Get(viewWeaponBtn.gameObject).onClick = onClick;
			viewBookBtn0 = GetChildButton("ViewBookBtn0");
			EventTriggerListener.Get(viewBookBtn0.gameObject).onClick = onClick;
			viewBookBtn1 = GetChildButton("ViewBookBtn1");
			EventTriggerListener.Get(viewBookBtn1.gameObject).onClick = onClick;
			viewBookBtn2 = GetChildButton("ViewBookBtn2");
			EventTriggerListener.Get(viewBookBtn2.gameObject).onClick = onClick;
            brokenDefBtn = GetChildButton("brokenDefBtn");
            EventTriggerListener.Get(brokenDefBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "ViewWeaponBtn":
                    if (roleData.Weapon != null)
                    {
                        Messenger.Broadcast<WeaponData>(NotifyTypes.ShowWeaponDetailPanel, roleData.Weapon);
                    }
                    break;
                case "ViewBookBtn0":
                    if (roleData.Books.Count > 0)
                    {
                        Messenger.Broadcast<BookData>(NotifyTypes.ShowBookDetailPanel, roleData.Books[0]);
                    }
                    break;
                case "ViewBookBtn1":
                    if (roleData.Books.Count > 1)
                    {
                        Messenger.Broadcast<BookData>(NotifyTypes.ShowBookDetailPanel, roleData.Books[1]);
                    }
                    break;
                case "ViewBookBtn2":
                    if (roleData.Books.Count > 2)
                    {
                        Messenger.Broadcast<BookData>(NotifyTypes.ShowBookDetailPanel, roleData.Books[2]);
                    }
                    break;
                case "brokenDefBtn":
                    AlertCtrl.Show("当受击者的防御-攻击者的攻击>=10000时则处于免疫伤害状态，降低受击者防御或者提高攻击者攻击可破解高防状态");
                    break;
                default:
                    Back();
                    break;
            }
        }

        public void UpdateData (RoleData role, List<SecretData> secrets) {
            roleData = role;
            secretsData = secrets;
            //处理通天塔量子强度影响的敌人成长率
            if (UserModel.CurrentUserData.CurrentAreaSceneName == "Area31") {
                int difficulty = PlayerPrefs.GetInt("TowerDifficulty");
                float growUp;
                switch (difficulty) {
                    case 0:
                    default:
                        growUp = 1;
                        break;
                    case 1:
                        growUp = 2;
                        break;
                    case 2:
                        growUp = 4;
                        break;
                }
                roleData.SetGrowUp(growUp);
            }
            roleData.MakeJsonToModel();
            roleData.Init();
            if (secretsData != null)
            {
                roleData.PlusSecretsToRole(secretsData);
                roleData.ClearPluses();
            }
            currentHostData = DbManager.Instance.GetHostRoleData();
            desc = string.Format("称谓:{0}\n门派:{1}\n地位:{2}", roleData.Name, Statics.GetOccupationName(roleData.Occupation), roleData.IsHost ? ("当家" + string.Format("(<color=\"{0}\">{1}</color>)", Statics.GetGenderColor(roleData.Gender), Statics.GetGenderDesc(roleData.Gender)) ) : roleData.IsKnight ? "门客" : "敌人");
            info = string.Format("状态:{0}\n气血:{1}/{2}\n外功:{3}\n外防:{4}{9}\n内功:{5}\n内防:{6}{10}\n轻功:{7}{11}\n{8}", Statics.GetInjuryName(roleData.Injury), roleData.HP, roleData.MaxHP, (int)roleData.PhysicsAttack, (int)roleData.PhysicsDefense, (int)roleData.MagicAttack, (int)roleData.MagicDefense, (int)roleData.Dodge, roleData.Desc == "" ? "" : "人物介绍:\n" + roleData.Desc, roleData.PhysicsDefense - currentHostData.PhysicsAttack >= 10000 ? "<color=\"#FF0000\">(高外防需破)</color>" : "", roleData.MagicDefense - currentHostData.MagicAttack >= 10000 ? "<color=\"#FF0000\">(高内防需破)</color>" : "", roleData.IsImmuneMaxHPReduce ? "\n<color=\"#FF0000\">免疫气血上限衰减</color>" : "");
		}

		public override void RefreshView () {
			if (roleData.Weapon != null) {
				weaponIconImage.gameObject.SetActive(true);
				weaponIconImage.sprite = Statics.GetIconSprite(roleData.Weapon.IconId);
                weaponFlashImage.gameObject.SetActive(((int)roleData.Weapon.Quality) >= ((int)QualityType.FlashGold));
			}
			else {
				weaponIconImage.gameObject.SetActive(false);
                weaponFlashImage.gameObject.SetActive(false);
			}
			if (roleData.Books.Count > 0) {
				bookIconImage0.gameObject.SetActive(true);
				bookIconImage0.sprite = Statics.GetIconSprite(roleData.Books[0].IconId);
                bookFlashImage0.gameObject.SetActive(((int)roleData.Books[0].Quality) >= ((int)QualityType.FlashGold));
			}
			else {
				bookIconImage0.gameObject.SetActive(false);
                bookFlashImage0.gameObject.SetActive(false);
			}
			if (roleData.Books.Count > 1) {
				bookIconImage1.gameObject.SetActive(true);
				bookIconImage1.sprite = Statics.GetIconSprite(roleData.Books[1].IconId);
                bookFlashImage1.gameObject.SetActive(((int)roleData.Books[1].Quality) >= ((int)QualityType.FlashGold));
			}
			else {
				bookIconImage1.gameObject.SetActive(false);
                bookFlashImage1.gameObject.SetActive(false);
			}
			if (roleData.Books.Count > 2) {
				bookIconImage2.gameObject.SetActive(true);
				bookIconImage2.sprite = Statics.GetIconSprite(roleData.Books[2].IconId);
                bookFlashImage2.gameObject.SetActive(((int)roleData.Books[2].Quality) >= ((int)QualityType.FlashGold));
			}
			else {
				bookIconImage2.gameObject.SetActive(false);
                bookFlashImage2.gameObject.SetActive(false);
			}
			roleIconImage.sprite = Statics.GetIconSprite(roleData.IconId);
			descText.text = desc;
			infoText.text = info;
            brokenDefBtn.gameObject.SetActive(roleData.PhysicsDefense - currentHostData.PhysicsAttack >= 10000 || roleData.MagicDefense - currentHostData.MagicAttack >= 10000);
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

        public static void Show(RoleData role, List<SecretData> secrets = null) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/RoleDetailPanelView", "RoleDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData(role, secrets);
			Ctrl.RefreshView();
		}
	}
}
