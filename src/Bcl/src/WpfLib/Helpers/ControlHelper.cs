using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Library.ComponentModel;
using Library.Data.Models;
using Library.Wpf.Bases;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Key = System.Windows.Input.Key;

namespace Library.Wpf.Helpers;

public static class ControlHelper
{
    public static DataGrid AddColumns(this DataGrid dataGrid, IEnumerable<IDataColumnBindingInfo> dataColumns)
    {
        Check.MustBeArgumentNotNull(dataGrid);
        dataColumns.ToDataGridColumn().ForEach(dataGrid.Columns.Add);
        return dataGrid;
    }

    public static void ApplyFilter(this ItemCollection items, Predicate<object> predicate)
    {
        foreach (var item in items.Cast<TreeViewItem>())
        {
            ApplyFilter(item.Items, predicate);
        }
        items.Filter = predicate;
    }

    public static THeaderedItemsControl BindDataContext<THeaderedItemsControl>(this THeaderedItemsControl itemsControl, object dataContext, string? header = null)
        where THeaderedItemsControl : HeaderedItemsControl
    {
        Check.MustBeArgumentNotNull(itemsControl);
        return InnerBindDataContext(itemsControl, dataContext, header);
    }

    public static TTreeView BindItems<TTreeView>(this TTreeView treeView, IEnumerable? items) where TTreeView : TreeView =>
        BindItems<TTreeView, TreeViewItem>(treeView, items);

    public static TItemsControl BindItems<TItemsControl, THeaderedItemsControl>(this TItemsControl treeView, IEnumerable? items)
        where TItemsControl : ItemsControl
        where THeaderedItemsControl : HeaderedItemsControl, new()
    {
        Check.MustBeArgumentNotNull(treeView);

        treeView.ItemsSource = null;
        treeView.Items.Clear();
        if (items is not null)
        {
            foreach (var item in items)
            {
                _ = treeView.Items.Add(new THeaderedItemsControl().InnerBindDataContext(item));
            }
        }
        return treeView;
    }

    public static TSelector BindItemsSource<TSelector>(this TSelector selector, IEnumerable? items)
            where TSelector : Selector
    {
        BindItemsSourceInner(selector, items, null);
        return selector;
    }

    public static TSelector BindItemsSource<TSelector>(this TSelector selector, IEnumerable? items, string? displayMemberPath)
            where TSelector : Selector
    {
        BindItemsSourceInner(selector, items, displayMemberPath);
        return selector;
    }

    public static TSelector BindItemsSource<TSelector>(this TSelector selector, IEnumerable? items, string? displayMemberPath, int? selectedIndex)
            where TSelector : Selector
    {
        BindItemsSourceInner(selector, items, displayMemberPath);

        if (selectedIndex is not null)
        {
            selector.SelectedIndex = selectedIndex.Value;
        }
        return selector;
    }

    public static TSelector BindItemsSource<TSelector>(this TSelector selector, IEnumerable? items, string? displayMemberPath, object? selectedValue)
            where TSelector : Selector
    {
        BindItemsSourceInner(selector, items, displayMemberPath);

        if (selectedValue is not null)
        {
            selector.SelectedValue = selectedValue;
        }
        return selector;
    }

    public static TreeView BindItemsSource([DisallowNull] this TreeView itemsControl, IEnumerable items, string? displayMemberPath = null)
    {
        Check.MustBeArgumentNotNull(itemsControl);

        itemsControl.Rebind(ItemsControl.ItemsSourceProperty);
        itemsControl.ItemsSource = null;
        itemsControl.ItemsSource = items;
        if (displayMemberPath is not null)
        {
            itemsControl.DisplayMemberPath = displayMemberPath;
        }
        itemsControl.Rebind(ItemsControl.ItemsSourceProperty);
        return itemsControl;
    }

    public static Selector BindItemsSourceToEnum<TEnum>(this Selector selector, TEnum? selectedItem = null) where TEnum : struct
        => BindItemsSource(selector, Enum.GetValues(typeof(TEnum)), null, selectedItem);

    public static TreeViewItem BindNewTreeViewItem(object dataContext, string? header = null) =>
            new TreeViewItem().BindDataContext(dataContext, header);

