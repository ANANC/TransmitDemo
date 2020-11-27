using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSInstanceDefine 
{
    //系统优先度
    public static Dictionary<ECSDefine.SystemType, ECSDefine.SystemPriority> SystemType2Priority = new Dictionary<ECSDefine.SystemType, ECSDefine.SystemPriority>()
    {
        //示例
        { ECSDefine.SystemType.Base,ECSDefine.SystemPriority.Normal },
    };

    //系统功能分类
    public static Dictionary<ECSDefine.SystemType, ECSDefine.SystemFunctionType> SystemType2Function = new Dictionary<ECSDefine.SystemType, ECSDefine.SystemFunctionType>()
    {
        //示例
        { ECSDefine.SystemType.Base,ECSDefine.SystemFunctionType.Logic },
    };

    //关键控件对应的处理系统
    public static Dictionary<ECSDefine.ComponentType, ECSDefine.SynchroValueRspSystemType> RequireComponentType2ExecuteSystem = new Dictionary<ECSDefine.ComponentType, ECSDefine.SynchroValueRspSystemType>()
    {
        //示例
        { ECSDefine.ComponentType.Base, ECSDefine.SynchroValueRspSystemType.Base},
    };

    //游戏业务模块优先度
    public static List<BaseUnit> GameUnitPriorityList = new List<BaseUnit>()
    {

    };

}
