using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Library.Wpf.Helpers;

public static class RichTextBoxHelper
{
    public static FlowDocument FormatCSharpCode(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();

        var document = new FlowDocument();
        var paragraph = new Paragraph();

        var indentLevel = 0;

        foreach (var token in root.DescendantTokens())
        {
            var tokenText = token.Text;

            // Manage indentation for blocks
            if (tokenText is "{" or "}")
            {
                if (tokenText == "}")
                {
                    indentLevel--;
                }

                paragraph.Inlines.Add(new LineBreak());
                AddIndentation(paragraph, indentLevel);
                paragraph.Inlines.Add(CreateColoredRun(token));

                if (tokenText == "{")
                {
                    indentLevel++;
                }

                paragraph.Inlines.Add(new LineBreak());
                AddIndentation(paragraph, indentLevel);
                continue;
            }

            // Add line breaks after semicolons
            if (tokenText == ";")
            {
                paragraph.Inlines.Add(CreateColoredRun(token));
                paragraph.Inlines.Add(new LineBreak());
                AddIndentation(paragraph, indentLevel);
                continue;
            }

            // Handle space between generic type arguments and parameters
            if (token.Parent is TypeArgumentListSyntax or TypeParameterListSyntax)
            {
                paragraph.Inlines.Add(CreateColoredRun(token));
                continue;
            }

            // Add tokens with appropriate spacing
            paragraph.Inlines.Add(CreateColoredRun(token));
            if (!tokenText.Equals(";") && !tokenText.Equals("(") && !tokenText.Equals(")") &&
                !tokenText.Equals("<") && !tokenText.Equals(">") && !tokenText.Equals("."))
            {
                //paragraph.Inlines.Add(new Run(" "));
            }
        }

        document.Blocks.Add(paragraph);
        return document;
    }

