using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PollingOperationObject : OperationObject
{

    private bool isUpdate = false;
    public bool IsUpdate { get { return isUpdate; } }

    public abstract void Update();
}
