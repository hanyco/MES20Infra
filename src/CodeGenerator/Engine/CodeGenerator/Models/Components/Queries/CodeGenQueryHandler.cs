using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Queries;
using System.ComponentModel;


namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries
{
    [Fluent]
    [Immutable]
    public sealed record CodeGenQueryHandler : CodeGenCqrsSegregateType
    {
        private CodeGenQueryHandler(CodeGenQueryParams queryParam, CodeGenQueryResult queryResult, IEnumerable<string>? securityKeys, IEnumerable<CodeGenProp>? props = null)
            : base("Handler", securityKeys, null, props)
        {
            this.QueryParam = queryParam;
            this.QueryResult = queryResult;
        }

        public CodeGenQueryParams QueryParam { get; }
        public CodeGenQueryResult QueryResult { get; }

        protected override bool HasPartialClass
        {
            get => true;
            set
            {
                if (!value)
                {
                    throw new InvalidEnumArgumentException("The Handler class must have partial");
                }

                base.HasPartialClass = value;
            }
        }

        public override SegregationRole Role { get; } = SegregationRole.QueryHandler;

        public static CodeGenQueryHandler New(CodeGenQueryParams queryParam, CodeGenQueryResult queryResult, IEnumerable<string>? securityKeys, params (Type Type, string Name)[] props)
            => new(queryParam, queryResult, securityKeys, props.Select(p => CodeGenProp.New(new CodeGenType(p.Type), p.Name, false, false)));

        protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
        {
            yield return $"{TypePath.New(typeof(IQueryHandler<,>))}<{cqrsName}Parameter, {cqrsName}Result>";
        }

        protected override Partials OnGetPartials()
            => base.OnGetPartials() | Partials.Handller;
        protected override Partials OnGetValidPartials()
            => base.OnGetValidPartials() | Partials.Handller;
    }
}
