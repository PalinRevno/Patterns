using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace SupportFramework.Patterns.Enums
{
    /// <summary>
    /// A static enumeration documentation helper
    /// </summary>
    public static class EnumsHelper
    {
        /// <summary>
        /// Retrieves the <see cref="System.ComponentModel.DescriptionAttribute"/> of the provided enumeration value
        /// </summary>
        /// <param name="value">The value itself</param>
        /// <returns>The documentation string or the value itself</returns>
        public static string GetDescription(Enum value)
        {
            string responseValue;
            FieldInfo enumField;
            DescriptionAttribute[] attributes;

            // Retrieving the provided value as a reflection field of the type
            enumField = value.GetType().GetField(value.ToString());

            // Getting the attributes of the enum value
            attributes = (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // Checking if there are any attributes at all
            if (attributes != null && attributes.Length > 0)
            {
                // Retrieving the description
                responseValue = attributes[0].Description;
            }
            else
            {
                // Returning the value itself
                responseValue = value.ToString();
            }

            return responseValue;
        }

        /// <summary>
        /// Parses the enum value directly and throws a error if fails
        /// </summary>
        /// <typeparam name="T">Any enum</typeparam>
        /// <param name="value">String representing a value of the provided Enum</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Initialized Enum value</returns>
        public static T Parse<T>(string value)
            where T : struct, IComparable
        {
            T responseValue;

            if (!Enum.TryParse<T>(value, out responseValue))
            {
                throw new ArgumentException("Could not parse the enum value : " + value + ". Of type : " + typeof(T).ToString() + ".");
            }

            return responseValue;
        }
    }
}
