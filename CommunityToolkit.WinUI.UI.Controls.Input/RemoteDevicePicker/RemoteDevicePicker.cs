// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System.RemoteSystems;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Picker Control to show List of Remote Devices that are accessible
    /// </summary>
    [TemplatePart(Name = "PART_ListDevices", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_ListDeviceTypes", Type = typeof(ComboBox))]
    [TemplatePart(Name = "PART_ProgressRing", Type = typeof(ProgressRing))]
    [TemplatePart(Name = "DiscoveryType", Type = typeof(ComboBox))]
    [TemplatePart(Name = "StatusType", Type = typeof(ComboBox))]
    [TemplatePart(Name = "AuthorizationType", Type = typeof(ComboBox))]
    [TemplatePart(Name = "AdvancedFiltersGrid", Type = typeof(Grid))]
    public sealed partial class RemoteDevicePicker : ContentDialog
    {
        private ListView _listDevices;
        private ComboBox _listDeviceTypes;
        private ProgressRing _progressRing;
        private Grid _commandSpace;
        private ComboBox _deviceDiscovery;
        private ComboBox _deviceStatus;
        private ComboBox _authorizationType;
        private Grid _advancedFiltersGrid;
        private RemoteSystemDiscoveryTypeFilter _discoveryFilter;
        private RemoteSystemAuthorizationKindFilter _authorizationKindFilter;
        private RemoteSystemStatusTypeFilter _statusFilter;

        /// <summary>
        /// Gets or sets List of All Remote Systems based on Selection Filter
        /// </summary>
        private ObservableCollection<RemoteSystem> RemoteSystems { get; set; }

        /// <summary>
        /// Gets or sets the DeviceList Selection Mode. Defaults to ListViewSelectionMode.Single
        /// </summary>
        public RemoteDeviceSelectionMode SelectionMode
        {
            get { return (RemoteDeviceSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="SelectionMode"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            nameof(SelectionMode),
            typeof(RemoteDeviceSelectionMode),
            typeof(RemoteDevicePicker),
            new PropertyMetadata(RemoteDeviceSelectionMode.Single));

        /// <summary>
        /// Gets or sets a value indicating whether Advanced Filters visible or not
        /// </summary>
        public bool ShowAdvancedFilters
        {
            get { return (bool)GetValue(ShowAdvancedFiltersProperty); }
            set { SetValue(ShowAdvancedFiltersProperty, value); }
        }

        /// <summary>
        /// Gets the dependency property for <see cref="ShowAdvancedFilters"/>.
        /// </summary>
        public static readonly DependencyProperty ShowAdvancedFiltersProperty = DependencyProperty.Register(
            nameof(ShowAdvancedFilters),
            typeof(bool),
            typeof(RemoteDevicePicker),
            new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDevicePicker"/> class.
        /// </summary>
        public RemoteDevicePicker()
        {
            DefaultStyleKey = typeof(RemoteDevicePicker);
            RemoteSystems = new ObservableCollection<RemoteSystem>();
            PrimaryButtonClick += RemoteDevicePicker_PrimaryButtonClick;

            _discoveryFilter = new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Any);
            _authorizationKindFilter = new RemoteSystemAuthorizationKindFilter(RemoteSystemAuthorizationKind.SameUser);
            _statusFilter = new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Any);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDevicePicker"/> class with filters.
        /// </summary>
        public RemoteDevicePicker(RemoteSystemDiscoveryType remoteSystemDiscoveryType, RemoteSystemAuthorizationKind remoteSystemAuthorizationKind, RemoteSystemStatusType remoteSystemStatusType)
        {
            DefaultStyleKey = typeof(RemoteDevicePicker);
            RemoteSystems = new ObservableCollection<RemoteSystem>();
            PrimaryButtonClick += RemoteDevicePicker_PrimaryButtonClick;

            _discoveryFilter = new RemoteSystemDiscoveryTypeFilter(remoteSystemDiscoveryType);
            _authorizationKindFilter = new RemoteSystemAuthorizationKindFilter(remoteSystemAuthorizationKind);
            _statusFilter = new RemoteSystemStatusTypeFilter(remoteSystemStatusType);
        }

        private void RemoteDevicePicker_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (_listDevices.SelectedItems.Count > 0)
            {
                _listDevices.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Initiate Picker UI
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IEnumerable<RemoteSystem>> PickDeviceAsync()
        {
            await ShowAsync();
            IEnumerable<RemoteSystem> devices = ReturnDevices();
            return devices;
        }

        internal IEnumerable<RemoteSystem> ReturnDevices()
        {
            List<RemoteSystem> returnValue = new List<RemoteSystem>();
            foreach (RemoteSystem item in _listDevices.SelectedItems)
            {
                returnValue.Add(item);
            }

            return returnValue.AsEnumerable();
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            Focus(FocusState.Programmatic);

            _listDevices = GetTemplateChild("PART_ListDevices") as ListView;
            _listDeviceTypes = GetTemplateChild("PART_ListDeviceTypes") as ComboBox;
            _progressRing = GetTemplateChild("PART_ProgressRing") as ProgressRing;
            _commandSpace = GetTemplateChild("CommandSpace") as Grid;
            _deviceDiscovery = GetTemplateChild("DiscoveryType") as ComboBox;
            _deviceStatus = GetTemplateChild("StatusType") as ComboBox;
            _authorizationType = GetTemplateChild("AuthorizationType") as ComboBox;
            _advancedFiltersGrid = GetTemplateChild("AdvancedFiltersGrid") as Grid;

            UnhookEvents();

            var deviceList = typeof(RemoteSystemKinds).GetProperties().Select(a => a.Name).ToList();
            deviceList.Add("All");
            deviceList.Add("Unknown");

            if (_listDeviceTypes != null)
            {
                _listDeviceTypes.ItemsSource = deviceList.OrderBy(a => a.ToString());
                _listDeviceTypes.SelectedIndex = 0;
                _listDeviceTypes.SelectionChanged += ListDeviceTypes_SelectionChanged;
            }

            if (_listDevices != null)
            {
                _listDevices.SelectionChanged += ListDevices_SelectionChanged;
                _listDevices.ContainerContentChanging += ListDevices_ContainerContentChanging;

                _listDevices.SelectionMode = SelectionMode == RemoteDeviceSelectionMode.Single ? ListViewSelectionMode.Single : ListViewSelectionMode.Multiple;
                _listDevices.IsMultiSelectCheckBoxEnabled = SelectionMode == RemoteDeviceSelectionMode.Multiple ? true : false;
                if (_listDevices.SelectionMode == ListViewSelectionMode.Single)
                {
                    _listDevices.DoubleTapped += ListDevices_DoubleTapped;
                }
            }

            if (_advancedFiltersGrid != null)
            {
                _advancedFiltersGrid.Visibility = ShowAdvancedFilters ? Visibility.Visible : Visibility.Collapsed;
            }

            LoadFilters();

            if (_deviceDiscovery != null)
            {
                _deviceDiscovery.SelectionChanged += Filters_SelectionChanged;
            }

            if (_authorizationType != null)
            {
                _authorizationType.SelectionChanged += Filters_SelectionChanged;
            }

            if (_deviceStatus != null)
            {
                _deviceStatus.SelectionChanged += Filters_SelectionChanged;
            }

            BuildFilters(_discoveryFilter, _authorizationKindFilter, _statusFilter);

            IsSecondaryButtonEnabled = false;

            UpdateProgressRing(true);
            UpdateList();

            base.OnApplyTemplate();
        }

        private void UnhookEvents()
        {
            if (_listDeviceTypes != null)
            {
                _listDeviceTypes.SelectionChanged -= ListDeviceTypes_SelectionChanged;
            }

            if (_listDevices != null)
            {
                _listDevices.SelectionChanged -= ListDevices_SelectionChanged;
                _listDevices.DoubleTapped -= ListDevices_DoubleTapped;
                _listDevices.ContainerContentChanging -= ListDevices_ContainerContentChanging;
                _listDeviceTypes.SelectedIndex = -1;
            }

            if (_deviceDiscovery != null)
            {
                _deviceDiscovery.SelectionChanged -= Filters_SelectionChanged;
            }

            if (_authorizationType != null)
            {
                _authorizationType.SelectionChanged -= Filters_SelectionChanged;
            }

            if (_deviceStatus != null)
            {
                _deviceStatus.SelectionChanged -= Filters_SelectionChanged;
            }
        }

        private void Filters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _discoveryFilter = new RemoteSystemDiscoveryTypeFilter((RemoteSystemDiscoveryType)Enum.Parse(typeof(RemoteSystemDiscoveryType), _deviceDiscovery?.SelectedValue.ToString()));
            _authorizationKindFilter = new RemoteSystemAuthorizationKindFilter((RemoteSystemAuthorizationKind)Enum.Parse(typeof(RemoteSystemAuthorizationKind), _authorizationType?.SelectedValue.ToString()));
            _statusFilter = new RemoteSystemStatusTypeFilter((RemoteSystemStatusType)Enum.Parse(typeof(RemoteSystemStatusType), _deviceStatus?.SelectedValue.ToString()));

            BuildFilters(_discoveryFilter, _authorizationKindFilter, _statusFilter);
        }

        private void LoadFilters()
        {
            var discoveryType = typeof(RemoteSystemDiscoveryType).GetEnumNames().OrderBy(a => a.ToString()).ToList();
            if (_deviceDiscovery != null)
            {
                _deviceDiscovery.ItemsSource = discoveryType;
                _deviceDiscovery.SelectedIndex = 0;
                if (_discoveryFilter != null)
                {
                    _deviceDiscovery.SelectedValue = _discoveryFilter.RemoteSystemDiscoveryType.ToString();
                }
            }

            var statusType = typeof(RemoteSystemStatusType).GetEnumNames().OrderBy(a => a.ToString()).ToList();
            if (_deviceStatus != null)
            {
                _deviceStatus.ItemsSource = statusType;
                _deviceStatus.SelectedIndex = 0;
                if (_statusFilter != null)
                {
                    _deviceStatus.SelectedValue = _statusFilter.RemoteSystemStatusType.ToString();
                }
            }

            var authType = typeof(RemoteSystemAuthorizationKind).GetEnumNames().OrderBy(a => a.ToString()).ToList();
            if (_authorizationType != null)
            {
                _authorizationType.ItemsSource = authType;
                _authorizationType.SelectedIndex = 0;
                if (_authorizationKindFilter != null)
                {
                    _authorizationType.SelectedValue = _authorizationKindFilter.RemoteSystemAuthorizationKind.ToString();
                }
            }
        }

        private void BuildFilters(RemoteSystemDiscoveryTypeFilter discoveryFilter, RemoteSystemAuthorizationKindFilter authorizationKindFilter, RemoteSystemStatusTypeFilter statusFilter)
        {
            var filters = new List<IRemoteSystemFilter>();
            if (discoveryFilter != null)
            {
                filters.Add(discoveryFilter);
            }

            if (authorizationKindFilter != null)
            {
                filters.Add(authorizationKindFilter);
            }

            if (statusFilter != null)
            {
                filters.Add(statusFilter);
            }

            var remoteDeviceHelper = new RemoteDeviceHelper(filters);
            RemoteSystems = remoteDeviceHelper.RemoteSystems;

            UpdateProgressRing(true);
            UpdateList();
        }

        private void ListDevices_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var model = (RemoteSystem)args.Item;
            switch (model.Status)
            {
                case RemoteSystemStatus.Available:
                    args.ItemContainer.IsEnabled = true;
                    args.ItemContainer.IsHitTestVisible = true;
                    break;

                case RemoteSystemStatus.DiscoveringAvailability:
                case RemoteSystemStatus.Unavailable:
                case RemoteSystemStatus.Unknown:
                    args.ItemContainer.IsEnabled = false;
                    args.ItemContainer.IsHitTestVisible = false;
                    break;
            }
        }

        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listDevices.SelectedItems.Count > 0)
            {
                IsSecondaryButtonEnabled = true;
            }
            else
            {
                IsSecondaryButtonEnabled = false;
            }
        }

        private void ListDevices_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ReturnDevices();
            Hide();
        }

        private void ListDeviceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateList();

        private void UpdateList()
        {
            var bindingList = new ObservableCollection<RemoteSystem>();
            if (RemoteSystems != null && _listDeviceTypes != null && _listDevices != null)
            {
                var bindinglist = _listDeviceTypes.SelectedValue.ToString().Equals("All")
                    ? RemoteSystems
                    : new ObservableCollection<RemoteSystem>(RemoteSystems.Where(a => a.Kind.Equals(_listDeviceTypes.SelectedValue.ToString(), StringComparison.OrdinalIgnoreCase)).ToList());

                _listDevices.ItemsSource = bindinglist;
                UpdateProgressRing(false);
            }
        }

        private void UpdateProgressRing(bool state)
        {
            if (_progressRing != null)
            {
                _progressRing.IsActive = state;
            }
        }
    }
}