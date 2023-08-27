﻿using Library.Results;
using Library.Wpf.Dialogs;

namespace Library.Wpf.Helpers;

public static class WpfResultHelper
{
    public static TResult ShowOrThrow<TResult>(this TResult result, object? owner = null, string? instruction = null, string? successMessage = null)
        where TResult : ResultBase
    {
        var res = result.ThrowOnFail(owner, instruction);
        var text = res.Message.IfNullOrEmpty(successMessage ?? owner?.ToString());
        MsgBox2.Inform(instruction, text);

        return res;
    }

    public static async Task<TResult> ShowOrThrowAsync<TResult>(this Task<TResult> result, object? owner = null, string? instruction = null, string? successMessage = null)
        where TResult : ResultBase =>
        ShowOrThrow(await result, owner, instruction);
}