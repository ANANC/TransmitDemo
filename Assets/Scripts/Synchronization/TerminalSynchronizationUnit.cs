using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalSynchronizationUnit : BaseUnit
{
    private SynchronizationExecutor synchronizationExecutor;

    private Operation synchroValueRepOperation;

    private List<SynchroValueRep> sendSynchroValueRepList;
    private List<SynchroValueRsp> receiveSynchroValueRsp;

    private IdDistributionChunk sendIdDistributionChunk;

    private ExecuteSystemUnit executeSystemUnit;

    public override void Init()
    {
        synchronizationExecutor = new SynchronizationExecutor();
        synchronizationExecutor.Init();

        sendIdDistributionChunk = new IdDistributionChunk();
        sendIdDistributionChunk.Init();
        sendIdDistributionChunk.SetInterval(2);

        sendSynchroValueRepList = new List<SynchroValueRep>();
        receiveSynchroValueRsp = new List<SynchroValueRsp>();


        executeSystemUnit = GlobalUnion.GetUnit<ExecuteSystemUnit>();
    }

    public override void UnInit()
    {
        synchronizationExecutor.UnInit();
        synchronizationExecutor = null;

        sendSynchroValueRepList.Clear();
        receiveSynchroValueRsp.Clear();

        executeSystemUnit = null;
    }

    public override void Update()
    {
    }

    public void RequireSynchroValueRep(int entityId, int componentId, ECSDefine.ComponentType componentType)
    {
        int reqId = sendIdDistributionChunk.Pop();
        ECSDefine.SynchronizationValueType synchronizationValueType = ECSDefine.SynchronizationValueType.Require;
        OperationObject operationObject = synchroValueRepOperation.CreateOperationObject((int)synchronizationValueType, reqId);
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

        sendSynchroValueRepList.Add(synchroValueRep);
    }

    public List<SynchroValueRep> PopSendSynchroValueRepList()
    {
        List<SynchroValueRep> synchroValueRepList = sendSynchroValueRepList;
        sendSynchroValueRepList.Clear();
        return synchroValueRepList;
    }

    public void ReceiveSynchroValueRsp(SynchroValueRsp synchroValueRsp)
    {
        receiveSynchroValueRsp.Add(synchroValueRsp);
    }

    public void ExecuteCacheSynchroValueRsp()
    {
        for (int index = 0; index < receiveSynchroValueRsp.Count; index++)
        {
            SynchroValueRsp synchroValueRsp = receiveSynchroValueRsp[index];

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
        receiveSynchroValueRsp.Clear();
    }
}
