using System;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    private Dictionary<Type, baseManager> managerMap = new Dictionary<Type, baseManager>();
    private Dictionary<Type, BaseScriptableObject> dicBaseScriptableObject = new Dictionary<Type, BaseScriptableObject>();


    [SerializeField]
    private List<BaseScriptableObject> baseScriptableObjects = new List<BaseScriptableObject>();

    void Awake()
    {
        ConnectBaseScriptableObject();

        Register<UIManager, UIManagerConfig>(Config => new UIManager(Config));



        InitAll();
    }

    void ConnectBaseScriptableObject()
    {
        for (int i = 0; i < baseScriptableObjects.Count; i++)
        {
            dicBaseScriptableObject.Add(baseScriptableObjects[i].type, baseScriptableObjects[i]);
        }
    }

    private void Register<TManager, TConfig>(Func<TConfig, TManager> factory) where TManager : baseManager where TConfig : BaseScriptableObject
    {
    
        TConfig config = (TConfig)dicBaseScriptableObject[typeof(TConfig)];
        TManager manager = factory(config);
        // ���׸��� new TManager �� �ȵǼ� ��¿������ Func<TConfig, TManager> factory ��� 
        RegisterMap(manager);
    }

    private void RegisterMap<T1>(T1 manager) where T1 : baseManager
    {
        managerMap[typeof(T1)] = manager;
    }

    // ���δ� �ʱ�ȭ 
    private void InitAll()
    {
        foreach (var manager in managerMap.Values)
        {
           manager.Init();
        }
    }

    // Ȥ�� ���� ���� 
    public T GetManager<T>() where T : baseManager
    {
        return (T)managerMap[typeof(T)];

        // ��� ����
        //GetManager<UIManager>().�Լ�      
    }

    void Update()
    {

     
    }
}