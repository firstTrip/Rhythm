using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmUITest : MonoBehaviour
{
    [Header("References")]
    public RectTransform beatContainer;
    public RectTransform timeCursor;
    public GameObject beatPointPrefab;

    [Header("Layout")]
    public float pixelsPerBeat = 80f;
    public float maxPulseScale = 1.5f;
    public float pulseRange = 0.25f;

    [Header("Rhythm")]
    public float bpm = 140f;

    const int BarsPerCircle = 4;
    const float BeatsPerBar = 4f;

    float secPerBeat;
    float circleTime;
    float beatsPerCircle;

    readonly List<UIBeat> beats = new();

    void Start()
    {
        secPerBeat = 60f / bpm;
        beatsPerCircle = BarsPerCircle * BeatsPerBar;
        circleTime = 0f;

        BuildCircleUI();
    }

    void Update()
    {
        circleTime += Time.deltaTime;

        float circleDuration = beatsPerCircle * secPerBeat;
        if (circleTime >= circleDuration)
        {
            circleTime -= circleDuration;
            OnCircleReset();
        }

        UpdateBeatsVisual();
    }

    void BuildCircleUI()
    {
        var pattern = new[]
        {
            NoteDivision.Quarter,
            NoteDivision.Eighth,
            NoteDivision.Eighth,
            NoteDivision.Half
        };

        float beatOffset = 0f;

        for (int bar = 0; bar < BarsPerCircle; bar++)
        {
            foreach (var div in pattern)
            {
                CreateBeatPoint(beatOffset, div);
                beatOffset += DivisionToBeat(div);
            }
        }
    }

    void OnCircleReset()
    {
        foreach (var beat in beats)
        {
            beat.rect.localScale = Vector3.one;
        }
    }

    void CreateBeatPoint(float beatOffset, NoteDivision div)
    {
        var go = Instantiate(beatPointPrefab, beatContainer);
        var rect = go.GetComponent<RectTransform>();
        var img = go.GetComponent<Image>();
        go.gameObject.SetActive(true);

        rect.anchoredPosition = new Vector2(
            beatOffset * pixelsPerBeat,
            0f
        );

        img.color = GetColorByDivision(div);

        beats.Add(new UIBeat
        {
            rect = rect,
            beatOffset = beatOffset
        });
    }

    void UpdateBeatsVisual()
    {
        float currentBeat = circleTime / secPerBeat;

        foreach (var beat in beats)
        {
            float diff = Mathf.Abs(currentBeat - beat.beatOffset);
            float t = Mathf.InverseLerp(pulseRange, 0f, diff);
            float scale = Mathf.Lerp(1f, maxPulseScale, t);

            beat.rect.localScale = Vector3.one * scale;
        }

        timeCursor.anchoredPosition = new Vector2(
            currentBeat * pixelsPerBeat,
            0f
        );
    }

    float DivisionToBeat(NoteDivision div)
    {
        return div switch
        {
            NoteDivision.Quarter => 1f,
            NoteDivision.Eighth => 0.5f,
            NoteDivision.Half => 2f,
            _ => 1f
        };
    }

    Color GetColorByDivision(NoteDivision div)
    {
        return div switch
        {
            NoteDivision.Quarter => Color.red,
            NoteDivision.Eighth => Color.cyan,
            NoteDivision.Half => Color.yellow,
            _ => Color.white
        };
    }

    class UIBeat
    {
        public RectTransform rect;
        public float beatOffset;
    }
}
