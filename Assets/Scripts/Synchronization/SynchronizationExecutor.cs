using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationExecutor : BaseObject
{
    private readonly ECSDefine.SystemFunctionType GameUnitSystemFunctionType = ECSDefine.SystemFunctionType.Logic;

    private ECSDefine.SystemFunctionType[] systemFunctionTypePriorityArray;
    private Dictionary<ECSDefine.SystemFunctionType, List<ECSDefine.SystemType>> function2SystemTypeDict;

    private ExecuteSystemUnit executeSystemUnit;
    private CommandUnit commandUnit;


    public override void Init()
    {
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
        systemFunctionTypePriorityArray = null;
        function2SystemTypeDict.Clear();

        commandUnit = null;
        executeSystemUnit = null;

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

    public void UpdateSystemExecute()
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

}
