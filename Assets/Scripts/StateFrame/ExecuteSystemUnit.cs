using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExecuteSystemUnit : BaseUnit
{
    private IdDistributionChunk systemIdDistributionChunk;
    private Operation systemOperation;

    private IdDistributionChunk systemComponentInfoIdDistributionChunk;
    private Operation systemComponentInfoOperation;
    private PollingOperation pollingSystemComponentInfoOperation;

    private ECSUnit ECSUnit;

    public override void Init()
    {
        systemOperation = new Operation();
        systemOperation.Init();
        systemOperation.SetName("System");

        pollingSystemComponentInfoOperation = new PollingOperation();
        pollingSystemComponentInfoOperation.Init();
        pollingSystemComponentInfoOperation.SetName("PollingSystem");
        pollingSystemComponentInfoOperation.SetOperationObjectSortFunc(PollingSystemComponentInfoOperationSort);

        systemComponentInfoOperation = new Operation();
        systemComponentInfoOperation.Init();
        systemComponentInfoOperation.SetName("SystemComponentInfo");

        InitSystemComponentInfoIdDistributionChunk();
        InitSystemIdDistributionChunk();

        ECSUnit = GlobalUnion.GetUnit<ECSUnit>();
    }
    private void InitSystemIdDistributionChunk()
    {
        systemIdDistributionChunk = new IdDistributionChunk();
        systemIdDistributionChunk.Init();
        systemIdDistributionChunk.SetFirstId((int)ECSDefine.SystemType.Base);
    }

    private void InitSystemComponentInfoIdDistributionChunk()
    {
        systemComponentInfoIdDistributionChunk = new IdDistributionChunk();
        systemComponentInfoIdDistributionChunk.Init();
        systemComponentInfoIdDistributionChunk.SetFirstId(1);
    }

    public override void UnInit()
    {
        pollingSystemComponentInfoOperation.UnInit();
        systemOperation.UnInit();
        systemComponentInfoOperation.UnInit();

        systemIdDistributionChunk.UnInit();
        systemComponentInfoIdDistributionChunk.UnInit();

        ECSUnit = null;
    }


    private int PollingSystemComponentInfoOperationSort(int leftSystem,int rightSystem)
    {
        return -1;
    }

    public void RegSystem(int entityId, ECSDefine.SystemType systemType)
    {

    }

    public void ExecuteSystem(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        BaseEntity entity = ECSUnit.GetEntity(entityId);
        if (entity == null)
        {
            Debug.LogError($"[ECSModule] ExecuteSystem Fail. Entity Is nil. entityId:{entityId}");
            return;
        }

        int systemId = systemIdDistributionChunk.Pop();
        BaseSystem system = CreateSystem(systemType, systemId);
        if (system == null)
        {
            return;
        }

        do
        {
            bool fillInSuccess = true;
            List<BaseSystem.ComponentInfo> componentInfos = system.GetComponentInfoList();
            for (int index = 0; index < componentInfos.Count; index++)
            {
                BaseSystem.ComponentInfo componentInfo = componentInfos[index];
                BaseComponent component = null;

                if (componentInfo.ComponentId != -1)
                {
                    component = entity.GetComponentByComponentId(componentInfo.ComponentId);
                }
                else if (componentInfo.ComponentType != ECSDefine.ComponentType.Base)
                {
                    component = entity.GetComponentByComponentType(componentInfo.ComponentType);
                }

                if (component == null)
                {
                    fillInSuccess = false;
                    Debug.LogError($"[ECSModule] ExecuteSystem Fail. Component Is nil. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)} componentId:{componentInfo.ComponentId } componentType:{componentInfo.ComponentType}");
                    break;
                }
                else
                {
                    componentInfo.Component = component;
                    componentInfo.ComponentId = component.GetComponentId();
                    componentInfo.ComponentType = component.GetComponentType();
                }
            }

            if (fillInSuccess)
            {
                system.Execute(entityId, componentInfos, expandData);
            }
        }
        while (false);

        DeleteSystem(system);//执行完毕回收
    }


    private BaseSystem CreateSystem(ECSDefine.SystemType systemType, int systemId)
    {
        OperationObject operationObject = systemOperation.CreateOperationObject((int)systemType, systemId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateSystem Fail. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return null;
        }

        BaseSystem system = operationObject as BaseSystem;

        system.SetGlobalUnionId(GlobalUnionId);

        system.SetSystemId(systemId);
        system.FillInComponentInfo();

        return system;
    }

    private BaseSystem GetSystem(int systemId)
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

    private void DeleteSystem(BaseSystem system)
    {
        ECSDefine.SystemType systemType = system.GetSystemType();
        systemOperation.DeleteOperationObject((int)systemType, system);
    }


    public BaseSystem.ComponentInfo PopSystemComponentInfo()
    {
        int systemComponentInfoId = systemComponentInfoIdDistributionChunk.Pop();
        OperationObject operationObject = systemComponentInfoOperation.CreateOperationObject((int)ECSDefine.SystemType.ComponentInfo, systemComponentInfoId);
        if(operationObject!=null)
        {
            BaseSystem.ComponentInfo componentInfo = (BaseSystem.ComponentInfo)operationObject;
            return componentInfo;
        }
        return null;
    }

    public void PushSystemComponentInfo(BaseSystem.ComponentInfo componentInfo)
    {
        systemComponentInfoOperation.DeleteOperationObject((int)ECSDefine.SystemType.ComponentInfo, componentInfo);
    }
}
