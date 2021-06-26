// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Microsoft.Toolkit.Uwp.SampleApp.Menu.Commands
{
#pragma warning disable SA1649 // File name must match first type name
    internal class NewProjectCommand : ICommand
#pragma warning restore SA1649 // File name must match first type name
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new MessageDialog("Create New Project");
            await dialog.ShowAsync();
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    internal class NewFileCommand : ICommand
#pragma warning restore SA1402 // File may only contain a single class
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new MessageDialog("Create New File");
            await dialog.ShowAsync();
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    internal class GenericCommand : ICommand
#pragma warning restore SA1402 // File may only contain a single class
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new MessageDialog(parameter.ToString());
            await dialog.ShowAsync();
        }
    }
}