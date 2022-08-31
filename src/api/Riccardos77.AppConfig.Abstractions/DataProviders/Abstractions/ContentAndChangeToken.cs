using Microsoft.Extensions.Primitives;

namespace Riccardos77.AppConfig.DataProviders.Abstractions
{
    public record struct ContentAndChangeToken<T>(T Content, IChangeToken ChangeToken);
}
