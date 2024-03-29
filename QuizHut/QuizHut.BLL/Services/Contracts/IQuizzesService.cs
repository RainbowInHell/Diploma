﻿ namespace QuizHut.BLL.Services.Contracts
{
    public interface IQuizzesService
    {
        Task<IEnumerable<T>> GetAllQuizzesAsync<T>(
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null,
            string categoryId = null);

        Task<IList<T>> GetQuizzesByCategoryIdAsync<T>(string id);
        
        Task<IList<T>> GetUnAssignedQuizzesToCategoryByCreatorIdAsync<T>(string creatorId);

        Task<IList<T>> GetUnAssignedQuizzesToEventAsync<T>(string creatorId = null);

        Task<IList<T>> GetQuizzesByEventId<T>(string eventId);

        Task<T> GetQuizByPasswordAsync<T>(string password);

        Task<bool> IsQuizAssignedToGroup(string participantId, string eventId, string quizId);

        Task CreateQuizAsync(string name, string description, int? timer, string creatorId, string password);

        Task UpdateQuizAsync(string id, string name, string description, int? timer, string password);

        Task DeleteQuizAsync(string id);
    }
}