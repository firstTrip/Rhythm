using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
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
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (var clip in clips)
        {
            _audioClips[clip.name] = clip;
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

    public void PlaySFX(string clipName, float pitch = 1.0f)
    {
        if (!_audioClips.ContainsKey(clipName)) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(_audioClips[clipName], sfxVolume * masterVolume);
    }
}
