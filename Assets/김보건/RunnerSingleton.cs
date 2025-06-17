using Fusion;
using UnityEngine;

public class RunnerSingleton : MonoBehaviour
{

    private void Awake()
    {
        // �ٸ� ������ ����ִ� ������Ʈ�� ������ ����
        if (Instance == null)
        {
            CreateAndAssignRunner();
        }
        else
        {
            // �ߺ��� RunnerSingleton ������Ʈ�� ����
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // �� ��ȯ ����
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
            Destroy(Instance.gameObject); // ���ʸ� ����
            Instance = null;
        }

    }

    public static NetworkRunner CreateRunner()
    {
        var singletonObj = Object.FindFirstObjectByType<RunnerSingleton>();
        if (singletonObj == null)
        {
            Debug.LogError("���ʽ̱��� ������Ʈ�� ���� ����");
            return null;
        }

        singletonObj.CreateAndAssignRunner();
        return Instance;
    }

    public static NetworkRunner Instance { get; private set; }

    [SerializeField] private NetworkRunner runnerPrefab;
}