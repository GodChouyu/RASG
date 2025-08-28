using Microsoft.CodeAnalysis;

namespace Analyzers;

public static class DiagnosticDescriptors
{
    #region U0001

    private const string U0001DiagnosticId = "U0001";

    private static readonly LocalizableString U0001Title =
        new LocalizableResourceString(nameof(Resources.U0001Title), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0001MessageFormat =
        new LocalizableResourceString(nameof(Resources.U0001MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0001Description =
        new LocalizableResourceString(nameof(Resources.U0001Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor RuleU0001 = new(U0001DiagnosticId, U0001Title, U0001MessageFormat,
        Category.Interoperability,
        DiagnosticSeverity.Warning, true, U0001Description);

    #endregion

    #region U0002

    private const string U0002DiagnosticId = "U0002";

    private static readonly LocalizableString U0002Title =
        new LocalizableResourceString(nameof(Resources.U0002Title), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0002MessageFormat =
        new LocalizableResourceString(nameof(Resources.U0002MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0002Description =
        new LocalizableResourceString(nameof(Resources.U0002Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor RuleU0002 = new(U0002DiagnosticId, U0002Title, U0002MessageFormat,
        Category.Interoperability,
        DiagnosticSeverity.Warning, true, U0002Description);

    #endregion

    #region U0003

    private const string U0003DiagnosticId = "U0003";

    private static readonly LocalizableString U0003Title =
        new LocalizableResourceString(nameof(Resources.U0003Title), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0003MessageFormat =
        new LocalizableResourceString(nameof(Resources.U0003MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString U0003Description =
        new LocalizableResourceString(nameof(Resources.U0003Description), Resources.ResourceManager,
            typeof(Resources));

    public static readonly DiagnosticDescriptor RuleU0003 = new(U0003DiagnosticId, U0003Title, U0003MessageFormat,
        Category.Interoperability,
        DiagnosticSeverity.Warning, true, U0003Description);

    #endregion
}