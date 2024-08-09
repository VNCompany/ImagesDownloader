using System.Windows.Markup;

namespace ImagesDownloader.Infrastructure;

internal class DISource : MarkupExtension
{
    public static Func<Type, object>? Resolver;

    public Type Type { get; set; } = null!;

    public override object? ProvideValue(IServiceProvider serviceProvider)
        => Resolver?.Invoke(Type) ?? null;
}
