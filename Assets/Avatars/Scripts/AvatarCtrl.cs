using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
    public class AvatarCtrl : MonoBehaviour {
        public Animator Animator;
        public SpriteRenderer Body;
        public SpriteRenderer Head;
        public SpriteRenderer RightHand;
        public SpriteRenderer LeftHand;

        static Dictionary<string, Sprite> spritesMapping = new Dictionary<string, Sprite>();
        /// <summary>
        /// 获取纸娃娃部件
        /// </summary>
        /// <returns>The avatar sprite.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="path">Path.</param>
        public static Sprite GetAvatarSprite(string id, string path = "Head") {
            string key = path + id;
            if (!spritesMapping.ContainsKey(key)) {
                Sprite sprite = Resources.Load<GameObject>("Prefabs/Avatars/" + path + "/" + id).GetComponent<SpriteRenderer>().sprite;
                spritesMapping.Add(key, sprite);
            }
            return spritesMapping[key];
        }

        /// <summary>
        /// 设置初始化数据
        /// </summary>
        /// <param name="headId">Head identifier.</param>
        /// <param name="clothId">Cloth identifier.</param>
        /// <param name="weaponId">Weapon identifier.</param>
        public void SetData(string headId = "10001", string clothId = "20001", string weaponId = "") {
            Head.sprite = GetAvatarSprite(headId, "Head");
            ChangeClose(clothId);
            if (weaponId != "") {
                PickUpWeapon(weaponId);
            }
        }

        /// <summary>
        /// 拿起兵器
        /// </summary>
        /// <param name="weaponId">Weapon identifier.</param>
        public void PickUpWeapon(string weaponId) {
            PickDownWeapon();
            GameObject weapon = Statics.GetPrefabClone("Prefabs/Avatars/Weapon/" + weaponId);
            RightHand.enabled = false;
            weapon.transform.SetParent(RightHand.transform);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localScale = Vector3.one;
            weapon.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 放下兵器
        /// </summary>
        public void PickDownWeapon() {
            RightHand.enabled = true;
            for (int i = RightHand.transform.childCount - 1; i >= 0; i--) {
                Destroy(RightHand.transform.GetChild(i).gameObject);    
            }
        }

        /// <summary>
        /// 换装
        /// </summary>
        /// <param name="clothId">Cloth identifier.</param>
        public void ChangeClose(string clothId) {
            Body.sprite = GetAvatarSprite(clothId, "Body");
            RightHand.sprite = GetAvatarSprite(clothId, "Hand");
            LeftHand.sprite = GetAvatarSprite(clothId, "Hand");
        }
    }
}
