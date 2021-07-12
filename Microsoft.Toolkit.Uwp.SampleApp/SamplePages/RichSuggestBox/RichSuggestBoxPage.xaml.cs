// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RichSuggestBoxPage : Page, IXamlRenderListener
    {
        private readonly List<SampleEmailDataType> _emailSamples = new List<SampleEmailDataType>()
        {
            new SampleEmailDataType() { FirstName = "Marcus", FamilyName = "Perryman" },
            new SampleEmailDataType() { FirstName = "Michael", FamilyName = "Hawker" },
            new SampleEmailDataType() { FirstName = "Matt", FamilyName = "Lacey" },
            new SampleEmailDataType() { FirstName = "Alexandre", FamilyName = "Chohfi" },
            new SampleEmailDataType() { FirstName = "Filip", FamilyName = "Wallberg" },
            new SampleEmailDataType() { FirstName = "Shane", FamilyName = "Weaver" },
            new SampleEmailDataType() { FirstName = "Vincent", FamilyName = "Gromfeld" },
            new SampleEmailDataType() { FirstName = "Sergio", FamilyName = "Pedri" },
            new SampleEmailDataType() { FirstName = "Alex", FamilyName = "Wilber" },
            new SampleEmailDataType() { FirstName = "Allan", FamilyName = "Deyoung" },
            new SampleEmailDataType() { FirstName = "Adele", FamilyName = "Vance" },
            new SampleEmailDataType() { FirstName = "Grady", FamilyName = "Archie" },
            new SampleEmailDataType() { FirstName = "Megan", FamilyName = "Bowen" },
            new SampleEmailDataType() { FirstName = "Ben", FamilyName = "Walters" },
            new SampleEmailDataType() { FirstName = "Debra", FamilyName = "Berger" },
            new SampleEmailDataType() { FirstName = "Emily", FamilyName = "Braun" },
            new SampleEmailDataType() { FirstName = "Christine", FamilyName = "Cline" },
            new SampleEmailDataType() { FirstName = "Enrico", FamilyName = "Catteneo" },
            new SampleEmailDataType() { FirstName = "Davit", FamilyName = "Badalyan" },
            new SampleEmailDataType() { FirstName = "Diego", FamilyName = "Siciliani" },
            new SampleEmailDataType() { FirstName = "Raul", FamilyName = "Razo" },
            new SampleEmailDataType() { FirstName = "Miriam", FamilyName = "Graham" },
            new SampleEmailDataType() { FirstName = "Lynne", FamilyName = "Robbins" },
            new SampleEmailDataType() { FirstName = "Lydia", FamilyName = "Holloway" },
            new SampleEmailDataType() { FirstName = "Nestor", FamilyName = "Wilke" },
            new SampleEmailDataType() { FirstName = "Patti", FamilyName = "Fernandez" },
            new SampleEmailDataType() { FirstName = "Pradeep", FamilyName = "Gupta" },
            new SampleEmailDataType() { FirstName = "Joni", FamilyName = "Sherman" },
            new SampleEmailDataType() { FirstName = "Isaiah", FamilyName = "Langer" },
            new SampleEmailDataType() { FirstName = "Irvin", FamilyName = "Sayers" },
            new SampleEmailDataType() { FirstName = "Tung", FamilyName = "Huynh" },
        };

        private readonly List<SampleDataType> _samples = new List<SampleDataType>()
        {
            new SampleDataType() { Text = "Account", Icon = Symbol.Account },
            new SampleDataType() { Text = "Add Friend", Icon = Symbol.AddFriend },
            new SampleDataType() { Text = "Attach", Icon = Symbol.Attach },
            new SampleDataType() { Text = "Attach Camera", Icon = Symbol.AttachCamera },
            new SampleDataType() { Text = "Audio", Icon = Symbol.Audio },
            new SampleDataType() { Text = "Block Contact", Icon = Symbol.BlockContact },
            new SampleDataType() { Text = "Calculator", Icon = Symbol.Calculator },
            new SampleDataType() { Text = "Calendar", Icon = Symbol.Calendar },
            new SampleDataType() { Text = "Camera", Icon = Symbol.Camera },
            new SampleDataType() { Text = "Contact", Icon = Symbol.Contact },
            new SampleDataType() { Text = "Favorite", Icon = Symbol.Favorite },
            new SampleDataType() { Text = "Link", Icon = Symbol.Link },
            new SampleDataType() { Text = "Mail", Icon = Symbol.Mail },
            new SampleDataType() { Text = "Map", Icon = Symbol.Map },
            new SampleDataType() { Text = "Phone", Icon = Symbol.Phone },
            new SampleDataType() { Text = "Pin", Icon = Symbol.Pin },
            new SampleDataType() { Text = "Rotate", Icon = Symbol.Rotate },
            new SampleDataType() { Text = "Rotate Camera", Icon = Symbol.RotateCamera },
            new SampleDataType() { Text = "Send", Icon = Symbol.Send },
            new SampleDataType() { Text = "Tags", Icon = Symbol.Tag },
            new SampleDataType() { Text = "UnFavorite", Icon = Symbol.UnFavorite },
            new SampleDataType() { Text = "UnPin", Icon = Symbol.UnPin },
            new SampleDataType() { Text = "Zoom", Icon = Symbol.Zoom },
            new SampleDataType() { Text = "ZoomIn", Icon = Symbol.ZoomIn },
            new SampleDataType() { Text = "ZoomOut", Icon = Symbol.ZoomOut },
        };

        private RichSuggestBox _rsb;
        private RichSuggestBox _tsb;
        private DispatcherQueueTimer _hoveringTimer;

        public RichSuggestBoxPage()
        {
            this.InitializeComponent();
            this._hoveringTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            Loaded += (sender, e) => { this.OnXamlRendered(this); };
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            if (this._rsb != null)
            {
                this._rsb.SuggestionChosen -= this.SuggestingBox_OnSuggestionChosen;
                this._rsb.SuggestionsRequested -= this.SuggestingBox_OnSuggestionsRequested;
            }

            if (this._tsb != null)
            {
                this._tsb.SuggestionChosen -= this.SuggestingBox_OnSuggestionChosen;
                this._tsb.SuggestionsRequested -= this.SuggestingBox_OnSuggestionsRequested;
                this._tsb.TokenHovered -= SuggestingBox_OnTokenHovered;
            }

            if (control.FindChild("SuggestingBox") is RichSuggestBox rsb)
            {
                this._rsb = rsb;
                this._rsb.SuggestionChosen += this.SuggestingBox_OnSuggestionChosen;
                this._rsb.SuggestionsRequested += this.SuggestingBox_OnSuggestionsRequested;
            }

            if (control.FindChild("PlainTextSuggestingBox") is RichSuggestBox tsb)
            {
                this._tsb = tsb;
                this._tsb.SuggestionChosen += this.SuggestingBox_OnSuggestionChosen;
                this._tsb.SuggestionsRequested += this.SuggestingBox_OnSuggestionsRequested;
                this._tsb.TokenHovered += this.SuggestingBox_OnTokenHovered;
            }

            if (control.FindChild("TokenListView1") is ListView tls1)
            {
                tls1.ItemsSource = this._rsb?.Tokens;
            }

            if (control.FindChild("TokenListView2") is ListView tls2)
            {
                tls2.ItemsSource = this._tsb?.Tokens;
            }
        }

        private async void SuggestingBox_OnTokenHovered(RichSuggestBox sender, RichSuggestTokenHoveredEventArgs args)
        {
            await Task.Delay(1);
            var flyout = (Flyout)FlyoutBase.GetAttachedFlyout(sender);
            if (flyout?.Content is ContentPresenter cp && sender.TextDocument.Selection.Type != SelectionType.Normal &&
                (!flyout.IsOpen || cp.Content != args.Token.Item))
            {
                cp.Content = args.Token.Item;
                flyout.ShowAt(sender, new FlyoutShowOptions
                {
                    Position = args.CurrentPoint.Position,
                    ExclusionRect = args.Rect,
                    ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway
                });
            }
        }

        private void SuggestingBox_OnSuggestionChosen(RichSuggestBox sender, SuggestionChosenEventArgs args)
        {
            if (args.Prefix == "#")
            {
                args.Format.Background = Colors.DarkOrange;
                args.Format.Foreground = Colors.OrangeRed;
                args.Text = ((SampleDataType)args.SelectedItem).Text;
            }
            else
            {
                args.Text = ((SampleEmailDataType)args.SelectedItem).DisplayName;
            }
        }

        private void SuggestingBox_OnSuggestionsRequested(RichSuggestBox sender, SuggestionsRequestedEventArgs args)
        {
            if (args.Prefix == "#")
            {
                sender.ItemsSource =
                    this._samples.Where(x => x.Text.Contains(args.Query, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                sender.ItemsSource =
                    this._emailSamples.Where(x => x.DisplayName.Contains(args.Query, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
