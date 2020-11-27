using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSUnit: BaseUnit
{
    private Operation entityOperation;
    private Operation componentOperation;

    private IdDistributionChunk copyComponentIdDistributionChunk;

    public override void Init()
    {
        entityOperation = new Operation();
        entityOperation.Init();
        entityOperation.SetName("Entity");

        componentOperation = new Operation();
        componentOperation.Init();
        componentOperation.SetName("Component");

        copyComponentIdDistributionChunk = new IdDistributionChunk();
        copyComponentIdDistributionChunk.Init();
        copyComponentIdDistributionChunk.SetFirstId(-1);
        copyComponentIdDistributionChunk.SetInterval(-1);
    }

    public override void UnInit()
    {
        entityOperation.UnInit();
        componentOperation.UnInit();
    }

    public BaseEntity CreateEntity(ECSDefine.EntityType entityType, int entityId)
    {
        OperationObject operationObject = entityOperation.CreateOperationObject((int)entityType, entityId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateEntity Fail. componentType:{Enum.GetName(typeof(ECSDefine.EntityType), entityType)}");

            return null;
        }

        BaseEntity entity = operationObject as BaseEntity;

        entity.SetGlobalUnionId(GlobalUnionId);

        entity.SetEntityId(entityId);
        entity.SetEntityType(entityType);

        return entity;
    }

    public BaseEntity GetEntity(int entityId)
    {
        OperationObject operationObject = entityOperation.GetOperationObject(entityId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] GetEntity Fail.No record. entityId:{entityId}");
            return null;
        }
        BaseEntity entity = operationObject as BaseEntity;
        return entity;
    }

    public void DeleteEntity(BaseEntity entity)
    {
        ECSDefine.EntityType entityType = entity.GetEntityType();
        entityOperation.DeleteOperationObject((int)entityType, entity);
    }

    public BaseComponent CreateComponent(int entityId, ECSDefine.ComponentType componentType, int componentId)
    {
        OperationObject operationObject = componentOperation.CreateOperationObject((int)componentType, componentId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateComponent Fail. componentType:{Enum.GetName(typeof(ECSDefine.ComponentType), componentType)}");
            return null;
        }

        BaseComponent component = operationObject as BaseComponent;

        component.SetGlobalUnionId(GlobalUnionId);

        component.SetEntityId(entityId);
        component.SetComponentId(componentId);
        component.SetComponentType(componentType);
        component.FillInExpandData();

        return component;
    }

    public BaseComponent GetComponent(int componentId)
    {
        OperationObject operationObject = componentOperation.GetOperationObject(componentId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] GetComponent Fail.No record. componentId:{componentId}");
            return null;
        }
        BaseComponent component = operationObject as BaseComponent;

        return component;
    }

    public void DeleteComponent(BaseComponent component)
    {
        ECSDefine.ComponentType componentType = component.GetComponentType();
        componentOperation.DeleteOperationObject((int)componentType, component);
    }

    public BaseComponent CopyComponent(BaseComponent resComponent)
    {
        if(resComponent == null)
        {
            return null;
        }

        int entityId = resComponent.GetEntityId();
        int componentId = resComponent.GetComponentId();
        ECSDefine.ComponentType componentType = resComponent.GetComponentType();
        BaseComponent.ComponentExpandData componentExpandData = resComponent.GetComponentExpandData();

        int newComponentId = copyComponentIdDistributionChunk.Pop();

        OperationObject operationObject = componentOperation.CreateOperationObject((int)componentType, newComponentId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CopyComponent Fail. componentType:{Enum.GetName(typeof(ECSDefine.ComponentType), componentType)}");
            return null;
        }

        BaseComponent newComponent = operationObject as BaseComponent;
        newComponent.SetGlobalUnionId(GlobalUnionId);

        newComponent.SetEntityId(entityId);
        newComponent.SetComponentId(componentId);
        newComponent.SetComponentType(componentType);
        newComponent.SetComponentExpandData(componentExpandData.Copy());

        return newComponent;
    }

}
