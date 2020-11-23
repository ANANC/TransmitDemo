﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : BaseUnit
{
    private List<BaseCommand> executeCommandList;

    private List<BaseCommand> sendCommandList;

    private List<BaseCommand> cacheCommandList;

    private Operation commandOperation;
    private IdDistributionChunk idDistributionChunk;

    private ExecuteSystemUnit ExecuteSystemUnit;

    public override void Init()
    {
        executeCommandList = new List<BaseCommand>();
        sendCommandList = new List<BaseCommand>();
        cacheCommandList = new List<BaseCommand>();

        InitOperation();
        InitGlobalIdDistributionChunk();

        ExecuteSystemUnit = GlobalUnion.GetUnit<ExecuteSystemUnit>();
    }

    private void InitOperation()
    {
        commandOperation = new Operation();
        commandOperation.Init();
        commandOperation.SetName("Command");
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

        ExecuteSystemUnit = null;
    }

    public override void Update()
    {
        for(int index = 0;index< cacheCommandList.Count;index++)
        {
            BaseCommand command = cacheCommandList[index];



            PushCommand(command.GetSystemType(), command);
        }
        cacheCommandList.Clear();
    }

    public void RequireCommand(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        BaseCommand command = PopCommand(systemType);
        if (command != null)
        {
            command.FillIn(entityId, systemType, expandData);

            sendCommandList.Add(command);

            ExecuteSystemUnit.ExecuteSystem(entityId, systemType, expandData);
        }
    }

    public void ReceiveCommand(int entityId, ECSDefine.SystemType systemType, BaseSystem.SystemExpandData expandData)
    {
        BaseCommand command = PopCommand(systemType);
        if (command != null)
        {
            command.FillIn(entityId, systemType, expandData);

            cacheCommandList.Add(command);
        }
    }

    public List<BaseCommand> PopSendCommands()
    {
        List<BaseCommand> commands = new List<BaseCommand>();
        for(int index = 0;index<sendCommandList.Count;index++)
        {
            BaseCommand command = sendCommandList[index];

            ECSDefine.SystemType systemType = command.GetSystemType();
            BaseCommand sendCommand = PopCommand(systemType);
            if(sendCommand !=null)
            {
                sendCommand.FillIn(command.GetEntityId(), systemType, command.GetExpandData());
            }
            PushCommand(systemType,command);
        }
        sendCommandList.Clear();

        return commands;
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

    public void PushCommand(ECSDefine.SystemType systemType, BaseCommand command)
    {
        commandOperation.DeleteOperationObject((int)systemType, command);
    }
}
