using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSDefine 
{

    public enum EntityType
    {
        Normal = 1000,
    };


    public enum ComponentType
    {
        Base = 2000,


    };

    public enum SystemType
    {
        Base = 3000,
    };


    public enum SystemPriority
    {
        Top,
        Normal,
        Bottom,
    };


}


public class ECSInstanceDefine
{


    public static Dictionary<ECSDefine.EntityType, Type> EntityType2ClassType = new Dictionary<ECSDefine.EntityType, Type>()
    {
        { ECSDefine.EntityType.Normal, typeof(BaseEntity) },
    };

    public static Dictionary<ECSDefine.ComponentType, Type> ComponentType2ClassType = new Dictionary<ECSDefine.ComponentType, Type>()
    {
        { ECSDefine.ComponentType.Base, typeof(BaseComponent) },
    };

}
