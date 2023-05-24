﻿namespace QuizHut.BLL.Services
{
    using System.Globalization;

    using Microsoft.EntityFrameworkCore;

    using QuizHut.BLL.Expression.Contracts;
    using QuizHut.BLL.Helpers;
    using QuizHut.BLL.MapperConfig;
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.DLL.Common;
    using QuizHut.DLL.Entities;
    using QuizHut.DLL.Repositories.Contracts;

    using TimeZoneConverter;

    public class EventsService : IEventsService
    {
        private readonly IRepository<Event> repository;

        private readonly IRepository<Quiz> quizRepository;

        private readonly IEventsGroupsService eventsGroupsService;

        private readonly IScheduledJobsService scheduledJobsService;

        private readonly IEmailSenderService emailSenderService;

        private readonly IExpressionBuilder expressionBuilder;

        public EventsService(
            IRepository<Event> repository,
            IRepository<Quiz> quizRepository,
            IEventsGroupsService eventsGroupsService,
            IScheduledJobsService scheduledJobsService,
            IEmailSenderService emailSenderService,
            IExpressionBuilder expressionBuilder)
        {
            this.repository = repository;
            this.quizRepository = quizRepository;
            this.eventsGroupsService = eventsGroupsService;
            this.scheduledJobsService = scheduledJobsService;
            this.emailSenderService = emailSenderService;
            this.expressionBuilder = expressionBuilder;
        }

        public async Task<IList<T>> GetAllEventsAsync<T>(
            string creatorId = null,
            string searchCriteria = null,
            string searchText = null)
        {
            var query = repository.AllAsNoTracking();

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            var emptyNameInput = searchText == null && searchCriteria == "Name";
            if (searchCriteria != null && !emptyNameInput)
            {
                var filter = expressionBuilder.GetExpression<Event>(searchCriteria, searchText);
                query = query.Where(filter);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<IList<T>> GetAllEventsByCreatorIdAndStatusAsync<T>(
            Status status,
            string creatorId,
            string searchText = null)
        {
            var query = repository
                .AllAsNoTracking()
                .Where(x => x.Status == status && x.CreatorId == creatorId);

            if (searchText != null)
            {
                var filter = expressionBuilder.GetExpression<Event>("Name", searchText);
                query = query.Where(filter);
            }

            return await query
                   .OrderByDescending(x => x.CreatedOn)
                   .To<T>()
                   .ToListAsync();
        }

        public async Task<IList<T>> GetAllEventsFilteredByStatusAndGroupAsync<T>(
            Status status, 
            string groupId, 
            string creatorId = null)
        {
            var query = repository
                .AllAsNoTracking()
                .Where(x => !x.EventsGroups
                .Any(x => x.GroupId == groupId));

            if (creatorId != null)
            {
                query = query.Where(x => x.CreatorId == creatorId);
            }

            return await query
              .Where(x => x.Status != status)
              .OrderByDescending(x => x.CreatedOn)
              .To<T>()
              .ToListAsync();
        }

        public async Task<IList<T>> GetAllEventsByGroupIdAsync<T>(string groupId)
        {
            return await repository
                .AllAsNoTracking()
                .Where(x => x.EventsGroups.Any(x => x.GroupId == groupId))
                .To<T>()
                .ToListAsync();
        }

        public async Task AssignQuizzesToEventAsync(IList<string> quizIds, string eventId)
        {
            var @event = await repository
                .All()
                .Include(e => e.Quizzes)
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();

            foreach (var quizId in quizIds)
            {
                var quiz = await quizRepository
                    .All()
                    .Where(x => x.Id == quizId)
                    .FirstOrDefaultAsync();

                @event.Quizzes.Add(quiz);
                @event.Status = GetStatus(@event.ActivationDateAndTime, @event.DurationOfActivity, quizId);

                repository.Update(@event);
                await repository.SaveChangesAsync();

                if (@event.Status != Status.Ended)
                {
                    await SheduleStatusChangeAsync(@event.ActivationDateAndTime, @event.DurationOfActivity, @event.Id, @event.Status);
                }
            }
        }

        public async Task AssignGroupsToEventAsync(IList<string> groupIds, string eventId)
        {
            foreach (var groupId in groupIds)
            {
                await eventsGroupsService.CreateEventGroupAsync(eventId, groupId);
            }
        }

        public async Task<string> CreateEventAsync(
            string name, 
            string activationDate, 
            string activeFrom, 
            string activeTo, 
            string creatorId)
        {
            var activationDateAndTime = GetActivationDateAndTimeUtc(activationDate, activeFrom);
            var durationOfActivity = GetDurationOfActivity(activationDate, activeFrom, activeTo);

            var @event = new Event
            {
                Name = name,
                Status = Status.Pending,
                ActivationDateAndTime = activationDateAndTime,
                DurationOfActivity = durationOfActivity,
                CreatorId = creatorId,
            };

            await repository.AddAsync(@event);
            await repository.SaveChangesAsync();

            return @event.Id;
        }

        public async Task UpdateEventAsync(string id, string name, string activationDate, string activeFrom, string activeTo)
        {
            var @event = await repository
                .All()
                .Include(e => e.Quizzes)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var activationDateAndTime = GetActivationDateAndTimeUtc(activationDate, activeFrom);
            var durationOfActivity = GetDurationOfActivity(activationDate, activeFrom, activeTo);

            @event.Name = name;
            @event.ActivationDateAndTime = activationDateAndTime;
            @event.DurationOfActivity = durationOfActivity;
            @event.Status = GetStatus(activationDateAndTime, durationOfActivity, @event.Quizzes.FirstOrDefault()?.Id);

            repository.Update(@event);
            await repository.SaveChangesAsync();

            if (@event.Quizzes.Count > 0)
            {
                await SheduleStatusChangeAsync(activationDateAndTime, durationOfActivity, id, @event.Status);
            }
        }

        public async Task DeleteQuizFromEventAsync(string eventId, string quizId)
        {
            var @event = await repository
                .All()
                .Include(x => x.Quizzes)
                .Where(x => x.Id == eventId)
                .FirstOrDefaultAsync();

            var quizToRemove = @event.Quizzes.FirstOrDefault(q => q.Id == quizId);

            @event.Quizzes.Remove(quizToRemove);

            if (!@event.Quizzes.Any())
            {
                if (@event.Status == Status.Active)
                {
                    @event.Status = Status.Pending;
                }
            }

            await scheduledJobsService.DeleteJobsAsync(@event.Id, true);

            repository.Update(@event);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(string eventId)
        {
            var @event = await repository
                .All()
                .Include(e => e.Quizzes)
                .Where(x => x.Id == eventId)
                .FirstOrDefaultAsync();

            foreach (var quiz in @event.Quizzes)
            {
                quiz.EventId = null;
                quizRepository.Update(quiz);
            }

            repository.Delete(@event);

            await quizRepository.SaveChangesAsync();
            await repository.SaveChangesAsync();
        }

        public async Task SendEmailsToEventGroups(string eventId, string quizId)
        {
            var eventInfo = await repository
                .AllAsNoTracking()
                .Where(x => x.Id == eventId)
                .Select(x => new
                {
                    EventName = x.Name,
                    Password = x.Quizzes
                        .Where(q => q.Id == quizId)
                        .Select(q => q.Password)
                        .FirstOrDefault(),
                    QuizName = x.Quizzes
                        .Where(q => q.Id == quizId)
                        .Select(q => q.Name)
                        .FirstOrDefault(),
                    Emails = x.EventsGroups
                        .SelectMany(eg => eg.Group.StudentsGroups.Select(sg => sg.Student.Email)),
                })
                .FirstOrDefaultAsync();

            foreach (var email in eventInfo.Emails)
            {
                var emailSubject = $"Пароль для викторины '{eventInfo.QuizName}' в мероприятии '{eventInfo.EventName}'";
                var emailContent = $"Пароль: {eventInfo.Password}";

                await emailSenderService.SendEmailAsync(email, emailSubject, emailContent);
            }
        }

        public string GetTimeErrorMessage(string activeFrom, string activeTo, string activationDate)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(DateTimeConverter.TimeZoneIana));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var activationDateAndTimeUtc = GetActivationDateAndTimeUtc(activationDate, activeFrom);
            var activationDateAndTimeToUserLocalTime = TimeZoneInfo.ConvertTimeFromUtc(activationDateAndTimeUtc, zone);

            if (userLocalTimeNow.Date > activationDateAndTimeToUserLocalTime.Date)
            {
                return "Invalid Activation Date";
            }

            var timeToStart = TimeSpan.Parse(activeFrom);
            var timeNow = userLocalTimeNow.TimeOfDay;
            var startHours = timeToStart.Hours;
            var nowHours = timeNow.Hours;
            var startMins = timeToStart.Minutes;
            var nowMins = timeNow.Minutes;
            var invalidStartingTime = startHours < nowHours || (startHours == nowHours && startMins < nowMins);

            if (userLocalTimeNow.Date == activationDateAndTimeToUserLocalTime.Date && invalidStartingTime)
            {
                return "Invalid Starting Time";
            }

            var duration = GetDurationOfActivity(activationDate, activeFrom, activeTo);
            if (duration.Hours <= 0 && duration.Minutes <= 0)
            {
                return "Invalid Duration Of Activity";
            }

            return null;
        }

        #region Help Methods

        private DateTime GetActivationDateAndTimeUtc(string activationDate, string activeFrom)
        {
            return DateTime.ParseExact(activationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                           .Add(TimeSpan.Parse(activeFrom))
                           .ToUniversalTime();
        }

        private TimeSpan GetDurationOfActivity(string activationDate, string activeFrom, string activeTo)
        {
            return DateTime.ParseExact(activationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                           .Add(TimeSpan.Parse(activeTo))
                           .ToUniversalTime() - GetActivationDateAndTimeUtc(activationDate, activeFrom);
        }

        private Status GetStatus(DateTime activationDateAndTime, TimeSpan durationOfActivity, string quizId)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(DateTimeConverter.TimeZoneIana));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var activationDateAndTimeToUserLocalTime = TimeZoneInfo.ConvertTimeFromUtc(activationDateAndTime, zone);

            if (quizId == null
                || userLocalTimeNow.Date < activationDateAndTimeToUserLocalTime.Date
                || activationDateAndTimeToUserLocalTime.TimeOfDay > userLocalTimeNow.TimeOfDay)
            {
                return Status.Pending;
            }

            var startHours = activationDateAndTimeToUserLocalTime.TimeOfDay.Hours;
            var nowHours = userLocalTimeNow.TimeOfDay.Hours;
            var startMins = activationDateAndTimeToUserLocalTime.TimeOfDay.Minutes;
            var nowMins = userLocalTimeNow.TimeOfDay.Minutes;

            var endHours = activationDateAndTimeToUserLocalTime.Add(durationOfActivity).TimeOfDay.Hours;
            var endMinutes = activationDateAndTimeToUserLocalTime.Add(durationOfActivity).TimeOfDay.Minutes;

            if (startHours <= nowHours && startMins <= nowMins
                && (endHours > nowHours || (endHours == nowHours && endMinutes >= nowMins)))
            {
                return Status.Active;
            }

            return Status.Ended;
        }

        private async Task<string[]> GetStudentsNamesByEventIdAsync(string id)
        {
            return await repository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.EventsGroups.SelectMany(x => x.Group.StudentsGroups.Select(x => x.Student.UserName)))
                .ToArrayAsync();
        }

        private async Task SheduleStatusChangeAsync(
            DateTime activationDateAndTime,
            TimeSpan durationOfActivity,
            string eventId,
            Status eventStatus)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(DateTimeConverter.TimeZoneIana));
            var userLocalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);
            var userTimeToUtc = userLocalTimeNow.ToUniversalTime();

            var activationDelay = activationDateAndTime - userTimeToUtc;
            var endingDelay = activationDateAndTime.Add(durationOfActivity) - userTimeToUtc;

            if (eventStatus == Status.Active)
            {
                await scheduledJobsService.DeleteJobsAsync(eventId, false);
                await scheduledJobsService.CreateEndEventJobAsync(eventId, endingDelay);
            }
            else
            {
                await scheduledJobsService.DeleteJobsAsync(eventId, true);
                await scheduledJobsService.CreateStartEventJobAsync(eventId, activationDelay);
                await scheduledJobsService.CreateEndEventJobAsync(eventId, endingDelay);
            }
        }

        #endregion
    }
}