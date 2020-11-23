using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSUnit: BaseUnit
{
    private Operation entityOperation;
    private Operation componentOperation;

    public override void Init()
    {
        entityOperation = new Operation();
        entityOperation.Init();
        entityOperation.SetName("Entity");

        componentOperation = new Operation();
        componentOperation.Init();
        componentOperation.SetName("Component");
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
        OperationObject operationObject = componentOperation.CreateOperationObject((int)componentType, entityId);
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
        component.FillIn();

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

}
