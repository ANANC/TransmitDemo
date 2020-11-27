using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExecuteSystemController
{
    void Init();

    void UnInit();

    void SetGlobalUnion(GlobalUnion globalUnion);

    BaseComponent RequireComponentControl(BaseComponent component);
}
