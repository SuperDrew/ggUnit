namespace GgUnit.Generator
{
    using System;

    using Fare;

    public static class RegexGenerator
    {
        private static readonly Random Random = new Random();
        private static Xeger xeger;

        public static string GenerateMatch(string regex)
        {
            xeger = new Xeger(regex, Random);
            return xeger.Generate();
        }
    }
}
