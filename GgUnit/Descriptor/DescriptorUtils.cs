namespace GgUnit.Descriptor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using GgUnit.Generator;

    public static class DescriptorUtils
    {
        /// <summary>
        /// Gets the DescriptorAttribute for a specific type's property.  Will return null if no DescriptorAttribute attribute is defined.
        /// </summary>
        /// <typeparam name="T">Type on which to look up properties DescriptorAttribute</typeparam>
        /// <param name="propertyName">Property name to retrieve DescriptorAttribute for</param>
        /// <returns>DescriptorAttribute</returns>
        public static DescriptorAttribute GetDescriptorFromPropertyName<T>(string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (null == propertyInfo)
            {
                var message = string.Format("Could not find property {0} for type {1}", propertyName, typeof(T).Name);
                throw new ArgumentException(message, "propertyName");
            }

            return GetDescriptorFromPropertyInfo(propertyInfo);
        }

        public static DescriptorAttribute GetDescriptor<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (null == memberExpression)
            {
                throw new ArgumentException("PropertyExpression cannot have a null Body", "propertyExpression");
            }

            var propertyInfo = memberExpression.Expression.Type.GetProperty(memberExpression.Member.Name);
            return GetDescriptorFromPropertyInfo(propertyInfo);
        }

        public static DescriptorAttribute GetDescriptorFromPropertyInfo(PropertyInfo propertyInfo)
        {
            DescriptorAttribute descriptorAttributeFromPropertyName;
            try
            {
                descriptorAttributeFromPropertyName = propertyInfo.GetCustomAttributes(typeof(DescriptorAttribute), false).Single() as DescriptorAttribute;
            }
            catch (InvalidOperationException)
            {
                // Swallow exception: this is meant for a property that doesn't have a DescriptorAttribute attribute.
                descriptorAttributeFromPropertyName = null;
            }

            return descriptorAttributeFromPropertyName;
        }

        /// <summary>
        /// This gets properties of a generic object T, it will only return properties that have DescriptorAttribute 
        /// associated with them and that match the PropertyTypeRequested.
        /// </summary>
        /// <typeparam name="T">Object type to fetch properties of.</typeparam>
        /// <param name="propertyTypeRequested">Filter for the type of properties fetched, defaults to All.</param>
        /// <param name="onlyIncludeInvalidateable">If true this only returns properties that can be made invalid.</param>
        /// <returns>IEnumerable of the PropertyInfo's</returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(
            PropertyTypeRequested propertyTypeRequested = PropertyTypeRequested.All, bool onlyIncludeInvalidateable = false)
        {
            var props = typeof(T).GetProperties();
            foreach (var propertyInfo in props)
            {
                var descriptor = GetDescriptorFromPropertyName<T>(propertyInfo.Name);
                if (null == descriptor)
                {
                    continue;
                }

                if (onlyIncludeInvalidateable && !descriptor.IsInvalidateable)
                {
                    continue;
                }

                switch (propertyTypeRequested)
                {
                    case PropertyTypeRequested.All:
                        yield return propertyInfo;
                        break;

                    case PropertyTypeRequested.Required:
                        if (descriptor.Required)
                        {
                            yield return propertyInfo;
                        }

                        break;

                    case PropertyTypeRequested.Optional:
                        if (!descriptor.Required)
                        {
                            yield return propertyInfo;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("propertyTypeRequested");
                }
            }
        }
    }
}