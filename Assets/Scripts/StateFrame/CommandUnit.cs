using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : BaseUnit
{
    private List<BaseCommand> executeCommandList;

    private List<BaseCommand> sendCommandList;

    private List<BaseCommand> cacheCommandList;

    private Operation commandOperation;
    private IdDistributionChunk idDistributionChunk;

    public override void Init()
    {
        executeCommandList = new List<BaseCommand>();
        sendCommandList = new List<BaseCommand>();
        cacheCommandList = new List<BaseCommand>();

        commandOperation = new Operation();
        commandOperation.Init();
        commandOperation.SetName("Command");

        InitGlobalIdDistributionChunk();
    }

    private void InitGlobalIdDistributionChunk()
    {
        idDistributionChunk = new IdDistributionChunk();
        idDistributionChunk.Init();
    }

    public override void UnInit()
    {
        idDistributionChunk.UnInit();

        commandOperation.UnInit();

        cacheCommandList.Clear();
        sendCommandList.Clear();
        executeCommandList.Clear();
    }


    public void RequireCommand(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        BaseCommand command = PopCommand(systemType);
        if (command != null)
        {
            command.Send(entityId, systemType, expandData);

            cacheCommandList.Add(command);
        }
    }

    public void ReceiveCommand()
    {

    }

    private BaseCommand PopCommand(ECSDefine.SystemType systemType)
    {
        int commandId = idDistributionChunk.Pop();
        OperationObject operationObject = commandOperation.CreateOperationObject((int)systemType, commandId);

        if (operationObject != null)
        {
            BaseCommand command = operationObject as BaseCommand;

            command.SetGlobalUnionId(GlobalUnionId);

            return command;
        }

        return null;
    }
}
