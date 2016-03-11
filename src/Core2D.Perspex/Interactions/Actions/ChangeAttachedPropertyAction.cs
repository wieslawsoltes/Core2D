// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Xaml.Interactions.Core;
using Perspex.Xaml.Interactivity;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Core2D.Perspex.Interactions.Actions
{
    /// <summary>
    /// An action that will change a specified attached property to a specified value when invoked.
    /// </summary>
    public sealed class ChangeAttachedPropertyAction : PerspexObject, IAction
    {
        /// <summary>
        /// Identifies the <seealso cref="PropertyOwnerType"/> dependency property.
        /// </summary>
        public static readonly PerspexProperty PropertyOwnerTypeProperty =
            PerspexProperty.Register<ChangeAttachedPropertyAction, Type>("PropertyOwnerType");

        /// <summary>
        /// Identifies the <seealso cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly PerspexProperty PropertyNameProperty =
            PerspexProperty.Register<ChangeAttachedPropertyAction, string>("PropertyName");

        /// <summary>
        /// Identifies the <seealso cref="TargetObject"/> dependency property.
        /// </summary>
        public static readonly PerspexProperty TargetObjectProperty =
            PerspexProperty.Register<ChangeAttachedPropertyAction, object>("TargetObject");

        /// <summary>
        /// Identifies the <seealso cref="Value"/> dependency property.
        /// </summary>
        public static readonly PerspexProperty ValueProperty =
            PerspexProperty.Register<ChangeAttachedPropertyAction, object>("Value");

        /// <summary>
        /// Gets or sets the owner type of the property to change. This is a dependency property.
        /// </summary>
        public Type PropertyOwnerType
        {
            get { return (Type)this.GetValue(ChangeAttachedPropertyAction.PropertyOwnerTypeProperty); }
            set { this.SetValue(ChangeAttachedPropertyAction.PropertyOwnerTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the property to change. This is a dependency property.
        /// </summary>
        public string PropertyName
        {
            get { return (string)this.GetValue(ChangeAttachedPropertyAction.PropertyNameProperty); }
            set { this.SetValue(ChangeAttachedPropertyAction.PropertyNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to set. This is a dependency property.
        /// </summary>
        public object Value
        {
            get { return this.GetValue(ChangeAttachedPropertyAction.ValueProperty); }
            set { this.SetValue(ChangeAttachedPropertyAction.ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the object whose property will be changed.
        /// If <seealso cref="TargetObject"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is a dependency property.
        /// </summary>
        public object TargetObject
        {
            get { return (object)this.GetValue(ChangeAttachedPropertyAction.TargetObjectProperty); }
            set { this.SetValue(ChangeAttachedPropertyAction.TargetObjectProperty, value); }
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
            if (this.GetValue(ChangeAttachedPropertyAction.TargetObjectProperty) != PerspexProperty.UnsetValue)
            {
                targetObject = this.TargetObject;
            }
            else
            {
                targetObject = sender as PerspexObject;
            }

            if (targetObject == null || this.PropertyName == null)
            {
                return false;
            }

            this.UpdatePerspexPropertyValue(targetObject);
            return true;
        }

        private void UpdatePerspexPropertyValue(object targetObject)
        {
            var property = PerspexPropertyRegistry.Instance.GetAttached(PropertyOwnerType).Where((p) => p.Name == this.PropertyName).FirstOrDefault();
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

                var pespexObject = targetObject as PerspexObject;
                if (pespexObject != null)
                {
                    pespexObject.SetValue(property, result);
                }
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
                    property.PropertyType.Name),
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
                    "Cannot find a property named {0} on type {1}.",
                    this.PropertyName,
                    this.PropertyOwnerType));
            }
            else if (property.IsReadOnly)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Cannot find a property named {0} on type {1}.",
                    this.PropertyName,
                    this.PropertyOwnerType));
            }
        }
    }
}
