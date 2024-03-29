﻿namespace QuizHut.Infrastructure.Commands.Base
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using QuizHut.Infrastructure.Commands.Base.Contracts;

    public abstract class CommandAsync : ICommandAsync
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object? parameter);

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        public abstract Task ExecuteAsync(object? parameter);

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}