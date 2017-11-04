using UnityEngine;
using System.Collections;

namespace Game {
    public class ExpData {
        /// <summary>
        /// 当前经验
        /// </summary>
        public long Cur;
        /// <summary>
        /// 经验上线
        /// </summary>
        public long Max;
        public ExpData(long cur, long max) {
            Cur = cur;
            Max = max;
        }
    }
}
