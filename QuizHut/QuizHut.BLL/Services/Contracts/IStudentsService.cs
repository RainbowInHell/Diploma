﻿namespace QuizHut.BLL.Services.Contracts
{
    public interface IStudentsService
    {
        Task<IList<T>> GetAllStudentsAsync<T>(
            string teacherId = null,
            string searchCriteria = null,
            string searchText = null);

        Task<IList<T>> GetAllStudentsByGroupIdAsync<T>(string groupId);

        Task<IList<T>> GetAllStudentsUnAssignedToGroup<T>(string groupId);

        Task<bool> AddStudentAsync(string email, string teacherId);

        Task DeleteStudentFromTeacherListAsync(string studentId, string teacherId);
    }
}