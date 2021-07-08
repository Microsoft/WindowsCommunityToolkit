// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// The <see cref="InvokeActionsActivity"/> is an <see cref="Activity"/> which allows bridging to performing any behavior based <see cref="IAction"/> within the schedule.
    /// </summary>
    [ContentProperty(Name = nameof(Actions))]
    public class InvokeActionsActivity : Activity
    {
        /// <summary>
        /// Identifies the <seealso cref="Actions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            nameof(Actions),
            typeof(ActionCollection),
            typeof(InvokeActionsActivity),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                if (GetValue(ActionsProperty) is not ActionCollection actionCollection)
                {
                    actionCollection = new ActionCollection();

                    SetValue(ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        /// <inheritdoc/>
        public override async Task InvokeAsync(UIElement element, CancellationToken token)
        {
            await base.InvokeAsync(element, token);

            Interaction.ExecuteActions(element, Actions, EventArgs.Empty);
        }
    }
}