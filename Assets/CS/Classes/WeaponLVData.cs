namespace Game {
    public class WeaponLVData {
        /// <summary>
        /// 当前兵器等级
        /// </summary>
        public int LV;
        /// <summary>
        /// 等级上限
        /// </summary>
        public int MaxLV;

        public WeaponLVData(int lv = 0, int maxlv = 10) {
            LV = lv;
            MaxLV = maxlv;
        }
    }
}
