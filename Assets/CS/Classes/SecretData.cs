using UnityEngine;
using System.Collections;

namespace Game {
    public class SecretData {
        /// <summary>
        /// 数据主键id
        /// </summary>
        public int PrimaryKeyId;
        /// <summary>
        /// The name.
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型
        /// </summary>
        public SecretType Type;
        /// <summary>
        /// Icon Id
        /// </summary>
        public string IconId;
        /// <summary>
        /// 品质
        /// </summary>
        public QualityType Quality;
        /// <summary>
        /// 归属秘籍id
        /// </summary>
        public string BelongToBookId;
        /// <summary>
        /// 整型参数
        /// </summary>
        public int IntValue;
        /// <summary>
        /// 浮点型参数
        /// </summary>
        public float FloatValue;

        public SecretData() {
            
        }
    }
}
