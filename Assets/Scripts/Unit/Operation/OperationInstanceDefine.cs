using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationInstanceDefine 
{
    public static Dictionary<int, Type> OperationObjectType2ClassType = new Dictionary<int, Type>()
    {
        //---- EntityType ----
        { (int)ECSDefine.EntityType.Normal, typeof(BaseEntity) },


        //---- ComponentType ----
        { (int)ECSDefine.ComponentType.Base, typeof(BaseComponent) },

        
        //---- SystemType ----
        { (int)ECSDefine.SystemType.Base, typeof(BaseSystem) },
    };
}
