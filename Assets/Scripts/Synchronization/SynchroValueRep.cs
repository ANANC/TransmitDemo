using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchroValueRep : OperationObject
{
    private int synchroValueRepId;
    public class SynchroValueRepStructure
    {
        public int entityId;
        public int componentId;
        public ECSDefine.ComponentType componentType;
    }

    private SynchroValueRepStructure synchroValueRepStructure;

    public override void Init()
    {
        synchroValueRepStructure = new SynchroValueRepStructure();
    }

    public override void UnInit()
    {
        synchroValueRepStructure = null;
    }

    public void SetSynchroValueRepId(int synchroValueRepId)
    {
        this.synchroValueRepId = synchroValueRepId;
    }

    public int GetSynchroValueRepId()
    {
        return synchroValueRepId;
    }
    public void SetEntityId(int entityId)
    {
        synchroValueRepStructure.entityId = entityId;
    }

    public int GetEntityId()
    {
        return synchroValueRepStructure.entityId;
    }
    public void SetComponentId(int componentId)
    {
        synchroValueRepStructure.componentId = componentId;
    }

    public int GetComponentId()
    {
        return synchroValueRepStructure.componentId;
    }
    public void SetComponentType(ECSDefine.ComponentType componentType)
    {
        synchroValueRepStructure.componentType = componentType;
    }

    public ECSDefine.ComponentType GetComponentType()
    {
        return synchroValueRepStructure.componentType;
    }

}
