using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInstanceDefine 
{
    private bool _isMainTerminal = false;

    public void SetIsMainTerminal(bool isMainTerminal)
    {
        _isMainTerminal = isMainTerminal;
    }
    public bool GetIsMainTerminal()
    {
        return _isMainTerminal;
    }
}
