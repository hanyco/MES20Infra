using System.CodeDom;

using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using Library.Validations;

namespace Library.CodeGeneration;

public sealed class CodeDomCodeGenProvider : ICodeGenProvider
{
    public static CodeDomCodeGenProvider New()
        => new();

    public Codes GenerateCode(in INameSpace nameSpace, in GenerateCodesParameters? arguments = default)
    {
        Check.MustBeArgumentNotNull(nameSpace);

        LibLogger.Debug("Generating Behind Code");
        var generateMain = arguments?.GenerateMainCode ?? false;
        var generatePart = arguments?.GeneratePartialCode ?? false;
        var mainUnit = generateMain ? new CodeCompileUnit() : null;
        var partUnit = generatePart ? new CodeCompileUnit() : null;
        var mainNs = mainUnit?.AddNewNameSpace(nameSpace.FullName).UseNameSpace(nameSpace.UsingNameSpaces);
        var partNs = partUnit?.AddNewNameSpace(nameSpace.FullName).UseNameSpace(nameSpace.UsingNameSpaces);
        CodeTypeDeclaration? codeTypeBuffer;
        foreach (var type in nameSpace.CodeGenTypes)
        {
            codeTypeBuffer = (type.IsPartial ? partNs : mainNs)?.UseNameSpace(type.UsingNameSpaces).AddNewClass(type.Name, type.BaseTypes, type.IsPartial);
            if (codeTypeBuffer == null)
            {
                continue;
            }

            foreach (var member in type.Members)
            {
                _ = member switch
                {
                    FieldInfo field => addField(codeTypeBuffer, field),
                    PropertyInfo property when property.HasBackingField => addProperty(codeTypeBuffer, property),
                    PropertyInfo property => addProperty(codeTypeBuffer, property),

                    _ => throw new NotImplementedException()
                };
            }
        }

        var codeList = new List<Code>();
        if (generateMain)
        {
            codeList.Add(new Code("Main", Languages.CSharp, mainUnit!.GenerateCode(), false));
        }
        if(generatePart)
        {
            codeList.Add(new Code("Partial", Languages.CSharp, partUnit!.GenerateCode(), true));
        }
        var result = new Codes(codeList);
        LibLogger.Debug("Generated Behind Code");
        return result;

        static CodeTypeDeclaration addProperty(in CodeTypeDeclaration codeType, in PropertyInfo property) => codeType.AddProperty(
            property.Type,
            property.Name,
            property.Comment,
            property.AccessModifier,
            property.Getter,
            property.Setter,
            property.InitCode,
            property.IsNullable);

        static CodeTypeDeclaration addField(CodeTypeDeclaration codeTypeBuffer, FieldInfo field) => 
            codeTypeBuffer.AddField(field.Type, field.Type, field.Comment, field.AccessModifier);
    }
}