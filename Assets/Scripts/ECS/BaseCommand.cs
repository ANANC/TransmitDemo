using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCommand: BaseObject
{
    private int entityId;
    private ECSDefine.SystemType systemType;
    private BaseSystem.SystemExpandData expandData;

    public override void Init()
    {
        entityId = 0;
        expandData = null;
    }

    public override void UnInit()
    {
        entityId = 0;
        expandData = null;
    }

    public void FillIn(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        this.entityId = entityId;
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


    public BaseSystem.SystemExpandData GetExpandData()
    {
        return expandData;
    }
}
