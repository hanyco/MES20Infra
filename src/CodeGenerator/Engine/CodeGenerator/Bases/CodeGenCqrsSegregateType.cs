using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.CodeGeneration.Helpers;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Bases
{
    public abstract record CodeGenCqrsSegregateType : CodeGenType, ICodeGenCqrsSegregate
    {

        protected CodeGenCqrsSegregateType(in string suffix, in CodeGenType? baseClass = null, in IEnumerable<CodeGenProp>? props = null)
            : base(in suffix, in baseClass, props: props) => this.Suffix = suffix;

        protected readonly string Suffix;
        string ICodeGenCqrsSegregate.Suffix => this.Suffix;

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        string? ISupportCommenting.Comment { get; set; }

        Partials ISupportsPartiality.Partials { get; set; } = Partials.None;

        protected virtual bool HasPartialClass { get; set; }

        bool ICodeGenCqrsSegregate.HasPartialClass
        {
            get => this.HasPartialClass;
            set => this.HasPartialClass = value;
        }
        public abstract SegregationRole Role { get; }

        public IEnumerable<string> GetIntefaces(string cqrsName)
        {
            var items = this.OnGetRequiredInterfaces(cqrsName);
            foreach (var item in items)
            {
                yield return item;
            }

            foreach (var item in this.GetBaseTypes())
            {
                yield return item.FullName;
            }
        }

        public Partials GetPartials()
            => this.OnGetPartials();
        public Partials GetValidPartials()
            => this.OnGetValidPartials();

        protected virtual IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
            => Enumerable.Empty<string>();
        protected virtual Partials OnGetValidPartials()
            => Partials.OnInitialize;
        protected virtual Partials OnGetPartials()
            => this.Cast().As<ISupportsPartiality>()!.Partials;
    }
}
