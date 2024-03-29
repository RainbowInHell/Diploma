﻿namespace QuizHut.ViewModels.MainViewModels.TeacherPartViewModels.EventViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using FontAwesome.Sharp;

    using QuizHut.BLL.Helpers.Contracts;
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Commands;
    using QuizHut.Infrastructure.Commands.Base;
    using QuizHut.Infrastructure.Commands.Base.Contracts;
    using QuizHut.Infrastructure.EntityViewModels.Events;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;
    using QuizHut.ViewModels.Contracts;

    class EventsViewModel : ViewModel, IMenuView
    {
        public string Title { get; set; } = "События";

        public IconChar IconChar { get; set; } = IconChar.CalendarDays;

        public Dictionary<string, string> SearchCriteriasInEnglish => new()
        {
            { "Название", "Name" },
            { "Активные", "Active" },
            { "В ожидании", "Pending" },
            { "Завершенные", "Ended" }
        };

        private readonly IEventsService eventsService;

        private readonly ISharedDataStore sharedDataStore;

        private readonly IDateTimeConverter dateTimeConverter;

        private readonly IExporter exporter;

        public EventsViewModel(
            IEventsService eventsService,
            ISharedDataStore sharedDataStore,
            IDateTimeConverter dateTimeConverter,
            IRenavigator eventActionsRenavigator,
            IRenavigator eventSettingRenavigator,
            IViewDisplayTypeService viewDisplayTypeService,
            IExporter exporter)
        {
            this.eventsService = eventsService;
            this.sharedDataStore = sharedDataStore;
            this.dateTimeConverter = dateTimeConverter;
            this.exporter = exporter;

            NavigateCreateEventCommand = new RenavigateCommand(eventActionsRenavigator, ViewDisplayType.Create, viewDisplayTypeService);
            NavigateEditEventCommand = new RenavigateCommand(eventActionsRenavigator, ViewDisplayType.Edit, viewDisplayTypeService);
            NavigateEventSettingsCommand = new RenavigateCommand(eventSettingRenavigator);

            LoadDataCommandAsync = new ActionCommandAsync(OnLoadDataCommandExecutedAsync);
            SearchCommandAsync = new ActionCommandAsync(OnSearchCommandAsyncExecute, CanSearchCommandAsyncExecute);
            RefreshSearchCommandAsync = new ActionCommandAsync(OnRefreshSearchCommandAsyncExecute);
            DeleteEventCommandAsync = new ActionCommandAsync(OnDeleteEventCommandExecutedAsync);
            ExportDataCommandAsync = new ActionCommandAsync(OnExportDataCommandAsyncExecute);
        }

        #region FieldsAndProperties

        public ObservableCollection<EventListViewModel> events;
        public ObservableCollection<EventListViewModel> Events
        {
            get => events;
            set => Set(ref events, value);
        }

        private EventListViewModel selectedEvent;
        public EventListViewModel SelectedEvent
        {
            get
            {
                sharedDataStore.SelectedEvent = selectedEvent;
                return selectedEvent;
            }
            set => Set(ref selectedEvent, value);
        }

        private string searchCriteria;
        public string SearchCriteria
        {
            get => searchCriteria;
            set => Set(ref searchCriteria, value);
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => Set(ref searchText, value);
        }

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set => Set(ref errorMessage, value);
        }

        #endregion

        #region NavigationCommands

        public ICommand NavigateCreateEventCommand { get; }

        public ICommand NavigateEditEventCommand { get; }

        public ICommand NavigateEventSettingsCommand { get; }

        #endregion

        #region LoadDataCommandAsync

        public ICommandAsync LoadDataCommandAsync { get; }

        private async Task OnLoadDataCommandExecutedAsync(object p)
        {
            await LoadEventsData();
        }

        #endregion

        #region SearchCommandAsync

        public ICommandAsync SearchCommandAsync { get; }

        private bool CanSearchCommandAsyncExecute(object p) => !string.IsNullOrEmpty(SearchCriteria) && !string.IsNullOrEmpty(SearchText);

        private async Task OnSearchCommandAsyncExecute(object p)
        {
            await LoadEventsData(SearchCriteriasInEnglish[SearchCriteria], SearchText);
        }

        #endregion

        #region RefreshSearchCommandAsync

        public ICommandAsync RefreshSearchCommandAsync { get; }

        private async Task OnRefreshSearchCommandAsyncExecute(object p)
        {
            SearchCriteria = null;
            SearchText = null;

            await LoadEventsData();
        }

        #endregion

        #region DeleteEventCommandAsync

        public ICommandAsync DeleteEventCommandAsync { get; }

        private async Task OnDeleteEventCommandExecutedAsync(object p)
        {
            await eventsService.DeleteEventAsync(SelectedEvent.Id);

            await LoadEventsData();
        }

        #endregion

        #region ExportDataCommand

        public ICommandAsync ExportDataCommandAsync { get; }

        private async Task OnExportDataCommandAsyncExecute(object p)
        {
            await exporter.GenerateExcelReportAsync(Events);
        }

        #endregion

        private async Task LoadEventsData(string searchCriteria = null, string searchText = null)
        {
            var events = await eventsService.GetAllEventsAsync<EventListViewModel>(sharedDataStore.CurrentUser.Id, searchCriteria, searchText);

            if (!events.Any())
            {
                ErrorMessage = "События не найдены";
            }
            else
            {
                foreach (var @event in events)
                {
                    @event.Duration = dateTimeConverter.GetDurationString(@event.ActivationDateAndTime, @event.DurationOfActivity);
                    @event.StartDate = dateTimeConverter.GetDate(@event.ActivationDateAndTime);
                }

                ErrorMessage = null;
            }

            Events = new(events);
        }
    }
}