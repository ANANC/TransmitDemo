
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUnion : PollingOperationObject
{
    private int globalUnionId;

    private Dictionary<Type, int> unitTypeDict;
    private Dictionary<Type, BaseUnit> unitDict;
    private PollingOperation globalUnionPollingOperation;

    private IdDistributionChunk idDistributionChunk;

    private GlobalInstanceDefine globalInstanceDefine;
    public GlobalInstanceDefine Define { get { return globalInstanceDefine; } }

    public void SetGlobalUnionId(int globalUnionId)
    {
        this.globalUnionId = globalUnionId;
    }

    public override void Init()
    {
        unitTypeDict = new Dictionary<Type, int>();
        unitDict = new Dictionary<Type, BaseUnit>();
        globalInstanceDefine = new GlobalInstanceDefine();

        InitPollingOperation();
        InitGlobalIdDistributionChunk();

    }

    private void InitPollingOperation()
    {
        globalUnionPollingOperation = new PollingOperation();
        globalUnionPollingOperation.Init();
        globalUnionPollingOperation.SetName("GlobalUnion");
        globalUnionPollingOperation.SetOperationObjectSortFunc(UnitSort);
    }

    private void InitGlobalIdDistributionChunk()
    {
        idDistributionChunk = new IdDistributionChunk();
        idDistributionChunk.Init();
        idDistributionChunk.SetFirstId((int)GlobalDefine.UnitType.Unit);
    }

    public override void UnInit()
    {
        idDistributionChunk.UnInit();
        globalUnionPollingOperation.UnInit();
        unitDict.Clear();
        unitTypeDict.Clear();
    }

    public int UnitSort(int leftUnit, int rightUnit)
    {
        return 1;
    }


    public override void Update()
    {
        globalUnionPollingOperation.Update();
    }

    public void AddUnit<T>(int unitTypeId) where T : BaseUnit
    {
        Type unitType = typeof(T);
        if (unitDict.ContainsKey(unitType))
        {
            Debug.LogError($"[GlobalUnion] AddUnit [{unitType.Name}] Fail. Has Record");
            return;
        }

        int unitId = idDistributionChunk.Pop();
        OperationObject operationObject = globalUnionPollingOperation.AddOperationObject(unitId, unitTypeId);
        if(operationObject!=null)
        {
            BaseUnit unit = operationObject as BaseUnit;
            unitDict.Add(unitType, unit);

            InitUnit(unit);
        }
        if(!unitTypeDict.ContainsKey(unitType))
        {
            unitTypeDict.Add(unitType, unitTypeId);
        }
    }

    private void InitUnit(BaseUnit unit)
    {
        unit.SetGlobalUnionId(globalUnionId);
    }

    public T GetUnit<T>() where T : BaseUnit
    {
        Type unitType = typeof(T);
        BaseUnit unit;
        if (unitDict.TryGetValue(unitType, out unit))
        {
            return (T)unit;
        }
        else
        {
            Debug.LogError($"[GlobalUnion] GetUnit [{unitType.Name}] Fail. No Record");
            return null;
        }
    }

    public void RemoveUnit<T>() where T : BaseUnit
    {
        Type unitType = typeof(T);
        BaseUnit unit;
        if (unitDict.TryGetValue(unitType, out unit))
        {
            int unitTypeId = unitTypeDict[unitType];
            globalUnionPollingOperation.RemoveOperationObject(unitTypeId, unit);
            unitDict.Remove(unitType);
        }
    }


}
