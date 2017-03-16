namespace GgUnit.Generator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    
    using Descriptor;

    public static class StringUtils
    {
        private const int DefaultStringLength = 10;
        private const int DefaultDecimalPoints = 15;
        private static readonly List<string> EmailExtensions = new List<string> { "com", "net", "org" };
        private static readonly Random Random = new Random();

        /// <summary>
        /// Creates a random string based on Descriptors MaxLength and CharacterType got 
        /// from the propertyExpression
        /// </summary>
        /// <param name="propertyExpression">Property to create a random valid value for.</param>
        /// <returns>string</returns>
        public static string CreateRandomValidValue<T>(Expression<Func<T>> propertyExpression)
        {
            return CreateRandomValidValue(DescriptorUtils.GetDescriptor(propertyExpression));
        }

        /// <summary>
        /// Creates a random string based on Descriptors MaxLength and CharacterType got 
        /// from the propertyExpression
        /// </summary>
        /// <param name="propertyInfo">Property to create a random valid value for.</param>
        /// <returns>string</returns>
        public static string CreateRandomValidValue(PropertyInfo propertyInfo)
        {
            return CreateRandomValidValue(DescriptorUtils.GetDescriptorFromPropertyInfo(propertyInfo));
        }

        /// <summary>
        /// Creates a random invalid value based on the DescriptorAttribute for the given property.
        /// </summary>
        /// <typeparam name="T">The type in which to look up the property and it's DescriptorAttribute.</typeparam>
        /// <param name="value">Value of T.</param>
        /// <param name="property">Property to create a invalid value for.</param>
        /// <param name="invalidationMethod">Which type of invalid value to create.</param>
        /// <returns>Object which should fail validation for the given parameters.</returns>
        public static object CreateRandomInvalidValue<T>(T value, PropertyInfo property, InvalidationMethod invalidationMethod = InvalidationMethod.IncorrectLength)
        {
            switch (invalidationMethod)
            {
                case InvalidationMethod.SetToNull:
                    return null;
                case InvalidationMethod.IncorrectLength:
                    return CreateRandomIncorrectLength(value, property);
                default:
                    throw new ArgumentOutOfRangeException("invalidationMethod");
            }
        }

        public static string CreateDecimalValue()
        {
            return (Random.NextDouble() * 100).ToString(CultureInfo.InvariantCulture);
        }

        public static string CreateNegativeDecimalValue()
        {
            return (Random.NextDouble() * -100).ToString(CultureInfo.InvariantCulture);
        }

        // ReSharper disable once UnusedMember.Local : Used via reflection
        public static T PickRandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Next(values.Length));
        }

        public static string ReplaceNullOrEmptyStringWithReadableValues(string value)
        {
            if (value == null)
            {
                return "<null>";
            }

            if (value == string.Empty)
            {
                return "<empty string>";
            }

            return value;
        }

        private static string CreateRandomEmailAddress()
        {
            return string.Format(
                "{0}@{1}.{2}",
                StringCreation.CreateRandomCfString(DefaultStringLength, characterType: CharacterType.DigitsOrLetters),
                StringCreation.CreateRandomCfString(DefaultStringLength, characterType: CharacterType.DigitsOrLetters),
                EmailExtensions[Random.Next(EmailExtensions.Count)]);
        }

        private static object CreateRandomIncorrectLength<T>(T value, PropertyInfo property)
        {
            var descriptor = DescriptorUtils.GetDescriptorFromPropertyName<T>(property.Name);

            // If property doesn't have a DescriptorAttribute give back a random string and log this.
            if (null == descriptor)
            {
                Debug.WriteLine("Generation for invalid value for property {0} with no DescriptorAttribute.", property);
                return StringCreation.CreateRandomCfString(DefaultStringLength, false, CharacterType.Letters);
            }

            // Detect if the property is a List
            if (property.GetValue(value) is IList && property.PropertyType.IsGenericType)
            {
                // Set the list to an empty new list
                var propertyConstructor = property.PropertyType.GetConstructor(Type.EmptyTypes);
                if (propertyConstructor != null)
                {
                    return propertyConstructor.Invoke(null);
                }

                throw new ArgumentException("Property is a List but has no default constructor", "property");
            }
            
            // If the descriptorAttribute is an enum what do I give back as invalid?
            if (null != descriptor.EnumType)
            {
                // TODO: This length doesn't necessarily reflect the invalidation length in the db for all uses of enums.
                return StringCreation.CreateRandomCfString(11, false, CharacterType.DigitsOrLetters);
            }

            // Return random string that isn't in the array if the array is defined.
            if (null != descriptor.Array)
            {
                var randomString = string.Empty;
                var isInArray = true;
                while (isInArray)
                {
                    randomString = StringCreation.CreateRandomCfString(11, false, CharacterType.DigitsOrLetters);
                    if (!descriptor.Array.Contains(randomString))
                    {
                        isInArray = false;
                    }
                }

                return randomString;
            }

            // Return value longer than maxLength is a regex is used.
            if (null != descriptor.Regex)
            {
                return CreateInvalidLengthCfString(descriptor);
            }

            // Return value generated from PropertyType
            switch (descriptor.PropertyType)
            {
                case PropertyType.String:
                    return CreateInvalidLengthCfString(descriptor);
                case PropertyType.Int:
                    return StringCreation.CreateRandomCfString(DefaultStringLength, false, CharacterType.Letters);
                case PropertyType.Decimal:
                    return StringCreation.CreateRandomCfString(DefaultStringLength, false, CharacterType.Letters);
                case PropertyType.Bool:
                    return StringCreation.CreateRandomCfString(DefaultStringLength, false, CharacterType.Letters);
                case PropertyType.Date:
                    return StringCreation.CreateRandomCfString(DefaultStringLength, false, CharacterType.DigitsOrLetters);
                case PropertyType.Email:
                    return StringCreation.CreateRandomCfString(DefaultStringLength, false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string CreateInvalidLengthCfString(DescriptorAttribute descriptorAttribute)
        {
            return StringCreation.CreateRandomCfString(
                descriptorAttribute.MaxLength + 1,
                false,
                CharacterType.Letters);
        }
        
        private static string CreateRandomValidValue(DescriptorAttribute descriptorAttribute)
        {
            // Return random enum value if enumType is defined
            if (null != descriptorAttribute.EnumType)
            {
                var methodInfo = typeof(StringUtils).GetMethod("PickRandomEnumValue");
                var genericMethod = methodInfo.MakeGenericMethod(descriptorAttribute.EnumType);
                var enumValue = genericMethod.Invoke(null, null);
                return enumValue.ToString();
            }

            // Return random member of array if it is defined.
            if (null != descriptorAttribute.Array)
            {
                return descriptorAttribute.Array[Random.Next(descriptorAttribute.Array.Length - 1)];
            }

            // Return value generated from Regex if it is defined.
            if (null != descriptorAttribute.Regex)
            {
                return RegexGenerator.GenerateMatch(descriptorAttribute.Regex);
            }

            // Return value generated from PropertyType
            switch (descriptorAttribute.PropertyType)
            {
                case PropertyType.String:
                    var characterType = descriptorAttribute.CharacterType == CharacterType.NotAssigned
                                            ? CharacterType.Anything
                                            : descriptorAttribute.CharacterType;
                    var maxLength = descriptorAttribute.MaxLength > 0 ? descriptorAttribute.MaxLength : DefaultStringLength;
                    return StringCreation.CreateRandomCfString(maxLength, false, characterType);
                case PropertyType.Int:
                    return Random.Next().ToString(CultureInfo.InvariantCulture);
                case PropertyType.Decimal:
                    var decimalPoints = descriptorAttribute.DecimalPoints >= 0 ? descriptorAttribute.DecimalPoints : DefaultDecimalPoints;
                    return Math.Round(Random.NextDouble() * 100, decimalPoints).ToString(CultureInfo.InvariantCulture);
                case PropertyType.Bool:
                    return CreateRandomBool();
                case PropertyType.Date:
                    return CreateRandomDate();
                case PropertyType.Email:
                    return CreateRandomEmailAddress();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string CreateRandomBool()
        {
            var randInt = Random.Next(0, 1);
            return randInt <= 0.5 ? "true" : "false";
        }

        private static string CreateRandomDate()
        {
            return DateTime.UtcNow.ToLongDateString();
        }
    }
}