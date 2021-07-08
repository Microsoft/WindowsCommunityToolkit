// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm.Input
{
    /// <summary>
    /// An interface expanding <see cref="IRelayCommand"/> to support asynchronous operations.
    /// </summary>
    public interface IAsyncRelayCommand : IRelayCommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the last scheduled <see cref="Task"/>, if available.
        /// This property notifies a change when the <see cref="Task"/> completes.
        /// </summary>
        Task? ExecutionTask { get; }

        /// <summary>
        /// Gets a value indicating whether running operations for this command can be canceled.
        /// </summary>
        bool CanBeCanceled { get; }

        /// <summary>
        /// Gets a value indicating whether a cancelation request has been issued for the current operation.
        /// </summary>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// Gets a value indicating whether the command currently has a pending operation being executed.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Provides a more specific version of <see cref="System.Windows.Input.ICommand.Execute"/>,
        /// also returning the <see cref="Task"/> representing the async operation being executed.
        /// </summary>
        /// <param name="parameter">The input parameter.</param>
        /// <returns>The <see cref="Task"/> representing the async operation being executed.</returns>
        Task ExecuteAsync(object? parameter);

        /// <summary>
        /// Communicates a request for cancelation.
        /// </summary>
        /// <remarks>
        /// If the underlying command is not running, or if it does not support cancelation, this method will perform no action.
        /// Note that even with a successful cancelation, the completion of the current operation might not be immediate.
        /// </remarks>
        void Cancel();
    }
}