using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseComponent : OperationObject
{
    public class ComponentExpandData { };

    private int entityId;
    private int componentId;
    private ComponentExpandData expandData;
    private ECSDefine.ComponentType componentType;

    public override void Init()
    {

    }

    public override void UnInit()
    {

    }

    public abstract void FillIn();

    public void SetEntityId(int id)
    {
        entityId = id;
    }

    public int GetEntityId()
    {
        return entityId;
    }

    public void SetComponentId(int id)
    {
        componentId = id;
    }

    public int GetComponentId()
    {
        return componentId;
    }

    public void SetComponentType(ECSDefine.ComponentType componentType)
    {
        this.componentType = componentType;
    }

    public ECSDefine.ComponentType GetComponentType()
    {
        return this.componentType;
    }

    public void SetComponentExpandData(ComponentExpandData expandData)
    {
        this.expandData = expandData;
    }

    public ComponentExpandData GetComponentExpandData()
    {
        return expandData;
    }

}
