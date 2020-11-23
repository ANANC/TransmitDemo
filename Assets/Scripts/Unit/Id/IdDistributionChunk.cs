using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdDistributionChunk
{
    private int firstId;
    private int interval;

    private int currentId;

    private bool isCanUseDiscardId;

    private List<int> discardIdList;

    public void Init()
    {
        discardIdList = new List<int>();

        SetFirstId(0);
        SetInterval(1);
        SetIsCanUseDiscardId(false);
    }

    public void UnInit()
    {
        discardIdList.Clear();
    }

    public void SetFirstId(int firstId)
    {
        this.firstId = firstId;
        currentId = this.firstId;
    }

    public void SetInterval(int interval)
    {
        this.interval = interval;
    }

    public void SetIsCanUseDiscardId(bool isCanUseDiscardId)
    {
        this.isCanUseDiscardId = isCanUseDiscardId;
    }

    public int Pop()
    {
        if (isCanUseDiscardId)
        {
            if(discardIdList.Count> 0)
            {
                return PopDiscardId();
            }
            else
            {
                return PopNewID();
            }
        }
        else
        {
            return PopNewID();
        }
    }

    private int PopDiscardId()
    {
        int id = discardIdList[1];
        discardIdList.Remove(1);
        return id;
    }

    private int PopNewID()
    {
        int id = currentId;
        currentId = currentId + interval;
        return id;
    }

    public void Push(int id)
    {
        discardIdList.Add(id);
    }
}
