using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSynchroValueRspSystem : BaseObject
{
    public override void Init()
    {
    }

    public override void UnInit()
    {

    }

    public abstract void Execute(SynchroValueRsp.SynchroValueRspStructure synchroValueRspStructure);
}
