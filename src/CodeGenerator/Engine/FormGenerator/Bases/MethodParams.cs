using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public sealed record CodeTypeMembers(CodeTypeMember? MemberOnMainClass, CodeTypeMember? MemberOnPartialClass);