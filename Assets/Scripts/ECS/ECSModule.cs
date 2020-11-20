using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSModule
{
    private Operation entityOperation;
    private Operation componentOperation;
    private Operation systemOperation;

    private void Init()
    {
        entityOperation = new Operation();
        entityOperation.Init();
        entityOperation.SetName("Entity");

        componentOperation = new Operation();
        componentOperation.Init();
        componentOperation.SetName("Component");

        systemOperation = new Operation();
        systemOperation.Init();
        systemOperation.SetName("System");

    }

    private void UnInit()
    {
        entityOperation.UnInit();
        componentOperation.UnInit();
        systemOperation.UnInit();
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

    public BaseSystem CreateSystem(ECSDefine.SystemType systemType, int systemId)
    {
        OperationObject operationObject = systemOperation.CreateOperationObject((int)systemType, systemId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateSystem Fail. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return null;
        }

        BaseSystem system = operationObject as BaseSystem;

        system.SetSystemId(systemId);
        system.FillInComponentInfo();

        return system;
    }

    public BaseSystem GetSystem(int systemId)
    {
        OperationObject operationObject = systemOperation.GetOperationObject(systemId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] GetSystem Fail.No record. systemId:{systemId}");
            return null;
        }
        BaseSystem system = operationObject as BaseSystem;

        return system;
    }

    public void DeleteSystem(BaseSystem system)
    {
        ECSDefine.SystemType systemType = system.GetSystemType();
        systemOperation.DeleteOperationObject((int)systemType, system);
    }

}
