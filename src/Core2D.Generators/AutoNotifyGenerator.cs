using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Core2D.Generators;

[Generator]
public class AutoNotifyGenerator : IIncrementalGenerator
{
    private const string DefaultAttributeNamespace = "Core2D.ViewModels";

    // Diagnostics
    private static readonly DiagnosticDescriptor MissingBaseTypeDescriptor = new(
        id: "C2DGEN001",
        title: "AutoNotify base type not found",
        messageFormat: "Configured AutoNotify base type '{0}' was not found in the compilation",
        category: "AutoNotifyGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The generator could not resolve the configured base type. Ensure the type exists and is referenced.");

    private static readonly DiagnosticDescriptor NestedTypeNotSupportedDescriptor = new(
        id: "C2DGEN002",
        title: "AutoNotify does not support nested types",
        messageFormat: "Type '{0}' is nested; AutoNotify only supports top-level partial classes",
        category: "AutoNotifyGenerator",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fields inside nested types are ignored. Move the type to the namespace scope or implement properties manually.");

    private static readonly DiagnosticDescriptor InvalidPropertyNameDescriptor = new(
        id: "C2DGEN003",
        title: "Invalid property name for field",
        messageFormat: "Could not derive a valid property name for field '{0}'; specify PropertyName or rename the field",
        category: "AutoNotifyGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The generator could not compute a valid property name from the field. Consider using the PropertyName argument or updating the field name.");

    private static readonly DiagnosticDescriptor UnexpectedExceptionDescriptor = new(
        id: "C2DGEN999",
        title: "Unexpected generator exception",
        messageFormat: "AutoNotify encountered an unexpected error: {0}",
        category: "AutoNotifyGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator threw an exception while processing. Check inner diagnostics and configuration.");

    private static readonly DiagnosticDescriptor TypeMustBePartialDescriptor = new(
        id: "C2DGEN004",
        title: "Type must be partial",
        messageFormat: "Type '{0}' must be 'partial' to use [AutoNotify]",
        category: "AutoNotifyGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Mark the class as partial to allow the generator to add the generated members.");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inject attribute + enum early so the semantic model can bind attributes
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("AccessModifier", SourceText.From(CreateModifierSource(DefaultAttributeNamespace), Encoding.UTF8));
            ctx.AddSource("AutoNotifyAttribute", SourceText.From(CreateAttributeSource(DefaultAttributeNamespace), Encoding.UTF8));
        });

        var configurationProvider = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) => GeneratorConfiguration.From(provider.GlobalOptions));

        // If a custom namespace is configured, also emit attribute + enum there
        context.RegisterSourceOutput(configurationProvider, static (spc, config) =>
        {
            if (!string.IsNullOrWhiteSpace(config.Namespace) && config.Namespace != DefaultAttributeNamespace)
            {
                var nsHint = config.Namespace.Replace('.', '_');
                spc.AddSource($"AccessModifier_{nsHint}", SourceText.From(CreateModifierSource(config.Namespace), Encoding.UTF8));
                spc.AddSource($"AutoNotifyAttribute_{nsHint}", SourceText.From(CreateAttributeSource(config.Namespace), Encoding.UTF8));
            }
        });

        var fieldDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is FieldDeclarationSyntax { AttributeLists.Count: > 0 },
            static (ctx, ct) =>
            {
                var fieldSyntax = (FieldDeclarationSyntax)ctx.Node;
                var list = new List<IFieldSymbol>();
                foreach (var variable in fieldSyntax.Declaration.Variables)
                {
                    if (ctx.SemanticModel.GetDeclaredSymbol(variable, ct) is IFieldSymbol fs)
                    {
                        list.Add(fs);
                    }
                }
                return list;
            })
            .Where(static list => list is { Count: > 0 });

        var combined = context.CompilationProvider
            .Combine(configurationProvider)
            .Combine(fieldDeclarations.Collect());

        context.RegisterSourceOutput(combined, static (spc, data) =>
        {
            try
            {
                var compilation = data.Left.Left;
                var configuration = data.Left.Right;
                var collectedLists = data.Right; // ImmutableArray<List<IFieldSymbol>>

                var notifySymbol = compilation.GetTypeByMetadataName(configuration.BaseType);
                if (notifySymbol is null)
                {
                    spc.ReportDiagnostic(Diagnostic.Create(MissingBaseTypeDescriptor, Location.None, configuration.BaseType));
                    return;
                }

                var fieldSymbols = new List<IFieldSymbol>();
                foreach (var list in collectedLists)
                {
                    foreach (var fieldSymbol in list)
                    {
                        var attributes = fieldSymbol.GetAttributes();
                        if (attributes.Any(ad => ad?.AttributeClass?.Name == "AutoNotifyAttribute"))
                        {
                            fieldSymbols.Add(fieldSymbol);
                        }
                    }
                }

                // TODO: https://github.com/dotnet/roslyn/issues/49385
                #pragma warning disable RS1024
                var groupedFields = fieldSymbols.GroupBy(f => f.ContainingType);
                #pragma warning restore RS1024

                foreach (var group in groupedFields)
                {
                    var classSymbol = group.Key;
                    // Only support top-level types; report otherwise
                    if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
                    {
                        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
                        spc.ReportDiagnostic(Diagnostic.Create(NestedTypeNotSupportedDescriptor, location, classSymbol.ToDisplayString()));
                        continue;
                    }

                    // Ensure the type is partial
                    try
                    {
                        var isPartial = false;
                        foreach (var decl in classSymbol.DeclaringSyntaxReferences)
                        {
                            if (decl.GetSyntax() is TypeDeclarationSyntax tds)
                            {
                                if (tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
                                {
                                    isPartial = true;
                                    break;
                                }
                            }
                        }
                        if (!isPartial)
                        {
                            var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
                            spc.ReportDiagnostic(Diagnostic.Create(TypeMustBePartialDescriptor, location, classSymbol.ToDisplayString()));
                            continue;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
                        spc.ReportDiagnostic(Diagnostic.Create(UnexpectedExceptionDescriptor, location, ex.Message));
                        continue;
                    }

                    string? classSource = null;
                    try
                    {
                        classSource = ProcessClass(
                            classSymbol,
                            group.ToList(),
                            notifySymbol,
                            configuration,
                            diag => spc.ReportDiagnostic(diag));
                    }
                    catch (System.Exception ex)
                    {
                        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
                        spc.ReportDiagnostic(Diagnostic.Create(UnexpectedExceptionDescriptor, location, ex.Message));
                        continue;
                    }

                    if (classSource is null)
                    {
                        continue;
                    }

                    spc.AddSource($"{classSymbol.Name}_AutoNotify.cs", SourceText.From(classSource, Encoding.UTF8));
                }
            }
            catch (System.Exception ex)
            {
                spc.ReportDiagnostic(Diagnostic.Create(UnexpectedExceptionDescriptor, Location.None, ex.Message));
            }
        });
    }

    private static string? ProcessClass(
        INamedTypeSymbol classSymbol,
        List<IFieldSymbol> fields,
        INamedTypeSymbol notifySymbol,
        GeneratorConfiguration configuration,
        System.Action<Diagnostic> reportDiagnostic)
    {
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        var addNotifyInterface = !classSymbol.Interfaces.Contains(notifySymbol);

        if (!SymbolEqualityComparer.Default.Equals(classSymbol, notifySymbol))
        {
            var baseType = classSymbol.BaseType;
            while (true)
            {
                if (baseType is null)
                {
                    break;
                }

                if (SymbolEqualityComparer.Default.Equals(baseType, notifySymbol))
                {
                    addNotifyInterface = false;
                    break;
                }

                baseType = baseType.BaseType;
            }
        }
        else
        {
            addNotifyInterface = false;
        }

        var source = new StringBuilder();

        var format = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeVariance
        );

        var namespaceUsing = string.IsNullOrWhiteSpace(configuration.Namespace)
            ? string.Empty
            : $"using {configuration.Namespace};\n";

        source.Append("// <auto-generated />\n");
        source.Append("#nullable enable\n");
        source.Append("using System.ComponentModel;\n");
        source.Append("using System.Runtime.Serialization;\n");
        if (!string.IsNullOrEmpty(namespaceUsing))
        {
            source.Append(namespaceUsing);
        }
        source.Append('\n');
        source.Append("namespace ");
        source.Append(namespaceName);
        source.Append("\n{\n");
        source.Append("\t[DataContract(IsReference = true)]\n");
        source.Append("    public");
        if (classSymbol.IsAbstract)
        {
            source.Append(" abstract");
        }
        source.Append(" partial class ");
        source.Append(classSymbol.ToDisplayString(format));
        if (addNotifyInterface)
        {
            source.Append(" : ");
            source.Append(notifySymbol.ToDisplayString());
        }
        source.Append("\n    {\n");

        foreach (var fieldSymbol in fields)
        {
            ProcessField(source, fieldSymbol, reportDiagnostic);
        }

        source.Append("\n    }\n}\n");

        return source.ToString();
    }

    private static void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, System.Action<Diagnostic> reportDiagnostic)
    {
        var fieldName = fieldSymbol.Name;
        var fieldType = fieldSymbol.Type;
        var attributeData = fieldSymbol.GetAttributes().First(ad => ad?.AttributeClass?.Name == "AutoNotifyAttribute");
        var overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;
        var propertyName = ChooseName(fieldName, overridenNameOpt);

        if (propertyName is null || propertyName.Length == 0 || propertyName == fieldName)
        {
            // Report a diagnostic that we can't process this field.
            var location = fieldSymbol.Locations.FirstOrDefault() ?? Location.None;
            reportDiagnostic(Diagnostic.Create(InvalidPropertyNameDescriptor, location, fieldName));
            return;
        }

        source.Append($@"
        protected static readonly PropertyChangedEventArgs {fieldName}PropertyChangedEventArgs = new PropertyChangedEventArgs(nameof({propertyName}));");

        var overridenIgnoreDataMemberOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "IgnoreDataMember").Value;
        var ignoreDataMember = false;

        if (overridenIgnoreDataMemberOpt is { IsNull: false, Value: not null })
        {
            ignoreDataMember = (bool)overridenIgnoreDataMemberOpt.Value;
        }

        source.Append(ignoreDataMember
            ? $@"
		[IgnoreDataMember]"
            : $@"
		[DataMember(IsRequired = false, EmitDefaultValue = true)]");

        var overridenSetterModifierOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "SetterModifier").Value;
        var setterModifier = ChooseSetterModifier(overridenSetterModifierOpt);
        source.Append(setterModifier is null
            ? $@"
        public {fieldType} {propertyName}
        {{
            get => {fieldName};
        }}"
            : $@"
        public {fieldType} {propertyName}
        {{
            get => {fieldName};
            {setterModifier}set => RaiseAndSetIfChanged(ref {fieldName}, value, {fieldName}PropertyChangedEventArgs);
        }}");

        static string? ChooseSetterModifier(TypedConstant overridenSetterModifierOpt)
        {
            if (overridenSetterModifierOpt is { IsNull: false, Value: not null })
            {
                var value = (int)overridenSetterModifierOpt.Value;
                return value switch
                {
                    // None
                    0 => null,
                    // Public
                    1 => "",
                    // Protected
                    2 => "protected ",
                    // Private
                    3 => "private ",
                    // Internal
                    4 => "internal ",
                    // Default
                    _ => ""
                };
            }
            else
            {
                return "";
            }
        }

        static string? ChooseName(string fieldName, TypedConstant overridenNameOpt)
        {
            if (!overridenNameOpt.IsNull)
            {
                return overridenNameOpt.Value?.ToString();
            }

            fieldName = fieldName.TrimStart('_');
            if (fieldName.Length == 0)
            {
                return string.Empty;
            }

            if (fieldName.Length == 1)
            {
                return fieldName.ToUpper();
            }

#pragma warning disable IDE0057 // Use range operator
            return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
#pragma warning restore IDE0057 // Use range operator
        }
    }

    private static string CreateAttributeSource(string namespaceName) => $$"""
                                                                           // <auto-generated />
                                                                           #nullable enable
                                                                           using System;

                                                                           namespace {{namespaceName}}
                                                                           {
                                                                               [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                                                                               sealed class AutoNotifyAttribute : Attribute
                                                                               {
                                                                                   public AutoNotifyAttribute()
                                                                                   {
                                                                                   }

                                                                                   public string? PropertyName { get; set; }

                                                                                   public AccessModifier SetterModifier { get; set; } = AccessModifier.Public;

                                                                                   public bool IgnoreDataMember { get; set; } = false;
                                                                               }
                                                                           }
                                                                           """;

    private static string CreateModifierSource(string namespaceName) => $$"""
// <auto-generated />
namespace {{namespaceName}}
{
    public enum AccessModifier
    {
        None = 0,
        Public = 1,
        Protected = 2,
        Private = 3,
        Internal = 4
    }
}
""";

    private sealed class GeneratorConfiguration
    {
        private const string NamespaceOption = "build_property.AutoNotifyNamespace";
        private const string BaseTypeOption = "build_property.AutoNotifyBaseType";
        private const string DefaultNamespace = "Core2D.ViewModels";
        private const string DefaultBaseType = "Core2D.ViewModels.ViewModelBase";

        private GeneratorConfiguration(string @namespace, string baseType)
        {
            Namespace = @namespace;
            BaseType = baseType;
        }

        public string Namespace { get; }

        public string BaseType { get; }

        public static GeneratorConfiguration From(AnalyzerConfigOptions options)
        {
            var namespaceName = DefaultNamespace;
            if (options.TryGetValue(NamespaceOption, out var namespaceValue) && !string.IsNullOrWhiteSpace(namespaceValue))
            {
                namespaceName = NormalizeNamespace(namespaceValue);
            }

            var baseType = DefaultBaseType;
            if (options.TryGetValue(BaseTypeOption, out var baseTypeValue) && !string.IsNullOrWhiteSpace(baseTypeValue))
            {
                baseType = NormalizeMetadataName(baseTypeValue);
            }

            return new GeneratorConfiguration(namespaceName, baseType);
        }

        private static string NormalizeNamespace(string value)
        {
            var trimmed = value.Trim();
            if (trimmed.StartsWith("global::", System.StringComparison.Ordinal))
            {
                trimmed = trimmed.Substring("global::".Length);
            }

            return trimmed;
        }

        private static string NormalizeMetadataName(string value)
        {
            var trimmed = value.Trim();
            if (trimmed.StartsWith("global::", System.StringComparison.Ordinal))
            {
                trimmed = trimmed.Substring("global::".Length);
            }

            return trimmed;
        }
    }
}
