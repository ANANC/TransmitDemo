using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationInstanceDefine 
{
    public static Dictionary<int, Type> OperationObjectType2ClassType = new Dictionary<int, Type>()
    {
        //---- GlobalUnion ----
        { (int)GlobalDefine.GlobalUnionType.Global, typeof(GlobalUnion) },

        
        //---- Unit ----
        { (int)GlobalDefine.UnitType.Unit, typeof(BaseUnit) },


        //---- EntityType ----
        { (int)ECSDefine.EntityType.Normal, typeof(BaseEntity) },


        //---- ComponentType ----
        { (int)ECSDefine.ComponentType.Base, typeof(BaseComponent) },

        
        //---- SystemType ----
        { (int)ECSDefine.SystemType.ComponentInfo, typeof(BaseSystem.ComponentInfo) },
        { (int)ECSDefine.SystemType.Base, typeof(BaseSystem) },

        
        //---- SynchroValueRspSystemType ----
        { (int)ECSDefine.SynchroValueRspSystemType.Base, typeof(BaseSynchroValueRspSystem) },


        //---- SynchronizationValueType ----
        { (int)ECSDefine.SynchronizationValueType.Require, typeof(SynchroValueRep) },
        { (int)ECSDefine.SynchronizationValueType.Response, typeof(SynchroValueRsp) },
    };
}
