using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollingOperation: Operation
{
    private Dictionary<int, List<PollingOperationObject>> addOperationObjectListDict;
    private Dictionary<int,List<PollingOperationObject>> removeOperationObjectListDict;
    private Dictionary<int, List<PollingOperationObject>> updateOperationObjectListDict;

    private List<int> operationObjectUpdatePriorityList;
    private Dictionary<int, bool> operationObjectUpdatePriorityDict;
    private Comparison<int> pollingoperationObjectTypeSortFunc;

    public new void Init()
    {
        base.Init();

        addOperationObjectListDict = new Dictionary<int, List<PollingOperationObject>>();
        removeOperationObjectListDict = new Dictionary<int, List<PollingOperationObject>>();
        updateOperationObjectListDict = new Dictionary<int, List<PollingOperationObject>>();
        operationObjectUpdatePriorityList = new List<int>();
        operationObjectUpdatePriorityDict = new Dictionary<int, bool>();
    }

    public new void UnInit()
    {
        operationObjectUpdatePriorityList.Clear();
        operationObjectUpdatePriorityDict.Clear();
        addOperationObjectListDict.Clear();
        removeOperationObjectListDict.Clear();
        updateOperationObjectListDict.Clear();

        base.UnInit();
    }

    public void SetOperationObjectSortFunc(Comparison<int> sortFunc)
    {
        pollingoperationObjectTypeSortFunc = sortFunc;
    }

    public void Update()
    {
        UpdateUpdateOperationObjectList();

        for(int index = 0;index< operationObjectUpdatePriorityList.Count;index++)
        {
            int operationObjectType = operationObjectUpdatePriorityList[index];
            UpdatePollingOperationObjectByOperationObjectType(operationObjectType);
        }
    }

    public void UpdateUpdateOperationObjectList()
    {
        bool change = false;

        if(addOperationObjectListDict.Count >0)
        {
            Dictionary<int, List<PollingOperationObject>>.Enumerator enumerator = addOperationObjectListDict.GetEnumerator();
            while(enumerator.MoveNext())
            {
                int operationObjectType = enumerator.Current.Key;
                List<PollingOperationObject> operationObjectList = enumerator.Current.Value;

                if(operationObjectList.Count >0)
                {
                    for(int index = 0;index< operationObjectList.Count;index++)
                    {
                        AddUpdateOperationObject(operationObjectType, operationObjectList[index]);
                        change = true;
                    }
                }
            }
            addOperationObjectListDict.Clear();
        }

        if(removeOperationObjectListDict.Count >0 )
        {
            Dictionary<int, List<PollingOperationObject>>.Enumerator enumerator = removeOperationObjectListDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int operationObjectType = enumerator.Current.Key;
                List<PollingOperationObject> operationObjectList = enumerator.Current.Value;

                if (operationObjectList.Count > 0)
                {
                    for (int index = 0; index < operationObjectList.Count; index++)
                    {
                        RemoveUpdateOperationObject(operationObjectType, operationObjectList[index]);
                        change = true;
                    }
                }
            }
            removeOperationObjectListDict.Clear();

        }

        if(change)
        {
            if(pollingoperationObjectTypeSortFunc!=null)
            {
                operationObjectUpdatePriorityList.Sort(pollingoperationObjectTypeSortFunc);
            }
        }
    }

    public void UpdatePollingOperationObjectByOperationObjectType(int operationObjectType)
    {
        List<PollingOperationObject> operationObjectList;
        if (updateOperationObjectListDict.TryGetValue(operationObjectType, out operationObjectList))
        {
            for (int operationObjectIndex = 0; operationObjectIndex < operationObjectList.Count; operationObjectIndex++)
            {
                PollingOperationObject pollingOperationObject = operationObjectList[operationObjectIndex];
                pollingOperationObject?.Update();
            }
        }
    }

    public OperationObject AddOperationObject(int operationObjectType, int operationObjectId)
    {
        OperationObject operationObject = CreateOperationObject(operationObjectType, operationObjectId);
        if (operationObject != null && operationObject is PollingOperationObject)
        {
            PollingOperationObject pollingOperationObject = operationObject as PollingOperationObject;
            if (pollingOperationObject.IsUpdate)
            {
                List<PollingOperationObject> operationObjectList;
                if (!addOperationObjectListDict.TryGetValue(operationObjectType, out operationObjectList))
                {
                    operationObjectList = new List<PollingOperationObject>();
                    addOperationObjectListDict.Add(operationObjectType, operationObjectList);
                }
                operationObjectList.Add(pollingOperationObject);
            }
            return operationObject;
        }
        else
        {
            Debug.LogError($"[PollingOperation] AddOperationObject Fail. Type[{operationObjectType}] is Not PollingOperationObject.");
        }
        return null;
    }


    public void RemoveOperationObject(int operationObjectType, PollingOperationObject operationObject)
    {
        List<PollingOperationObject> operationObjectList;
        if (!removeOperationObjectListDict.TryGetValue(operationObjectType, out operationObjectList))
        {
            operationObjectList = new List<PollingOperationObject>();
            removeOperationObjectListDict.Add(operationObjectType, operationObjectList);
        }
        operationObjectList.Add(operationObject);
    }


    private void AddUpdateOperationObject(int operationObjectType, PollingOperationObject operationObject)
    {
        List<PollingOperationObject> operationObjectList;
        if (!updateOperationObjectListDict.TryGetValue(operationObjectType, out operationObjectList))
        {
            operationObjectList = new List<PollingOperationObject>();
            removeOperationObjectListDict.Add(operationObjectType, operationObjectList);
        }
        operationObjectList.Add(operationObject);

        bool active;
        if(!operationObjectUpdatePriorityDict.TryGetValue(operationObjectType,out active))
        {
            operationObjectUpdatePriorityDict.Add(operationObjectType, true);
            active = false;
        }
        if(active)
        {
            operationObjectUpdatePriorityDict[operationObjectType] = true;
            operationObjectUpdatePriorityList.Add(operationObjectType);
        }
    }

    private void RemoveUpdateOperationObject(int operationObjectType, PollingOperationObject operationObject)
    {
        List<PollingOperationObject> operationObjectList;
        if (updateOperationObjectListDict.TryGetValue(operationObjectType, out operationObjectList))
        {
            if (operationObjectList.Remove(operationObject) && operationObjectList.Count == 0)
            {
                bool active;
                if (operationObjectUpdatePriorityDict.TryGetValue(operationObjectType,out active))
                {
                    if(active == true)
                    {
                        operationObjectUpdatePriorityDict[operationObjectType] = false;
                        operationObjectUpdatePriorityList.Remove(operationObjectType);
                    }
                }
            }
        }
    }
}
