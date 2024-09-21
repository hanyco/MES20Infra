using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Library.CodeGeneration.Models;
using Library.Exceptions;
using Library.Results;
using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.Helpers;

public static class SourceCodeHelper
{
    public static readonly Action<Result<string?>> ShowDiskOperationResult = result =>
        MsgBox2.Inform(result.Message??"Saved successfully.", controls: ButtonInfo.ToButtons(
                new("OK", (e1, e2) => MsgBox2.GetOnButtonClick(e1, e2).Parent.Close()),
                new("Open Containing Folder…", (e1, e2) => { _ = Process.Start("explorer.exe", result.Value!); })).ToArray());

    public static async Task<Result<string?>> SaveToFileAskAsync(this Codes codes)
    {
        if (codes?.Any() is not true)
        {
            return Result.Fail<string?>(value: "No code generated.");
        }
        var result = codes.Count switch
        {
            1 => await saveCode(codes[0]),
            _ => await saveCodesFunc(codes),
        };
        if (result.IsSucceed)
        {
            ShowDiskOperationResult(result);
        }
        return result;

        static async Task<Result<string?>> saveCode(Code? code)
        {
            if (code == null)
            {
                return Result.Fail<string?>(value: "No code found to save.");
            }
            using var dlg = new SaveFileDialog();
            dlg.FileName = code.FileName;
            var resp = dlg.ShowDialog();
            if (resp != DialogResult.OK)
            {
                return Result.Fail<string?>(new OperationCancelledException());
            }

            await File.WriteAllTextAsync(dlg.FileName, code.Statement);
            var result = Path.GetDirectoryName(dlg.FileName)!;
            return Result.Success<string?>(result, message: "Code(s) saved");
        }
        static async Task<Result<string?>> saveCodesFunc(Codes codes)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() is not DialogResult.OK)
            {
                return Result.Fail<string?>();
            }
            var result = dlg.SelectedPath;
            var meaningfulCodes = codes.Compact();
            if (meaningfulCodes?.Any() is not true)
            {
                return Result.Fail<string?>(message:"No codes found to save.");
            }
            foreach (var code in meaningfulCodes)
            {
                var filePath = Path.Combine(result, code.FileName);
                await File.WriteAllTextAsync(filePath, code.Statement);
            }
            return Result.Success<string?>(result, message: "Code(s) saved");
        }
    }

    public static async Task<Result<string?>> SaveToFileAskAsync(this Code code)
        => code == default
            ? Result.Fail<string?>(value: "No code found to save.")
            : await SaveToFileAskAsync(new Codes(code));
}