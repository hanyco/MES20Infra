namespace HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels
{
    //[Fluent]
    //[Immutable]
    //public readonly struct CodeGenControllerModel : ICodeGenControllerModel, IEquatable<CodeGenControllerModel>
    //{
    //    private readonly List<ICodeGenApi> _apiList;
    //    public CodeGenControllerModel(string name)
    //    {
    //        this.Name = name;
    //        this._apiList = new();
    //        this.NameSpace = null;
    //    }

    //    public IEnumerable<ICodeGenApi> Apis => this._apiList.AsEnumerable();

    //    public string Name { get; }

    //    public string? NameSpace { get; }

    //    public CodeGenControllerModel AddApi(ICodeGenApi api)
    //    {
    //        this._apiList.Add(api);
    //        return this;
    //    }

    //    public CodeGenControllerModel AddApiGet(in CodeGenType returnType,
    //        in string apiName = "Get",
    //        in string? httpMethodName = null,
    //        in int? httpMethodOrder = null,
    //        in IEnumerable<(CodeGenType Type, string Name)>? arguments = null,
    //        ICodeGenCqrsSegregate? cqrsSegregate = null)
    //        => this.AddApi(new CodeGenApiType<HttpGetAttribute>(returnType, apiName, httpMethodName, httpMethodOrder, arguments, cqrsSegregate));

    //    public bool Equals(CodeGenControllerModel other)
    //        => this.NameSpace == other.NameSpace && this.Name == other.Name;

    //    public override bool Equals(object? obj)
    //        => obj is CodeGenControllerModel model && this.Equals(model);

    //    public IEnumerable<(Type Type, string Name)> GetCtorArgs()
    //    {
    //        if (this.Apis.Any(api => api.HttpMethodType == typeof(HttpGetAttribute)))
    //        {
    //            yield return (typeof(IQueryProcessor), "QueryProcessor");
    //        }
    //    }

    //    public override int GetHashCode()
    //        => HashCode.Combine(this.NameSpace, this.Name);


    //    public static bool operator ==(CodeGenControllerModel left, CodeGenControllerModel right)
    //        => left.Equals(right);
    //    public static bool operator !=(CodeGenControllerModel left, CodeGenControllerModel right)
    //        => !(left == right);
    //}
}
