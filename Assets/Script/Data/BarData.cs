using System.Collections.Generic;
using UnityEngine;

public class BarData 
{
    public float startTime;
    public Color barColor;
    public int attackId;
    public List<SubBeatData> subBeats = new();

    public bool isFailed = false;
}
