using LibraryManagementSystem.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementSystem.ViewModel
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            MouseUpMinimizeCommand = new ClickEventCommand(OnMouseUpMinimize);
            MouseUpShutdownCommand = new ClickEventCommand(OnMouseUpShutdown);
        }

        public ICommand MouseUpMinimizeCommand { get; }
        public ICommand MouseUpShutdownCommand { get; }

        private void OnMouseUpMinimize(object parameter)
        {
            if (parameter is Window window)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void OnMouseUpShutdown(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}

