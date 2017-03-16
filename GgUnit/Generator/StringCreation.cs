namespace GgUnit.Generator
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    using GgUnit.Descriptor;

    public static class StringCreation
    {
        /// <summary>
        /// Creates a random string that does not start or end with spaces, this is to work around the Cold Fusion de-serialize trimming whitespace.
        /// </summary>
        /// <param name="length">Length of string to create.</param>
        /// <param name="replaceTroublesomeCharacters">Replaces any troublesome character in the string with a different random letter.</param>
        /// <param name="characterType">Specify the type of characters to generate.</param>
        /// <returns>random string of length requested</returns>
        public static string CreateRandomCfString(int length, bool replaceTroublesomeCharacters = true, CharacterType characterType = CharacterType.Anything)
        {
            var randomString = CreateRandomString(length, characterType);
            var trimmedString = randomString.Trim();
            var trimAmount = randomString.Length - trimmedString.Length;
            if (trimAmount > 0)
            {
                var trimReplacement = CreateRandomString(trimAmount, characterType, 33);
                randomString = trimmedString + trimReplacement;
            }

            if (replaceTroublesomeCharacters)
            {
                randomString = randomString.Replace("#", CreateRandomString(1, CharacterType.DigitsOrLetters));
                randomString = randomString.Replace("\\", CreateRandomString(1, CharacterType.DigitsOrLetters));
            }

            return randomString;
        }

        /// <summary>
        /// Generates a random string of a specified length using RNGCryptoServiceProvider.  
        /// Does not include any bytes that fall outside the lowerByte to upperByte range.
        /// Defaults to ASCII range.
        /// </summary>
        /// <param name="length">Length of string to create.</param>
        /// <param name="characterType"></param>
        /// <param name="lowerByte">Lower ASCII bound, does not include characters with decimal representation below this.</param>
        /// <param name="upperByte">Upper ASCII bound, does not include characters with decimal representation above this.</param>
        /// <returns></returns>
        public static string CreateRandomString(
            int length, 
            CharacterType characterType = CharacterType.Anything,
            int lowerByte = 32, 
            int upperByte = 126)
        {
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            var stringBuilder = new StringBuilder();
            var bytes = new byte[1];
            var firstCharacter = true;
            while (stringBuilder.Length < length)
            {
                // TODO Rather than randomly missing the correct byte and character type generate in the correct byte range.
                randomNumberGenerator.GetBytes(bytes);
                if (bytes[0] < lowerByte || bytes[0] > upperByte)
                {
                    continue;
                }

                var c = (char)bytes[0];
                if (characterType == CharacterType.Digits && firstCharacter && c == '0')
                {
                    continue;
                }

                firstCharacter = !AppendCharIfOfCorrectCharacterType(characterType, c, stringBuilder);
            }

            return stringBuilder.ToString();
        }

        private static bool AppendCharIfOfCorrectCharacterType(
            CharacterType characterType,
            char c,
            StringBuilder stringBuilder)
        {
            var charWasAppended = false;
            switch (characterType)
            {
                case CharacterType.Digits:
                    if (char.IsDigit(c))
                    {
                        charWasAppended = AppendChar(c, stringBuilder);
                    }

                    break;
                case CharacterType.Letters:
                    if (char.IsLetter(c))
                    {
                        charWasAppended = AppendChar(c, stringBuilder);
                    }

                    break;
                case CharacterType.LettersUpperCase:
                    if (char.IsLetter(c))
                    {
                        charWasAppended = AppendChar(
                            c.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(),
                            stringBuilder);
                    }

                    break;
                case CharacterType.DigitsOrLetters:
                    if (char.IsLetterOrDigit(c))
                    {
                        charWasAppended = AppendChar(c, stringBuilder);
                    }

                    break;
                case CharacterType.Anything:
                    charWasAppended = AppendChar(c, stringBuilder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("characterType");
            }

            return charWasAppended;
        }

        private static bool AppendChar<T>(T c, StringBuilder stringBuilder)
        {
            stringBuilder.Append(c);
            return true;
        }
    }
}