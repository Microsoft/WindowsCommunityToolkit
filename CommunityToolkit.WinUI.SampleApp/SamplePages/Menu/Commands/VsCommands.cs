// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.Menu.Commands
{
#pragma warning disable SA1649 // File name must match first type name
    internal class NewProjectCommand : ICommand
#pragma warning restore SA1649 // File name must match first type name
    {
#pragma warning disable CS0067 // An event was declared but never used in the class in which it was declared.
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // An event was declared but never used in the class in which it was declared.

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = "Create New Project",
                CloseButtonText = "Close",
                XamlRoot = Shell.Current.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    internal class NewFileCommand : ICommand
#pragma warning restore SA1402 // File may only contain a single class
    {
#pragma warning disable CS0067 // An event was declared but never used in the class in which it was declared.
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // An event was declared but never used in the class in which it was declared.

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = "Create New File",
                CloseButtonText = "Close",
                XamlRoot = Shell.Current.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    internal class GenericCommand : ICommand
#pragma warning restore SA1402 // File may only contain a single class
    {
#pragma warning disable CS0067 // An event was declared but never used in the class in which it was declared.
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // An event was declared but never used in the class in which it was declared.

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var dialog = new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = parameter.ToString(),
                CloseButtonText = "Close",
                XamlRoot = Shell.Current.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}