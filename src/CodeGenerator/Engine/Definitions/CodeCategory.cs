namespace HanyCo.Infra.CodeGeneration.Definitions;

public enum CodeCategory
{
    Dto,
    Query,
    Command,
    Page,
    Component,
    Converter,
}

public static class CodeTemplates
{
    public const string DEFAULT_CODE = "throw new System.NotImplementedException();";
    public readonly static FormattableString CallQuery = $@"{INDENT.Repeat(3)}this.{dataContextValidatorName}();
{INDENT.Repeat(3)}var dto = this.DataContext;
{INDENT.Repeat(3)}On{calleeName}Calling(cqrs);
{INDENT.Repeat(3)}var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);
{INDENT.Repeat(3)}On{calleeName}Called(cqrs, cqResult);";
}