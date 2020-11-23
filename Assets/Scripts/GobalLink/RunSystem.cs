using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSystem
{
    private static RunSystem runSystem;
    public static RunSystem System
    {
        get
        {
            if (runSystem == null)
            {
                runSystem = new RunSystem();
                runSystem.Init();
            }
            return runSystem;
        }
    }

    private Dictionary<int, GlobalUnion> globalUnionDict;
    private PollingOperation globalUnionPollingOperation;

    private IdDistributionChunk idDistributionChunk;

    public void Init()
    {
        globalUnionDict = new Dictionary<int, GlobalUnion>();

        InitPollingOperation();
        InitGlobalIdDistributionChunk();
    }

    private void InitPollingOperation()
    {
        globalUnionPollingOperation = new PollingOperation();
        globalUnionPollingOperation.Init();
        globalUnionPollingOperation.SetName("RunSystem");
    }

    private void InitGlobalIdDistributionChunk()
    {
        idDistributionChunk = new IdDistributionChunk();
        idDistributionChunk.Init();
        idDistributionChunk.SetFirstId((int)GlobalDefine.GlobalUnionType.Global);
    }

    public void UnInit()
    {
        idDistributionChunk.UnInit();

        globalUnionPollingOperation.UnInit();
    }

    public GlobalUnion CreateGlobalUnion()
    {
        int globalId = idDistributionChunk.Pop();
        OperationObject operationObject = globalUnionPollingOperation.AddOperationObject(globalId, (int)GlobalDefine.GlobalUnionType.Global);
        if (operationObject != null)
        {
            GlobalUnion globalUnion = operationObject as GlobalUnion;
            InitGlobalUnion(globalUnion, globalId);

            globalUnionDict.Add(globalId, globalUnion);
            return globalUnion;
        }
        return null;
    }

    private void InitGlobalUnion(GlobalUnion globalUnion, int globalId)
    {
        globalUnion.SetGlobalUnionId(globalId);
    }

    public GlobalUnion GetGlobalUnion(int globalUnionId)
    {
        GlobalUnion globalUnion;
        if(globalUnionDict.TryGetValue(globalUnionId,out globalUnion))
        {
            return globalUnion;
        }
        else
        {
            Debug.LogError($"[GlobalUnion] GetGlobalUnion Fail. globalUnionId[{globalUnionId}] no record");
            return null;
        }
    }


    public void Update()
    {
        globalUnionPollingOperation.Update();
    }

}
