//using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
//using Library.CodeGeneration.Models;
//using Library.Coding;
//using Library.Validations;
//using System.CodeDom;
//using static Library.Coding.FluencyHelper;

//namespace TestConApp;
//public enum AccessModifier
//{
//    Private = 0,
//    Protected = 1,
//    Public = 2,
//    Internal = 3,
//}

//public interface IClassMember
//{
//    string Name { get; set; }
//    bool IsStatic { get; set; }
//    IList<IClass> Attributes { get; }
//    AccessModifier AccessModifier { get; set; }
//}

//internal class ClassMember : IClassMember
//{
//    public string Name { get; set; }
//    public bool IsStatic { get; set; }
//    public IList<IClass> Attributes { get; } = new List<IClass>();
//    public AccessModifier AccessModifier { get; set; }
//}

//public interface IProperty : IClassMember
//{
//    TypePath Type { get; set; }
//    string? BackingFieldName { get; set; }
//    bool HasGetter { get; set; }
//    bool HasSetter { get; set; }
//    bool IsNullable { get; set; }
//    bool IsList { get; set; }

//    static IProperty New(string type, string name) =>
//        new Property { Name = name, Type = type };
//}

//internal class Property : ClassMember, IProperty
//{
//    public TypePath Type { get; set; }
//    public bool HasGetter { get; set; }
//    public bool HasSetter { get; set; }
//    public string? BackingFieldName { get; set; }
//    public bool IsNullable { get; set; }
//    public bool IsList { get; set; }
//}

//public interface IMethod : IClassMember
//{
//    bool IsConstructor { get; set; }
//    TypePath? ResultType { get; set; }
//    static IMethod New(string? returnType, string? name) =>
//        new Method { ResultType = returnType, Name = name ?? string.Empty };
//}

//internal class Method : ClassMember, IMethod
//{
//    public bool IsConstructor { get; set; }
//    public TypePath? ResultType { get; set; }
//}

//public interface IField : IClassMember
//{
//    bool IsReadOnly { get; set; }
//    TypePath Type { get; set; }
//    static IField New(string type, string name) =>
//        new Field { Name = name, Type = type };
//}

//internal class Field : ClassMember, IField
//{
//    public bool IsReadOnly { get; set; }
//    public TypePath Type { get; set; }
//}

//public interface IClass
//{
//    IList<IProperty> Properties { get; }
//    IList<IMethod> Methods { get; }
//    IList<IField> Fields { get; }
//    string Name { get; }
//    TypePath? BaseTypeName { get; set; }
//    string NameSpace { get; set; }

//    static IClass New(string name) => new Class { Name = name };
//}


//internal class Class : IClass
//{
//    public IList<IProperty> Properties { get; }
//    public IList<IMethod> Methods { get; }
//    public IList<IField> Fields { get; }
//    public string Name { get; internal set; }
//    public TypePath? BaseTypeName { get; set; }
//    public string NameSpace { get; set; }
//}

//public static class CodeGeneratorManager
//{
//    public static IClass AddField(this IClass @class, IField member) =>
//        @class.AddMemebr(member);

//    public static IClass AddProperty(this IClass @class, IProperty member) =>
//        @class.AddMemebr(member);
//    public static IClass AddMethod(this IClass @class, IMethod member) =>
//        @class.AddMemebr(member);

//    public static IClass AddMemebr(this IClass @class, IClassMember member) =>
//        member switch
//        {
//            IField field => @class,
//            IProperty property => Fluent(@class, x => x.Properties.Add(property)),
//            IMethod method => @class,
//            _ => throw new NotImplementedException()
//        };

//    public static Code Generate(this IClass @class)
//    {
//        var a = new CodeGenType(
//            @class.ArgumentNotNull().Name,
//            @class.BaseTypeName?.ToString() is { } btn ? new(btn) : null,
//            @class.Properties.Select(x => CodeGenProp.New(CodeGenType.New(x.Type.ToString()), x.Name, x.IsList, x.IsNullable, x.HasGetter, x.HasSetter)));
//        var unit = new CodeCompileUnit();
//        return Code.Empty;
//    }
//}

