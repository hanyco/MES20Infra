using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Validations;
using Library.Wpf.Dialogs;

namespace HanyCo.Infra.UI.Helpers;

public static class SourceCodeHelper
{
    private static readonly Func<Code?, Task<Result<string?>>> _saveCodeFunc = async code =>
    {
        if (code == null)
        {
            return Result<string?>.CreateFail("No code found to save.");
        }
        using var dlg = new SaveFileDialog();
        dlg.FileName = code.FileName;
        var resp = dlg.ShowDialog();
        if (resp != DialogResult.OK)
        {
            return Result<string?>.Fail;
        }

        await File.WriteAllTextAsync(dlg.FileName, code.Statement);
        var result = Path.GetDirectoryName(dlg.FileName)!;
        return Result<string?>.CreateSuccess(result, message: "Code(s) saved");
    };

    private static readonly Func<Codes, Task<Result<string?>>> _saveCodesFunc = async codes =>
    {
        using var dlg = new FolderBrowserDialog();
        if (dlg.ShowDialog() is not DialogResult.OK)
        {
            return Result<string?>.Fail;
        }
        var result = dlg.SelectedPath;
        var meaningfullCodes = codes.Compact();
        if (meaningfullCodes?.Any() is not true)
        {
            return Result<string?>.CreateFail("No codes found to save.");
        }
        foreach (var code in meaningfullCodes)
        {
            var filePath = Path.Combine(result, code.FileName);
            await File.WriteAllTextAsync(filePath, code.Statement);
        }
        return Result<string?>.CreateSuccess(result, message: "Code(s) saved");
    };

    private static readonly Action<Result<string?>> _showResult = result =>
        MsgBox2.Inform(result.Message, controls: ButtonInfo.ToButtons(
                new("OK", (e1, e2) => MsgBox2.GetOnButtonClick(e1, e2).Parent.Close()),
                new("Open Containing Folder…", (e1, e2) => { _ = Process.Start("explorer.exe", result.Value!); })).ToArray());

    public static async Task<Result<string?>> SaveSourceToDiskAsync<TViewModel>(this ICodeGeneratorService<TViewModel> codeGeneratorService,
                                                                                TViewModel viewModel,
                                                                                Func<Task> validatorAsync)
    {
        Check.IfArgumentNotNull(codeGeneratorService);
        Check.IfArgumentNotNull(viewModel);

        if (validatorAsync is not null)
        {
            await validatorAsync();
        }

        var codes = codeGeneratorService.GenerateCodes(viewModel).GetValue();
        var result = await codes.SaveToFileAsync();
        return result;
    }

    public static async Task<Result<string?>> SaveToFileAsync(this Codes codes)
    {
        if (codes?.Any() is not true)
        {
            return Result<string?>.CreateFail(message: "No code generated.");
        }
        var result = codes.Count switch
        {
            1 => await _saveCodeFunc(codes[0]),
            _ => await _saveCodesFunc(codes),
        };
        if (result.IsSucceed)
        {
            _showResult(result);
        }
        return result;
    }

    public static async Task<Result<string?>> SaveToFileAsync(this Code code) 
        => code == default
            ? Result<string?>.CreateFail("No code found to save.")
            : await SaveToFileAsync(new Codes(code));
}