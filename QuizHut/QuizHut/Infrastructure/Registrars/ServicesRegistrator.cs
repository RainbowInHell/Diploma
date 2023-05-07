﻿namespace QuizHut.Infrastructure.Registrars
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using QuizHut.BLL.Dto.DtoValidators;
    using QuizHut.BLL.Expression.Contracts;
    using QuizHut.BLL.Expression;
    using QuizHut.BLL.Services;
    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Services;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.Services;

    using SendGrid.Extensions.DependencyInjection;

    public static class ServicesRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSendGrid(opt =>
                opt.ApiKey = configuration.GetValue<string>("EmailSender:ApiKey"));

            services.AddSingleton<LoginRequestValidator>();
            services.AddSingleton<RegisterRequestValidator>();
            services.AddSingleton<EmailRequestValidator>();
            services.AddSingleton<PasswordRequestValidator>();

            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            services.AddSingleton<IUserAccountService, UserAccountService>();
            services.AddSingleton<IStudentService, StudentService>();
            services.AddSingleton<IExpressionBuilder, ExpressionBuilder>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserDialogService, UserDialogService>();

            return services;
        }
    }
}