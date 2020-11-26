using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECSInstanceDefine 
{
    public static Dictionary<ECSDefine.SystemType, ECSDefine.SystemPriority> SystemType2Priority = new Dictionary<ECSDefine.SystemType, ECSDefine.SystemPriority>()
    {
        //示例
        { ECSDefine.SystemType.Base,ECSDefine.SystemPriority.Normal },
    };

    public static Dictionary<ECSDefine.SystemType, ECSDefine.SystemFunctionType> SystemType2Function = new Dictionary<ECSDefine.SystemType, ECSDefine.SystemFunctionType>()
    {
        //示例
        { ECSDefine.SystemType.Base,ECSDefine.SystemFunctionType.Logic },
    };

}
