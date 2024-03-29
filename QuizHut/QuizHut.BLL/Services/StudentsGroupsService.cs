﻿namespace QuizHut.BLL.Services
{
    using Microsoft.EntityFrameworkCore;
    
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.DLL.Entities;
    using QuizHut.DLL.Repositories.Contracts;

    public class StudentsGroupsService : IStudentsGroupsService
    {
        private readonly IRepository<StudentGroup> repository;

        public StudentsGroupsService(IRepository<StudentGroup> repository)
        {
            this.repository = repository;
        }

        public async Task CreateStudentGroupAsync(string groupId, string studentId)
        {
            var studentGroup = new StudentGroup() { GroupId = groupId, StudentId = studentId };

            await repository.AddAsync(studentGroup);

            await repository.SaveChangesAsync();
        }

        public async Task DeleteStudentGroupAsync(string groupId, string studentId)
        {
            var studentGroup = await repository
                .All()
                .Where(x => x.GroupId == groupId && x.StudentId == studentId)
                .FirstOrDefaultAsync();

            repository.Delete(studentGroup);

            await repository.SaveChangesAsync();
        }
    }
}