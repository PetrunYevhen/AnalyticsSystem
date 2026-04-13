namespace Analytics.Domain.Entities.Source;

public enum DataSourceStatus
{
    Connected = 1,
    Disconnected = 2,
    Error = 3,
    Syncing = 4
}