using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchroValueRsp : OperationObject
{
    private int synchroValueRspId;
    public class SynchroValueRspStructure
    {
        public ECSDefine.ComponentType componentType;
        public int entityId;
        public int componentId;
        public bool isEntityAlive;
        public bool isComponentAlive;
        public BaseComponent.ComponentExpandData expandData;
    }

    private SynchroValueRspStructure synchroValueRspStructure;

    public override void Init()
    {
        synchroValueRspStructure = new SynchroValueRspStructure();
    }

    public override void UnInit()
    {
        synchroValueRspStructure = null;
    }

    public void SetSynchroValueRspId(int synchroValueRspId)
    {
        this.synchroValueRspId = synchroValueRspId;
    }

    public int GetSynchroValueRspId()
    {
        return synchroValueRspId;
    }

    public SynchroValueRspStructure GetSynchroValueRspStructure()
    {
        return synchroValueRspStructure;
    }

    public void SetSynchroValueRspStructure(SynchroValueRspStructure synchroValueRspStructure)
    {
        this.synchroValueRspStructure = synchroValueRspStructure;
    }
    public void SetEntityId(int entityId)
    {
        synchroValueRspStructure.entityId = entityId;
    }

    public int GetEntityId()
    {
        return synchroValueRspStructure.entityId;
    }

    public void SetComponentId(int componentId)
    {
        synchroValueRspStructure.componentId = componentId;
    }

    public int GetComponentId()
    {
        return synchroValueRspStructure.componentId;
    }
    public void SetComponentType(ECSDefine.ComponentType componentType)
    {
        synchroValueRspStructure.componentType = componentType;
    }

    public ECSDefine.ComponentType GetComponentType()
    {
        return synchroValueRspStructure.componentType;
    }
    public void SetIsEntityAlive(bool isEntityAlive)
    {
        synchroValueRspStructure.isEntityAlive = isEntityAlive;
    }

    public bool GetIsEntityAlive()
    {
        return synchroValueRspStructure.isEntityAlive;
    }

    public void SetIsComponentAlive(bool isComponentAlive)
    {
        synchroValueRspStructure.isComponentAlive = isComponentAlive;
    }

    public bool GetIsComponentAlive()
    {
        return synchroValueRspStructure.isComponentAlive;
    }

    public void SetExpandData(BaseComponent.ComponentExpandData expandData)
    {
        synchroValueRspStructure.expandData = expandData;
    }

    public BaseComponent.ComponentExpandData GetExpandData()
    {
        return synchroValueRspStructure.expandData;
    }

}
