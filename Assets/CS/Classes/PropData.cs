using UnityEngine;
using System.Collections;

namespace Game {
    /// <summary>
    /// 游戏道具实体类
    /// </summary>
    public class PropData {
        /// <summary>
        /// 道具类型
        /// </summary>
        public PropType Type;
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int Num;
        /// <summary>
        /// 最大数量
        /// </summary>
        public int Max;
        /// <summary>
        /// 整型参数
        /// </summary>
        public int IntValue;
        /// <summary>
        /// 单精度浮点数参数
        /// </summary>
        public float FloatValue;
        /// <summary>
        /// 字符串参数
        /// </summary>
        public string StringValue;

        public PropData(PropType type, int num, int max = 10, int intValue = 0, float floatValue = 0, string stringValue = "") {
            Type = type;
            Max = max;
            Num = Mathf.Clamp(num, 0, Max);
            IntValue = intValue;
            FloatValue = floatValue;
            StringValue = stringValue;
        }
    }
}
