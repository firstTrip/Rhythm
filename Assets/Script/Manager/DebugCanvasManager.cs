using TMPro;
using UnityEngine;

public class DebugCanvasManager : MonoSingleton<DebugCanvasManager>
{
    [SerializeField] TextMeshProUGUI timer;


    public void SetTimer(string txt)
    {
        timer.text = txt;
    }
}
