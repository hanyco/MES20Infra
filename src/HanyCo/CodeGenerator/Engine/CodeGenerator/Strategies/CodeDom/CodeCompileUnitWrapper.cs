using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Markers;
using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Strategies.CodeDom;

[Fluent]
[Worker]
internal sealed class CodeCompileUnitWrapper(CodeCompileUnit unit, string codeName, bool isPartial = false, params string[] directives) : ICodeGeneratorUnit
{
    private readonly CodeCompileUnit _unit = unit;
    private readonly string _codeName = codeName;
    private readonly bool _isPartial = isPartial;
    private readonly string[] _directives = directives;

    public Code GenerateCode(in GenerateCodesParameters? arguments = null) =>
        new(this._codeName, Languages.CSharp, this._unit.GenerateCode(this._directives), this._isPartial);
}
