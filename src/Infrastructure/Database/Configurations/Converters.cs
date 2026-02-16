using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectTracker.Domain.ValueObjects;

namespace ProjectTracker.Infrastructure.Database.Configurations;

internal static class Converters
{
    public static ValueConverter<ProjectKey, string> ProjectKeyConverter { get; } =
        new(v => v, v => v);

    public static ValueConverter<IssueKey, string> IssueKeyConverter { get; } =
        new(v => v, v => IssueKey.Parse(v));

    public static ValueConverter<Title, string> TitleConverter { get; } =
        new(v => v, v => v);
}
