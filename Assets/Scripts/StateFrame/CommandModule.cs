using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandModule 
{
    private List<BaseCommand> executeCommandList;

    private List<BaseCommand> sendCommandList;

    private List<BaseCommand> cacheCommandList;

    private Operation commandOperation;

    public void Init()
    {
        executeCommandList = new List<BaseCommand>();
        sendCommandList = new List<BaseCommand>();
        cacheCommandList = new List<BaseCommand>();

        commandOperation = new Operation();
        commandOperation.Init();
        commandOperation.SetName("Command");
    }

    public void UnInit()
    {
        commandOperation.UnInit();

        cacheCommandList.Clear();
        sendCommandList.Clear();
        executeCommandList.Clear();
    }


    public void RequireCommand(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        BaseCommand command = PopCommand()
    }

    private BaseCommand PopCommand(ECSDefine.SystemType systemType)
    {
        OperationObject operationObject = commandOperation.CreateOperationObject(systemType,);

        BaseCommand command;
        if (commmandPool.Count == 0)
        {
            command = new BaseCommand();
        }
        else
        {
            command = commmandPool[0];
            commmandPool.RemoveAt(0);
        }

        command.Init();


        return command;
    }
}
