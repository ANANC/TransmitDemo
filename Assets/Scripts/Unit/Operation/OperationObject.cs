using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OperationObject 
{
    public enum OperationObjectType { };

    public abstract void Init();
    public abstract void UnInit();
}
