using UnityEngine;

public class BeatManager : MonoSingleton<BeatManager>
{
    public float bpm = 120f;
    public Color[] beatColors = { Color.red, Color.blue, Color.green, Color.yellow };
    public int currentBeatIndex;

    [Header("Difficulty Settings")]
    public float beatComplexityMultiplier = 1.0f; 

    private float _beatInterval;
    private float _nextBeatTime;
    private AudioSource _bgm;

    protected override void Awake()
    {
        base.Awake();
        _beatInterval = 60f / bpm;
        _bgm = GetComponent<AudioSource>();
    }

    public BeatRating GetHitRating()
    {
        float currentTime = _bgm.time;
        float diff = Mathf.Abs(currentTime - (_nextBeatTime - _beatInterval));

        if (diff < 0.08f) return BeatRating.Perfect;
        if (diff < 0.18f) return BeatRating.Normal;
        return BeatRating.Miss;
    }

    public float GetProgressInMeasure()
    {
        float totalMeasureDuration = _beatInterval * 4;
        return (_bgm.time % totalMeasureDuration) / totalMeasureDuration;
    }

    public float GetDifficultyBonus()
    {
        return 1.0f + (beatComplexityMultiplier - 1.0f);
    }

    void Update()
    {
        if (_bgm.time >= _nextBeatTime)
        {
            currentBeatIndex = (currentBeatIndex + 1) % 4;
            _nextBeatTime += _beatInterval;

            CameraManager.Instance.Shake(0.1f, 0.05f);
        }
    }
}
