// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// This class provides static helper methods for background tasks.
    /// </summary>
    public static class BackgroundTaskHelper
    {
        /// <summary>
        /// Determines whether a background task is registered.
        /// </summary>
        /// <param name="backgroundTaskName">The name of the background task class</param>
        /// <returns><c>true</c> if a background task was registered; otherwise, <c>false</c>.</returns>
        public static bool IsBackgroundTaskRegistered(string backgroundTaskName)
        {
            return BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == backgroundTaskName);
        }

        /// <summary>
        /// Determines whether a background task is registered.
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task. This class has to implement IBackgroundTask</param>
        /// <returns><c>true</c> if a background task was registered; otherwise, <c>false</c>.</returns>
        public static bool IsBackgroundTaskRegistered(Type backgroundTaskType)
        {
            return IsBackgroundTaskRegistered(backgroundTaskType.Name);
        }

        /// <summary>
        /// Registers a background task with conditions.
        /// If the task is already registered, return null.
        /// Or set <paramref name="forceRegister"/> to true to un-register the old one and then re-register.
        /// </summary>
        /// <param name="backgroundTaskName">Name of the background task class</param>
        /// <param name="backgroundTaskEntryPoint">Entry point of the background task.</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <param name="forceRegister">Indicate if the background task will be force installed in the case of being already registered</param>
        /// <param name="enforceConditions">Indicate if the background task should quit if condition is no longer valid</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns>Background Task that was registered with the system</returns>
        public static BackgroundTaskRegistration Register(string backgroundTaskName, string backgroundTaskEntryPoint, IBackgroundTrigger trigger, bool forceRegister = false, bool enforceConditions = true, params IBackgroundCondition[] conditions)
        {
            // Check if the task is already registered.
            if (IsBackgroundTaskRegistered(backgroundTaskName))
            {
                BackgroundTaskRegistration previouslyRegistered = GetBackgroundTask(backgroundTaskName) as BackgroundTaskRegistration;

                if (forceRegister)
                {
                    Unregister(previouslyRegistered);
                }
                else
                {
                    return null;
                }
            }

            // build the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = backgroundTaskName
            };

            // check if we are registering in SPM mode
            if (backgroundTaskEntryPoint != string.Empty)
            {
                builder.TaskEntryPoint = backgroundTaskEntryPoint;
            }

            builder.CancelOnConditionLoss = enforceConditions;

            // add the conditions if we have them.
            conditions?.ToList().ForEach(builder.AddCondition);

            builder.SetTrigger(trigger);

            // Register it
            BackgroundTaskRegistration registered = builder.Register();

            return registered;
        }

        /// <summary>
        /// Registers a background task with conditions.
        /// If the task is already registered and has the same trigger, returns the existing registration if it has the same trigger.
        /// If the task is already registered but has different trigger, return null by default.
        /// Or set <paramref name="forceRegister"/> to true to un-register the old one and then re-register.
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task. This class has to implement IBackgroundTask</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <param name="forceRegister">Indicate if the background task will be force installed in the case of being already registered</param>
        /// <param name="enforceConditions">Indicate if the background task should quit if condition is no longer valid</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns>Background Task that was registered with the system</returns>
        public static BackgroundTaskRegistration Register(Type backgroundTaskType, IBackgroundTrigger trigger, bool forceRegister = false, bool enforceConditions = true, params IBackgroundCondition[] conditions)
        {
            return Register(backgroundTaskType.Name, backgroundTaskType.FullName, trigger, forceRegister, enforceConditions, conditions);
        }

        /// <summary>
        /// Registers under the Single Process Model. WARNING: Single Process Model only works with Windows 10 Anniversary Update (14393) and later.
        /// Registers a background task with conditions.
        /// If the task is already registered and has the same trigger, returns the existing registration if it has the same trigger.
        /// If the task is already registered but has different trigger, return null by default.
        /// Or set <paramref name="forceRegister"/> to true to un-register the old one and then re-register.
        /// </summary>
        /// <param name="backgroundTaskName">The name of the background task class</param>
        /// <param name="trigger">Trigger that indicate when the background task should be invoked</param>
        /// <param name="forceRegister">Indicate if the background task will be force installed in the case of being already registered</param>
        /// <param name="enforceConditions">Indicate if the background task should quit if condition is no longer valid</param>
        /// <param name="conditions">Optional conditions for the background task to run with</param>
        /// <returns>Background Task that was registered with the system</returns>
        public static BackgroundTaskRegistration Register(string backgroundTaskName, IBackgroundTrigger trigger, bool forceRegister = false, bool enforceConditions = true, params IBackgroundCondition[] conditions)
        {
            return Register(backgroundTaskName, string.Empty, trigger, forceRegister, enforceConditions, conditions);
        }

        /// <summary>
        /// Unregisters a background task.
        /// </summary>
        /// <param name="backgroundTaskType">The type of the background task</param>
        /// <param name="forceExit">Force the background task to quit if it is currently running (at the time of unregistering). Default value is true.</param>
        public static void Unregister(Type backgroundTaskType, bool forceExit = true)
        {
            Unregister(backgroundTaskType.Name, forceExit);
        }

        /// <summary>
        /// Unregisters a background task.
        /// </summary>
        /// <param name="backgroundTaskName">The name of the background task class</param>
        /// <param name="forceExit">Force the background task to quit if it is currently running (at the time of unregistering). Default value is true.</param>
        public static void Unregister(string backgroundTaskName, bool forceExit = true)
        {
            Unregister(GetBackgroundTask(backgroundTaskName), forceExit);
        }

        /// <summary>
        /// Unregisters a background task.
        /// </summary>
        /// <param name="backgroundTask">A background task that was previously registered with the system</param>
        /// <param name="forceExit">Force the background task to quit if it is currently running (at the time of unregistering). Default value is true.</param>
        public static void Unregister(IBackgroundTaskRegistration backgroundTask, bool forceExit = true)
        {
            backgroundTask?.Unregister(forceExit);
        }

        /// <summary>
        /// Returns the registered background task of the given type.
        /// </summary>
        /// <param name="backgroundTaskType">Type of the background task class. This class has to implement IBackgroundTask</param>
        /// <returns>The registered background task if it exists; otherwise, <c>null</c>.</returns>
        public static IBackgroundTaskRegistration GetBackgroundTask(Type backgroundTaskType)
        {
            return GetBackgroundTask(backgroundTaskType.Name);
        }

        /// <summary>
        /// Returns the registered background task of the given type.
        /// </summary>
        /// <param name="backgroundTaskName">Name of the background task class</param>
        /// <returns>The registered background task if it exists; otherwise, <c>null</c>.</returns>
        public static IBackgroundTaskRegistration GetBackgroundTask(string backgroundTaskName)
        {
            return BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == backgroundTaskName).Value;
        }
    }
}