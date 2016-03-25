// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Xaml.Interactions.Core;
using Perspex.Xaml.Interactivity;
using System;
using System.Globalization;
using System.Reflection;

namespace Core2D.Perspex.Interactions.Actions
{
    /// <summary>
    /// An action that will change a specified property to a specified value when invoked.
    /// </summary>
    public sealed class ChangePropertyAction : PerspexObject, IAction
    {
        /// <summary>
        /// Identifies the <seealso cref="PropertyName"/> perspex property.
        /// </summary>
        public static readonly PerspexProperty<string> PropertyNameProperty =
            PerspexProperty.Register<ChangePropertyAction, string>(nameof(PropertyName));

        /// <summary>
        /// Identifies the <seealso cref="TargetObject"/> perspex property.
        /// </summary>
        public static readonly PerspexProperty<object> TargetObjectProperty =
            PerspexProperty.Register<ChangePropertyAction, object>(nameof(TargetObject));

        /// <summary>
        /// Identifies the <seealso cref="Value"/> perspex property.
        /// </summary>
        public static readonly PerspexProperty<object> ValueProperty =
            PerspexProperty.Register<ChangePropertyAction, object>(nameof(Value));

        /// <summary>
        /// Gets or sets the name of the property to change. This is a perspex property.
        /// </summary>
        public string PropertyName
        {
            get { return this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to set. This is a perspex property.
        /// </summary>
        public object Value
        {
            get { return this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the object whose property will be changed.
        /// If <seealso cref="TargetObject"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is a perspex property.
        /// </summary>
        public object TargetObject
        {
            get { return this.GetValue(TargetObjectProperty); }
            set { this.SetValue(TargetObjectProperty, value); }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>True if updating the property value succeeds; else false.</returns>
        public object Execute(object sender, object parameter)
        {
            object targetObject;
            if (this.GetValue(TargetObjectProperty) != PerspexProperty.UnsetValue)
            {
                targetObject = this.TargetObject;
            }
            else
            {
                targetObject = sender;
            }

            if (targetObject == null || this.PropertyName == null)
            {
                return false;
            }

            PerspexObject perspexObject = targetObject as PerspexObject;
            if (perspexObject != null)
            {
                PerspexProperty perspexProperty = PerspexPropertyRegistry.Instance.FindRegistered(perspexObject, this.PropertyName);
                if (perspexProperty != null)
                {
                    this.UpdatePerspexPropertyValue(perspexObject, perspexProperty);
                    return true;
                }
            }

            this.UpdatePropertyValue(targetObject);
            return true;
        }

        private void UpdatePropertyValue(object targetObject)
        {
            Type targetType = targetObject.GetType();
            PropertyInfo propertyInfo = targetType.GetRuntimeProperty(this.PropertyName);
            this.ValidateProperty(targetType.Name, propertyInfo);

            Exception innerException = null;
            try
            {
                object result = null;
                string valueAsString = null;
                Type propertyType = propertyInfo.PropertyType;
                TypeInfo propertyTypeInfo = propertyType.GetTypeInfo();
                if (this.Value == null)
                {
                    // The result can be null if the type is generic (nullable), or the default value of the type in question
                    result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
                }
                else if (propertyTypeInfo.IsAssignableFrom(this.Value.GetType().GetTypeInfo()))
                {
                    result = this.Value;
                }
                else
                {
                    valueAsString = this.Value.ToString();
                    result = propertyTypeInfo.IsEnum ? Enum.Parse(propertyType, valueAsString, false) :
                        TypeConverterHelper.Convert(valueAsString, propertyType.FullName);
                }

                propertyInfo.SetValue(targetObject, result, new object[0]);
            }
            catch (FormatException e)
            {
                innerException = e;
            }
            catch (ArgumentException e)
            {
                innerException = e;
            }

            if (innerException != null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                    this.Value != null ? this.Value.GetType().Name : "null",
                    this.PropertyName,
                    propertyInfo.PropertyType.Name),
                    innerException);
            }
        }

        /// <summary>
        /// Ensures the property is not null and can be written to.
        /// </summary>
        private void ValidateProperty(string targetTypeName, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find a property named {0} on type {1}.",
                    this.PropertyName,
                    targetTypeName));
            }
            else if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find a property named {0} on type {1}.",
                    this.PropertyName,
                    targetTypeName));
            }
        }

        private void UpdatePerspexPropertyValue(PerspexObject perspexObject, PerspexProperty property)
        {
            this.ValidatePerspexProperty(property);

            Exception innerException = null;
            try
            {
                object result = null;
                string valueAsString = null;
                Type propertyType = property.PropertyType;
                TypeInfo propertyTypeInfo = propertyType.GetTypeInfo();
                if (this.Value == null)
                {
                    // The result can be null if the type is generic (nullable), or the default value of the type in question
                    result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
                }
                else if (propertyTypeInfo.IsAssignableFrom(this.Value.GetType().GetTypeInfo()))
                {
                    result = this.Value;
                }
                else
                {
                    valueAsString = this.Value.ToString();
                    result = propertyTypeInfo.IsEnum ? Enum.Parse(propertyType, valueAsString, false) :
                        TypeConverterHelper.Convert(valueAsString, propertyType.FullName);
                }

                perspexObject.SetValue(property, result);
            }
            catch (FormatException e)
            {
                innerException = e;
            }
            catch (ArgumentException e)
            {
                innerException = e;
            }

            if (innerException != null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot assign value of type {0} to property {1} of type {2}. The {1} property can be assigned only values of type {2}.",
                    this.Value?.GetType().Name ?? "null",
                    this.PropertyName,
                    perspexObject?.GetType().Name ?? "null"),
                    innerException);
            }
        }

        /// <summary>
        /// Ensures the property is not null and can be written to.
        /// </summary>
        private void ValidatePerspexProperty(PerspexProperty property)
        {
            if (property == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find a property named {0}.",
                    this.PropertyName));
            }
            else if (property.IsReadOnly)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find a property named {0}.",
                    this.PropertyName));
            }
        }
    }
}
