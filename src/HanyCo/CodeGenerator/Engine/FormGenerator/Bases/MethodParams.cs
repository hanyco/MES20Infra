using Library.CodeGeneration.v2.Back;

using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

public sealed record CodeTypeMembers(CodeTypeMember? MemberOnMainClass, CodeTypeMember? MemberOnPartialClass);
public sealed record ClassMembers(IMember? MemberOnMainClass, IMember? MemberOnPartialClass);