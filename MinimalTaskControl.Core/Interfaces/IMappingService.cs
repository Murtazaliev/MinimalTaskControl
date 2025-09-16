namespace MinimalTaskControl.Core.Interfaces;

public interface IMappingService
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
}
