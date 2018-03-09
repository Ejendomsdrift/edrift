using System;

namespace CommonSyncData.Configuration
{
    public interface IServiceConfiguration
    {
        TimeSpan Delay { get; }
        bool IsEnabled { get; }
    }
}
