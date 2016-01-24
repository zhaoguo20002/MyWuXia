using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class SoundManager {
		SoundSource bgm;
		GameObject parent;
		public SoundManager() {
			bgm = null;
			parent = GameObject.Find("Global");
		}

		/// <summary>
		/// 播放背景音乐
		/// </summary>
		/// <param name="soundId">Sound identifier.</param>
		/// <param name="loop">If set to <c>true</c> loop.</param>
		/// <param name="delay">Delay.</param>
		public void PlayBGM(string soundId, bool loop = true, float delay = 0) {
			StopBGM();
			bgm = Statics.GetSoundPrefabClone(soundId).GetComponent<SoundSource>();
			if (bgm != null) {
				bgm.Play(loop, delay);
			}
		}

		/// <summary>
		/// 停止播放背景音乐
		/// </summary>
		public void StopBGM() {
			if (bgm != null) {
				bgm.Stop();
			}
		}

		/// <summary>
		/// 暂停播放背景音乐
		/// </summary>
		public void PauseBGM() {
			if (bgm != null) {
				bgm.Pause();
			}
		}

		/// <summary>
		/// 取消暂停播放背景音乐
		/// </summary>
		public void UnPauseBGM() {
			if (bgm != null) {
				bgm.UnPause();
			}
		}

		/// <summary>
		/// 设置背景音乐是否静音
		/// </summary>
		public void MuteBGM(bool mute) {
			if (bgm != null) {
				bgm.Mute(mute);
			}
		}

		/// <summary>
		/// 切换背景音乐静音/非静音
		/// </summary>
		public void ToggleBGM() {
			if (bgm != null) {
				bgm.Toggle();
			}
		}

		/// <summary>
		/// 播放音效
		/// </summary>
		/// <param name="soundId">Sound identifier.</param>
		/// <param name="delay">Delay.</param>
		public void PushSound(string soundId, float delay = 0) {
			SoundSource sound = Statics.GetSoundPrefabClone(soundId).GetComponent<SoundSource>();
			if (sound != null) {
				if (parent != null) {
					sound.transform.SetParent(parent.transform);
				}
				sound.Play(false, delay);
			}
		}

		/// <summary>
		/// 清空音效
		/// </summary>
		public void ClearSounds() {
			if (parent != null) {
				for (int i = parent.transform.childCount - 1; i >= 0; i--) {
					MonoBehaviour.Destroy(parent.transform.GetChild(i).gameObject);
				}
			}
		}

		static SoundManager _instance;
		public static SoundManager GetInstance() {
			if (_instance == null) {
				_instance = new SoundManager();
			}
			return _instance;
		}
	}
}
