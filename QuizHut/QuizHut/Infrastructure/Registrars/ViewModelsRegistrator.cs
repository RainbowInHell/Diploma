﻿namespace QuizHut.Infrastructure.Registrars
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    using QuizHut.BLL.Dto.DtoValidators;
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Services;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;
    using QuizHut.ViewModels.Factory;
    using QuizHut.ViewModels.StartViewModels;
    using QuizHut.ViewModels.MainViewModels;
    using QuizHut.ViewModels.MainViewModels.GroupViewModels;
    using QuizHut.ViewModels.MainViewModels.CategoryViewModels;
    using QuizHut.ViewModels.MainViewModels.EventViewModels;
    using QuizHut.BLL.Helpers.Contracts;

    public static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<CreateViewModel<AuthorizationViewModel>>(services => () => CreateAuthorizationViewModel(services));
            services.AddSingleton<CreateViewModel<ResetPasswordViewModel>>(services => () => CreateResetPasswordViewModel(services));
            services.AddSingleton<CreateViewModel<StudentRegistrationViewModel>>(services => () => CreateStudentRegistrationViewModel(services));
            services.AddSingleton<CreateViewModel<TeacherRegistrationViewModel>>(services => () => CreateTeacherRegistrationViewModel(services));

            services.AddSingleton<CreateViewModel<HomeViewModel>>(services => () => services.GetRequiredService<HomeViewModel>());
            services.AddSingleton<CreateViewModel<UserProfileViewModel>>(services => () => services.GetRequiredService<UserProfileViewModel>());
            services.AddSingleton<CreateViewModel<ResultsViewModel>>(services => () => services.GetRequiredService<ResultsViewModel>());

            services.AddSingleton<CreateViewModel<EventsViewModel>>(services => () => CreateEventsViewModel(services));
            services.AddSingleton<CreateViewModel<EventActionsViewModel>>(services => () => CreateEventActionsViewModel(services));
            services.AddSingleton<CreateViewModel<EventSettingsViewModel>>(services => () => CreateEventSettingsViewModel(services));

            services.AddSingleton<CreateViewModel<GroupsViewModel>>(services => () => CreateGroupsViewModel(services));
            services.AddSingleton<CreateViewModel<GroupActionsViewModel>>(services => () => CreateGroupActionsViewModel(services));
            services.AddSingleton<CreateViewModel<GroupSettingsViewModel>>(services => () => CreateGroupSettingsViewModel(services));

            services.AddSingleton<CreateViewModel<CategoriesViewModel>>(services => () => CreateCategoriesViewModel(services));
            services.AddSingleton<CreateViewModel<CategoryActionsViewModel>>(services => () => CreateCategoryActionsViewModel(services));
            services.AddSingleton<CreateViewModel<CategorySettingsViewModel>>(services => () => CreateCategorySettingsViewModel(services));

            services.AddSingleton<CreateViewModel<QuizzesViewModel>>(services => () => services.GetRequiredService<QuizzesViewModel>());
            services.AddSingleton<CreateViewModel<StudentsViewModel>>(services => () => services.GetRequiredService<StudentsViewModel>());

            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<UserProfileViewModel>();
            services.AddTransient<ResultsViewModel>();

            services.AddTransient<EventsViewModel>();
            services.AddTransient<EventActionsViewModel>();
            services.AddTransient<EventSettingsViewModel>();

            services.AddTransient<GroupsViewModel>();
            services.AddTransient<GroupActionsViewModel>();
            services.AddTransient<GroupSettingsViewModel>();

            services.AddTransient<CategoriesViewModel>();
            services.AddTransient<CategoryActionsViewModel>();
            services.AddTransient<CategorySettingsViewModel>();

            services.AddTransient<QuizzesViewModel>();
            services.AddTransient<StudentsViewModel>();

            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            services.AddSingleton<ViewModelRenavigate<AuthorizationViewModel>>();
            services.AddSingleton<ViewModelRenavigate<StudentRegistrationViewModel>>();
            services.AddSingleton<ViewModelRenavigate<TeacherRegistrationViewModel>>();
            services.AddSingleton<ViewModelRenavigate<ResetPasswordViewModel>>();
            services.AddSingleton<ViewModelRenavigate<HomeViewModel>>();

            services.AddSingleton<ViewModelRenavigate<EventsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<EventActionsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<EventSettingsViewModel>>();

            services.AddSingleton<ViewModelRenavigate<GroupsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<GroupActionsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<GroupSettingsViewModel>>();

            services.AddSingleton<ViewModelRenavigate<CategoriesViewModel>>();
            services.AddSingleton<ViewModelRenavigate<CategoryActionsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<CategorySettingsViewModel>>();

            return services;
        }

        private static AuthorizationViewModel CreateAuthorizationViewModel(IServiceProvider services)
        {
            return new AuthorizationViewModel(
                services.GetRequiredService<IUserAccountService>(),
                services.GetRequiredService<LoginRequestValidator>(),
                services.GetRequiredService<ViewModelRenavigate<StudentRegistrationViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<TeacherRegistrationViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<ResetPasswordViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<HomeViewModel>>());
        }

        private static ResetPasswordViewModel CreateResetPasswordViewModel(IServiceProvider services)
        {
            return new ResetPasswordViewModel(
                services.GetRequiredService<IUserAccountService>(),
                services.GetRequiredService<EmailRequestValidator>(),
                services.GetRequiredService<PasswordRequestValidator>(),
                services.GetRequiredService<ViewModelRenavigate<AuthorizationViewModel>>());
        }

        private static TeacherRegistrationViewModel CreateTeacherRegistrationViewModel(IServiceProvider services)
        {
            return new TeacherRegistrationViewModel(
                services.GetRequiredService<ViewModelRenavigate<AuthorizationViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<StudentRegistrationViewModel>>());
        }

        private static StudentRegistrationViewModel CreateStudentRegistrationViewModel(IServiceProvider services)
        {
            return new StudentRegistrationViewModel(
                services.GetRequiredService<IUserAccountService>(),
                services.GetRequiredService<RegisterRequestValidator>(),
                services.GetRequiredService<ViewModelRenavigate<AuthorizationViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<TeacherRegistrationViewModel>>());
        }

        private static GroupsViewModel CreateGroupsViewModel(IServiceProvider services)
        {
            return new GroupsViewModel(
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<IDateTimeConverter>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupSettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static GroupActionsViewModel CreateGroupActionsViewModel(IServiceProvider services)
        {
            return new GroupActionsViewModel(
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<IEventsService>(),
                services.GetRequiredService<IStudentsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupSettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static GroupSettingsViewModel CreateGroupSettingsViewModel(IServiceProvider services)
        {
            return new GroupSettingsViewModel(
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<IEventsService>(),
                services.GetRequiredService<IStudentsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static CategoriesViewModel CreateCategoriesViewModel(IServiceProvider services)
        {
            return new CategoriesViewModel(
                services.GetRequiredService<ICategoriesService>(),
                services.GetRequiredService<IDateTimeConverter>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<CategoryActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<CategorySettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static CategorySettingsViewModel CreateCategorySettingsViewModel(IServiceProvider services)
        {
            return new CategorySettingsViewModel(
                services.GetRequiredService<ICategoriesService>(),
                services.GetRequiredService<IQuizzesService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<CategoryActionsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static CategoryActionsViewModel CreateCategoryActionsViewModel(IServiceProvider services)
        {
            return new CategoryActionsViewModel(
                services.GetRequiredService<ICategoriesService>(),
                services.GetRequiredService<IQuizzesService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<CategoriesViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<CategorySettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static EventsViewModel CreateEventsViewModel(IServiceProvider services)
        {
            return new EventsViewModel(
                services.GetRequiredService<IEventsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<IDateTimeConverter>(),
                services.GetRequiredService<ViewModelRenavigate<EventActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<EventSettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static EventActionsViewModel CreateEventActionsViewModel(IServiceProvider services)
        {
            return new EventActionsViewModel(
                services.GetRequiredService<IEventsService>(),
                services.GetRequiredService<IQuizzesService>(),
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<EventsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<EventSettingsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }

        private static EventSettingsViewModel CreateEventSettingsViewModel(IServiceProvider services)
        {
            return new EventSettingsViewModel(
                services.GetRequiredService<IEventsService>(),
                services.GetRequiredService<IQuizzesService>(),
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<EventActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<EventActionsViewModel>>(),
                services.GetRequiredService<IViewDisplayTypeService>());
        }
    }
}