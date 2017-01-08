using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class SkillData {
		/// <summary>
		/// The identifier.
		/// </summary>
		public string Id;
		/// <summary>
		/// The name.
		/// </summary>
		public string Name;
		/// <summary>
		/// 招式类型
		/// </summary>
		public SkillType Type;
		/// <summary>
		/// 图标Id
		/// </summary>
		public string IconId;
		/// <summary>
		/// The buff datas.
		/// </summary>
		public List<BuffData> BuffDatas;
		/// <summary>
		/// The de buff datas.
		/// </summary>
		public List<BuffData> DeBuffDatas;
		/// <summary>
		/// 发招概率[0-100]
		/// </summary>
		public float Rate;
		/// <summary>
		/// 额外招式索引Id集合
		/// </summary>
		public List<string> ResourceAddedSkillIds;
		/// <summary>
		/// 额外招式
		/// </summary>
		public List<SkillData> AddedSkillDatas;
		/// <summary>
		/// 招式描述
		/// </summary>
		public string Desc;
		/// <summary>
		/// 粒子特效路径
		/// </summary>
		public string EffectSrc;
		/// <summary>
		/// 技能音效Id
		/// </summary>
        public string EffectSoundId;
        /// <summary>
        /// 技能冷却时间 (单位:秒)
        /// </summary>
        public float CDTime;

        //初始帧
        long initFrame;
        //当前技能冷却累加帧数
        long cDAddFrame;
        //当前技能冷却结束帧
        long cDEndFrame;

		public SkillData() {
			IconId = "";
			BuffDatas = new List<BuffData>();
			DeBuffDatas = new List<BuffData>();
			ResourceAddedSkillIds = new List<string>();
			AddedSkillDatas = new List<SkillData>();
			Rate = 100;
			Desc = "";
			EffectSrc = "";
			EffectSoundId = "";
            CDTime = 1;
            initFrame = 0;
            Reset();
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			AddedSkillDatas.Clear();
			for (int i = 0; i < ResourceAddedSkillIds.Count; i++) {
				AddedSkillDatas.Add(JsonManager.GetInstance().GetMapping<SkillData>("Skills", ResourceAddedSkillIds[i]));
			}
            cDAddFrame = (long)Statics.ClearError((double)CDTime / (double)Global.FrameCost);
		}

		/// <summary>
		/// 获取最终输出的招式
		/// </summary>
		/// <returns>The real skill.</returns>
		public SkillData GetRealSkill() {
			for (int i = AddedSkillDatas.Count - 1; i >= 0; i--) {
				if (AddedSkillDatas[i].IsTrigger()) {
					return AddedSkillDatas[i];
				}
			}
			return this;
		}

		/// <summary>
		/// 判断是否触发概率
		/// </summary>
		/// <returns><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</returns>
		public bool IsTrigger() {
			return UnityEngine.Random.Range(0f, 100f) <= Rate;
		}

        /// <summary>
        /// 更新技能冷却时间
        /// </summary>
        /// <param name="time">Time.</param>
        public void UpdateCDTime(float time) {
            CDTime = time;
            cDAddFrame = (long)Statics.ClearError((double)CDTime / (double)Global.FrameCost);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() {
            cDEndFrame = initFrame;
        }

        /// <summary>
        /// 开始CD计时
        /// </summary>
        /// <param name="frame">Frame.</param>
        public void StartCD(long frame) {
            cDEndFrame = frame + cDAddFrame;
        }

        /// <summary>
        /// CD时间延长一帧
        /// </summary>
        public void ExtendOneFrameCD() {
            cDEndFrame++;
        }

        /// <summary>
        /// CD时间是否过期
        /// </summary>
        /// <returns><c>true</c> if this instance is CD timeout the specified frame; otherwise, <c>false</c>.</returns>
        /// <param name="frame">Frame.</param>
        public bool IsCDTimeout(long frame) {
            return frame > cDEndFrame;
        }

        /// <summary>
        /// 获取当前技能CD进度
        /// </summary>
        /// <returns>The CD progress.</returns>
        /// <param name="frame">Frame.</param>
        public float GetCDProgress(long frame) {
            return Mathf.Clamp01((float)(cDEndFrame - frame) / (float)cDAddFrame);
        }
	}
}
