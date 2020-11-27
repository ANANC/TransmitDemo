using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalExecuteSystemController : IExecuteSystemController
{
    private GlobalUnion globalUnion;

    private ECSUnit ecsUnit;
    private SynchronizationUnit synchronizationUnit;

    public void Init()
    {
        synchronizationUnit = globalUnion.GetUnit<SynchronizationUnit>();
        ecsUnit = globalUnion.GetUnit<ECSUnit>();
    }

    public void UnInit()
    {
        synchronizationUnit = null;
        ecsUnit = null;
    }


    public void SetGlobalUnion(GlobalUnion globalUnion)
    {
        this.globalUnion = globalUnion;
    }

    public BaseComponent RequireComponentControl(BaseComponent component)
    {
        ECSDefine.ComponentType componentType = component.GetComponentType();
        if (synchronizationUnit.IsHingeComponentType(componentType))
        {
            synchronizationUnit.RequireSynchroValueRep(component.GetEntityId(),component.GetComponentId(), component.GetComponentType());
            component = ecsUnit.CopyComponent(component);
        }

        return component;
    }
}
