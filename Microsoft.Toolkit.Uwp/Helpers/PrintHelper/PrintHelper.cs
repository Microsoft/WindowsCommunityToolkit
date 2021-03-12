// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Helper class used to simplify document printing.
    /// It allows you to render a framework element per page.
    /// </summary>
    /// <remarks>Based on https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/Printing/cs/PrintHelper.cs</remarks>
    public class PrintHelper : IDisposable
    {
        /// <summary>
        /// Occurs when a print was successful.
        /// </summary>
        public event Action OnPrintSucceeded;

        /// <summary>
        /// Occurs when a print fails.
        /// </summary>
        public event Action OnPrintFailed;

        /// <summary>
        /// Occurs when a print is canceled by the user.
        /// </summary>
        public event Action OnPrintCanceled;

        /// <summary>
        /// Occurs after print preview pages are generated.
        /// </summary>
        /// <remarks>
        /// You can use this event to tweak the final rendering by adding/moving controls in the page.
        /// </remarks>
        public event Action<List<FrameworkElement>> OnPreviewPagesCreated;

        /// <summary>
        /// Gets or sets the percentage of the app's margin width.
        /// </summary>
        public double ApplicationContentMarginLeft { get; set; } = 0.03;

        /// <summary>
        /// Gets or sets the percentage of the app's margin height.
        /// </summary>
        public double ApplicationContentMarginTop { get; set; } = 0.03;

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        private PrintDocument _printDocument;

        /// <summary>
        /// Marker interface for document source.
        /// </summary>
        private IPrintDocumentSource _printDocumentSource;

        /// <summary>
        /// The list of elements used to store the print preview pages.
        /// This gives easy access to any desired preview page.
        /// </summary>
        private List<FrameworkElement> _printPreviewPages;

        /// <summary>
        /// A hidden canvas used to hold pages we wish to print.
        /// </summary>
        private Canvas _printCanvas;

        private Panel _canvasContainer;
        private string _printTaskName;
        private Dictionary<FrameworkElement, PrintHelperStateBag> _stateBags = new Dictionary<FrameworkElement, PrintHelperStateBag>();
        private bool _directPrint = false;

        /// <summary>
        /// The list of elements to print.
        /// </summary>
        private List<FrameworkElement> _elementsToPrint;

        /// <summary>
        /// The options for the print dialog.
        /// </summary>
        private PrintHelperOptions _printHelperOptions;

        /// <summary>
        /// The default options for the print dialog.
        /// </summary>
        private PrintHelperOptions _defaultPrintHelperOptions;

        /// <summary>
        /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintHelper"/> class.
        /// </summary>
        /// <param name="canvasContainer">XAML panel used to attach printing canvas. Can be hidden in your UI with Opacity = 0 for instance</param>
        /// <param name="defaultPrintHelperOptions">Default settings for the print tasks</param>
        /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates, or null if this is being called from the UI thread.</param>
        public PrintHelper(Panel canvasContainer, PrintHelperOptions defaultPrintHelperOptions = null, DispatcherQueue dispatcherQueue = null)
        {
            DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            if (canvasContainer == null)
            {
                throw new ArgumentNullException();
            }

            _printPreviewPages = new List<FrameworkElement>();
            _printCanvas = new Canvas();
            _printCanvas.Opacity = 0;

            _canvasContainer = canvasContainer;
            _canvasContainer.RequestedTheme = ElementTheme.Light;

            _elementsToPrint = new List<FrameworkElement>();

            _defaultPrintHelperOptions = defaultPrintHelperOptions ?? new PrintHelperOptions();

            RegisterForPrinting();
        }

        /// <summary>
        /// Adds an element to the list of elements to print.
        /// </summary>
        /// <param name="element">Framework element to print.</param>
        /// <remarks>The element cannot have a parent; It must not be included in any visual tree.</remarks>
        public void AddFrameworkElementToPrint(FrameworkElement element)
        {
            if (element.Parent != null)
            {
                throw new ArgumentException("Printable elements cannot have a parent.");
            }

            _elementsToPrint.Add(element);
        }

        /// <summary>
        /// Removes an element from the list of elements to print.
        /// </summary>
        /// <param name="element">Framework element to remove</param>
        public void RemoveFrameworkElementToPrint(FrameworkElement element)
        {
            _elementsToPrint.Remove(element);
        }

        /// <summary>
        /// Removes all elements from the list of elements to print.
        /// </summary>
        public void ClearListOfPrintableFrameworkElements()
        {
            _elementsToPrint.Clear();
        }

        /// <summary>
        /// Starts the print task.
        /// </summary>
        /// <param name="printTaskName">Name of the print task to use</param>
        /// <param name="directPrint">Directly print the content of the container instead of relying on list built with <see cref="AddFrameworkElementToPrint(FrameworkElement)"/> method</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ShowPrintUIAsync(string printTaskName, bool directPrint = false)
        {
            this._directPrint = directPrint;

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            // Launch print process
            _printTaskName = printTaskName;
            await PrintManager.ShowPrintUIAsync();
        }

        /// <summary>
        /// Starts the print task.
        /// </summary>
        /// <param name="printTaskName">Name of the print task to use</param>
        /// <param name="printHelperOptions">Settings for the print task</param>
        /// <param name="directPrint">Directly print the content of the container instead of relying on list built with <see cref="AddFrameworkElementToPrint(FrameworkElement)"/> method</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ShowPrintUIAsync(string printTaskName, PrintHelperOptions printHelperOptions, bool directPrint = false)
        {
            _printHelperOptions = printHelperOptions;

            return ShowPrintUIAsync(printTaskName, directPrint);
        }

        /// <summary>
        /// Release associated resources
        /// </summary>
        public void Dispose()
        {
            if (_printDocument == null)
            {
                return;
            }

            _printCanvas = null;
            DispatcherQueue.EnqueueAsync(() =>
            {
                _printDocument.Paginate -= CreatePrintPreviewPages;
                _printDocument.GetPreviewPage -= GetPrintPreviewPage;
                _printDocument.AddPages -= AddPrintPages;
            });
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        private void RegisterForPrinting()
        {
            _printDocument = new PrintDocument();
            _printDocumentSource = _printDocument.DocumentSource;
            _printDocument.Paginate += CreatePrintPreviewPages;
            _printDocument.GetPreviewPage += GetPrintPreviewPage;
            _printDocument.AddPages += AddPrintPages;
        }

        private async Task DetachCanvas()
        {
            if (!_directPrint)
            {
                await DispatcherQueue.EnqueueAsync(() =>
                {
                    _canvasContainer.Children.Remove(_printCanvas);
                    _printCanvas.Children.Clear();
                });
            }

            _stateBags.Clear();

            // Clear the cache of preview pages
            await ClearPageCache();

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs</param>
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask(_printTaskName, sourceRequested =>
            {
                ApplyPrintSettings(printTask);

                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    await DispatcherQueue.EnqueueAsync(
                        async () =>
                        {
                            foreach (var element in _stateBags.Keys)
                            {
                                _stateBags[element].Restore(element);
                            }

                            _stateBags.Clear();
                            _canvasContainer.RequestedTheme = ElementTheme.Default;
                            await DetachCanvas();

                            switch (args.Completion)
                            {
                                case PrintTaskCompletion.Failed:
                                    OnPrintFailed?.Invoke();
                                    break;

                                case PrintTaskCompletion.Canceled:
                                    OnPrintCanceled?.Invoke();
                                    break;

                                case PrintTaskCompletion.Submitted:
                                    OnPrintSucceeded?.Invoke();
                                    break;
                            }
                        }, DispatcherQueuePriority.Normal);
                };

                sourceRequested.SetSource(_printDocumentSource);
            });
        }

        private void ApplyPrintSettings(PrintTask printTask)
        {
            _printHelperOptions = _printHelperOptions ?? _defaultPrintHelperOptions;

            IEnumerable<string> displayedOptionsToAdd = _printHelperOptions.DisplayedOptions;

            if (!_printHelperOptions.ExtendDisplayedOptions)
            {
                printTask.Options.DisplayedOptions.Clear();
            }

            foreach (var displayedOption in displayedOptionsToAdd)
            {
                if (!printTask.Options.DisplayedOptions.Contains(displayedOption))
                {
                    printTask.Options.DisplayedOptions.Add(displayedOption);
                }
            }

            if (printTask.Options.Binding != PrintBinding.NotAvailable)
            {
                printTask.Options.Binding = _printHelperOptions.Binding == PrintBinding.Default ? printTask.Options.Binding : _printHelperOptions.Binding;
            }

            if (printTask.Options.Bordering != PrintBordering.NotAvailable)
            {
                printTask.Options.Bordering = _printHelperOptions.Bordering == PrintBordering.Default ? printTask.Options.Bordering : _printHelperOptions.Bordering;
            }

            if (printTask.Options.MediaType != PrintMediaType.NotAvailable)
            {
                printTask.Options.MediaType = _printHelperOptions.MediaType == PrintMediaType.Default ? printTask.Options.MediaType : _printHelperOptions.MediaType;
            }

            if (printTask.Options.MediaSize != PrintMediaSize.NotAvailable)
            {
                printTask.Options.MediaSize = _printHelperOptions.MediaSize == PrintMediaSize.Default ? printTask.Options.MediaSize : _printHelperOptions.MediaSize;
            }

            if (printTask.Options.HolePunch != PrintHolePunch.NotAvailable)
            {
                printTask.Options.HolePunch = _printHelperOptions.HolePunch == PrintHolePunch.Default ? printTask.Options.HolePunch : _printHelperOptions.HolePunch;
            }

            if (printTask.Options.Duplex != PrintDuplex.NotAvailable)
            {
                printTask.Options.Duplex = _printHelperOptions.Duplex == PrintDuplex.Default ? printTask.Options.Duplex : _printHelperOptions.Duplex;
            }

            if (printTask.Options.ColorMode != PrintColorMode.NotAvailable)
            {
                printTask.Options.ColorMode = _printHelperOptions.ColorMode == PrintColorMode.Default ? printTask.Options.ColorMode : _printHelperOptions.ColorMode;
            }

            if (printTask.Options.Collation != PrintCollation.NotAvailable)
            {
                printTask.Options.Collation = _printHelperOptions.Collation == PrintCollation.Default ? printTask.Options.Collation : _printHelperOptions.Collation;
            }

            if (printTask.Options.PrintQuality != PrintQuality.NotAvailable)
            {
                printTask.Options.PrintQuality = _printHelperOptions.PrintQuality == PrintQuality.Default ? printTask.Options.PrintQuality : _printHelperOptions.PrintQuality;
            }

            if (printTask.Options.Staple != PrintStaple.NotAvailable)
            {
                printTask.Options.Staple = _printHelperOptions.Staple == PrintStaple.Default ? printTask.Options.Staple : _printHelperOptions.Staple;
            }

            if (printTask.Options.Orientation != PrintOrientation.NotAvailable)
            {
                printTask.Options.Orientation = _printHelperOptions.Orientation == PrintOrientation.Default ? printTask.Options.Orientation : _printHelperOptions.Orientation;
            }

            _printHelperOptions = null;
        }

        /// <summary>
        /// This is the event handler for <see cref="PrintDocument.Paginate"/>.
        /// It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        private async void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            // Get the PrintTaskOptions
            PrintTaskOptions printingOptions = e.PrintTaskOptions;

            // Get the page description to determine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            if (_directPrint)
            {
                _canvasContainer.RequestedTheme = ElementTheme.Light;
                foreach (FrameworkElement element in this._canvasContainer.Children)
                {
                    _printPreviewPages.Add(element);
                }
            }
            else
            {
                // Attach the canvas
                if (!_canvasContainer.Children.Contains(_printCanvas))
                {
                    _canvasContainer.Children.Add(_printCanvas);
                }

                _canvasContainer.RequestedTheme = ElementTheme.Light;

                // Clear the cache of preview pages
                await ClearPageCache();

                // Clear the print canvas of preview pages
                _printCanvas.Children.Clear();

                var printPageTasks = new List<Task>();
                foreach (var element in _elementsToPrint)
                {
                    printPageTasks.Add(AddOnePrintPreviewPage(element, pageDescription));
                }

                await Task.WhenAll(printPageTasks);
            }

            OnPreviewPagesCreated?.Invoke(_printPreviewPages);

            PrintDocument printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            _printCanvas.UpdateLayout();
            printDoc.SetPreviewPageCount(_printPreviewPages.Count, PreviewPageCountType.Intermediate);
            printDoc.SetPreviewPage(_printPreviewPages.Count, _printPreviewPages[_printPreviewPages.Count - 1]);
        }

        /// <summary>
        /// This is the event handler for <see cref="PrintDocument.GetPreviewPage"/>.
        /// It provides a specific print preview page, in the form of an UIElement, to an instance of PrintDocument.
        /// PrintDocument subsequently converts the UIElement into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;
            if (_printPreviewPages.Count() > 0)
            {
                printDoc.SetPreviewPage(e.PageNumber, _printPreviewPages[e.PageNumber - 1]);
            }
        }

        /// <summary>
        /// This is the event handler for <see cref="PrintDocument.AddPages"/>.
        /// It provides all pages to be printed, in the form of UIElements, to an instance of PrintDocument.
        /// PrintDocument subsequently converts the UIElements into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to add each page to be printed
            for (int i = 0; i < _printPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                _printDocument.AddPage(_printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// Creates and adds one print preview page to the internal cache of print preview pages stored in <see cref="_printPreviewPages"/>.
        /// </summary>
        /// <param name="element">FrameworkElement used to represent the "printing page"</param>
        /// <param name="printPageDescription">Printer's page description</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private Task AddOnePrintPreviewPage(FrameworkElement element, PrintPageDescription printPageDescription)
        {
            var page = new Page();

            // Save state
            if (!_stateBags.ContainsKey(element))
            {
                var stateBag = new PrintHelperStateBag(DispatcherQueue);
                stateBag.Capture(element);
                _stateBags.Add(element, stateBag);
            }

            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            // Get the margins size
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set up the "printable area" on the "paper"
            element.VerticalAlignment = VerticalAlignment.Top;
            element.HorizontalAlignment = HorizontalAlignment.Left;

            if (element.Width > element.Height)
            {
                var newWidth = page.Width - marginWidth;

                element.Height = element.Height * (newWidth / element.Width);
                element.Width = newWidth;
            }
            else
            {
                var newHeight = page.Height - marginHeight;

                element.Width = element.Width * (newHeight / element.Height);
                element.Height = newHeight;
            }

            element.Margin = new Thickness(marginWidth / 2, marginHeight / 2, marginWidth / 2, marginHeight / 2);
            page.Content = element;

            return DispatcherQueue.EnqueueAsync(
                () =>
                {
                    // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
                    // through layout so that the linked containers correctly distribute the content inside them.
                    _printCanvas.Children.Add(page);
                    _printCanvas.UpdateLayout();
                    _printCanvas.InvalidateMeasure();

                    // Add the page to the page preview collection
                    _printPreviewPages.Add(page);
                }, DispatcherQueuePriority.High);
        }

        private Task ClearPageCache()
        {
            return DispatcherQueue.EnqueueAsync(() =>
            {
                if (!_directPrint)
                {
                    foreach (Page page in _printPreviewPages)
                    {
                        page.Content = null;
                    }
                }

                _printPreviewPages.Clear();
            });
        }
    }
}
