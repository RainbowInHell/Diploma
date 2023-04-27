﻿namespace QuizHut.Infrastructure.Registrars
{
    using Microsoft.Extensions.DependencyInjection;
    using QuizHut.ViewModels.LoginViewModels;
    using QuizHut.ViewModels.MainViewModels;
    using QuizHut.Views.Windows;

    public static class ViewsRegistrator
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<LoginViewModel>();
                    var window = new LoginView { DataContext = model };

                    return window;
                });

            services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<MainViewModel>();
                    var window = new MainView { DataContext = model };

                    return window;
                });

            //services.AddTransient<MainView>();

            return services;
        }
    }
}