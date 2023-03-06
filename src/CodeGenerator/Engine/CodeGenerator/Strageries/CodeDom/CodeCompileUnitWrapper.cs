using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Markers;
using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Strageries.CodeDom;

[Fluent]
[Worker]
internal sealed class CodeCompileUnitWrapper : ICodeGeneratorUnit
{
    private readonly CodeCompileUnit _unit;
    private readonly string _codeName;
    private readonly bool _isPartial;
    private readonly string[] _directives;

    public CodeCompileUnitWrapper(CodeCompileUnit unit, string codeName, bool isPartial = false, params string[] directives)
    {
        this._unit = unit;
        this._codeName = codeName;
        this._isPartial = isPartial;
        this._directives = directives;
    }

    public Code GenerateCode(in GenerateCodesParameters? arguments = null) =>
        new(this._codeName, Languages.CSharp, this._unit.GenerateCode(this._directives), this._isPartial);
}
