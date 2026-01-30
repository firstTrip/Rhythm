using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoSingleton<SoundManager>
{

    [System.Serializable]
    public struct SoundData
    {
        public SoundType type;
        public AudioClip clip;
    }

    public List<SoundData> soundList; // 인스펙터에서 enum과 clip을 매칭
    private Dictionary<SoundType, AudioClip> _soundDict = new Dictionary<SoundType, AudioClip>();

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource; // 단발성 효과음용

    [Header("Settings")]
    [Range(0, 1)] public float masterVolume = 1.0f;
    [Range(0, 1)] public float bgmVolume = 0.8f;
    [Range(0, 1)] public float sfxVolume = 1.0f;

    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();
        LoadClips();
    }

    private void LoadClips()
    {
        foreach (var data in soundList)
        {
            if (!_soundDict.ContainsKey(data.type))
                _soundDict.Add(data.type, data.clip);
        }
    }

    public void PlayBGM(string clipName, bool loop = true)
    {
        if (!_audioClips.ContainsKey(clipName)) return;

        bgmSource.clip = _audioClips[clipName];
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }

    public void PlaySFX(SoundType type, float pitch = 1.0f, float volume = 1.0f)
    {
        if (_soundDict.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] {type}에 해당하는 클립이 없습니다.");
        }
    }
}
