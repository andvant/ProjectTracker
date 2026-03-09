namespace ProjectTracker.Application.Extensions;

internal static class TimeProviderExtensions
{
    public static DateOnly Today(this TimeProvider timeProvider) =>
        DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
}
