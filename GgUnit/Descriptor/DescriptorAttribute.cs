namespace GgUnit.Descriptor
{
    using System;

    /// <summary>
    /// Describes a property for use in generating test case data for the property.  
    /// The different methods for describing the property have a priority in which they are evaluated.
    /// </summary>
    public class DescriptorAttribute : Attribute
    {
        public readonly int MaxLength;
        public readonly bool Required;
        public readonly CharacterType CharacterType;
        public readonly PropertyType PropertyType;
        public readonly string Regex;
        public readonly Type EnumType;
        public readonly string[] Array;
        public readonly int DecimalPoints;
        public readonly bool IsInvalidateable = true;

        public DescriptorAttribute(string[] array, bool required = false)
        {
            this.Array = array;
            this.Required = required;
        }

        public DescriptorAttribute(
            Type enumType,
            bool required = false)
        {
            this.EnumType = enumType;
            this.Required = required;
        }

        public DescriptorAttribute(
            int maxLength,
            Type enumType,
            bool required = false)
        {
            this.MaxLength = maxLength;
            this.EnumType = enumType;
            this.Required = required;
        }

        public DescriptorAttribute(
            int maxLength,
            string regex,
            bool required = false)
        {
            this.MaxLength = maxLength;
            this.Required = required;
            this.Regex = regex;
        }

        public DescriptorAttribute(
            PropertyType propertyType,
            bool required = false,
            bool isInvalidateable = true)
        {
            this.PropertyType = propertyType;
            this.Required = required;
            this.IsInvalidateable = isInvalidateable;
        }

        public DescriptorAttribute(
            int maxLength,
            PropertyType propertyType,
            bool required = false)
        {
            this.MaxLength = maxLength;
            this.PropertyType = propertyType;
            this.Required = required;
        }

        public DescriptorAttribute(
            int maxLength,
            bool required = false,
            CharacterType characterType = CharacterType.Anything,
            bool isInvalidateable = true)
        {
            this.MaxLength = maxLength;
            this.Required = required;
            this.CharacterType = characterType;
            this.PropertyType = PropertyType.String;
            this.IsInvalidateable = isInvalidateable;
        }

        public DescriptorAttribute(
           PropertyType propertyType,
           bool required,
           int decimalPoints)
        {
            this.PropertyType = propertyType;
            this.Required = required;
            this.DecimalPoints = decimalPoints;
        }
    }
}