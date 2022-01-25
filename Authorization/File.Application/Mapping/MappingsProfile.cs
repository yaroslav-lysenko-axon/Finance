using AutoMapper;
using File.Application.DTO;
using File.Domain.Models;

namespace File.Application.Mapping
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<ImageDetails, ImageDetailsDto>();
            CreateMap<FileDetails, FileDetailsDto>()
                .ForMember(dest => dest.ImageDetailsDto, opt => opt.MapFrom(fileDetails => fileDetails.ImageDetails));
        }
    }
}
