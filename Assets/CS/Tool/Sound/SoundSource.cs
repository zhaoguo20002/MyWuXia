using UnityEngine;
using System.Collections;

public class SoundSource : MonoBehaviour {
	public AudioSource Audio;
	public float Pitch = 1;
	public float Volume = 1;
	bool _loop;
	bool _mute;
	void Awake() {
		Audio = GetComponent<AudioSource>();
		if (Audio == null) {
			enabled = false;
		}
		Audio.playOnAwake = false;
		Audio.pitch = Pitch;
		Audio.volume = Volume;
	}

	// Use this for initialization
	void Start () {
		_loop = false;
		_mute = false;
		Audio.volume = Volume;
	}

	/// <summary>
	/// 播放声音
	/// </summary>
	/// <param name="delay">Delay.</param>
	public void Play(bool loop = false, float delay = 0) {
		if (!Audio.isPlaying) {
			_loop = loop;
			Audio.loop = _loop;
			Audio.PlayDelayed(delay);
			Audio.volume = Volume;
		}
	}

	/// <summary>
	/// 停止声音
	/// </summary>
	public void Stop() {
		if (Audio.isPlaying) {
			Audio.Stop();
		}
		Destroy();
	}

	/// <summary>
	/// 暂停声音
	/// </summary>
	public void Pause() {
		if (Audio.isPlaying) {
			Audio.Pause();
		}
	}

	/// <summary>
	/// 取消暂停声音
	/// </summary>
	public void UnPause() {
		if (!Audio.isPlaying) {
			Audio.UnPause();
		}
	}

	/// <summary>
	/// 设置是否静音
	/// </summary>
	/// <param name="mute">If set to <c>true</c> mute.</param>
	public void Mute(bool mute) {
		_mute = _mute;
		Audio.mute = _mute;
	}

	/// <summary>
	/// 切换静音/非静音
	/// </summary>
	public void Toggle() {
		_mute = _mute ? false : true;
		Mute(_mute);
	}

	/// <summary>
	/// 销毁声音
	/// </summary>
	public void Destroy() {
		Destroy(gameObject);
	}

	void Update() {
		if (!Audio.loop && (Audio.time + 0.01f) >= Audio.clip.length) {
			Destroy();
		}
	}
}
