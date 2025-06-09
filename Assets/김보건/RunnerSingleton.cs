using Fusion;
using UnityEngine;

public class RunnerSingleton : MonoBehaviour
{
    public static NetworkRunner Instance { get; private set; }

    [SerializeField] private NetworkRunner runnerPrefab;

    private void Awake()
    {
        // 다른 씬에서 살아있는 오브젝트가 없으면 생성
        if (Instance == null)
        {
            CreateAndAssignRunner();
        }
        else
        {
            // 중복된 RunnerSingleton 오브젝트는 제거
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // 씬 전환 유지
    }

    private void CreateAndAssignRunner()
    {
        Instance = Instantiate(runnerPrefab);
        Instance.name = "NetworkRunner";
        DontDestroyOnLoad(Instance);
    }

    public static void ClearRunner()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject); // 러너만 제거
            Instance = null;
        }
    }

    public static NetworkRunner CreateRunner()
    {
        var singletonObj = Object.FindFirstObjectByType<RunnerSingleton>();
        if (singletonObj == null)
        {
            Debug.LogError("RunnerSingleton 오브젝트가 씬에 없음 (CreateRunner 실패)");
            return null;
        }

        singletonObj.CreateAndAssignRunner();
        return Instance;
    }
}