namespace NetDoc.Parser
{
    using Roslyn.Compilers.Common;

    public static class Helper
    {
        public static string RetrieveAccessModifier(CommonAccessibility declaredAccessibility)
        {
            var accessModifier = declaredAccessibility.ToString().ToLowerInvariant();

            if (declaredAccessibility == CommonAccessibility.ProtectedAndInternal ||
                declaredAccessibility == CommonAccessibility.ProtectedOrInternal)
            {
                accessModifier = "protected internal";
            }

            return accessModifier;
        }
    }
}