//public class CodeGeneratorManagerTest
//{
//    public void Test()
//    {
//        var code = IClass.New("thisClass")
//                         .AddField(IField.New("int", "_age"))
//                         .AddField(IField.New("string", "_name"))
//                         .AddProperty(IProperty.New("int", "Age").With(x => x.BackingFieldName = "_age"))
//                         .AddProperty(IProperty.New("string", "Name").With(x => x.BackingFieldName = "_name"))
//                         .AddMethod(IMethod.New(null, null).With(x => x.IsConstructor = true))
//                         .Generate();
//    }
//}

//async Task realTestAsync()
//{
//    var dataContextType = "Temp.Data.CreateOrderDto";

//    var orderPage = new BlazorPage("OrderPage").SetNameSpace("HanyCo.Mes.Sales").SetDataContext(dataContextType);

//    var containerDiv = HtmlDiv.New().SetIsForm().SetIsRightToLeft().SetIsContainerFluid();
//    var header = HtmlDiv.New().SetIsRow();
//    var content = HtmlDiv.New().SetPosition(col: 12).SetIsForm();
//    var footer = HtmlDiv.New().SetIsButtonBar();

//    var p1 = new HtmlParagraph(body: "سیستم جامع") { BootStrapCol = 6 };
//    var p2 = new BlazorParagraph(body: "@CurrentTime") { BootStrapCol = 6 };

//    _ = header.AddChild(p1).AddChild(p2);

//    var orderDetailsComponent = new BlazorComponent("OrderDetailsComponent").SetNameSpace("HanyCo.Mes.Sales").SetPosition(order: 1)
//                             .SetDataContext(dataContextType);
//    _ = orderDetailsComponent.AddChild(new BlazorLabel("OrderIdLabel", body: "شماره فاکتور").SetAsFormLabel().SetPosition(col: 1))
//                             .AddChild(new BlazorTextBox("OrderIdTextBox", bind: "OrderNo").SetPosition(col: 2))
//                             .AddChild(new BlazorButton("selectOrderButton", body: "...", onClick: "selectOrderButton_Click"));
//    _ = orderDetailsComponent.AddChild(new HtmlLabel("dateTimeLabel", body: "تاریخ").SetAsFormLabel().SetPosition(col: 1))
//                             .AddChild(new BlazorDatePicker("dateTimeTextBox", bind: "CreateDate").SetIsEnabled(false).SetPosition(col: 2));
//    _ = orderDetailsComponent.AddChild(HtmlHr.New().SetPosition(row: 2));
//    _ = content.AddChild(orderDetailsComponent);

//    var orderItemsComponent = new BlazorComponent("OrderItemsComponent").SetPosition(order: 2).SetDataContext(dataContextType);
//    _ = orderItemsComponent.AddChild(HtmlHr.New().SetPosition(row: 2));
//    _ = content.AddChild(orderItemsComponent);

//    var orderCustomerComponent = new BlazorComponent("OrderCustomerComponent").SetNameSpace("HanyCo.Mes.Sales").SetPosition(order: 3).SetDataContext(dataContextType);
//    _ = orderCustomerComponent.AddChild(new BlazorLabel("customerNameLabel", body: "مشتری").SetAsFormLabel().SetPosition(col: 1))
//                              .AddChild(new BlazorTextBox("customerNameTextBox", bind: "CustomerName").SetPosition(col: 2));
//    _ = orderCustomerComponent.AddChild(new HtmlLabel("customerPhoneLabel", body: "تلفن").SetAsFormLabel().SetPosition(col: 1))
//                              .AddChild(new BlazorTextBox("customerPhoneTextBox", bind: "CustomerPhone").SetPosition(col: 3));
//    _ = orderCustomerComponent.AddChild(HtmlHr.New().SetPosition(row: 2));
//    _ = content.AddChild(orderCustomerComponent);

