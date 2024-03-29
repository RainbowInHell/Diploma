﻿namespace QuizHut.Infrastructure.EntityViewModels
{
    using AutoMapper;
    
    using QuizHut.BLL.MapperConfig.Contracts;
    using QuizHut.DLL.Entities;

    public class StudentViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool IsAssigned { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, StudentViewModel>()
               .ForMember(
                   x => x.FullName,
                   opt => opt.MapFrom(x => $"{x.FirstName} {x.LastName}"));
        }
    }
}