using System.Reflection;

namespace Rocky.BLL.Helpers;

public class ObjectMapper
{
    public static TTarget Map<TSource, TTarget>(TSource source)
            where TTarget : new()
    {
        if (source == null)
            return default;

        TTarget target = new TTarget();

        foreach (var sourceProperty in typeof(TSource).GetProperties())
        {
            var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name, BindingFlags.Public | BindingFlags.Instance);

            if (targetProperty != null && targetProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source, null);
                targetProperty.SetValue(target, value, null);
            }
        }

        return target;
    }
}