    public static TreeViewItem BindNewTreeViewItems(this TreeViewItem parentItem, IEnumerable items)
    {
        Check.MustBeArgumentNotNull(parentItem);
        if (items is not null)
        {
            foreach (var item in items)
            {
                _ = parentItem.Items.Add(BindNewTreeViewItem(item));
            }
        }

        return parentItem;
    }

    public static void BindToElementPath(this UIElement source,
            FrameworkElement target,
            DependencyProperty targetDependencyProperty,
            string path,
            BindingMode bindingMode = BindingMode.TwoWay) =>
            target?.SetBinding(targetDependencyProperty, new Binding { Source = source, Path = new PropertyPath(path), Mode = bindingMode });

    public static void EnsureVisible(this ListViewItem item) =>
            item.ArgumentNotNull().Parent.Cast().As<ListView>()?.ScrollIntoView(item);

    public static void EnsureVisibleItem(this DataGrid dg, object item) =>
            dg.ArgumentNotNull().ScrollIntoView(item);

    public static void EnsureVisibleItem(this ListBox lv, object item) =>
            lv.ArgumentNotNull().ScrollIntoView(item);

    public static void ExpandAll(this TreeViewItem item, bool isExpanded = true)
    {
        Check.MustBeArgumentNotNull(item);

        var parent = item;
        do
        {
            parent.IsExpanded = isExpanded;
        } while ((parent = parent?.Parent.Cast().As<TreeViewItem>()) is not null);

        item.BringIntoView();
    }

    [return: NotNullIfNotNull(nameof(treeView))]
    public static TreeView? FilterItems(
        this TreeView? treeView,
        string? filterText,
        Func<TreeViewItem, string?> getItemText,
        in ItemCollection? roots = null,
        StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        Check.MustBeArgumentNotNull(getItemText);

        var theRoots = roots.IfNull(treeView.ArgumentNotNull().Items).Cast<TreeViewItem?>();

        foreach (var item in theRoots.Compact())
        {
            filterTreeViewItems(item, filterText, getItemText, stringComparison);
        }
        return treeView;

        static void filterTreeViewItems(in TreeViewItem item, in string? filterText, in Func<TreeViewItem, string?> getItemText, in StringComparison stringComparison)
        {
            var itemText = getItemText(item);
            item.Visibility = filterText.IsNullOrEmpty() || (itemText?.Contains(filterText, stringComparison) ?? false)
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (item.Visibility == Visibility.Visible)
            {
                item.IsExpanded = true;
                var parent = item.Parent as TreeViewItem;
                while (parent is not null)
                {
                    parent.Visibility = Visibility.Visible;
                    parent.IsExpanded = true;
                    parent = parent.Parent as TreeViewItem;
                }
            }

            foreach (TreeViewItem child in item.Items)
            {
                filterTreeViewItems(child, filterText, getItemText, stringComparison);
            }
        }
    }

    public static void Flick(FrameworkElement element, int duration = 500) => CodeHelper.Catch(() => Animations.Flick(element, duration));

    public static IEnumerable<TreeViewItem> GetAllItems([DisallowNull] this TreeView tree) =>
            EnumerableHelper.SelectAllChildren(tree.Items.Cast<TreeViewItem>(), item => item.Items.Cast<TreeViewItem>());

    public static DataGridCell? GetCell(this DataGrid grid, int row, int column)
    {
        Check.MustBeArgumentNotNull(grid);
        var rowContainer = grid.GetRow(row);
        if (rowContainer is null)
        {
            return null;
        }

        var presenter = rowContainer.GetVisualChild<DataGridCellsPresenter>();
        if (presenter is null)
        {
            return null;
        }
        var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        if (cell is not null)
        {
            return cell;
        }

        grid.ScrollIntoView(rowContainer, grid.Columns[column]);
        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        return cell;
    }

    public static IEnumerable<TControl> GetChildren<TControl>(this Visual visual, bool loopThrough = true)
        => visual.ArgumentNotNull().GetChildren(loopThrough).OfType<TControl>();

