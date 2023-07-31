using System.Net;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

public class IPWhitelist
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IPWhitelist> _logger;
    private readonly byte[][] _whitelist;

    public IPWhitelist(RequestDelegate next, ILogger<IPWhitelist> logger, string whitelist)
    {
        var ips = whitelist.Split(',');
        _whitelist = new byte[ips.Length][];
        for (var i = 0; i < ips.Length; i++)
        {
            _whitelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
        }

        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress;
        _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

        var badIp = true;
        if (remoteIp != null)
        {
            var bytes = remoteIp.GetAddressBytes();
            foreach (var address in _whitelist)
            {
                if (address.SequenceEqual(bytes))
                {
                    badIp = false;
                    break;
                }
            }
        }

        if (badIp)
        {
            _logger.LogWarning("Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        await _next.Invoke(context);
    }
}
