using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity: OperationObject
{
    private int entityId;
    private ECSDefine.EntityType entityType;
    private List<BaseComponent> componentList;
    private Dictionary<ECSDefine.ComponentType, List<BaseComponent>> componentDict;

    public override void Init()
    {
        componentList = new List<BaseComponent>();
        componentDict = new Dictionary<ECSDefine.ComponentType, List<BaseComponent>>();
    }

    public override void UnInit()
    {
        for(int index = 0;index< componentList.Count;index++)
        {
            componentList[index].UnInit();
        }

        componentList.Clear();
        componentDict.Clear();
    }

    public void SetEntityId(int id)
    {
        entityId = id;
    }

    public int GetEntityId()
    {
        return entityId;
    }

    public void SetEntityType(ECSDefine.EntityType entityType)
    {
        this.entityType = entityType;
    }

    public ECSDefine.EntityType GetEntityType()
    {
        return entityType;
    }


    public void AddComponent(BaseComponent component)
    {
        ECSDefine.ComponentType componentType = component.GetComponentType();
        componentList.Add(component);

        List<BaseComponent> componentCollection;
        if (!componentDict.TryGetValue(componentType,out componentCollection))
        {
            componentCollection = new List<BaseComponent>();
            componentDict.Add(componentType, componentCollection);
        }
        componentCollection.Add(component);
    }

    public void RemoveComponent(BaseComponent component)
    {
        ECSDefine.ComponentType componentType = component.GetComponentType();
        componentList.Remove(component);

        List<BaseComponent> componentCollection;
        if (componentDict.TryGetValue(componentType, out componentCollection))
        {
            componentCollection.Remove(component);
        }
    }

    public List<BaseComponent> GetAllComponent()
    {
        return componentList;
    }

    public BaseComponent GetComponentByComponentType(ECSDefine.ComponentType componentType, int id = -1)
    {
        BaseComponent component = null;

        List<BaseComponent> componentCollection;
        if (componentDict.TryGetValue(componentType, out componentCollection))
        {
            if (componentCollection.Count == 1)
            {
                component = componentCollection[0];
            }
            else if (componentCollection.Count > 1)
            {
                for(int index = 0;index< componentCollection.Count;index++)
                {
                    if(componentCollection[index].GetComponentId() == id)
                    {
                        component = componentCollection[index];
                        break;
                    }
                }
            }
        }

        return component;
    }

    public BaseComponent GetComponentByComponentId(int id)
    {
        BaseComponent component = null;
        for (int index = 0;index< componentList.Count;index++)
        {
            if(componentList[index].GetComponentId() == id)
            {
                component = componentList[index];
            }
        }

        return component;
    }
}
