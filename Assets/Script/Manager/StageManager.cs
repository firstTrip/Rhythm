using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    public StageData currentStageData;
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (currentStageData != null)
        {
            ApplyStageSettings();
        }
    }

    public void ApplyStageSettings()
    {
        SpawnManager.Instance.Init(currentStageData);
    }
}
