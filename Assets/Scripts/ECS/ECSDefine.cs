
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
        ComponentInfo = 3000,
        Base = 3001,
    };

    public enum SynchroValueRspSystemType
    {
        Base = 4000,
    }


    public enum SynchronizationValueType
    {
        Require = 5000,
        Response = 5001,
    }


    public enum SystemFunctionType
    {
        Create,
        Logic,
        Statistics,
        Death,
    }


    public enum SystemPriority
    {
        Top,
        Normal,
        Bottom,
    };


}
