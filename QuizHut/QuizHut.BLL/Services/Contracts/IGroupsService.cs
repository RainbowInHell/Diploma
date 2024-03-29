﻿namespace QuizHut.BLL.Services.Contracts
{
    public interface IGroupsService
    {
        Task<IList<T>> GetAllGroupsAsync<T>(string creatorId = null, string eventId = null, string searchText = null);

        Task<IEnumerable<T>> GetAllGroupsByEventIdAsync<T>(string eventId);

        Task AssignStudentsToGroupAsync(string groupId, IList<string> studentsIds);

        Task AssignEventsToGroupAsync(string groupId, IList<string> eventsIds);

        Task<string> CreateGroupAsync(string name, string creatorId);

        Task UpdateGroupNameAsync(string groupId, string newName);

        Task DeleteGroupAsync(string groupId);

        Task DeleteStudentFromGroupAsync(string groupId, string studentId);

        Task DeleteEventFromGroupAsync(string groupId, string eventId);
    }
}