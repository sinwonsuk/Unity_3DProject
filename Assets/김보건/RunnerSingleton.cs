using Fusion;
using UnityEngine;

public class RunnerSingleton : MonoBehaviour
{
    public static NetworkRunner Instance { get; private set; }

    [SerializeField] private NetworkRunner runnerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            CreateAndAssignRunner();
        }
        else
        {
            Destroy(gameObject);
        }
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
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    public static NetworkRunner CreateRunner()
    {
        var singletonObj = Object.FindFirstObjectByType<RunnerSingleton>();
        if (singletonObj == null)
        {
            Debug.LogError("RunnerSingleton ������Ʈ�� ���� ����.");
            return null;
        }

        singletonObj.CreateAndAssignRunner(); //�ʱ�ȭ
        return Instance;
    }
}