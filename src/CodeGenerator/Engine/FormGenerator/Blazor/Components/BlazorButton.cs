using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;
using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components
{
    public sealed class BlazorButton : HtmlButton, IBlazorComponent, ISupportsBehindCodeMember
    {
        public BlazorButton(
            string? id = null,
            string? name = null,
            string? onClick = null,
            string? body = null,
            ButtonType type = ButtonType.FormButton,
            string? prefix = null)
            : base(id, name, onClick, body, type, prefix)
        {
        }

        public override string? OnClick
        {
            get => this.GetAttribute("onclick", isBlazorAttribute: true);
            set => this.SetAttribute("onclick", value, isBlazorAttribute: true);
        }
        public string? NameSpace { get; }

        public IEnumerable<GenerateCodeTypeMemberResult> GenerateTypeMembers(GenerateCodesParameters arguments)
        {
            if (this.OnClick.IsNullOrEmpty() || this.Action is not null)
            {
                return Enumerable.Empty<GenerateCodeTypeMemberResult>();
            }

            var main = CodeDomHelper.NewMethod(this.OnClick, accessModifiers: System.CodeDom.MemberAttributes.Private);
            return EnumerableHelper.ToEnumerable(new GenerateCodeTypeMemberResult(main, null));
        }
    }
}
