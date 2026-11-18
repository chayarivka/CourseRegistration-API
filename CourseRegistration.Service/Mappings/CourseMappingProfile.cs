
using AutoMapper;
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;
using CourseRegistration.Core.Enums;


namespace CourseRegistration.Service.Mappings
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<CreateCourseDto, Course>();

            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.RowVersion,
                    opt => opt.Ignore());


            CreateMap<Course, CourseResponseDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AvailableSeats,
                    opt => opt.MapFrom(src =>
                        src.Capacity - src.Registrations.Count(
                            r => r.Status == RegistrationStatus.Active)));

        }
    }
}
