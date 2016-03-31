// Original source code
// http://www.codeproject.com/KB/cs/stringenum.aspx
// CodeBureau - Matt Simner
// Licensed under The Code Project Open License (CPOL) http://www.codeproject.com/info/cpol10.aspx

using System;
using System.Collections;
using System.Reflection;

namespace netDxf
{
    #region Class StringEnum

    /// <summary>
    /// Helper class for working with 'extended' enums using <see cref="StringValueAttribute"/> attributes.
    /// </summary>
    public class StringEnum
    {
        #region Instance implementation

        private readonly Type enumType;
        private static readonly Hashtable stringValues = new Hashtable();

        /// <summary>
        /// Creates a new <see cref="StringEnum"/> instance.
        /// </summary>
        /// <param name="enumType">Enum type.</param>
        public StringEnum(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("Supplied type must be an Enum.  Type was {0}", enumType));

            this.enumType = enumType;
        }

        /// <summary>
        /// Gets the string value associated with the given enum value.
        /// </summary>
        /// <param name="valueName">Name of the enum value.</param>
        /// <returns>String Value</returns>
        public string GetStringValue(string valueName)
        {
            string stringValue;
            try
            {
               Enum type = (Enum) Enum.Parse(this.enumType, valueName);
                stringValue = GetStringValue(type);
            }
            catch
            {
                return null;
            } //Swallow!

            return stringValue;
        }

        /// <summary>
        /// Gets the string values associated with the enum.
        /// </summary>
        /// <returns>String value array</returns>
        public Array GetStringValues()
        {
            ArrayList values = new ArrayList();
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in this.enumType.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                        values.Add(attrs[0].Value);
            }

            return values.ToArray();
        }

        /// <summary>
        /// Gets the values as a 'bindable' list data source.
        /// </summary>
        /// <returns>IList for data binding</returns>
        public IList GetListValues()
        {
            Type underlyingType = Enum.GetUnderlyingType(this.enumType);

            ArrayList values = new ArrayList();
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in this.enumType.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                    {
                        object str = Convert.ChangeType(Enum.Parse(this.enumType, fi.Name), underlyingType);
                        if (str == null)
                            throw new Exception();
                        values.Add(new DictionaryEntry(str, attrs[0].Value));
                    }
            }

            return values;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string stringValue)
        {
            return Parse(this.enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string stringValue, bool ignoreCase)
        {
            return Parse(this.enumType, stringValue, ignoreCase) != null;
        }

        /// <summary>
        /// Gets the underlying enum type for this instance.
        /// </summary>
        /// <value></value>
        public Type EnumType
        {
            get { return this.enumType; }
        }

        #endregion

        #region Static implementation

        /// <summary>
        /// Gets a string value for a particular enum value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>String Value associated via a <see cref="StringValueAttribute"/> attribute, or null if not found.</returns>
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (stringValues.ContainsKey(value))
                output = ((StringValueAttribute) stringValues[value]).Value;
            else
            {
                //Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                    {
                        stringValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
            }
            return output;
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value (case sensitive).
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="stringValue">String value.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue)
        {
            return Parse(type, stringValue, false);
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="stringValue">String value.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue, bool ignoreCase)
        {
            object output = null;
            string enumStringValue = null;

            if (!type.IsEnum)
                throw new ArgumentException(string.Format("Supplied type must be an Enum.  Type was {0}", type));

            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in type.GetFields())
            {
                //Check for our custom attribute
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
                if (attrs != null)
                    if (attrs.Length > 0)
                        enumStringValue = attrs[0].Value;

                //Check for equality then select actual enum value.
                if (string.Compare(enumStringValue, stringValue, ignoreCase) == 0)
                {
                    if (Enum.IsDefined(type, fi.Name))
                        output = Enum.Parse(type, fi.Name);
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string stringValue)
        {
            return Parse(enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string stringValue, bool ignoreCase)
        {
            return Parse(enumType, stringValue, ignoreCase) != null;
        }

        #endregion
    }

    #endregion

    #region Class StringValueAttribute

    /// <summary>
    /// Simple attribute class for storing String Values
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        private readonly string value;

        /// <summary>
        /// Creates a new <see cref="StringValueAttribute"/> instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public StringValueAttribute(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value
        {
            get { return this.value; }
        }
    }

    #endregion
}