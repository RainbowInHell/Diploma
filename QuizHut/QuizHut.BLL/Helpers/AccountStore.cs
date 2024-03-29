﻿namespace QuizHut.BLL.Helpers
{
    using QuizHut.BLL.Helpers.Contracts;
    using QuizHut.DLL.Entities;

    public class AccountStore : IAccountStore
    {
        private ApplicationUser currentUser;
        public ApplicationUser CurrentUser
        {
            get => currentUser;
            set
            {
                currentUser = value;
                StateChanged?.Invoke();
            }
        }

        public UserRole CurrentUserRole { get ; set; } = UserRole.Unauthorised;

        public event Action StateChanged;
    }
}