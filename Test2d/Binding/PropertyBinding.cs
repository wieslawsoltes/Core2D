// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Test2d
{
    /// <summary>
    /// Connects the property of binding target object and the property of binding source object.
    /// </summary>
    public class PropertyBinding : ObservableObject
    {
        private PropertyBindingMode _mode;
        private object _source;
        private string _sourcePropertyName;
        private object _target;
        private string _targetPropertyName;

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// </summary>
        public PropertyBindingMode Mode
        {
            get { return _mode; }
            set { Update(ref _mode, value); }
        }

        /// <summary>
        /// Gets or sets the object to use as the binding source.
        /// </summary>
        public object Source
        {
            get { return _source; }
            set { Update(ref _source, value); }
        }

        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        public string SourcePropertyName
        {
            get { return _sourcePropertyName; }
            set { Update(ref _sourcePropertyName, value); }
        }

        /// <summary>
        /// Gets or sets the object to use as the binding target.
        /// </summary>
        public object Target
        {
            get { return _target; }
            set { Update(ref _target, value); }
        }

        /// <summary>
        /// Gets or sets the path to the binding target property.
        /// </summary>
        public string TargetPropertyName
        {
            get { return _targetPropertyName; }
            set { Update(ref _targetPropertyName, value); }
        }

        /// <summary>
        /// Initializes a new instance of the PropertyBinding class.
        /// </summary>
        public PropertyBinding()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PropertyBinding class and initializes binding.
        /// </summary>
        /// <param name="source">The Source object for the binding.</param>
        /// <param name="sourcePropertyName">The Source property for the binding.</param>
        /// <param name="target">The Target object for the binding.</param>
        /// <param name="targetPropertyName">The Target property for the binding.</param>
        /// <param name="mode">The binding mode.</param>
        public PropertyBinding(
            object source,
            string sourcePropertyName,
            object target,
            string targetPropertyName,
            PropertyBindingMode mode)
        {
            SetBinding(source, sourcePropertyName, target, targetPropertyName, mode);
        }

        /// <summary>
        /// Creates a new instance of the PropertyBinding class and initializes binding.
        /// </summary>
        /// <param name="source">The Source object for the binding.</param>
        /// <param name="sourcePropertyName">The Source property for the binding.</param>
        /// <param name="target">The Target object for the binding.</param>
        /// <param name="targetPropertyName">The Target property for the binding.</param>
        /// <param name="mode">The binding mode.</param>
        /// <returns></returns>
        public static PropertyBinding Create(
            object source,
            string sourcePropertyName,
            object target,
            string targetPropertyName,
            PropertyBindingMode mode = PropertyBindingMode.Default)
        {
            return new PropertyBinding(
                source,
                sourcePropertyName,
                target,
                targetPropertyName,
                mode);
        }
        
        /// <summary>
        /// Initializes binding
        /// </summary>
        /// <param name="source">The Source object for the binding.</param>
        /// <param name="sourcePropertyName">The Source property for the binding.</param>
        /// <param name="target">The Target object for the binding.</param>
        /// <param name="targetPropertyName">The Target property for the binding.</param>
        /// <param name="mode">The binding mode.</param>
        public void SetBinding(
            object source, 
            string sourcePropertyName, 
            object target, 
            string targetPropertyName, 
            PropertyBindingMode mode = PropertyBindingMode.Default)
        {
            StopObserving();

            _source = source;
            _sourcePropertyName = sourcePropertyName;
            _target = target;
            _targetPropertyName = targetPropertyName;
            _mode = mode;

            StartObserving();
            OneTimeInitialization();       
        }

        /// <summary>
        /// Starts observing the binding source and/or target properties.
        /// </summary>
        public void StartObserving()
        {
            if (_source != null && _target != null)
            {
                var inpcSource = _source as INotifyPropertyChanged;
                if (inpcSource != null)
                    inpcSource.PropertyChanged += ObserveSourceChanges;

                var inpcTarget = _target as INotifyPropertyChanged;
                if (inpcTarget != null)
                    inpcTarget.PropertyChanged += ObserveTargetChanges;
            }
        }

        /// <summary>
        /// Stops observing the binding source and/or target properties.
        /// </summary>
        public void StopObserving()
        {
            if (_source != null && _target != null)
            {
                var inpcSource = _source as INotifyPropertyChanged;
                if (inpcSource != null)
                    inpcSource.PropertyChanged -= ObserveSourceChanges;

                var inpcTarget = _target as INotifyPropertyChanged;
                if (inpcTarget != null)
                    inpcTarget.PropertyChanged -= ObserveTargetChanges;
            }
        }

        private void ObserveSourceChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _sourcePropertyName)
            {
                if (_mode == PropertyBindingMode.OneWay 
                    || _mode == PropertyBindingMode.TwoWay)
                {
                    UpdateTarget();
                }
            }
        }

        private void ObserveTargetChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _targetPropertyName)
            {
                if (_mode == PropertyBindingMode.TwoWay 
                    || _mode == PropertyBindingMode.OneWayToSource)
                {
                    UpdateSource();
                }
            }
        }

        private void OneTimeInitialization()
        {
            if (_mode == PropertyBindingMode.OneTime 
                || _mode == PropertyBindingMode.OneWay 
                || _mode == PropertyBindingMode.TwoWay)
            {
                UpdateTarget();
            }
            else if (_mode == PropertyBindingMode.OneWayToSource)
            {
                UpdateSource();
            }
        }

        /// <summary>
        /// Indicates whether the PropertyBinding properties are valid.
        /// </summary>
        /// <returns></returns>
        public bool IsBindingValid()
        {
            return _source != null
                && !string.IsNullOrWhiteSpace(_sourcePropertyName)
                && _target != null
                && !string.IsNullOrWhiteSpace(_targetPropertyName);
        }

        /// <summary>
        /// Updates the source property value using the current target property value.
        /// </summary>
        public void UpdateSource()
        {
            if (IsBindingValid())
            {
                try
                {
                    var value = GetPropertyValue(_target, _targetPropertyName);
                    SetPropertyValue(_source, _sourcePropertyName, value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Updates the target property value using the current source property value.
        /// </summary>
        public void UpdateTarget()
        {
            if (IsBindingValid())
            {
                try
                {
                    var value = GetPropertyValue(_source, _sourcePropertyName);
                    SetPropertyValue(_target, _targetPropertyName, value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be returned.</param>
        /// <param name="propertyName">The object property name.</param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj
                .GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != property && property.CanRead)
            {
                return property.GetValue(obj, null);
            }
            return null;
        }

        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="propertyName">The object property name.</param>
        /// <param name="value">The new property value.</param>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var property = obj
                .GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != property && property.CanWrite)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType)
                    ?? property.PropertyType;
                object result = (value == null) ? 
                    null : 
                    Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

                property.SetValue(obj, result, null);
            }
        }
    }
}
