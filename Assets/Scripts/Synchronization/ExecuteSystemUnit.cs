using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExecuteSystemUnit : BaseUnit
{
    private IdDistributionChunk systemIdDistributionChunk;
    private Operation systemOperation;
    private Dictionary<ECSDefine.SystemFunctionType, PollingOperation> functionSystemOperationDict;

    private Dictionary<int, Dictionary<ECSDefine.SystemType, BaseSystem>> entityRegFunctionSystemDict;

    private IdDistributionChunk systemComponentInfoIdDistributionChunk;
    private Operation systemComponentInfoOperation;

    private ECSUnit ECSUnit;

    public override void Init()
    {
        systemOperation = new Operation();
        systemOperation.Init();
        systemOperation.SetName("System");

        functionSystemOperationDict = new Dictionary<ECSDefine.SystemFunctionType, PollingOperation>();
        entityRegFunctionSystemDict = new Dictionary<int, Dictionary<ECSDefine.SystemType, BaseSystem>>();

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
        systemOperation.UnInit();
        systemComponentInfoOperation.UnInit();

        systemIdDistributionChunk.UnInit();
        systemComponentInfoIdDistributionChunk.UnInit();

        functionSystemOperationDict.Clear();

        ECSUnit = null;
    }

    public void UpdateFunctionSystemsByFunctionTyep(ECSDefine.SystemFunctionType systemFunctionType,ECSDefine.SystemType systemType)
    {
        PollingOperation functionSystems;
        if(!functionSystemOperationDict.TryGetValue(systemFunctionType,out functionSystems))
        {
            functionSystems.UpdateUpdateOperationObjectList();
            functionSystems.UpdatePollingOperationObjectByOperationObjectType((int)systemType);
        }
    }

    private int PollingFunctionSystemOperationSort(int leftSystem,int rightSystem)
    {
        return -1;
    }

    public void RegSystem(int entityId, ECSDefine.SystemType systemType)
    {
        BaseEntity entity = ECSUnit.GetEntity(entityId);
        if (entity == null)
        {
            Debug.LogError($"[ECSModule] RegSystem Fail. Entity Is nil. entityId:{entityId}");
            return;
        }

        int systemId = systemIdDistributionChunk.Pop();
        BaseSystem system = CreateFunctionSystem(systemType, systemId);
        if (system == null)
        {
            return;
        }

        bool success = true;

        do
        {
            List<BaseSystem.ComponentInfo> componentInfos;
            bool fillInSuccess = FillInComponentInfo(system, entity, out componentInfos);

            if (!fillInSuccess)
            {
                success = false;
                break;
            }

            system.FillInExecute(entityId, componentInfos);
        }
        while (false);

        if(!success)
        {
            DeleteFunctionSystem(systemType,system);
        }
    }

    private void UnRegSystem(int entityId, ECSDefine.SystemType systemType)
    {
        BaseEntity entity = ECSUnit.GetEntity(entityId);
        if (entity == null)
        {
            Debug.LogError($"[ECSModule] UnRegSystem Fail. Entity Is nil. entityId:{entityId}");
            return;
        }

        Dictionary<ECSDefine.SystemType, BaseSystem> regFunctionSystemDict;
        if (!entityRegFunctionSystemDict.TryGetValue(entityId, out regFunctionSystemDict))
        {
            return;
        }

        BaseSystem functionSystem;
        if (!regFunctionSystemDict.TryGetValue(systemType,out functionSystem))
        {
            return;
        }

        DeleteFunctionSystem(systemType, functionSystem);
    }

    private BaseSystem CreateFunctionSystem(ECSDefine.SystemType systemType, int systemId)
    {
        ECSDefine.SystemFunctionType systemFunction;
        if (!ECSInstanceDefine.SystemType2Function.TryGetValue(systemType, out systemFunction))
        {
            Debug.LogError($"[ECSModule] CreateFunctionSystem Fail. No SystemFunctionType Reocrd. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return null;
        }

        PollingOperation functionSystemOperation;
        if (!functionSystemOperationDict.TryGetValue(systemFunction, out functionSystemOperation))
        {
            functionSystemOperation = new PollingOperation();

            functionSystemOperation.Init();
            functionSystemOperation.SetName(Enum.GetName(typeof(ECSDefine.SystemFunctionType), systemFunction) + "System");

            functionSystemOperationDict.Add(systemFunction, functionSystemOperation);
        }

        OperationObject operationObject = functionSystemOperation.AddOperationObject((int)systemType, systemId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateSystem Fail. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return null;
        }

        return InitSystem(operationObject, systemType, systemId);
    }

    private void DeleteFunctionSystem(ECSDefine.SystemType systemType,BaseSystem system)
    {
        ECSDefine.SystemFunctionType systemFunction;
        if (!ECSInstanceDefine.SystemType2Function.TryGetValue(systemType, out systemFunction))
        {
            Debug.LogError($"[ECSModule] CreateFunctionSystem Fail. No SystemFunctionType Reocrd. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return;
        }

        PollingOperation functionSystemOperation;
        if (!functionSystemOperationDict.TryGetValue(systemFunction, out functionSystemOperation))
        {
            functionSystemOperation = new PollingOperation();

            functionSystemOperation.Init();
            functionSystemOperation.SetName(Enum.GetName(typeof(ECSDefine.SystemFunctionType), systemFunction) + "System");

            functionSystemOperationDict.Add(systemFunction, functionSystemOperation);
        }

        functionSystemOperation.RemoveOperationObject((int)systemType, system);
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
        BaseSystem system = CreateImmediatelyExecuteSystem(systemType, systemId);
        if (system == null)
        {
            return;
        }

        do
        {
            if (system.GetSystemFunctionType() != ECSDefine.SystemFunctionType.Logic)
            {
                Debug.LogError($"[ECSModule] ExecuteSystem Fail. Only Can ImmediatelyExecute Logic Type. entityId:{entityId} systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
                break;
            }

            List<BaseSystem.ComponentInfo> componentInfos;
            bool fillInSuccess = FillInComponentInfo(system, entity,out componentInfos);

            if (!fillInSuccess)
            {
                break;
            }

            system.SetGlobalUnionId(GlobalUnionId);
            system.Execute(entityId, componentInfos, expandData);
        }
        while (false);

        DeleteImmediatelyExecuteSystem(system);//执行完毕回收
    }


    private BaseSystem CreateImmediatelyExecuteSystem(ECSDefine.SystemType systemType, int systemId)
    {
        OperationObject operationObject = systemOperation.CreateOperationObject((int)systemType, systemId);
        if (operationObject == null)
        {
            Debug.LogError($"[ECSModule] CreateSystem Fail. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            return null;
        }

        return InitSystem(operationObject, systemType, systemId);
    }

    private void DeleteImmediatelyExecuteSystem(BaseSystem system)
    {
        ECSDefine.SystemType systemType = system.GetSystemType();
        system.SetGlobalUnionId(0);
        systemOperation.DeleteOperationObject((int)systemType, system);
    }

    private BaseSystem InitSystem(OperationObject operationObject, ECSDefine.SystemType systemType, int systemId)
    {
        BaseSystem system = operationObject as BaseSystem;

        system.SetSystemId(systemId);
        system.SetSystemType(systemType);

        ECSDefine.SystemPriority systemPriority;
        if (!ECSInstanceDefine.SystemType2Priority.TryGetValue(systemType, out systemPriority))
        {
            Debug.LogError($"[ECSModule] GetSystemType2Priority Fail. No Reocrd. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            systemPriority = ECSDefine.SystemPriority.Normal;
        }
        system.SetSystemPriority(systemPriority);

        ECSDefine.SystemFunctionType systemFunctionType;
        if (!ECSInstanceDefine.SystemType2Function.TryGetValue(systemType, out systemFunctionType))
        {
            Debug.LogError($"[ECSModule] SystemType2Function Fail. No Reocrd. systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            systemFunctionType = ECSDefine.SystemFunctionType.Logic;
        }
        system.SetSystemFunctionType(systemFunctionType);

        system.FillInComponentInfo();

        return system;
    }

    private bool FillInComponentInfo(BaseSystem system,BaseEntity entity, out List<BaseSystem.ComponentInfo> componentInfos)
    {
        bool fillInSuccess = true;
        componentInfos = system.PopComponentInfoList();
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
                Debug.LogError($"[ECSModule] ExecuteSystem Fail. Component Is nil. {entity.GetEntityId()}  componentId:{componentInfo.ComponentId } componentType:{componentInfo.ComponentType}");
                break;
            }
            else
            {
                componentInfo.Component = component;
                componentInfo.ComponentId = component.GetComponentId();
                componentInfo.ComponentType = component.GetComponentType();
            }
        }
        return fillInSuccess;
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
