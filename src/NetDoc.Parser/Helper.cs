namespace NetDoc.Parser
{
    using System;
    using Microsoft.CodeAnalysis;

    public static class Helper
    {
        public static string RetrieveAccessModifier(Accessibility declaredAccessibility)
        {
            var accessModifier = declaredAccessibility.ToString().ToLowerInvariant();

            if (declaredAccessibility == Accessibility.ProtectedAndInternal ||
                declaredAccessibility == Accessibility.ProtectedOrInternal)
            {
                accessModifier = "protected internal";
            }

            return accessModifier;
        }
    }
}
