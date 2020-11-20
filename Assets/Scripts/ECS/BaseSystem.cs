using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : OperationObject
{
    public class ComponentInfo
    {
        public int ComponentId;
        public ECSDefine.ComponentType ComponentType;
        public BaseComponent Component;
    }

    public class SystemExpandData { }

    private int systemId;
    private List<ComponentInfo> componentInfoList;
    private ECSDefine.SystemPriority systemPriority;
    private ECSDefine.SystemType systemType;
    private SystemExpandData systemExpandData;

    public override void Init()
    {
        componentInfoList = new List<ComponentInfo>();
        systemExpandData = null;
    }

    public override void UnInit()
    {
        componentInfoList.Clear();
        systemExpandData = null;
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

    public void AddNeedComponentInfo(ECSDefine.ComponentType componentType,int componentId = -1)
    {
        ComponentInfo componentInfo = new ComponentInfo();
        componentInfo.ComponentId = componentId;
        componentInfo.ComponentType = componentType;

        componentInfoList.Add(componentInfo);
    }

    public void SetSystemType(ECSDefine.SystemType systemType)
    {
        this.systemType = systemType;
    }

    public ECSDefine.SystemType GetSystemType()
    {
        return systemType;
    }

    public void SetSystemPriority(ECSDefine.SystemPriority  priority)
    {
        systemPriority = priority;
    }

    public ECSDefine.SystemPriority GetSystemPriority()
    {
        return systemPriority;
    }

    public void Execute(List<BaseComponent> componentList,SystemExpandData systemExpandData)
    {
        this.systemExpandData = systemExpandData;

        for(int index = 0;index< componentList.Count;index++)
        {
            BaseComponent paramComponent = componentList[index];

            ComponentInfo componentInfo = new ComponentInfo();
            componentInfo.ComponentId = paramComponent.GetComponentId();
            componentInfo.ComponentType = paramComponent.GetComponentType();
            componentInfo.Component = paramComponent;
            componentInfoList.Add(componentInfo);
        }

        SystemExecute();
    }

    public abstract void SystemExecute();
}
