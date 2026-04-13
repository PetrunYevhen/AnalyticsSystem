using Domain;

namespace Analytics.Domain.Entities.DataSource;

public class DataSource : Entity
{
    public Guid TenantId { get; private  set; }
    public DataSourceType DataSourceType { get; private  set; }
    public string Name { get; private set; }
    
    public DataSourceStatus Status { get; private  set; }
    public DateTime LastSyncDate { get; private  set; }
    
    public string JsonData { get; private  set; }
    
    private DataSource(){}

    public DataSource(Guid tenantId, DataSourceType dataSourceType, string name, string jsonData)
    {
        TenantId = tenantId;
        DataSourceType = dataSourceType;
        Name = name;
        JsonData = jsonData;
    }
    
    public void UpdateStatus(DataSourceStatus newStatus)
    {
        Status = newStatus;
    }

    public void MarkSynced()
    {
        LastSyncDate = DateTime.UtcNow;
    }
}