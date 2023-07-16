using System.Runtime.Serialization;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

[Serializable]
public class ProxyRequestException : Exception
{
    public int Code { get; set; }

    public ProxyRequestException(int code)
    {
        Code = code;
    }

    public ProxyRequestException(int code, string message) : base(message)
    {
        Code = code;
    }

    public ProxyRequestException(int code, string message, Exception inner) : base(message, inner)
    {
        Code = code;
    }

    protected ProxyRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
