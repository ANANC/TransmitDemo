using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTerminalExecuteSystemController : IExecuteSystemController
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
        return component;
    }
}