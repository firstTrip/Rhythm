using System.Collections.Generic;
using UnityEngine;

public class RhythmBuilder : MonoSingleton<RhythmBuilder>
{
    public Color[] beatColors = { Color.red, Color.blue, Color.green, Color.yellow };

    public List<RhythmCircle> Build(
        float songLength,
        float bpm,
        NoteDivision[] pattern,
        BeatTemplate[] templates)
    {
        float secPerQuarter = 60f / bpm;
        float barLength = secPerQuarter * 4;
        float circleLength = barLength * 4;

        int totalCircles = Mathf.FloorToInt(songLength / circleLength);

        float timeCursor = 0f;
        var circles = new List<RhythmCircle>();

        for (int c = 0; c < totalCircles; c++)
        {
            var circle = new RhythmCircle();

            for (int b = 0; b < 4; b++)
            {
                var bar = new BarData();
                bar.startTime = timeCursor;
                bar.barColor = beatColors[b];

                float subCursor = timeCursor;
                int lane = 0;

                foreach (var div in pattern)
                {
                    bar.subBeats.Add(new SubBeatData
                    {
                        time = subCursor,
                        division = div,
                        beatTemplate = templates[lane % 4]
                    });

                    subCursor += DivisionToSeconds(div, secPerQuarter);
                    lane++;
                }

                timeCursor += barLength;
                circle.bars.Add(bar);
            }

            circles.Add(circle);
        }

        return circles;
    }

    public float DivisionToSeconds(NoteDivision div, float q)
    {
        return div switch
        {
            NoteDivision.Quarter => q,
            NoteDivision.Eighth => q * 0.5f,
            NoteDivision.Half => q * 2f,
            NoteDivision.Whole => q * 4f,
            NoteDivision.Sixteenth => q * 0.25f,
            _ => q
        };
    }
}
