using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections;
using System.Linq;

namespace ErrorProne.NET.CoreAnalyzers
{
    [DiagnosticAnalyzer("C#")]
    public sealed class PublicIEnumerablePropertyAnalyzer : DiagnosticAnalyzerBase
    {
        private const string DiagnosticId = DiagnosticIds.PublicIEnumerableProperty;
        private const string Category = "Performance";
        private static readonly string Title = "Usage of IEnumerable as a public property type";
        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, true);

        private static readonly string _iEnumerableName = typeof(IEnumerable).Name;

        public PublicIEnumerablePropertyAnalyzer() : base(_rule)
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzePublicProperties, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzePublicProperties(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PropertyDeclarationSyntax node)
            {
                if (node.Type is SimpleNameSyntax simpleNameSyntax && simpleNameSyntax.Identifier.ValueText == _iEnumerableName)
                {
                    if (node.Modifiers.Any(m => m.ValueText == "public" || m.ValueText == "internal"))
                    {
                        ISymbol declaredSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                        Diagnostic diagnostic = Diagnostic.Create(_rule, declaredSymbol.Locations[0]);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
