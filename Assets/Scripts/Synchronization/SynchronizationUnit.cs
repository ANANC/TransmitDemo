using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationUnit : BaseUnit
{
    private readonly ECSDefine.SystemFunctionType GameUnitSystemFunctionType = ECSDefine.SystemFunctionType.Logic;

    private ECSDefine.SystemFunctionType[] systemFunctionTypePriorityArray;
    private Dictionary<ECSDefine.SystemFunctionType, List<ECSDefine.SystemType>> function2SystemTypeDict;

    private ExecuteSystemUnit executeSystemUnit;
    private CommandUnit commandUnit;

    private Operation SynchroValueRepOperation;
    private Operation SynchroValueRspOperation;
    private IdDistributionChunk synchroValueIdDistributionChunk;

    private List<SynchroValueRep> synchroValueRepList;
    private List<SynchroValueRsp> synchroValueRspList;

    private List<SynchroValueRsp> sendSynchroValueRspList;

    private ECSUnit _ecsUnit;
    private ECSUnit ecsUnit
    {
        get
        {
            if(_ecsUnit == null)
            {
                _ecsUnit = GlobalUnion.GetUnit<ECSUnit>();
            }
            return _ecsUnit;
        }
    }

    public override void Init()
    {
        SynchroValueRepOperation = new Operation();
        SynchroValueRepOperation.Init();
        SynchroValueRepOperation.SetName("SynchroValueRep");

        SynchroValueRspOperation = new Operation();
        SynchroValueRspOperation.Init();
        SynchroValueRspOperation.SetName("SynchroValueRsp");

        synchroValueIdDistributionChunk = new IdDistributionChunk();
        synchroValueIdDistributionChunk.Init();
        synchroValueIdDistributionChunk.SetInterval(2);

        InitSystemFunctionTypePriority();
        InitFunction2SystemTypeDict();

        executeSystemUnit = GlobalUnion.GetUnit<ExecuteSystemUnit>();
        commandUnit = GlobalUnion.GetUnit<CommandUnit>();
    }

    private void InitSystemFunctionTypePriority()
    {
        Array systemFunctionTypeArray = Enum.GetValues(typeof(ECSDefine.SystemFunctionType));
        systemFunctionTypePriorityArray = new ECSDefine.SystemFunctionType[systemFunctionTypeArray.Length];
        IEnumerator enumerator = systemFunctionTypeArray.GetEnumerator();
        int index = 0;
        while (enumerator.MoveNext())
        {
            systemFunctionTypePriorityArray[index++] = (ECSDefine.SystemFunctionType)enumerator.Current;
        }
    }

    private void InitFunction2SystemTypeDict()
    {
        function2SystemTypeDict = new Dictionary<ECSDefine.SystemFunctionType, List<ECSDefine.SystemType>>();
        Dictionary<ECSDefine.SystemType, ECSDefine.SystemFunctionType>.Enumerator enumerator = ECSInstanceDefine.SystemType2Function.GetEnumerator();
        while (enumerator.MoveNext())
        {
            ECSDefine.SystemFunctionType systemFunctionType = enumerator.Current.Value;
            ECSDefine.SystemType systemType = enumerator.Current.Key;

            List<ECSDefine.SystemType> systemTypeList;
            if (!function2SystemTypeDict.TryGetValue(systemFunctionType, out systemTypeList))
            {
                systemTypeList = new List<ECSDefine.SystemType>();
                function2SystemTypeDict.Add(systemFunctionType, systemTypeList);
            }
            systemTypeList.Add(systemType);
        }

        Dictionary<ECSDefine.SystemFunctionType, List<ECSDefine.SystemType>>.Enumerator systemFunctionTypeEnumerator = function2SystemTypeDict.GetEnumerator();
        while (systemFunctionTypeEnumerator.MoveNext())
        {
            List<ECSDefine.SystemType> systemTypeList = systemFunctionTypeEnumerator.Current.Value;
            systemTypeList.Sort(SystemFunctionTypeSystemSotr);
        }
    }

    public override void UnInit()
    {
        SynchroValueRepOperation.UnInit();
        SynchroValueRspOperation.UnInit();

        systemFunctionTypePriorityArray = null;
        function2SystemTypeDict.Clear();

        commandUnit = null;
        executeSystemUnit = null;

        _ecsUnit = null;
    }

    public override void Update()
    {

    }

    private int SystemFunctionTypeSystemSotr(ECSDefine.SystemType leftSystemType, ECSDefine.SystemType rightSystemType)
    {
        ECSDefine.SystemPriority leftSystemPriority;
        if (!ECSInstanceDefine.SystemType2Priority.TryGetValue(leftSystemType, out leftSystemPriority))
        {
            leftSystemPriority = ECSDefine.SystemPriority.Normal;
        }

        ECSDefine.SystemPriority rightSystemPriority;
        if (!ECSInstanceDefine.SystemType2Priority.TryGetValue(rightSystemType, out rightSystemPriority))
        {
            rightSystemPriority = ECSDefine.SystemPriority.Normal;
        }

        return (int)leftSystemPriority.CompareTo((int)rightSystemPriority);
    }

    private void UpdateSystemExecute()
    {
        commandUnit.CacheCommandListToExecuteCommandList();

        //功能系统顺序
        for (int index = 0; index < systemFunctionTypePriorityArray.Length; index++)
        {
            ECSDefine.SystemFunctionType systemFunctionType = systemFunctionTypePriorityArray[index];

            List<ECSDefine.SystemType> systemTypeList;
            if (function2SystemTypeDict.TryGetValue(systemFunctionType, out systemTypeList))
            {
                // 执行系统顺序
                for (int systemIndex = 0; systemIndex < systemTypeList.Count; systemIndex++)
                {
                    ECSDefine.SystemType systemType = systemTypeList[systemIndex];

                    // 执行命令
                    commandUnit.PopSystemTypeCommandList(systemType);

                    // 执行功能系统
                    executeSystemUnit.UpdateFunctionSystemsByFunctionTyep(systemFunctionType, systemType);
                }

                if (GameUnitSystemFunctionType == systemFunctionType)
                {
                    for (int unitIndex = 0; unitIndex < ECSInstanceDefine.GameUnitPriorityList.Count; unitIndex++)
                    {
                        ECSInstanceDefine.GameUnitPriorityList[unitIndex].Update();
                    }
                }
            }
        }
    }

    public bool IsHingeComponentType(ECSDefine.ComponentType componentType)
    {
        return ECSInstanceDefine.RequireComponentType2ExecuteSystem.ContainsKey(componentType);
    }

    public void RequireSynchroValueRep(int entityId,int componentId, ECSDefine.ComponentType componentType)
    {
        int reqId = synchroValueIdDistributionChunk.Pop();
        ECSDefine.SynchronizationValueType synchronizationValueType = ECSDefine.SynchronizationValueType.Require;
        OperationObject operationObject = SynchroValueRepOperation.CreateOperationObject((int)synchronizationValueType, reqId);
        if (operationObject == null)
        {
            Debug.LogError($"[SynchronizationUnit] RequireComponent Fail. SynchroValueRep is nil. synchronizationValueType:{Enum.GetName(typeof(ECSDefine.SynchronizationValueType), synchronizationValueType)}");
            return;
        }

        SynchroValueRep synchroValueRep = operationObject as SynchroValueRep;
        synchroValueRep.SetSynchroValueRepId(reqId);
        synchroValueRep.SetEntityId(entityId);
        synchroValueRep.SetComponentId(componentId);
        synchroValueRep.SetComponentType(componentType);

        synchroValueRepList.Add(synchroValueRep);
    }

    public void ReceiveSynchroValueRep(SynchroValueRep synchroValueRep)
    {
        synchroValueRepList.Add(synchroValueRep);
    }

    public void ExecuteCacheSynchroValueRep()
    {
        for(int index = 0;index< synchroValueRepList.Count;index++)
        {
            SynchroValueRep synchroValueRep = synchroValueRepList[index];
            SynchroValueRsp synchroValueRsp = SynchroValueRepToSynchroValueRsp(synchroValueRep);
            if(synchroValueRsp == null)
            {
                continue;
            }
            sendSynchroValueRspList.Add(synchroValueRsp);

        }
        synchroValueRepList.Clear();
    }

    private SynchroValueRsp SynchroValueRepToSynchroValueRsp(SynchroValueRep synchroValueRep)
    {
        int synchroValueRepId = synchroValueRep.GetSynchroValueRepId();
        int synchroValueRspId = synchroValueRepId + 1;
        OperationObject operationObject = SynchroValueRspOperation.CreateOperationObject((int)ECSDefine.SynchronizationValueType.Response, synchroValueRspId);
        if(operationObject == null)
        {
            Debug.LogError("[SynchronizationUnit] SynchroValueRepToSynchroValueRsp Fail. Create SynchroValueRsp Fail");
            return null;
        }

        int entityId = synchroValueRep.GetEntityId();
        int componentId = synchroValueRep.GetComponentId();
        ECSDefine.ComponentType componentType = synchroValueRep.GetComponentType();

        SynchroValueRsp synchroValueRsp = operationObject as SynchroValueRsp;
        synchroValueRsp.SetSynchroValueRspId(synchroValueRspId);
        synchroValueRsp.SetComponentId(componentId);
        synchroValueRsp.SetComponentType(componentType);
        synchroValueRep.SetEntityId(entityId);

        BaseEntity entity = ecsUnit.GetEntity(entityId);
        if(entity == null)
        {
            synchroValueRsp.SetIsEntityAlive(true);
            BaseComponent component = entity.GetComponentByComponentId(componentId);
            synchroValueRsp.SetIsComponentAlive(component != null);
        }
        else
        {
            synchroValueRsp.SetIsEntityAlive(false);
            synchroValueRsp.SetIsComponentAlive(false);
        }

        return synchroValueRsp;
    }

    public void ReceiveSynchroValueRsp(SynchroValueRsp synchroValueRsp)
    {
        synchroValueRspList.Add(synchroValueRsp);
    }

    public void ExecuteCacheSynchroValueRsp()
    {
        for(int index = 0;index< synchroValueRspList.Count;index++)
        {
            SynchroValueRsp synchroValueRsp = synchroValueRspList[index];

            ECSDefine.ComponentType componentType = synchroValueRsp.GetComponentType();
            ECSDefine.SynchroValueRspSystemType synchroValueRspSystemType;
            if (!ECSInstanceDefine.RequireComponentType2ExecuteSystem.TryGetValue(componentType, out synchroValueRspSystemType))
            {
                Debug.LogError($"[SynchronizationUnit] ExecuteCacheSynchroValueRsp Fail. {Enum.GetName(typeof(ECSDefine.ComponentType), componentType)}");
                continue;
            }
            SynchroValueRsp.SynchroValueRspStructure synchroValueRspStructure = synchroValueRsp.GetSynchroValueRspStructure();
            executeSystemUnit.ExecuteSynchroValueRspSystem(synchroValueRspSystemType, synchroValueRspStructure);
        }
        synchroValueRspList.Clear();
    }
}
