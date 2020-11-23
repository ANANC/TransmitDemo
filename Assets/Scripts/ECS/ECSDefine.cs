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
