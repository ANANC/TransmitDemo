using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : BaseObject
{
    public class ComponentInfo: BaseObject
    {
        public int ComponentId;
        public ECSDefine.ComponentType ComponentType;
        public BaseComponent Component;

        public override void Init() {
            ComponentId = -1;
            ComponentType = ECSDefine.ComponentType.Base;
            Component = null;
        }
        public override void UnInit() { Init(); }
    }

    public class SystemExpandData { }

    private int systemId;
    private ECSDefine.SystemPriority systemPriority;
    private ECSDefine.SystemType systemType;

    private int entityId;
    private List<ComponentInfo> componentInfoList;
    private SystemExpandData systemExpandData;

    private ExecuteSystemUnit ExecuteSystemUnit;
    public override void Init()
    {
        componentInfoList = new List<ComponentInfo>();
        systemExpandData = null;

        ExecuteSystemUnit = GlobalUnion.GetUnit<ExecuteSystemUnit>();
    }

    public override void UnInit()
    {
        for(int index = 0;index< componentInfoList.Count;index++)
        {
            ExecuteSystemUnit.PushSystemComponentInfo(componentInfoList[index]);
        }
        componentInfoList.Clear();
        systemExpandData = null;
        ExecuteSystemUnit = null;
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

    public void SetSystemPriority(ECSDefine.SystemPriority  priority)
    {
        systemPriority = priority;
    }

    public ECSDefine.SystemPriority GetSystemPriority()
    {
        return systemPriority;
    }

    public void Execute(int entityId,List<ComponentInfo> componentInfoList,SystemExpandData systemExpandData)
    {
        this.entityId = entityId;
        this.componentInfoList = componentInfoList;
        this.systemExpandData = systemExpandData;

        SystemExecute();
    }

    public abstract void SystemExecute();
}
