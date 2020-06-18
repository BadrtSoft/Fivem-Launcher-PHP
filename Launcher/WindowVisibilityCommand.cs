using System;
using System.Windows;
using System.Windows.Input;

namespace Launcher
{
    public class WindowVisibilityCommand: ICommand
    {
        public void Execute(object parameter)
        {
            Application.Current.MainWindow.Visibility = Application.Current.MainWindow.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}