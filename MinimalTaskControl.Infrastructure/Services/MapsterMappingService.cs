using MapsterMapper;
using MinimalTaskControl.Core.Interfaces;

namespace MinimalTaskControl.Infrastructure.Services;

public class MapsterMappingService(IMapper mapper) : IMappingService
{
    private readonly IMapper _mapper = mapper;

    public TDestination Map<TDestination>(object source) => _mapper.Map<TDestination>(source);
    public TDestination Map<TSource, TDestination>(TSource source) => _mapper.Map<TSource, TDestination>(source);
}

