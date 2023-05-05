﻿namespace QuizHut.Views.Windows
{
    using System.Windows;
    using System.Windows.Input;

    public partial class LoginView : Window
    {
        public LoginView(object dataContex)
        {
            InitializeComponent();

            DataContext = dataContex;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}