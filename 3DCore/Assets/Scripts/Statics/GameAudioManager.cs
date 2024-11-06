using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudioManager : SingletonMono<GameAudioManager>
{
	AudioSource _asBGM;
	AudioSource[] _asSFX;
	string nowBGM = string.Empty;

	// 기본 볼륨. 옵션 등에 연결.
	float _fBgmVolume = 0.6f;
	float _fSfxVolume = 0.6f;

	static AudioMixer _audioMix = null;

	// 동시 처리할 최대 sfx 수량.
	// 많을수록 부하.
	int MAX_COUNT_SFX = 20;

	// 풀링 리스트.
	static List<AudioSource> _sfxReadyAudioSources = new List<AudioSource>();
	static List<AudioSource> _sfxPlayAudioSources = new List<AudioSource>();

	// 오디오 믹서를 담아두는 딕셔너리.
	static Dictionary<string, AudioMixerGroup> _dicAudioMixGroup = new Dictionary<string, AudioMixerGroup>();

	private void Awake()
	{
		Create();
	}

	public void Create()
	{
		// 오디오 매니저 초기화.

		_sfxReadyAudioSources.Clear();
		_sfxPlayAudioSources.Clear();

		if (null == _asBGM)
		{
			_asBGM = new AudioSource();
			_asBGM = gameObject.AddComponent<AudioSource>();
			_asBGM.playOnAwake = false;
			_asBGM.loop = true;
			_asBGM.volume = _fBgmVolume;
		}

		for (int i = 0; i < MAX_COUNT_SFX; i++)
		{
			var oAudioGameObj = new GameObject("SFXAudio");
			oAudioGameObj.transform.SetParent(this.gameObject.transform, false);

			var audios = oAudioGameObj.AddComponent<AudioSource>();
			audios.playOnAwake = false;
			audios.loop = false;
			audios.volume = _fSfxVolume;
			audios.rolloffMode = AudioRolloffMode.Linear;
			_sfxReadyAudioSources.Add(audios);
		}

		_dicAudioMixGroup.Clear();
		if (_audioMix == null)
			_audioMix = Resources.Load<AudioMixer>("audios/AudioMixer");

		_audioMix.FindMatchingGroups("Master");
		_dicAudioMixGroup.Add("Master", _audioMix.FindMatchingGroups("Master")[0]);

		for (int i = 0; i < ComType.AudioMixPaths.Length; i++)
		{
			string path = ComType.AudioMixPaths[i];
			var mix = _audioMix.FindMatchingGroups(path);

			if (mix != null)
			{
				_dicAudioMixGroup.Add(path, _audioMix.FindMatchingGroups(path)[0]);
			}
		}
	}

	/// <summary>
	/// 오디오 스냅샷 자동 변경.
	/// 신 이름에 맞도록.
	/// </summary>
	/// <param name="scene"></param>
	public static void ChangeAudioMixSnapShot(string scene)
	{
		AudioMixerSnapshot snapShot = _audioMix.FindSnapshot(scene);

		if (snapShot != null)
			_audioMix.TransitionToSnapshots(new AudioMixerSnapshot[] { snapShot }, new float[] { .5f }, Time.deltaTime * 5.0f);
		else
		{
			snapShot = _audioMix.FindSnapshot("Default");
			_audioMix.TransitionToSnapshots(new AudioMixerSnapshot[] { snapShot }, new float[] { .5f }, Time.deltaTime * 5.0f);
		}
	}

	/// <summary>
	/// 풀링 처리를 위한 메서드
	/// </summary>
	/// <returns>audiosource</returns>
	private static AudioSource GetSFXReadyAudioSource()
	{
		for (int i = 0; i < _sfxPlayAudioSources.Count; i++)
		{
			var tmp = _sfxPlayAudioSources[i];

			if (tmp.isPlaying == false)
			{
				_sfxReadyAudioSources.Add(tmp);
				_sfxPlayAudioSources.RemoveAt(i);
			}
		}

		if (_sfxReadyAudioSources.Count > 0)
		{
			var tmp = _sfxReadyAudioSources[0];

			_sfxReadyAudioSources.RemoveAt(0);
			_sfxPlayAudioSources.Add(tmp);

			return tmp;
		}
		else
		{
			var tmp = _sfxPlayAudioSources[0];
			_sfxPlayAudioSources.Add(tmp);
			_sfxPlayAudioSources.RemoveAt(0);

			return tmp;
		}
	}

	/// <summary>
	/// 최종 sfx 재생 메서드.
	/// </summary>
	/// <param name="audioClip"></param>
	/// <param name="mixerGroup"></param>
	/// <param name="loop"></param>
	/// <param name="pitch"></param>
	private static void PlaySFX(AudioClip audioClip, string mixerGroup = "Master/SFX", bool loop = false, float pitch = 1.0f)
	{
		if (null == Singleton) return;

		if (_dicAudioMixGroup.ContainsKey(mixerGroup) == false)
			GameManager.Log($"Mixer Group Not found : {mixerGroup}", "red");

		if (0f >= Singleton._fSfxVolume) return;

		var selectAudio = GetSFXReadyAudioSource();
		selectAudio.Stop();

		selectAudio.volume = Singleton._fSfxVolume;
		selectAudio.clip = audioClip;
		selectAudio.loop = loop;
		selectAudio.spatialBlend = 0.0f;
		selectAudio.outputAudioMixerGroup = _dicAudioMixGroup[mixerGroup];
		selectAudio.reverbZoneMix = 0f;
        selectAudio.rolloffMode = AudioRolloffMode.Linear;
        selectAudio.Play();
	}

	/// <summary>
	/// 최종 3d sfx 재생 메서드
	/// </summary>
	/// <param name="audioClip"></param>
	/// <param name="vec"></param>
	/// <param name="minDistance"></param>
	/// <param name="maxDistance"></param>
	/// <param name="mixerGroup"></param>
	/// <param name="loop"></param>
	private static void _PlaySFX3D(AudioClip audioClip, Vector3 vec, float minDistance = 1, float maxDistance = 30, string mixerGroup = "Master/SFX/Battle", bool loop = false)
	{
		if (null == Singleton) return;

		if (_dicAudioMixGroup.ContainsKey(mixerGroup) == false)
			GameManager.Log($"Mixer Group Not found : {mixerGroup}", "red");

		if (0f >= Singleton._fSfxVolume) return;

		var selectAudio = GetSFXReadyAudioSource();

		selectAudio.pitch = 1.0f;
		selectAudio.volume = Singleton._fSfxVolume;
		selectAudio.clip = audioClip;
		selectAudio.loop = loop;
		selectAudio.outputAudioMixerGroup = _dicAudioMixGroup[mixerGroup];
		selectAudio.transform.position = vec;
		selectAudio.spatialBlend = 1f;
		selectAudio.minDistance = minDistance;
		selectAudio.maxDistance = maxDistance;
		selectAudio.reverbZoneMix = 0f;
        selectAudio.rolloffMode = AudioRolloffMode.Linear;
        selectAudio.Play();
	}

	private static IEnumerator _PlaySFXRoutine(AudioClip audioClip, float delay, bool bLoop, string mixerGroup, float pitch = 1.0f)
	{
		yield return new WaitForSeconds(delay);

		PlaySFX(audioClip, mixerGroup, bLoop, pitch);
	}

	private static IEnumerator _PlayBGMRoutine(AudioClip audioClip, bool bLoop)
	{
		WaitForEndOfFrame frame = new WaitForEndOfFrame();

		if (true == Singleton._asBGM.isPlaying)
		{
			while (Singleton._asBGM.volume > 0f)
			{
				Singleton._asBGM.volume -= 0.01f;
				yield return frame;
			}

			Singleton._asBGM.Stop();
		}

		if (null != audioClip)
		{
			Singleton._asBGM.loop = bLoop;
			Singleton._asBGM.clip = audioClip;

			Singleton._asBGM.outputAudioMixerGroup = _dicAudioMixGroup[ComType.BGM_MIX];
			Singleton._asBGM.Play();

			while (Singleton._asBGM.volume < Singleton._fBgmVolume)
			{
				Singleton._asBGM.volume += 0.01f;
				yield return frame;
			}
		}
	}

	private static void _PlayBGMInstantly(AudioClip audioClip, bool bLoop)
	{
		if (null != audioClip)
		{
			Singleton._asBGM.volume = Singleton._fBgmVolume;
			Singleton._asBGM.loop = bLoop;
			Singleton._asBGM.clip = audioClip;
			Singleton._asBGM.outputAudioMixerGroup = _dicAudioMixGroup[ComType.BGM_MIX];
			Singleton._asBGM.Play();
		}
	}

	public static void PlaySFX(AudioClip audioClip, float delay = 0f, string mixerGroup = "Master/SFX/Battle", bool loop = false, float pitch = 1.0f)
	{
		if (null == audioClip) return;

		if (delay > 0f)
		{
			Singleton.StartCoroutine(_PlaySFXRoutine(audioClip, delay, loop, mixerGroup, pitch));
		}
		else
		{
			PlaySFX(audioClip, mixerGroup, loop, pitch);
		}
	}

	public static void PlaySFX3D(AudioClip audioClip, Vector3 vec, float minDistance = 1, float maxDistance = 30, string mixerGroup = "Master/SFX/Battle", bool loop = false)
	{
		if (null == audioClip) return;

		_PlaySFX3D(audioClip, vec, minDistance, maxDistance, mixerGroup, loop);
	}

	public static void PlayBGM(AudioClip audioClip, bool bLoop = true)
	{
		if (null == audioClip) return;

		_PlayBGMInstantly(audioClip, bLoop);
	}

	public static void PlaySFX(string audioName, float delay = 0f, bool loop = false, string mixKey = "Master/SFX/Battle", float pitch = 1.0f)
	{
		if (false == string.IsNullOrEmpty(audioName))
		{
			AudioClip clip = GameResourceManager.Singleton.LoadAudioClip(audioName);
			if (null != clip)
			{
				PlaySFX(clip, delay, mixKey, loop, pitch);
			}
			else
				GameManager.Log("audioClip is null : PlaySFX(), " + audioName, "red");
		}
	}

	public static void PlaySFX3D(string audioName, Vector3 vec, float minDistance = 1, float maxDistance = 30, string mixKey = "Master/SFX/Battle", bool loop = false)
	{
		if (false == string.IsNullOrEmpty(audioName))
		{
			AudioClip clip = GameResourceManager.Singleton.LoadAudioClip(audioName);

			if (null != clip)
			{
				PlaySFX3D(clip, vec, minDistance, maxDistance, mixKey, loop);
			}
			else
				GameManager.Log("audioClip is null : PlaySFX3D(), " + audioName, "red");
		}
	}

	public static bool IsPlaySFX(string audioName)
	{
		for (int i = 0; i < _sfxPlayAudioSources.Count; i++)
		{
			if (_sfxPlayAudioSources[i].isPlaying)
			{
				if (_sfxPlayAudioSources[i].clip.name.Equals(audioName))
				{
					return true;
				}
			}
		}

		return false;
	}

	public static void StopSFX(string audioName)
	{
		for (int i = 0; i < _sfxPlayAudioSources.Count; i++)
		{
			if (_sfxPlayAudioSources[i].name == audioName)
				_sfxPlayAudioSources[i].Stop();
		}
	}

	public static void StopSFXAll()
	{
		for (int i = 0; i < _sfxPlayAudioSources.Count; ++i)
		{
			_sfxPlayAudioSources[i].loop = false;
			_sfxPlayAudioSources[i].Stop();
		}
	}

    public static void PlayBGM(string audioName)
	{
		if (false == string.IsNullOrEmpty(audioName))
		{
			if (true == Singleton._asBGM.isPlaying)
			{
				if (true == Singleton.nowBGM.Equals(audioName))
					return;
				else
				{
					Singleton.StartCoroutine(ChangeBGMRoutine(audioName));
					return;
				}
			}

			AudioClip clip = GameResourceManager.Singleton.LoadAudioClip(audioName);
			if (null != clip)
			{
				Singleton.nowBGM = audioName;
				PlayBGM(clip, true);
			}
		}
	}

	private static IEnumerator ChangeBGMRoutine(string audioName)
	{
		WaitForEndOfFrame frame = new WaitForEndOfFrame();
		float fTime = 1f;

		while (0f < fTime)
		{
			fTime -= Time.deltaTime;
			Singleton._asBGM.volume = fTime / 1f * Singleton._fBgmVolume;
			yield return frame;
		}

		Singleton._asBGM.volume = 0f;
		Singleton._asBGM.Stop();

		AudioClip clip = GameResourceManager.Singleton.LoadAudioClip(audioName);

		if (null != clip)
		{
			Singleton.nowBGM = audioName;
			Singleton._asBGM.clip = clip;
			Singleton._asBGM.loop = true;
			Singleton._asBGM.volume = 0f;
			Singleton._asBGM.outputAudioMixerGroup = _dicAudioMixGroup[ComType.BGM_MIX];
			Singleton._asBGM.Play();
		}

		if (0f != Singleton._fBgmVolume)
		{
			fTime = 0f;

			while (1f > fTime)
			{
				fTime += Time.deltaTime;
				Singleton._asBGM.volume = fTime * Singleton._fBgmVolume;
				yield return frame;
			}

			Singleton._asBGM.volume = Singleton._fBgmVolume;
		}
	}

	public static void StopBGM(bool bInstantStop = true)
	{
		if (bInstantStop == false)
		{
			Singleton.nowBGM = string.Empty;
			Singleton.StartCoroutine(_PlayBGMRoutine(null, false));
		}
		else
		{
			Singleton._asBGM.Stop();
		}
	}
}
