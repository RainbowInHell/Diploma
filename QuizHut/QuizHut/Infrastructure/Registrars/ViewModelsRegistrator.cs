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
            services.AddSingleton<CreateViewModel<EventsViewModel>>(services => () => services.GetRequiredService<EventsViewModel>());

            services.AddSingleton<CreateViewModel<GroupsViewModel>>(services => () => CreateGroupsViewModel(services));
            services.AddSingleton<CreateViewModel<GroupActionsViewModel>>(services => () => CreateGroupActionsViewModel(services));
            services.AddSingleton<CreateViewModel<GroupSettingsViewModel>>(services => () => CreateGroupSettingsViewModel(services));

            services.AddSingleton<CreateViewModel<CategoriesViewModel>>(services => () => services.GetRequiredService<CategoriesViewModel>());
            services.AddSingleton<CreateViewModel<CategoryActionsViewModel>>(services => () => services.GetRequiredService<CategoryActionsViewModel>());
            services.AddSingleton<CreateViewModel<CategorySettingsViewModel>>(services => () => services.GetRequiredService<CategorySettingsViewModel>());

            services.AddSingleton<CreateViewModel<QuizzesViewModel>>(services => () => services.GetRequiredService<QuizzesViewModel>());
            services.AddSingleton<CreateViewModel<StudentsViewModel>>(services => () => services.GetRequiredService<StudentsViewModel>());

            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<UserProfileViewModel>();
            services.AddTransient<ResultsViewModel>();
            services.AddTransient<EventsViewModel>();

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

            services.AddSingleton<ViewModelRenavigate<GroupActionsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<GroupsViewModel>>();
            services.AddSingleton<ViewModelRenavigate<GroupSettingsViewModel>>();

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
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupSettingsViewModel>>(),
                services.GetRequiredService<IGroupSettingsTypeService>());
        }

        private static GroupActionsViewModel CreateGroupActionsViewModel(IServiceProvider services)
        {
            return new GroupActionsViewModel(
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<IStudentsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupSettingsViewModel>>(),
                services.GetRequiredService<IGroupSettingsTypeService>());
        }

        private static GroupSettingsViewModel CreateGroupSettingsViewModel(IServiceProvider services)
        {
            return new GroupSettingsViewModel(
                services.GetRequiredService<IGroupsService>(),
                services.GetRequiredService<IStudentsService>(),
                services.GetRequiredService<ISharedDataStore>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<ViewModelRenavigate<GroupActionsViewModel>>(),
                services.GetRequiredService<IGroupSettingsTypeService>());
        }
    }
}