//    var orderPriceComponent = new BlazorComponent("OrderPriceComponent").SetNameSpace("HanyCo.Mes.Sales").SetPosition(order: 4).SetDataContext(dataContextType);
//    _ = orderPriceComponent.AddChild(new BlazorLabel("itemsCountLabel", body: "تعداد اقلام").SetAsFormLabel().SetPosition(row: 1, col: 1))
//                           .AddChild(new BlazorLabel("itemsCountTextBox", bind: "ItemsCount", body: "تعداد اقلام").SetPosition(row: 1, col: 2));
//    _ = orderPriceComponent.AddChild(new BlazorLabel("itemsPriceLabel", body: "جمع مبلغ").SetAsFormLabel().SetPosition(row: 2, col: 1))
//                           .AddChild(new BlazorLabel("itemsPriceTextBox", bind: "ItemsPrice", body: "جمع مبلغ").SetPosition(row: 2, col: 2));
//    _ = orderPriceComponent.AddChild(new BlazorLabel("shippingCostLabel", body: "هزینه ارسال").SetAsFormLabel().SetPosition(row: 3, col: 1))
//                           .AddChild(new BlazorLabel("shippingCostTextBox", bind: "ShippingCost", body: "هزینه ارسال").SetPosition(row: 3, col: 2));
//    _ = orderPriceComponent.AddChild(new BlazorLabel("totalPriceLabel", body: "جمع کل").SetAsFormLabel().SetPosition(row: 4, col: 1))
//                           .AddChild(new BlazorLabel("totalPriceTextBox", bind: "totalPrice", body: "جمع کل").SetPosition(row: 4, col: 2));
//    _ = orderPriceComponent.AddChild(HtmlHr.New().SetPosition(row: 5));
//    _ = content.AddChild(orderPriceComponent);

//    var saveButton = new BlazorButton("SaveButton", body: "ذخیره") { BootStrapCol = 1, IsDefaultButton = true }
//        .SetAction("SaveOrder", new CommandCqrsSergregation(dataContextType, new(dataContextType, null!), new("Temp.Data.CreateOrderResult", "Temp.Data.CreateOrderResultDto")));

//    var saveAndPrintButton = new BlazorButton("SaveAndPrintButton", body: "ذخیره و چاپ") { BootStrapCol = 1 }
//        .SetAction("SaveAndPrintOrder", new CommandCqrsSergregation("CreateAndPrintOrder", new(dataContextType, null!), new("Temp.Data.CreateAndPrintOrderResult", "Temp.Data.CreateAndPrintOrderResultDto")));

//    var cancelButton = new BlazorButton("CancelButton", body: "انصراف") { BootStrapCol = 1, IsCancellButton = true };

//    var backButton = new BlazorButton("BackButton", body: "بازگشت") { BootStrapCol = 1, IsCancellButton = true };

//    var footerComponent = BlazorComponent.New("FooterComponent").SetNameSpace("HanyCo.Mes.Sales")
//                                         .SetDataContext(dataContextType)
//                                         .AddChild(saveButton, saveAndPrintButton, cancelButton, backButton);

//    _ = footer.AddChild(footerComponent);

//    _ = containerDiv.AddChild(header, content, footer);
//    _ = orderPage.AddChild(containerDiv);

//    const string path = @"D:\Temp\Pages";
//    await saveComponentToFileAsync(orderDetailsComponent, path, new GenerateCodesParameters(true, true, true));
//    await saveComponentToFileAsync(orderItemsComponent, path, new GenerateCodesParameters(true, true, true));
//    await saveComponentToFileAsync(orderCustomerComponent, path, new GenerateCodesParameters(true, true, true));
//    await saveComponentToFileAsync(orderPriceComponent, path, new GenerateCodesParameters(true, true, true));
//    await saveComponentToFileAsync(footerComponent, path, new GenerateCodesParameters(true, true, true));
//    await savePageToFileAsync(orderPage, path, new GenerateCodesParameters(true, true, true));
//}

//async Task saveComponentToFileAsync(ICodeGenerator blazorComponent, string path, GenerateCodesParameters? arguments = null)
//{
//    var codes = blazorComponent.GenerateCodes(arguments);
//    await saveToFileAsync(codes, path);
//}

//async Task savePageToFileAsync(BlazorPage blazorPage, string path, GenerateCodesParameters? arguments = null)
//{
//    var codes = blazorPage.GenerateCodes(arguments);
//    await saveToFileAsync(codes, path);
//}

//async Task saveToFileAsync(IEnumerable<Code> codes, string path)
//{
//    foreach (var code in codes)
//    {
//        if (!code.FileName.IsNullOrEmpty())
//        {
//            await File.WriteAllTextAsync(Path.Combine(path, code.FileName), code.Statement);
//        }
//    }
//}
