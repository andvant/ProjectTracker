namespace ProjectTracker.Domain.Extensions;

public static class CollectionExtensions
{
    public static void AddIfNotNull<T>(this ICollection<T> collection, T? item)
    {
        if (item is not null)
        {
            collection.Add(item);
        }
    }

    public static void AddIfNotThere<T>(this ICollection<T> collection, T item) where T : Entity
    {
        if (!collection.Any(e => e.Id == item.Id))
        {
            collection.Add(item);
        }
    }

    public static void RemoveIfExists<T>(this ICollection<T> collection, T item) where T : Entity
    {
        var existing = collection.FirstOrDefault(e => e.Id == item.Id);

        if (existing is not null)
        {
            collection.Remove(existing);
        }
    }
}
