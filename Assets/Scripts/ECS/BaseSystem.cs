using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : BaseObject
{
    public class ComponentInfo : BaseObject
    {
        public int ComponentId;
        public ECSDefine.ComponentType ComponentType;

        public override void Init()
        {
            ComponentId = -1;
            ComponentType = ECSDefine.ComponentType.Base;
        }
        public override void UnInit() { Init(); }
    }

    public class SystemExpandData { }

    private int systemId;
    private ECSDefine.SystemPriority systemPriority;
    private ECSDefine.SystemFunctionType systemFunctionType;
    private ECSDefine.SystemType systemType;

    private int entityId;
    private List<ComponentInfo> componentInfoList;
    private SystemExpandData systemExpandData;

    private ExecuteSystemUnit ExecuteSystemUnit;

    public override void Init()
    {
        componentInfoList = new List<ComponentInfo>();
        systemExpandData = null;

    }

    public override void UnInit()
    {
        for (int index = 0; index < componentInfoList.Count; index++)
        {
            ExecuteSystemUnit.PushSystemComponentInfo(componentInfoList[index]);
        }
        componentInfoList.Clear();
        systemExpandData = null;
        ExecuteSystemUnit = null;
    }

    public void Set(ExecuteSystemUnit executeSystemUnit)
    {
        ExecuteSystemUnit = executeSystemUnit;
    }

    public abstract void FillInComponentInfo();

    public void SetSystemId(int systemId)
    {
        this.systemId = systemId;
    }

    public int GetSystemId()
    {
        return systemId;
    }

    public void AddNeedComponentInfo(ECSDefine.ComponentType componentType, int componentId = -1)
    {
        ComponentInfo componentInfo = ExecuteSystemUnit.PopSystemComponentInfo();
        if (componentInfo != null)
        {
            componentInfo.ComponentId = componentId;
            componentInfo.ComponentType = componentType;

            componentInfoList.Add(componentInfo);
        }
    }

    public List<ComponentInfo> GetComponentInfoList()
    {
        return componentInfoList;
    }

    public void SetSystemType(ECSDefine.SystemType systemType)
    {
        this.systemType = systemType;
    }

    public ECSDefine.SystemType GetSystemType()
    {
        return systemType;
    }
    public void SetSystemFunctionType(ECSDefine.SystemFunctionType systemFunctionType)
    {
        this.systemFunctionType = systemFunctionType;
    }

    public ECSDefine.SystemFunctionType GetSystemFunctionType()
    {
        return systemFunctionType;
    }


    public void SetSystemPriority(ECSDefine.SystemPriority priority)
    {
        systemPriority = priority;
    }

    public ECSDefine.SystemPriority GetSystemPriority()
    {
        return systemPriority;
    }

    public void Execute(int entityId, SystemExpandData systemExpandData)
    {
        this.entityId = entityId;
        this.systemExpandData = systemExpandData;

        __Execute();
    }

    public void FillInExecute(int entityId)
    {
        this.entityId = entityId;
        this.systemExpandData = null;
    }

    public virtual void FillInSystemExpandData(GlobalUnion globalUnion) { }

    public override void Update()
    {
        if (!__Execute())
        {
            Debug.LogError($"[System] componentdata No enable. Unreg. entityId:{entityId} systemType:{Enum.GetName(typeof(ECSDefine.SystemType), systemType)}");
            ExecuteSystemUnit.UnRegSystem(entityId, systemType);
        }
    }

    private bool __Execute()
    {
        List<BaseComponent> componentList = ExecuteSystemUnit.RequireComponentList(entityId, componentInfoList);
        bool isExecute = false;
        if (componentList == null)
        {
            if (componentInfoList.Count == 0)
            {
                isExecute = true;
            }
        }
        else
        {
            if (componentInfoList.Count == componentList.Count)
            {
                isExecute = true;
            }
        }

        if (isExecute)
        {
            SystemExecute(componentList);
        }

        return isExecute;
    }


    public abstract void SystemExecute(List<BaseComponent> componentList);
}
