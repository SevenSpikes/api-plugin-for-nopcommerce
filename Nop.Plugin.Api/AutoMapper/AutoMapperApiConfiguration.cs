using AutoMapper;
using AutoMapper.Configuration;

namespace Nop.Plugin.Api.AutoMapper
{
    public static class AutoMapperApiConfiguration
    {
        private static MapperConfigurationExpression _mapperConfigurationExpression;
        private static IMapper _mapper;

        public static MapperConfigurationExpression MapperConfigurationExpression
        {
            get
            {
                if (_mapperConfigurationExpression == null)
                {
                    _mapperConfigurationExpression = new MapperConfigurationExpression(); ;
                }

                return _mapperConfigurationExpression;
            }
        }

        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    var mapperConfiguration = new MapperConfiguration(MapperConfigurationExpression);

                    _mapper = mapperConfiguration.CreateMapper();
                }

                return _mapper;
            }
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}