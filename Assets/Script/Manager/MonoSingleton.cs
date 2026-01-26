using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            // 앱 종료 시 싱글톤 호출로 인해 유령 오브젝트가 생기는 것 방지
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] {typeof(T)} 인스턴스는 이미 앱 종료로 인해 파괴되었습니다.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 1. 씬에 이미 배치되어 있는지 확인
                    _instance = (T)FindAnyObjectByType(typeof(T));

                    // 2. 씬에 없다면 새로 생성
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"{typeof(T)} (Singleton)";

                        // 3. 씬 전환 시 파괴되지 않도록 설정
                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // 이미 인스턴스가 있는데 다른 오브젝트가 존재한다면 파괴
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}