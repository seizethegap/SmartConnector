using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using SmartConnector.ViewModels;

namespace SmartConnector.Commands
{
    public class RemoteControlCommand : ICommand
    {
        public RemoteControlCommand(SmartConnectorViewModel view_model)
        {
            _ViewModel = view_model;
        }

        private SmartConnectorViewModel _ViewModel;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _ViewModel.RemoteControlState();
        }
    }
}