    private static void AddIndentation(Paragraph paragraph, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++)
        {
            paragraph.Inlines.Add(new Run("    "));
        }
    }

    private static Run CreateColoredRun(SyntaxToken token)
    {
        var run = new Run();
        var text = token.Text;

        // Colorize keywords, literals, and other token types
        if (token.IsKeyword())
        {
            run.Foreground = Brushes.Blue;
        }
        else if (token.IsKind(SyntaxKind.StringLiteralToken))
        {
            run.Foreground = Brushes.Brown;
        }
        else if (token.IsKind(SyntaxKind.NumericLiteralToken))
        {
            run.Foreground = Brushes.Magenta;
        }
        else if (token.IsKind(SyntaxKind.MultiLineCommentTrivia) || token.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            run.Foreground = Brushes.Green;
        }
        else
        {
            run.Foreground = Brushes.Black;
        }

        if(token.IsKeyword() || token.IsKind(SyntaxKind.IdentifierToken))
        {
            text += " ";
        }
        else if (text == ":")
        {
            text = " : ";
        }
        else if (text == "(")
        {
            text = " (";
        }

        run.Text = text;
        return run;
    }


    //public static RichTextBox InsertCSharpCodeToDocument(this RichTextBox @this, Code code)
    //{
    //    if (code.Language != Languages.CSharp)
    //    {
    //        Throw<NotSupportedException>();
    //    }

    // var detectedTypes = new List<string>(); var keyWordRules = new (string[] Keys, Brush Brush)[]
    // { (LanguageKeywords.Keywords, Brushes.Blue), (LanguageKeywords.PrimitiveTypes,
    // Brushes.DarkGreen), (new[]{"ICommandProcessor", "IEnumerable", "IQueryProcessor"},
    // Brushes.Green) }; var genericRegExes = new[] { @"IEnumerable<\w+>", @"Task<\w+>" }; var
    // document = new FlowDocument(); var paragraphs = TextToParagraphs(code.Statement, lineProcess, wordProcess);

    // document.Blocks.AddRange(paragraphs); @this.Document = document; return @this;

    // (bool Found, IEnumerable<Inline>? Inline)? lineProcess((string CurrentLine, string? PrevLine)
    // line) { var preprocessors = new[] { @"#region", @"#endregion" }; return
    // line.CurrentLine.Trim().StartsWith("///") ? ((bool Found, IEnumerable<Inline>?
    // Inline)?)(true, EnumerableHelper.AsEnumerable(new Italic(new Run(line.CurrentLine)) {
    // Foreground = Brushes.DarkGreen })) : line.CurrentLine.Trim().StartsWith("//") ? ((bool Found,
    // IEnumerable<Inline>? Inline)?)(true, EnumerableHelper.AsEnumerable(new Italic(new
    // Run(line.CurrentLine)) { Foreground = Brushes.DimGray })) :
    // line.CurrentLine.Trim().StartsWithAny(preprocessors) ? (true,
    // EnumerableHelper.AsEnumerable(new Run(line.CurrentLine) { Foreground = Brushes.Gray })) :
    // null; } (bool Found, IEnumerable<Inline>? Inline)? wordProcess((string CurrentWord, string?
    // PrevWord) word) { //! Recursive return formatWord(word.CurrentWord, word.PrevWord);

    // (bool Found, IEnumerable<Inline>? Inlines) formatWord(in string current, in string? previous)
    // { var curr = current.Trim().Remove("?").Remove(";"); var prev =
    // previous?.Trim().Remove("?").Remove(";"); var inlines = new List<Inline>(); bool done;

    // done = processMembers(curr, ref prev, inlines); if (!done) { done =
    // processKeywordRules(keyWordRules, curr, inlines); } if (!done) { done =
    // processTypeMemorization(detectedTypes, curr, inlines); } if (!done) { done =
    // processRegExRules(curr, inlines); } if (!done) { done = processTypeDetection(detectedTypes,
    // curr, prev, inlines); } if (!done) { inlines.Add(new Run(curr)); }

    // if (current.Trim().EndsWith("?")) { inlines.Add(new Run("?")); } if
    // (current.Trim().EndsWith(";")) { inlines.Add(new Run(";")); } if (current.EndsWith(" ")) {
    // inlines.Add(new Run(" ")); }

    // return (inlines.Count != 0, inlines);

    // static bool processKeywordRules((string[] Keys, Brush Brush)[] keyWordRules, string curr,
    // List<Inline> inlines) { foreach (var rule in keyWordRules) { if (rule.Keys.Contains(curr)) {
    // inlines.Add(new Run(curr) { Foreground = rule.Brush }); return true; } }

    // return false; } static bool processTypeMemorization(List<string> detectedTypes, string curr,
    // List<Inline> inlines) { if (detectedTypes.Contains(curr)) { inlines.Add(new Run(curr) {
    // Foreground = Brushes.Green }); return true; }

    // return false; } bool processRegExRules(string curr, List<Inline> inlines) { if
    // (matchAny(curr, genericRegExes, out var pattern)) { var genParam =
    // StringHelper.GetPhrase(curr, 0, '<', '>')!; var genClass = curr[..curr.IndexOf('<')];

    // var (Found, Inlines) = formatWord(genParam, null); var genClassFormatResult =
    // formatWord(genClass, null);

    // var genParamInlines = Found ? Inlines : EnumerableHelper.AsEnumerable(new Run(genParam)); var
    // genClassInlines = genClassFormatResult.Found ? genClassFormatResult.Inlines :
    // EnumerableHelper.AsEnumerable(new Run(genClass));

    // var open = EnumerableHelper.AsEnumerable(new Run("<")); var close =
    // EnumerableHelper.AsEnumerable(new Run(">"));

    // var result = (new[] { genClassInlines!, open, genParamInlines!, close }).SelectAll();
    // inlines.AddRange(result); return true; }

    // return false; } static bool processTypeDetection(List<string> detectedTypes, string curr,
    // string? prev, List<Inline> inlines) { if (prev is "class" or "interface" or ":") {
    // detectedTypes.Add(curr); inlines.Add(new Run(curr) { Foreground = Brushes.Green }); return
    // true; }

    // return false; } bool processMembers(string curr, ref string? prev, List<Inline> inlines) { if
    // (curr.Contains('.')) { var members = curr.Split('.');

    // for (var index = 0; index < members.Length; index++) { var member = members[index]; var
    // (Found, Inlines) = formatWord(member, prev); inlines.AddRange(Inlines!); if (index <
    // members.Length - 1) { inlines.Add(new Run(".")); }

    //                        prev = member;
    //                    }
    //                    return true;
    //                }
    //                return false;
    //            }
    //            static bool matchAny(in string word, in IEnumerable<string> regexPatterns, [NotNullWhen(true)] out string? matchedPattern)
    //            {
    //                foreach (var pattern in regexPatterns)
    //                {
    //                    if (Regex.Match(word, pattern).Success)
    //                    {
    //                        matchedPattern = pattern;
    //                        return true;
    //                    }
    //                }
    //                matchedPattern = null;
    //                return false;
    //            }
    //        }
    //    }
    //}

    //private static IEnumerable<Paragraph> TextToParagraphs(string text,
    //        Func<(string CurrentLine, string? PrevLine), (bool Found, IEnumerable<Inline>? Inline)?> lineProcessor,
    //    Func<(string currentWork, string? PrevWord), (bool Found, IEnumerable<Inline>? Inline)?> wordProcessor)
    //{
    //    Check.MustBeArgumentNotNull(text);
    //    Check.MustBeArgumentNotNull(lineProcessor);
    //    Check.MustBeArgumentNotNull(wordProcessor);

    // var lines = text.Split("\r\n"); string? prevLine = null; foreach (var line in lines) { var
    // paragraph = new Paragraph(); if (line?.Trim().IsNullOrEmpty() ?? true) {
    // paragraph.Inlines.Add(new Run(line)); continue; } var lineProcessResult =
    // lineProcessor((line, prevLine)); prevLine = line; if (lineProcessResult?.Found ?? false) {
    // paragraph.Inlines.AddRange(lineProcessResult.Value.Inline); } else { var words = line.Split("
    // ").Select(x => string.Concat(x, " ")); string? prevWord = null; foreach (var word in words) {
    // if (word is not null and not "" and not " " and not "\r\n") { prevWord =
    // processWord(wordProcessor, paragraph, prevWord, word); } else { paragraph.Inlines.Add(new
    // Run(word)); } } } yield return paragraph; }

    //    static string? processWord(Func<(string currentWork, string? PrevWord), (bool Found, IEnumerable<Inline>? Inline)?> wordProcessor, Paragraph paragraph, string? prevWord, string word)
    //    {
    //        var wordProcessResult = wordProcessor((word, prevWord));
    //        prevWord = word;
    //        if (wordProcessResult?.Found ?? false)
    //        {
    //            paragraph.Inlines.AddRange(wordProcessResult.Value.Inline);
    //        }
    //        else
    //        {
    //            paragraph.Inlines.Add(new Run(word));
    //        }
    //        return prevWord;
    //    }
    //}
}