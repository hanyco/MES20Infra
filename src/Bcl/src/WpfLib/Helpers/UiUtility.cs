﻿using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using Library.Results;
using Library.Wpf.Dialogs;
using Library.Wpf.Windows;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace Library.Wpf.Helpers;

public static class UiUtility
{
    public static async Task<Result> AskToSaveIfChangedAsync<TPage>([DisallowNull] this TPage page, [DisallowNull] string ask = "Do you want to save changes?", CancellationToken cancellationToken = default)
        where TPage : IStatefulPage, IAsyncSavePage =>
        page.NotNull().IsViewModelChanged
        ? MsgBox2.AskWithCancel(ask) switch
        {
            TaskDialogResult.Cancel or TaskDialogResult.Close => Result.Failed,
            TaskDialogResult.Yes => await page.SaveToDbAsync(cancellationToken),
            TaskDialogResult.No => Result.Succeed,
            _ => Result.Failed
        }
        : Result.Succeed;
}
