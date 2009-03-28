using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Generation;
using Docu.Parsing.Model;
using Docu.Tests.Utils;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Generation
{
    [TestFixture]
    public class HtmlGenerationTests : BaseFixture
    {
        private HtmlGenerator generator;

        [SetUp]
        public void CreateGenerator()
        {
            var templates = new Dictionary<string, string>();

            // get all TestTemplate attributes and stick their content in the generator
            foreach (var pair in from method in GetType().GetMethods()
                                 from attribute in method.GetCustomAttributes(typeof(TestTemplateAttribute), true)
                                 select new { method.Name, ((TestTemplateAttribute)attribute).Content })
                templates.Add(pair.Name, pair.Content);

            generator = new HtmlGenerator(templates);
        }

        private string Convert(ViewData data)
        {
            var callStack = new StackTrace();
            var caller = callStack.GetFrame(1);

            return generator.Convert(caller.GetMethod().Name, data);
        }

        [TestTemplate("${Assemblies[0].FullName}")]
        public void ShouldOutputAssemblies()
        {
            Convert(new ViewData { Assemblies = new[] { typeof(ViewData).Assembly } })
                .ShouldEqual(typeof(ViewData).Assembly.FullName);
        }

        [TestTemplate("${Namespaces[0].Name}")]
        public void ShouldOutputSimpleNamespace()
        {
            Convert(new ViewData { Namespaces = Namespaces("Example") })
                .ShouldEqual("Example");
        }

        [TestTemplate("${Namespace.Name}")]
        public void ShouldOutputShortcutNamespace()
        {
            Convert(new ViewData { Namespace = Namespace("Example") })
                .ShouldEqual("Example");
        }

        [TestTemplate("<for each=\"var ns in Namespaces.Where(x => x.Name == 'Test')\">${ns.Name}</for>")]
        public void ShouldOutputNamespaceThatUsesLinq()
        {
            Convert(new ViewData { Namespaces = Namespaces("Example", "Test") })
                .ShouldEqual("Test");
        }

        [TestTemplate("${Formatter.Format(Type.Summary)}")]
        public void ShouldOutputSimpleSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>(ns);
            type.Summary.AddChild(new InlineText("Hello world!"));
            
            Convert(new ViewData { Type = type })
                .ShouldEqual("Hello world!");
        }

        [TestTemplate("<for each=\"var method in Type.Methods\">${method.Name}(${OutputMethodParams(method)})</for>")]
        public void ShouldOutputOverloadedMethods()
        {
            var ns = Namespace("Example");
            var type = Type<ClassWithOverload>(ns);
            var parameterType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Methods.Add(new Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method()), typeof(ClassWithOverload))));
            type.Methods.Add(new Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method(null)), typeof(ClassWithOverload))));
            type.Methods[1].Parameters.Add(new MethodParameter("one", parameterType));
            
            Convert(new ViewData { Type = type })
                .ShouldEqual("Method()Method(string one)"); // nasty, I know
        }

        [TestTemplate("<for each=\"var method in Type.Methods\">${method.ReturnType.PrettyName}</for>")]
        public void ShouldOutputMethodReturnType()
        {
            var ns = Namespace("Example");
            var type = Type<ReturnMethodClass>(ns);
            var returnType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Methods.Add(new Method(Identifier.FromMethod(Method<ReturnMethodClass>(x => x.Method()), typeof(ReturnMethodClass))));
            type.Methods[0].ReturnType = returnType;

            Convert(new ViewData { Type = type })
                .ShouldEqual("string");
        }

        [TestTemplate("<for each=\"var property in Type.Properties\">${property.ReturnType.PrettyName}</for>")]
        public void ShouldOutputPropertyReturnType()
        {
            var ns = Namespace("Example");
            var type = Type<PropertyType>(ns);
            var returnType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Properties.Add(new Property(Identifier.FromProperty(Property<PropertyType>(x => x.Property), typeof(PropertyType))));
            type.Properties[0].ReturnType = returnType;

            Convert(new ViewData { Type = type })
                .ShouldEqual("string");
        }
    }
}