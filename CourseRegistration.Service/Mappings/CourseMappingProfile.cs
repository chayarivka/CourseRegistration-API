
using AutoMapper;
using CourseRegistration.Core.DTOs;
using CourseRegistration.Core.Entities;


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

        }
    }
}