    public static IEnumerable<Visual> GetChildren(this Visual visual, bool loopThrough = true)
    {
        var result = new List<Visual>();
        EnumerableHelper.ForEach(visual,
            c =>
            {
                return !loopThrough && !Equals(c, visual)
                    ? []
                    : c is not TabItem tabItem
                        ? Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(c)).Select(i => (Visual)VisualTreeHelper.GetChild(c, i))
                        : tabItem.Content is Visual
                            ? ([tabItem.Content.Cast().As<Visual>()!, tabItem.Header.Cast().As<Visual>()!])
                            : Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(c)).Select(i => (Visual)VisualTreeHelper.GetChild(c, i));
            },
            result.Add,
            null);
        return result.RemoveNulls();
    }

    public static DependencyProperty GetDependencyProperty<TType, TOwnerType>(string propertyName,
            Action<TOwnerType, DependencyPropertyChangedEventArgs>? onDependencyPropertyChanged = null,
            Action<TOwnerType, string>? onPropertyChanged = null,
            Func<TType, bool>? validateValue = null,
            CoerceValueCallback? coerceValueCallback = null,
            TType? defaultValue = default) where TOwnerType : class
    {
        return coerceValueCallback is null
            ? DependencyProperty.Register(
                propertyName,
                typeof(TType),
                typeof(TOwnerType),
                new FrameworkPropertyMetadata(
                    defaultValue ?? default,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    callback)
                { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                value => validateValue is null || validateValue((TType)value))
            : DependencyProperty.Register(
                propertyName,
                typeof(TType),
                typeof(TOwnerType),
                new FrameworkPropertyMetadata(
                    defaultValue ?? default,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    callback,
                    coerceValueCallback));
        void callback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not TOwnerType owner)
            {
                return;
            }

            onDependencyPropertyChanged?.Invoke(owner, e);
            onPropertyChanged?.Invoke(owner, e.Property.Name);
        }
    }

    public static ItemsControl? GetGrandParent(this TreeViewItem item)
    {
        var parent = VisualTreeHelper.GetParent(item);

        while (parent is not (TreeViewItem or TreeView))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as ItemsControl;
    }

    public static IntPtr GetHandle(this Window window) =>
            new WindowInteropHelper(window).Handle;

    public static bool GetIsViewModelChanged<TPage>(this TPage page)
            where TPage : IStatefulPage => page.IsViewModelChanged;

    public static TModel? GetModel<TModel>(this TreeViewItem? item)
        where TModel : class => item?.DataContext as TModel;

    public static TModel? GetModel<TModel>(this RoutedPropertyChangedEventArgs<object> e)
        where TModel : class => e?.NewValue.Cast().As<TreeViewItem>()?.DataContext.Cast().As<TModel>();

    public static TParent? GetParentByType<TParent>(this DependencyObject depObj)
            where TParent : DependencyObject
    {
        var parent = depObj;
        while ((parent = VisualTreeHelper.GetParent(parent)) is not null)
        {
            if (parent is TParent)
            {
                return parent as TParent;
            }
        }

        return null;
    }

    public static Window GetParentWindow(this DependencyObject dependencyObject) =>
            Window.GetWindow(dependencyObject);

    public static DataGridRow GetRow(this DataGrid grid, int index)
    {
        Check.MustBeArgumentNotNull(grid);

        var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
        if (row is not null)
        {
            return row;
        }

        grid.UpdateLayout();
        grid.ScrollIntoView(grid.Items[index]);
        row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
        return row;
    }

    public static T? GetSelectedModel<T>(this TreeView treeView) where T : class
            => treeView.ArgumentNotNull().SelectedItem.Cast().As<TreeViewItem>().GetModel<T>();

    [Obsolete("Use listView.GetSelection, instead.")]
    public static T? GetSelection<T>(this SelectionChangedEventArgs e)
            => e.ArgumentNotNull().AddedItems.Cast<object?>().FirstOrDefault().Cast().To<T?>();

    public static T? GetSelection<T>([DisallowNull] this ListView listView, SelectionChangedEventArgs e)
            => e?.AddedItems.Any() is true ? e.AddedItems[0].Cast().To<T?>() : GetSelections<T>(listView).FirstOrDefault();

    public static IEnumerable<T?> GetSelections<T>([DisallowNull] this ListView listView)
            => listView.ArgumentNotNull().SelectedItems.Cast<object?>().Select(x => x.Cast().To<T?>());

    public static string GetText([DisallowNull] this RichTextBox rtb)
    {
        Check.MustBeArgumentNotNull(rtb);

        var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        return textRange.Text;
    }

    public static TViewModel? GetViewModelByDataContext<TViewModel>(this Page page, Func<TViewModel?>? getDefaultViewModel = null)
            where TViewModel : class => page.DataContext is TViewModel viewModel ? viewModel : (getDefaultViewModel?.Invoke() ?? default);

    public static T? GetVisualChild<T>(this Visual parent) where T : Visual
    {
        var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (var index = 0; index < numVisuals; index++)
        {
            var v = (Visual)VisualTreeHelper.GetChild(parent, index);
            var child = v as T ?? GetVisualChild<T>(v);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    public static System.Windows.Forms.IWin32Window GetWin32Window(this Window window) =>
            new Win32WindowForm(window);

    public static TItemsControl HandleChanges<TItemsControl, TNotifyCollectionChanged>(this TItemsControl control, TNotifyCollectionChanged collection, Action<TItemsControl, TNotifyCollectionChanged, NotifyCollectionChangedEventArgs> handler)
            where TItemsControl : ItemsControl
            where TNotifyCollectionChanged : INotifyCollectionChanged
    {
        collection.CollectionChanged += (e1, e2) => handler?.Invoke(control, collection, e2);
        return control;
    }

    public static void HandleKeyDown(this MultiSelector multiSelector, KeyEventArgs e)
    {
        Check.MustBeArgumentNotNull(multiSelector);

        Check.MustBeArgumentNotNull(e);

        switch (e.Key)
        {
            case Key.Home when Keyboard.Modifiers == ModifierKeys.Control:
                multiSelector.SelectedIndex = 0;
                break;

            case Key.End when Keyboard.Modifiers == ModifierKeys.Control:
                multiSelector.SelectedIndex = multiSelector.Items.Count;
                break;

            case Key.A when Keyboard.Modifiers == ModifierKeys.Control:
                multiSelector.SelectAll();
                break;

            case Key.Space:
                {
                    var items = multiSelector.SelectedItems.Cast<dynamic>().ToArray();
                    _ = items.Enumerate(instance => instance.IsChecked = items.Any(o => !o.IsChecked));
                    break;
                }

            case Key.None:
                break;

            case Key.Cancel:
                break;

            case Key.Back:
                break;

            case Key.Tab:
                break;

            case Key.LineFeed:
                break;

            case Key.Clear:
                break;

            case Key.Enter:
                break;

            case Key.Pause:
                break;

            case Key.Capital:
                break;

            case Key.HangulMode:
                break;

            case Key.JunjaMode:
                break;

            case Key.FinalMode:
                break;

            case Key.HanjaMode:
                break;

            case Key.Escape:
                break;

            case Key.ImeConvert:
                break;

            case Key.ImeNonConvert:
                break;

            case Key.ImeAccept:
                break;

            case Key.ImeModeChange:
                break;

            case Key.PageUp:
                break;

            case Key.Next:
                break;

            case Key.End:
                break;

            case Key.Home:
                break;

            case Key.Left:
                break;

            case Key.Up:
                break;

            case Key.Right:
                break;

            case Key.Down:
                break;

            case Key.Select:
                break;

            case Key.Print:
                break;

            case Key.Execute:
                break;

            case Key.PrintScreen:
                break;

            case Key.Insert:
                break;

            case Key.Delete:
                break;

            case Key.Help:
                break;

            case Key.D0:
                break;

            case Key.D1:
                break;

            case Key.D2:
                break;

            case Key.D3:
                break;

            case Key.D4:
                break;

            case Key.D5:
                break;

            case Key.D6:
                break;

            case Key.D7:
                break;

            case Key.D8:
                break;

            case Key.D9:
                break;

            case Key.A:
                break;

            case Key.B:
                break;

            case Key.C:
                break;

            case Key.D:
                break;

            case Key.E:
                break;

            case Key.F:
                break;

            case Key.G:
                break;

            case Key.H:
                break;

            case Key.I:
                break;

            case Key.J:
                break;

            case Key.K:
                break;

            case Key.L:
                break;

            case Key.M:
                break;

            case Key.N:
                break;

            case Key.O:
                break;

            case Key.P:
                break;

            case Key.Q:
                break;

            case Key.R:
                break;

            case Key.S:
                break;

            case Key.T:
                break;

            case Key.U:
                break;

            case Key.V:
                break;

            case Key.W:
                break;

            case Key.X:
                break;

            case Key.Y:
                break;

            case Key.Z:
                break;

            case Key.LWin:
                break;

            case Key.RWin:
                break;

            case Key.Apps:
                break;

            case Key.Sleep:
                break;

            case Key.NumPad0:
                break;

            case Key.NumPad1:
                break;

            case Key.NumPad2:
                break;

            case Key.NumPad3:
                break;

            case Key.NumPad4:
                break;

            case Key.NumPad5:
                break;

            case Key.NumPad6:
                break;

            case Key.NumPad7:
                break;

            case Key.NumPad8:
                break;

            case Key.NumPad9:
                break;

            case Key.Multiply:
                break;

            case Key.Add:
                break;

            case Key.Separator:
                break;

            case Key.Subtract:
                break;

            case Key.Decimal:
                break;

            case Key.Divide:
                break;

            case Key.F1:
                break;

            case Key.F2:
                break;

            case Key.F3:
                break;

            case Key.F4:
                break;

            case Key.F5:
                break;

            case Key.F6:
                break;

            case Key.F7:
                break;

            case Key.F8:
                break;

            case Key.F9:
                break;

            case Key.F10:
                break;

            case Key.F11:
                break;

            case Key.F12:
                break;

            case Key.F13:
                break;

            case Key.F14:
                break;

            case Key.F15:
                break;

            case Key.F16:
                break;

            case Key.F17:
                break;

            case Key.F18:
                break;

            case Key.F19:
                break;

            case Key.F20:
                break;

            case Key.F21:
                break;

            case Key.F22:
                break;

            case Key.F23:
                break;

            case Key.F24:
                break;

            case Key.NumLock:
                break;

            case Key.Scroll:
                break;

            case Key.LeftShift:
                break;

            case Key.RightShift:
                break;

            case Key.LeftCtrl:
                break;

            case Key.RightCtrl:
                break;

            case Key.LeftAlt:
                break;

            case Key.RightAlt:
                break;

            case Key.BrowserBack:
                break;

            case Key.BrowserForward:
                break;

            case Key.BrowserRefresh:
                break;

            case Key.BrowserStop:
                break;

            case Key.BrowserSearch:
                break;

            case Key.BrowserFavorites:
                break;

            case Key.BrowserHome:
                break;

            case Key.VolumeMute:
                break;

            case Key.VolumeDown:
                break;

            case Key.VolumeUp:
                break;

            case Key.MediaNextTrack:
                break;

            case Key.MediaPreviousTrack:
                break;

            case Key.MediaStop:
                break;

            case Key.MediaPlayPause:
                break;

            case Key.LaunchMail:
                break;

            case Key.SelectMedia:
                break;

            case Key.LaunchApplication1:
                break;

            case Key.LaunchApplication2:
                break;

            case Key.Oem1:
                break;

            case Key.OemPlus:
                break;

            case Key.OemComma:
                break;

            case Key.OemMinus:
                break;

            case Key.OemPeriod:
                break;

            case Key.Oem2:
                break;

            case Key.Oem3:
                break;

            case Key.AbntC1:
                break;

            case Key.AbntC2:
                break;

            case Key.Oem4:
                break;

            case Key.Oem5:
                break;

            case Key.Oem6:
                break;

            case Key.Oem7:
                break;

            case Key.Oem8:
                break;

            case Key.Oem102:
                break;

            case Key.ImeProcessed:
                break;

            case Key.System:
                break;

            case Key.DbeAlphanumeric:
                break;

            case Key.DbeKatakana:
                break;

            case Key.DbeHiragana:
                break;

            case Key.DbeSbcsChar:
                break;

            case Key.DbeDbcsChar:
                break;

            case Key.DbeRoman:
                break;

            case Key.Attn:
                break;

            case Key.CrSel:
                break;

            case Key.DbeEnterImeConfigureMode:
                break;

            case Key.DbeFlushString:
                break;

            case Key.DbeCodeInput:
                break;

            case Key.DbeNoCodeInput:
                break;

            case Key.DbeDetermineString:
                break;

            case Key.DbeEnterDialogConversionMode:
                break;

            case Key.OemClear:
                break;

            case Key.DeadCharProcessed:
                break;

            default:
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(source))]
    [return: NotNullIfNotNull(nameof(defaultItems))]
    public static ItemCollection? IfNull(this ItemCollection? source, ItemCollection? defaultItems)
        => source ?? defaultItems;

    public static Visual InitializeChildren(Visual parent)
    {
        var children = parent.GetChildren();
        foreach (var child in children)
        {
            if (child is IInitialzable initializable)
            {
                initializable.Initialize();
            }
            _ = InitializeChildren(child);
        }
        return parent;
    }

    public static async Task<Visual> InitializeChildrenAsync(Visual parent)
    {
        var children = parent.GetChildren();
        foreach (var child in children)
        {
            if (child is IAsyncInitialzable initializable)
            {
                await initializable.InitializeAsync();
            }
            _ = InitializeChildren(child);
        }
        return parent;
    }

    public static bool IsDesignTime() =>
        Application.Current?.MainWindow is null;

    public static bool MoveToNextUIElement() =>
        Keyboard.FocusedElement is UIElement elementWithFocus && elementWithFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

    public static void PerformClick(this Button button)
    {
        var peer = new ButtonAutomationPeer(button);
        var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
        invokeProvider?.Invoke();
    }

    public static void Rebind(this DependencyObject target, in DependencyProperty dependencyProperty)
        => BindingOperations.GetBindingExpressionBase(target, dependencyProperty)?.UpdateTarget();

    public static void Rebind(this TextBox target)
        => Rebind(target, TextBox.TextProperty);

    public static void RebindDataContext([DisallowNull] this FrameworkElement element)
        => RebindDataContext(element, element?.DataContext);

    public static void RebindDataContext([DisallowNull] this FrameworkElement element, object? dataContext)
    {
        Check.MustBeArgumentNotNull(element);

        element.DataContext = null;
        element.DataContext = dataContext;
    }

    public static TreeView RebindItemsSource(this TreeView control)
    {
        Check.MustBeArgumentNotNull(control);

        control.Rebind(ItemsControl.ItemsSourceProperty);
        var selectedItem = control.SelectedItem;
        var itemsSource = control.ItemsSource;
        control.ItemsSource = null;
        control.ItemsSource = itemsSource;
        _ = control.SetSelectedItem(selectedItem);
        return control;
    }

    [return: NotNullIfNotNull(nameof(uiElement))]
    public static TUIElement? Refresh<TUIElement>(this TUIElement? uiElement)
        where TUIElement : UIElement
    {
        //uiElement?.Dispatcher.Invoke(Methods.Empty, DispatcherPriority.Render);
        uiElement?.UpdateLayout();
        uiElement?.InvalidateVisual();
        return uiElement;
    }

    public static IEnumerable<dynamic>? RetrieveCheckedItems(this MultiSelector dg)
        => dg?.Items.Cast<dynamic>().Where(item => item.IsChecked == true).Cast<object>().Select(item => item.Cast().As<dynamic>()).Compact();

    public static IEnumerable<TItem>? RetrieveCheckedItems<TItem>(this MultiSelector dg)
            => dg?.Items.Cast<dynamic>().Where(item => item.IsChecked == true).Cast<object>().Select(item => item.Cast().To<TItem>());

    public static TResult RunCodeBlock<TResult>(this FrameworkElement element, [DisallowNull] in Func<TResult> action, [DisallowNull] in ILogger logger, in string? start, in string? end = null, in string? error = null, bool changeMousePointer = true)
    {
        Check.MustBeArgumentNotNull(action);
        Check.MustBeArgumentNotNull(element);
        Check.MustBeArgumentNotNull(logger);
        Check.MustBeArgumentNotNull(action);

        var cursor = element.Cursor;
        try
        {
            if (!start.IsNullOrEmpty())
            {
                logger.Debug(start);
            }
            if (changeMousePointer)
            {
                element.Cursor = Cursors.Wait;
            }

            var result = action();
            if (!end.IsNullOrEmpty())
            {
                logger.Info(end);
            }
            return result;
        }
        catch
        {
            if (!error.IsNullOrEmpty())
            {
                logger.Error(error);
            }

            throw;
        }
        finally
        {
            element.Cursor = cursor;
        }
    }

    public static FrameworkElement RunCodeBlock(this FrameworkElement element, [DisallowNull] Action action, [DisallowNull] in ILogger logger, in string? start, in string? end = null, in string? error = null, bool changeMousePointer = true) => RunCodeBlock(element, () =>
                                                                                                                                                                                                                                                             {
                                                                                                                                                                                                                                                                 action();
                                                                                                                                                                                                                                                                 return element;
                                                                                                                                                                                                                                                             }, logger, start, end, error, changeMousePointer);

    public static async Task<TResult> RunCodeBlockAsync<TResult>(this FrameworkElement element, [DisallowNull] Func<Task<TResult>> action, [DisallowNull] ILogger logger, string? start, string? end = null, string? error = null, bool changeMousePointer = true) => await RunCodeBlock(element, action, logger, start, end, error, changeMousePointer);

    public static void RunInControlThread(this DispatcherObject control, in Action action)
            => control?.Dispatcher.Invoke(action);

    public static TPage SetIsViewModelChanged<TPage>(this TPage page, bool isViewModelChanged)
            where TPage : IStatefulPage
    {
        page.IsViewModelChanged = isViewModelChanged;
        return page;
    }

    public static bool SetProperty<TValue>(
            this INotifyPropertyChanged item,
            ref TValue field,
            in TValue newValue,
            in Action<PropertyChangedEventArgs> invokePropertyChanged,
            [CallerMemberName] in string? propertyName = null)
    {
        if (!Equals(field, newValue))
        {
            field = newValue;
            invokePropertyChanged?.Invoke(new(propertyName));
            return true;
        }
        return false;
    }

    public static TViewModel SetProperty<TViewModel, TValue>(
            this TViewModel item,
            ref TValue field,
            in TValue newValue,
            in Action<string?> invokePropertyChanged,
            [CallerMemberName] in string? propertyName = null)
            where TViewModel : INotifyPropertyChanged
    {
        //if (!Equals(field, newValue))
        {
            field = newValue;
            invokePropertyChanged?.Invoke(propertyName);
        }
        return item;
    }

    /// <summary>
    /// Selects an item in a hierarchical ItemsControl using a set of options
    /// </summary>
    /// <typeparam name="TItem">The type of the items present in the control and in the options</typeparam>
    /// <param name="control">The ItemsControl to select an item in</param>
    /// <param name="info">The options used for the selection process</param>
    public static TItemsControl SetSelectedItem<TItemsControl, TItem>(TItemsControl control, SetSelectedInfo<TItem> info)
        where TItemsControl : ItemsControl
    {
        Check.MustBeArgumentNotNull(control);
        Check.MustBeArgumentNotNull(info?.Items);

        var currentItem = info.Items.First();

        if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        // Compare each item in the container and look for the next item in the chain.
        {
            foreach (var item in control.Items)
            {
                // Convert the item if a conversion method exists. Otherwise just cast the item to
                // the desired type.
                var convertedItem = info.ConvertMethod is not null ? info.ConvertMethod(item) : (TItem)item;

                // Compare the converted item with the item in the chain
                if (info.CompareMethod is not null && info.CompareMethod(convertedItem, currentItem))
                {
                    var container = (ItemsControl)control.ItemContainerGenerator.ContainerFromItem(item);

                    // Replace with the remaining items in the chain
                    info.Items = info.Items.Skip(1);

                    // If no items are left in the chain, then we're finished
                    if (!info.Items.Any())
                    {
                        // Select the last item
                        if (info.OnSelected is not null)
                        {
                            info.OnSelected(container, info);
                        }
                    }
                    else
                    // Request more items and continue the search
                    if (info.OnNeedMoreItems is not null)
                    {
                        info.OnNeedMoreItems(container, info);
                        _ = SetSelectedItem(container, info);
                    }

                    break;
                }
            }
        }
        else
        {
            // If the item containers haven't been generated yet, attach an event and wait for the
            // status to change.
            EventHandler? selectWhenReadyMethod = null;

            var method = selectWhenReadyMethod;
            selectWhenReadyMethod = (ds, de) =>
            {
                if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    // Stop listening for status changes on this container
                    control.ItemContainerGenerator.StatusChanged -= method;

                    // Search the container for the item chain
                    _ = SetSelectedItem(control, info);
                }
            };

            control.ItemContainerGenerator.StatusChanged += selectWhenReadyMethod;
        }
        return control;
    }

    public static void SetValue(this RangeBase pb, double step)
    {
        Check.MustBeArgumentNotNull(pb);

        Animations.AnimateDouble(pb, RangeBase.ValueProperty, pb.Value, step, 100);
    }

    public static TPage SetViewModelByDataContext<TPage, TViewModel>(this TPage page, TViewModel? value, Action viewModelPropertyChanged)
        where TPage : LibPageBase, IStatefulPage
        where TViewModel : class, INotifyPropertyChanged =>
        SetViewModelByDataContext(page, value, (_, _) => viewModelPropertyChanged?.Invoke());

    public static TPage SetViewModelByDataContext<TPage, TViewModel>(this TPage page, TViewModel? value, Action<string?> viewModelPropertyChanged)
        where TPage : LibPageBase, IStatefulPage
        where TViewModel : class, INotifyPropertyChanged =>
        SetViewModelByDataContext(page, value, (_, e) => viewModelPropertyChanged?.Invoke(e.PropertyName));

    public static TPage SetViewModelByDataContext<TPage, TViewModel>(this TPage page, TViewModel? value, PropertyChangedEventHandler? viewModelPropertyChanged = null)
        where TPage : LibPageBase, IStatefulPage
        where TViewModel : class, INotifyPropertyChanged
    {
        var old = page.DataContext.Cast().As<TViewModel>();
        if (old == value)
        {
            return page;
        }

        if (old != null && viewModelPropertyChanged != null)
        {
            old.PropertyChanged -= viewModelPropertyChanged;
        }

        page.DataContext = value;
        if (value != null)
        {
            if (viewModelPropertyChanged != null)
            {
                value.PropertyChanged += viewModelPropertyChanged;
                viewModelPropertyChanged(page, new PropertyChangedEventArgs(nameof(page.DataContext)));
            }
            else
            {
                value.PropertyChanged += (_, __) =>
                {
                    if (page is { } and { IsUnloaded: false })
                    {
                        _ = page.SetIsViewModelChanged(true);
                    }
                };
            }
        }
        return page.SetIsViewModelChanged(false);
    }

    public static bool? ShowDialog<TWindow>(this Window owner) where TWindow : Window, new()
        => owner.ShowDialog(() => new TWindow(), out var window);

    public static (bool? Result, TWindow Window) ShowDialog<TWindow>(this Window owner, Func<TWindow> creator)
        where TWindow : Window
    {
        var result = owner.ShowDialog(creator, out var window);
        return (result, window);
    }

    public static bool? ShowDialog<TWindow>(this Window owner, Func<TWindow> creator, out TWindow window)
        where TWindow : Window
    {
        Check.MustBeArgumentNotNull(creator);

        window = creator();
        window.Owner = owner;
        return window.ShowDialog();
    }

    public static bool? ShowDialog<TWindow>(this Window owner, out TWindow window) where TWindow : Window, new()
        => owner.ShowDialog(() => new TWindow(), out window);

    private static void BindItemsSourceInner<TSelector>(TSelector selector, IEnumerable? items, string? displayMemberPath)
        where TSelector : Selector
    {
        Check.MustBeArgumentNotNull(selector);

        selector.ItemsSource = null;
        selector.ItemsSource = items;
        if (displayMemberPath is not null)
        {
            selector.DisplayMemberPath = displayMemberPath;
        }
    }

    private static THeaderedItemsControl InnerBindDataContext<THeaderedItemsControl>(this THeaderedItemsControl itemsControl, object dataContext, string? header = null)
        where THeaderedItemsControl : HeaderedItemsControl
    {
        itemsControl.DataContext = dataContext;
        itemsControl.Header = header ?? dataContext?.ToString();
        return itemsControl;
    }
}