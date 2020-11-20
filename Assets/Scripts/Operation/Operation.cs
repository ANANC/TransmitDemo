using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation
{
    private string name;

    private Dictionary<int, OperationObject> activeOperationObjectDict;

    private Dictionary<int, List<OperationObject>> operationObjectTypePoolDict;

    public void Init()
    {
        activeOperationObjectDict = new Dictionary<int, OperationObject>();
        operationObjectTypePoolDict = new Dictionary<int, List<OperationObject>>();
    }

    public void UnInit()
    {
        Dictionary<int, OperationObject>.Enumerator enumerator = activeOperationObjectDict.GetEnumerator();
        while(enumerator.MoveNext())
        {
            enumerator.Current.Value.UnInit();
        }

        activeOperationObjectDict.Clear();
        operationObjectTypePoolDict.Clear();
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public OperationObject CreateOperationObject(int operationObjectType, int operationObjectId)
    {
        OperationObject operationObject = PopOperationObject(operationObjectType);
        if (operationObject == null)
        {
            return null;
        }

        activeOperationObjectDict.Add(operationObjectId, operationObject);

        return operationObject;
    }

    public OperationObject GetOperationObject(int operationObjectId)
    {
        OperationObject operationObject;
        if (!activeOperationObjectDict.TryGetValue(operationObjectId, out operationObject))
        {
            Debug.LogError($"[OperationObject] GetOperationObject Fail.No record. operationObjectId:{operationObjectId}");
            return null;
        }
        return operationObject;
    }

    public void DeleteOperationObject(int operationObjectType,OperationObject operationObject)
    {
        PushOperationObject(operationObjectType, operationObject);
    }

    private OperationObject PopOperationObject(int operationObjectType)
    {
        OperationObject operationObject = null;

        List<OperationObject> operationObjecPool;
        if (!operationObjectTypePoolDict.TryGetValue(operationObjectType, out operationObjecPool))
        {
            operationObjecPool = new List<OperationObject>();
            operationObjectTypePoolDict.Add(operationObjectType, operationObjecPool);
        }

        if (operationObjecPool.Count == 0)
        {
            Type classType;
            if (OperationInstanceDefine.OperationObjectType2ClassType.TryGetValue(operationObjectType, out classType))
            {
                object instance = System.Activator.CreateInstance(classType);
                if (instance != null)
                {
                    operationObject = instance as OperationObject;
                }
            }
        }
        else
        {
            operationObject = operationObjecPool[0];
            operationObjecPool.RemoveAt(0);
        }

        if (operationObject != null)
        {
            operationObject.Init();
        }
        else
        {
            Debug.LogError($"[OperationObject] {name} OperationObject Create Fail.No ClassType. operationObjectType:{operationObjectType}");
        }
        return operationObject;
    }

    private void PushOperationObject(int operationObjectType, OperationObject operationObject)
    {
        List<OperationObject> operationObjecPool;
        if (!operationObjectTypePoolDict.TryGetValue(operationObjectType, out operationObjecPool))
        {
            operationObjecPool = new List<OperationObject>();
            operationObjectTypePoolDict.Add(operationObjectType, operationObjecPool);
        }

        operationObject.UnInit();
        operationObjecPool.Add(operationObject);
    }

}
