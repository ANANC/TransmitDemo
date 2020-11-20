using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCommand
{
    private int entityId;
    private List<int> componentIdList;
    private ECSDefine.SystemType systemType;
    private BaseSystem.SystemExpandData expandData;

    public void Init()
    {
        entityId = 0;
        componentIdList = null;
        expandData = null;
    }

    public void UnInit()
    {
        entityId = 0;
        expandData = null;
        if(componentIdList != null)
        {
            componentIdList.Clear();
        }
    }

    public void Send(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        this.entityId = entityId;
        this.systemType = systemType;
        this.expandData = expandData;
    }

    public void Receive(int entityId, List<int> componentIdList, ECSDefine.SystemType systemType,BaseSystem.SystemExpandData expandData)
    {
        this.entityId = entityId;
        this.componentIdList = componentIdList;
        this.systemType = systemType;
        this.expandData = expandData;
    }

    public int GetEntityId()
    {
        return entityId;
    }

    public ECSDefine.SystemType GetSystemType()
    {
        return systemType;
    }

    public List<int> GetComponentIdList()
    {
        return componentIdList;
    }

    public BaseSystem.SystemExpandData GetExpandData()
    {
        return expandData;
    }
}
