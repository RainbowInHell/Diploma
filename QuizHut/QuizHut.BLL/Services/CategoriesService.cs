﻿namespace QuizHut.BLL.Services
{
    using Microsoft.EntityFrameworkCore;
    
    using QuizHut.BLL.Expression.Contracts;
    using QuizHut.BLL.MapperConfig;
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.DLL.Entities;
    using QuizHut.DLL.Repositories.Contracts;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> repository;

        private readonly IRepository<Quiz> quizRepository;

        private readonly IExpressionBuilder expressionBuilder;

        public CategoriesService(
            IRepository<Category> repository,
            IRepository<Quiz> quizRepository,
            IExpressionBuilder expressionBuilder)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
            this.expressionBuilder = expressionBuilder;
        }

        public async Task<IEnumerable<T>> GetAllCategories<T>(string creatorId, string searchText = null)
        {
            var query = repository
                .AllAsNoTracking()
                .Where(x => x.CreatorId == creatorId);

            if (searchText != null)
            {
                var filter = expressionBuilder.GetExpression<Category>("Name", searchText);
                query = query.Where(filter);
            }

            return await query
                .OrderByDescending(x => x.CreatedOn)
                .To<T>()
                .ToListAsync();
        }

        public async Task AssignQuizzesToCategoryAsync(string categoryId, IEnumerable<string> quizzesIds)
        {
            foreach (var quizId in quizzesIds)
            {
                var quiz = await quizRepository
                    .All()
                    .Where(x => x.Id == quizId)
                    .FirstOrDefaultAsync();

                quiz.CategoryId = categoryId;
                quizRepository.Update(quiz);
            }

            await quizRepository.SaveChangesAsync();
        }

        public async Task<string> CreateCategoryAsync(string name, string creatorId)
        {
            var category = new Category() { Name = name, CreatorId = creatorId };

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            return category.Id;
        }

        public async Task UpdateCategoryNameAsync(string id, string newName)
        {
            var category = await repository
                .All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            category.Name = newName;
            repository.Update(category);

            await repository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await repository
                .All()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            repository.Delete(category);

            await repository.SaveChangesAsync();
        }

        public async Task DeleteQuizFromCategoryAsync(string categoryId, string quizId)
        {
            var quiz = await quizRepository
                .All()
                .Where(x => x.Id == quizId)
                .FirstOrDefaultAsync();

            quiz.CategoryId = null;
            quizRepository.Update(quiz);

            await quizRepository.SaveChangesAsync();
        }
    }
}