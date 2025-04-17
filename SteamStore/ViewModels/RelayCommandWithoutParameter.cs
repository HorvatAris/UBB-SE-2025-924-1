// <copyright file="RelayCommandWithoutParameter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class RelayCommandWithoutParameter : ICommand
    {
            private readonly Action execute;
            private readonly Func<bool> canExecute;

            public RelayCommandWithoutParameter(Action execute, Func<bool> canExecute = null)
            {
                this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
                this.canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => this.canExecute?.Invoke() ?? true;

            public void Execute(object parameter) => this.execute();

            public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
