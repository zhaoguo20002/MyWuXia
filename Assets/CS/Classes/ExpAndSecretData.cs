using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
    public class ExpAndSecretData {
        /// <summary>
        /// 秘籍修为
        /// </summary>
        public ExpData Exp;   
        /// <summary>
        /// 秘籍已领悟的诀要集合
        /// </summary>
        public List<SecretData> Secrets;

        public ExpAndSecretData() {
            Exp = new ExpData(0, 0);
            Secrets = new List<SecretData>();
        }
    }
}
