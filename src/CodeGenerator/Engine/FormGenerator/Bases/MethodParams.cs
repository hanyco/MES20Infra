using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases
{
    public sealed record GenerateCodeTypeMemberResult(CodeTypeMember? Main, CodeTypeMember? Partial);
}
