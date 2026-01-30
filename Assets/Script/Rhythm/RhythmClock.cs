using UnityEngine;

public class RhythmClock : MonoSingleton<RhythmClock>
{
    [SerializeField] private bool useMetronome = true;

    [Header("Settings")]
    [SerializeField] public float bpm = 120f;
    [SerializeField] public float stageDuration = 180f;

    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; } = false;

    private float _startTime;
    private float _beatInterval;
    private int _lastBeatCount = -1;

    public System.Action<int> OnBeat;
    public System.Action OnStageEnd;

    public void StartClock()
    {
        _beatInterval = 60f / bpm;
        _startTime = (float)AudioSettings.dspTime;
        ElapsedTime = 0f;
        IsRunning = true;
    }

    void Update()
    {
        if (!IsRunning) return;

        ElapsedTime = (float)AudioSettings.dspTime - _startTime;

        if (ElapsedTime >= stageDuration)
        {
            StopClock();
            OnStageEnd?.Invoke();
            return;
        }

        int currentBeatCount = Mathf.FloorToInt(ElapsedTime / _beatInterval);
        if (currentBeatCount > _lastBeatCount)
        {
            _lastBeatCount = currentBeatCount;
            int beatInBar = _lastBeatCount % 4;
            OnBeat?.Invoke(beatInBar);
            
            if (useMetronome)
                PlayMetronome(beatInBar);
        }
    }
    private void PlayMetronome(int beatIndex)
    {
        var pitch = (beatIndex == 0) ? 1.5f : 1.0f;
        SoundManager.Instance.PlaySFX(SoundType.MetronomeNormal,pitch);
    }

    public void StopClock()
    {
        IsRunning = false;
        Time.timeScale = 0;
    }
}
