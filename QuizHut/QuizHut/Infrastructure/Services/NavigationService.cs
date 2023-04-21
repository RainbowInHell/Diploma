﻿namespace QuizHut.Infrastructure.Services
{
    using System;

    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;
    using QuizHut.ViewModels.Contracts;

    internal class NavigationService : ViewModel, INavigationService
    {
        private ViewModel currentView;
        private readonly Func<Type, ViewModel> _viewModelFactory;

        public ViewModel CurrentView
        {
            get => currentView;
            private set
            {
                Set(ref currentView, value);
            }
        }
        
        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            NavigateTo(typeof(TViewModel));
        }

        public void NavigateTo(Type viewModelType)
        {
            ViewModel viewModel = _viewModelFactory.Invoke(viewModelType);
            if (viewModel is IResettable resettable)
            {
                resettable.Reset();
            }
            CurrentView = viewModel;
        }
    }
}