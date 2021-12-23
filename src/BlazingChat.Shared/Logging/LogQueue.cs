using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.Logging;

public class LogQueue
{
    private readonly Channel<LogMessage> _channel = Channel.CreateUnbounded<LogMessage>(new UnboundedChannelOptions
    {
        SingleReader = true
    });

    public ValueTask WriteLog(LogMessage message, CancellationToken token = default) =>
        _channel.Writer.WriteAsync(message, token);

    public IAsyncEnumerable<LogMessage> ReadLogs(CancellationToken token = default) =>
        _channel.Reader.ReadAllAsync(token);
}
