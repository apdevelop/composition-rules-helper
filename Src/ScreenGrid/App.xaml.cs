﻿namespace ScreenGrid
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += AppDispatcherUnhandledException;

            var vm = new ViewModels.ScreenGridViewModel();
            var view = new Views.ScreenGridWindow
            {
                DataContext = vm,
            };

            view.Show();

            // Use for easy debug
            ////vm.GridMode = Models.Grids.GridType.GoldenCircles;
            ////vm.SelectedLineColor = new ViewModels.ColorItemViewModel(System.Windows.Media.Colors.Black);
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(ex.ToString());
            }
            else
            {
                MessageBox.Show("Unhandled Exception");
            }

            Environment.Exit(1);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
