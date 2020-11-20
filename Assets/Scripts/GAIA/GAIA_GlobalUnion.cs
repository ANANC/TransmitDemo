using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAIA_GlobalUnion 
{
    private static GAIA_GlobalUnion globalInstance;
    public static GAIA_GlobalUnion Union
    {
        get { return globalInstance; }
    }

    private List<GAIA_IBaseUnit> ManagerList;
    private Dictionary<string, IManager> ManagerDict;

    private List<IManager> UpdateList;

    private List<IManager> AddList;
    private List<IManager> DeleteList;


    public static void ResetGlobalUnionEnvionment(GAIA_GlobalUnion globalUnion)
    {
        globalInstance = globalUnion;
    }

    public void Init()
    {

    }

    public void UnInit()
    {

    }

    public void Start()
    {
        if (ManagerList.Count > 0)
        {
            for (int index = 0; index < ManagerList.Count; index++)
            {
                ManagerList[index].Start();
            }
        }
    }

    public void Update()
    {
        if (AddList.Count > 0)
        {
            for (int index = 0; index < AddList.Count; index++)
            {
                UpdateList.Add(AddList[index]);
            }
        }

        if (DeleteList.Count > 0)
        {
            for (int index = 0; index < DeleteList.Count; index++)
            {
                UpdateList.Remove(DeleteList[index]);
            }
        }

        if (UpdateList.Count > 0)
        {
            for (int index = 0; index < UpdateList.Count; index++)
            {
                UpdateList[index].Update();
            }
        }
    }

    public void AddManager(IManager manager)
    {
        manager.Init();
        ManagerDict.Add(manager.GetType().Name, manager);
        ManagerList.Add(manager);

        AddList.Add(manager);
    }

    public void RemoveManager<T>() where T : IManager
    {
        IManager manager;
        string type = typeof(T).Name;
        if (ManagerDict.TryGetValue(type, out manager))
        {
            ManagerDict.Remove(type);
            ManagerList.Remove(manager);

            DeleteList.Add(manager);
        }
    }

    public T Get<T>() where T : IManager
    {
        IManager manager;
        string type = typeof(T).Name;
        if (ManagerDict.TryGetValue(type, out manager))
        {
            return (T)manager;
        }
        return default(T);
    }

}
