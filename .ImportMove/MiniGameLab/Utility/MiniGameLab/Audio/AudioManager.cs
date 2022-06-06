using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace CommonStuff
{
	public class AudioManager : IDCreator
	{
		public static AudioManager Instance;

		[Space] [Space]
		[Range(0, 1)] public float sfxVolume;

		[Range(0, 1)] public float musicVolume;

		[Space] [Space]
		[SerializeField] private List<AudioData> AudioDataList;

		public List<AudioData> GetAudioDataList() => AudioDataList;


		private List<AudioData> sfxAudioDatas = new List<AudioData>();
		private List<AudioData> musicAudioDatas = new List<AudioData>();

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			if (sfxAudioDatas.Count <= 0)
			{
				sfxAudioDatas = GetAudioDataByGroup(AudioGroup.SFX);
			}

			if (musicAudioDatas.Count <= 0)
			{
				musicAudioDatas = GetAudioDataByGroup(AudioGroup.MUSIC);
			}
		}

		// private void AddNewAudioSources()
		// {
		//     for (int i = 0; i < AudioDataList.Count; i++)
		//     {
		//         AudioData aud = AudioDataList[i];
		//         int Count = aud.AudioSourceCount;
		//         for (int j = 0; j < Count; j++)
		//         {
		//             GameObject go = new GameObject();
		//             go.transform.SetParent(transform);
		//             AudioSource source = go.AddComponent<AudioSource>();
		//             aud.AudioSources.Add(source);
		//             go.SetActive(false);
		//         }
		//     }
		// }

		public override void SetIdentifiers()
		{
			SetIdentifiers(AudioDataList);
		}

		public void Play(String audioType, float volume = -1f, float pitch = -1f, bool loop = false)
		{
			// return if global settings is OFF ...

			if (!String.IsNullOrEmpty(audioType))
			{
				AudioData _audioData = AudioDataList.Find(data => data.id == audioType);
				AudioSource _audioSource = GetAudioSource(_audioData);
				AudioClip _clip = _audioData.SingleClip ? _audioData.AudioClip : _audioData.AudioClipsCollection.AudioClips[Random.Range(0, _audioData.AudioClipsCollection.AudioClips.Count)];
				float _pitch = _audioData.RandomizePitch
					? Random.Range(_audioData.PitchMinMax.PitchMin, _audioData.PitchMinMax.PitchMax)
					: _audioData.Pitch;

				if (_audioSource == null) return;
				_audioSource.clip = _clip;

				float offsetVolume = (_audioData.Group == AudioGroup.SFX ? sfxVolume : musicVolume);

				_audioSource.volume = volume >= 0 ? volume : _audioData.Volume * offsetVolume;

				_audioSource.loop = _audioData.Loop;
				_audioSource.pitch = pitch >= 0 ? pitch : _pitch;
				_audioSource.Play();
			}

			DeactivateUnusedAudioSources();
		}

		public void Stop(String audioType)
		{
			if (!String.IsNullOrEmpty(audioType))
			{
				AudioData audioData = AudioDataList.Find(data => data.id == audioType);
				for (int i = 0; i < audioData.AudioSources.Count; i++)
				{
					audioData.AudioSources[i].Stop();
				}
			}
		}

		public void StopAll(AudioGroup audioGroup)
		{
			List<AudioData> audioDataList = AudioDataList.FindAll(data => data.Group == audioGroup);
			for (int j = 0; j < audioDataList.Count; j++)
			{
				AudioData audioData = audioDataList[j];
				for (int i = 0; i < audioData.AudioSources.Count; i++)
				{
					audioData.AudioSources[i].Stop();
				}
			}
		}

		public void StopAll()
		{
			for (int j = 0; j < AudioDataList.Count; j++)
			{
				AudioData audioData = AudioDataList[j];
				for (int i = 0; i < audioData.AudioSources.Count; i++)
				{
					audioData.AudioSources[i].Stop();
				}
			}
		}


		public AudioSource GetAudioSource(AudioData audiodata)
		{
			AudioSource audioSource = audiodata.AudioSources?.Find(source => source.isPlaying == false);
			if (!audiodata.ForcePlay)
			{
				if (audioSource != null) audioSource.gameObject.SetActive(true);
				return audioSource;
			}

			if (audioSource == null)
				audioSource = audiodata.AudioSources[Random.Range(0, audiodata.AudioSources.Count)];
			audioSource.gameObject.SetActive(true);
			return audioSource;
		}

		public AudioSource GetAudioSource(string audioType)
		{
			if (!String.IsNullOrEmpty(audioType))
			{
				AudioData _audioData = AudioDataList.Find(data => data.id == audioType);
				return GetAudioSource(_audioData);
			}

			return null;
		}

		void DeactivateUnusedAudioSources()
		{
			for (int i = 0; i < AudioDataList.Count; i++)
			{
				AudioData aud = AudioDataList[i];
				for (int j = 0; j < aud.AudioSources.Count; j++)
				{
					AudioSource audioSource = aud.AudioSources[j];
					if (audioSource.isPlaying) continue;
					audioSource.Stop();
					audioSource.gameObject.SetActive(false);
				}
			}
		}


		public void SetVolume(AudioGroup audioGroup, float volume)
		{
			List<AudioData> audioDatas = new List<AudioData>();


			if (audioGroup == AudioGroup.SFX)
			{
				audioDatas.Clear();
				audioDatas.AddRange(sfxAudioDatas);
				sfxVolume = volume;
			}
			else if (audioGroup == AudioGroup.MUSIC)
			{
				audioDatas.Clear();
				audioDatas.AddRange(musicAudioDatas);
				musicVolume = volume;
			}


			foreach (var audioData in audioDatas)
			{
				foreach (var audioSource in audioData.AudioSources)
				{
					audioSource.volume = audioData.Volume * volume;
				}
			}
		}


		public List<AudioData> GetAudioDataByGroup(AudioGroup audioGroup)
		{
			List<AudioData> audioDatas = new List<AudioData>();

			foreach (var audioData in AudioDataList)
			{
				if (audioGroup == audioData.Group)
				{
					audioDatas.Add(audioData);
				}
			}

			return audioDatas;
		}
	}

	[Serializable]
	public class AudioData : PropertyID
	{
		public bool SingleClip = true;
		[ShowIf("SingleClip")] [AllowNesting] public AudioClip AudioClip;
		[HideIf("SingleClip")] [AllowNesting] public AudioClipCollection AudioClipsCollection;
		public AudioGroup Group;

		[Space]
		[Range(0f, 1f)] public float Volume;

		public bool RandomizePitch = false;
		[HideIf("RandomizePitch")] [AllowNesting] [Range(0f, 3f)] public float Pitch = 1;
		[ShowIf("RandomizePitch")] [AllowNesting] public PitchMinMax PitchMinMax;
		public bool Loop = false;
		public List<AudioSource> AudioSources = new List<AudioSource>();
		public bool ForcePlay;
	}

	[Serializable]
	public class PitchMinMax
	{
		[Range(0f, 3f)] public float PitchMin = 0f;
		[Range(0f, 3f)] public float PitchMax = 3f;
	}

	[Serializable]
	public class AudioClipCollection
	{
		public List<AudioClip> AudioClips;
	}

	[Serializable]
	public enum AudioGroup
	{
		SFX,
		MUSIC,
	}
}