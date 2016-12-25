using UnityEngine;
using System.Collections;

namespace Game {
    public class RoleCtrl : MonoBehaviour {
        public AvatarCtrl Avatar;
        public string HeadId = "";
        public string ClothId = ""; 
        public string WeaponId = "";
        // Use this for initialization
        void Start () {
            if (HeadId != "" && ClothId != "") {
                Avatar.SetData(HeadId, ClothId, WeaponId);
            }
        }
    }
}
