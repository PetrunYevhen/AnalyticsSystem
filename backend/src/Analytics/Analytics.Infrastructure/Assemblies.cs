using System.Reflection;
using Analytics.Application.Contracts;

namespace Analytics.Infrastructure;

public class Assemblies
{
    public static readonly Assembly Application = typeof(IAnalyticsModule).Assembly;
}