using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{
    public StageData currentStageData; // 현재 플레이 중인 스테이지 데이터
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
        // 1. 배경음악 재생
        //if (currentStageData.stageBGM != null)
        //{
        //    _audioSource.clip = currentStageData.stageBGM;
        //    _audioSource.loop = true;
        //    _audioSource.Play();
        //}

        // 2. 스포너에 몬스터 목록 전달
        SpawnManager.Instance.Init(currentStageData);

        // 3. 스킬 매니저/아이템 매니저에 사용할 풀(Pool) 정보 전달
        // SkillManager.Instance.SetAvailableSkills(currentStageData.availableSkills);

        Debug.Log($"{currentStageData.stageName} 스테이지 세팅 완료!");
    }
}
