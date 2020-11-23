
public abstract class BaseObject : PollingOperationObject
{
    private int globalUnionId;

    public void SetGlobalUnionId(int globalUnionId)
    {
        this.globalUnionId = globalUnionId;
    }

    protected GlobalUnion GlobalUnion
    {
        get
        {
            return RunSystem.System.GetGlobalUnion(globalUnionId);
        }
    }

    protected int GlobalUnionId
    {
        get
        {
            return globalUnionId;
        }
    }

    public override void Update()
    {
    }
}
