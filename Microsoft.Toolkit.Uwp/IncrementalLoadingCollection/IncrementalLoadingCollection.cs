// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp
{
    /// <summary>
    /// This class represents an <see cref="ObservableCollection{IType}"/> whose items can be loaded incrementally.
    /// </summary>
    /// <typeparam name="TSource">
    /// The data source that must be loaded incrementally.
    /// </typeparam>
    /// <typeparam name="IType">
    /// The type of collection items.
    /// </typeparam>
    /// <seealso cref="IIncrementalSource{TSource}"/>
    /// <seealso cref="ISupportIncrementalLoading"/>
    public class IncrementalLoadingCollection<TSource, IType> : ObservableCollection<IType>,
         ISupportIncrementalLoading
         where TSource : Collections.IIncrementalSource<IType>
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called when a retrieval operation begins.
        /// </summary>
        public Action OnStartLoading { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called when a retrieval operation ends.
        /// </summary>
        public Action OnEndLoading { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called if an error occurs during data retrieval. The actual <see cref="Exception"/> is passed as an argument.
        /// </summary>
        public Action<Exception> OnError { get; set; }

        /// <summary>
        /// Gets a value indicating the source of incremental loading.
        /// </summary>
        protected TSource Source { get; }

        /// <summary>
        /// Gets a value indicating how many items that must be retrieved for each incremental call.
        /// </summary>
        protected int ItemsPerPage { get; }

        /// <summary>
        /// Gets or sets a value indicating The zero-based index of the current items page.
        /// </summary>
        protected int CurrentPageIndex { get; set; }

        private bool _isLoading;
        private bool _hasMoreItems;
        private CancellationToken _cancellationToken;
        private bool _refreshOnLoad;

        /// <summary>
        /// Gets a value indicating whether new items are being loaded.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            private set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));

                    if (_isLoading)
                    {
                        OnStartLoading?.Invoke();
                    }
                    else
                    {
                        OnEndLoading?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains more items to retrieve.
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                return _hasMoreItems;
            }

            private set
            {
                if (value != _hasMoreItems)
                {
                    _hasMoreItems = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasMoreItems)));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingCollection{TSource, IType}"/> class optionally specifying how many items to load for each data page.
        /// </summary>
        /// <param name="itemsPerPage">
        /// The number of items to retrieve for each call. Default is 20.
        /// </param>
        /// <param name="onStartLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation begins.
        /// </param>
        /// <param name="onEndLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation ends.
        /// </param>
        /// <param name="onError">
        /// An <see cref="Action"/> that is called if an error occurs during data retrieval.
        /// </param>
        /// <seealso cref="IIncrementalSource{TSource}"/>
        public IncrementalLoadingCollection(int itemsPerPage = 20, Action onStartLoading = null, Action onEndLoading = null, Action<Exception> onError = null)
            : this(Activator.CreateInstance<TSource>(), itemsPerPage, onStartLoading, onEndLoading, onError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingCollection{TSource, IType}"/> class using the specified <see cref="IIncrementalSource{TSource}"/> implementation and, optionally, how many items to load for each data page.
        /// </summary>
        /// <param name="source">
        /// An implementation of the <see cref="IIncrementalSource{TSource}"/> interface that contains the logic to actually load data incrementally.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of items to retrieve for each call. Default is 20.
        /// </param>
        /// <param name="onStartLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation begins.
        /// </param>
        /// <param name="onEndLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation ends.
        /// </param>
        /// <param name="onError">
        /// An <see cref="Action"/> that is called if an error occurs during data retrieval.
        /// </param>
        /// <seealso cref="IIncrementalSource{TSource}"/>
        public IncrementalLoadingCollection(TSource source, int itemsPerPage = 20, Action onStartLoading = null, Action onEndLoading = null, Action<Exception> onError = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;

            OnStartLoading = onStartLoading;
            OnEndLoading = onEndLoading;
            OnError = onError;

            ItemsPerPage = itemsPerPage;
            _hasMoreItems = true;
        }

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">
        /// The number of items to load.
        /// </param>
        /// <returns>
        /// An object of the <see cref="LoadMoreItemsAsync(uint)"/> that specifies how many items have been actually retrieved.
        /// </returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();

        /// <summary>
        /// Clears the collection and triggers/forces a reload of the first page
        /// </summary>
        /// <returns>This method does not return a result</returns>
        public Task RefreshAsync()
        {
            if (IsLoading)
            {
                _refreshOnLoad = true;
            }
            else
            {
                var previousCount = Count;
                Clear();
                CurrentPageIndex = 0;
                HasMoreItems = true;

                if (previousCount == 0)
                {
                    // When the list was empty before clearing, the automatic reload isn't fired, so force a reload.
                    return LoadMoreItemsAsync(0).AsTask();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Actually performs the incremental loading.
        /// </summary>
        /// <param name="cancellationToken">
        /// Used to propagate notification that operation should be canceled.
        /// </param>
        /// <returns>
        /// Returns a collection of <typeparamref name="IType"/>.
        /// </returns>
        protected virtual async Task<IEnumerable<IType>> LoadDataAsync(CancellationToken cancellationToken)
        {
            var result = await Source.GetPagedItemsAsync(CurrentPageIndex, ItemsPerPage, cancellationToken)
                .ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            throw t.Exception;
                        }

                        if (t.IsCompletedSuccessfully)
                        {
                            CurrentPageIndex += 1;
                        }

                        return t.Result;
                    }, cancellationToken);

            return result;
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            uint resultCount = 0;
            _cancellationToken = cancellationToken;

            // TODO (2021.05.05): Make use common AsyncMutex class.
            // AsyncMutex is located at Microsoft.Toolkit.Uwp.UI.Media/Extensions/System.Threading.Tasks/AsyncMutex.cs at the time of this note.
            await _mutex.WaitAsync();
            try
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    IEnumerable<IType> data = null;
                    try
                    {
                        IsLoading = true;
                        data = await LoadDataAsync(_cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // The operation has been canceled using the Cancellation Token.
                    }
                    catch (Exception ex) when (OnError != null)
                    {
                        OnError.Invoke(ex);
                    }

                    if (data != null && data.Any() && !_cancellationToken.IsCancellationRequested)
                    {
                        resultCount = (uint)data.Count();

                        foreach (var item in data)
                        {
                            Add(item);
                        }
                    }
                    else
                    {
                        HasMoreItems = false;
                    }
                }
            }
            finally
            {
                IsLoading = false;

                if (_refreshOnLoad)
                {
                    _refreshOnLoad = false;
                    await RefreshAsync();
                }

                _mutex.Release();
            }

            return new LoadMoreItemsResult { Count = resultCount };
        }
    }
}