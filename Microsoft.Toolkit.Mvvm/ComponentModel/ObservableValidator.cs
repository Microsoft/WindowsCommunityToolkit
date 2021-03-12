// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.Mvvm.ComponentModel
{
    /// <summary>
    /// A base class for objects implementing the <see cref="INotifyDataErrorInfo"/> interface. This class
    /// also inherits from <see cref="ObservableObject"/>, so it can be used for observable items too.
    /// </summary>
    public abstract class ObservableValidator : ObservableObject, INotifyDataErrorInfo
    {
        /// <summary>
        /// The <see cref="ConditionalWeakTable{TKey,TValue}"/> instance used to track compiled delegates to validate entities.
        /// </summary>
        private static readonly ConditionalWeakTable<Type, Action<object>> EntityValidatorMap = new();

        /// <summary>
        /// The <see cref="ConditionalWeakTable{TKey, TValue}"/> instance used to track display names for properties to validate.
        /// </summary>
        /// <remarks>
        /// This is necessary because we want to reuse the same <see cref="ValidationContext"/> instance for all validations, but
        /// with the same behavior with repsect to formatted names that new instances would have provided. The issue is that the
        /// <see cref="ValidationContext.DisplayName"/> property is not refreshed when we set <see cref="ValidationContext.MemberName"/>,
        /// so we need to replicate the same logic to retrieve the right display name for properties to validate and update that
        /// property manually right before passing the context to <see cref="Validator"/> and proceed with the normal functionality.
        /// </remarks>
        private static readonly ConditionalWeakTable<Type, Dictionary<string, string>> DisplayNamesMap = new();

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="HasErrors"/>.
        /// </summary>
        private static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new(nameof(HasErrors));

        /// <summary>
        /// The <see cref="ValidationContext"/> instance currenty in use.
        /// </summary>
        private readonly ValidationContext validationContext;

        /// <summary>
        /// The <see cref="Dictionary{TKey,TValue}"/> instance used to store previous validation results.
        /// </summary>
        private readonly Dictionary<string, List<ValidationResult>> errors = new();

        /// <summary>
        /// Indicates the total number of properties with errors (not total errors).
        /// This is used to allow <see cref="HasErrors"/> to operate in O(1) time, as it can just
        /// check whether this value is not 0 instead of having to traverse <see cref="errors"/>.
        /// </summary>
        private int totalErrors;

        /// <inheritdoc/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
        /// This constructor will create a new <see cref="ValidationContext"/> that will
        /// be used to validate all properties, which will reference the current instance
        /// and no additional services or validation properties and settings.
        /// </summary>
        protected ObservableValidator()
        {
            this.validationContext = new ValidationContext(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
        /// This constructor will create a new <see cref="ValidationContext"/> that will
        /// be used to validate all properties, which will reference the current instance.
        /// </summary>
        /// <param name="items">A set of key/value pairs to make available to consumers.</param>
        protected ObservableValidator(IDictionary<object, object?>? items)
        {
            this.validationContext = new ValidationContext(this, items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
        /// This constructor will create a new <see cref="ValidationContext"/> that will
        /// be used to validate all properties, which will reference the current instance.
        /// </summary>
        /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance to make available during validation.</param>
        /// <param name="items">A set of key/value pairs to make available to consumers.</param>
        protected ObservableValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        {
            this.validationContext = new ValidationContext(this, serviceProvider, items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
        /// This constructor will store the input <see cref="ValidationContext"/> instance,
        /// and it will use it to validate all properties for the current viewmodel.
        /// </summary>
        /// <param name="validationContext">
        /// The <see cref="ValidationContext"/> instance to use to validate properties.
        /// <para>
        /// This instance will be passed to all <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult})"/>
        /// calls executed by the current viewmodel, and its <see cref="ValidationContext.MemberName"/> property will be updated every time
        /// before the call is made to set the name of the property being validated. The property name will not be reset after that, so the
        /// value of <see cref="ValidationContext.MemberName"/> will always indicate the name of the last property that was validated, if any.
        /// </para>
        /// </param>
        protected ObservableValidator(ValidationContext validationContext)
        {
            this.validationContext = validationContext;
        }

        /// <inheritdoc/>
        public bool HasErrors => this.totalErrors > 0;

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This method is just like <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/>, just with the addition
        /// of the <paramref name="validate"/> parameter. If that is set to <see langword="true"/>, the new value will be
        /// validated and <see cref="ErrorsChanged"/> will be raised if needed. Following the behavior of the base method,
        /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
        /// are not raised if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(ref T field, T newValue, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(ref field, newValue, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,bool,string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(ref field, newValue, comparer, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event. Similarly to
        /// the <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/> method, this overload should only be
        /// used when <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/> can't be used directly.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This method is just like <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/>, just with the addition
        /// of the <paramref name="validate"/> parameter. As such, following the behavior of the base method,
        /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
        /// are not raised if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(oldValue, newValue, callback, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,Action{T},bool,string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(oldValue, newValue, comparer, callback, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>, with the difference being that this
        /// method is used to relay properties from a wrapped model in the current instance. For more info, see the docs for
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            bool propertyChanged = SetProperty(oldValue, newValue, model, callback, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>,
        /// with the difference being that this method is used to relay properties from a wrapped model in the
        /// current instance. For more info, see the docs for
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            bool propertyChanged = SetProperty(oldValue, newValue, comparer, model, callback, propertyName);

            if (propertyChanged && validate)
            {
                ValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(ref T,T,string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(ref T field, T newValue, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(ref field, newValue, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(ref T,T,IEqualityComparer{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(ref field, newValue, comparer, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(T oldValue, T newValue, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(T,T,IEqualityComparer{T},Action{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, comparer, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, model, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
        }

        /// <summary>
        /// Clears the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to clear validation errors for.
        /// If a <see langword="null"/> or empty name is used, all entity-level errors will be cleared.
        /// </param>
        protected void ClearErrors(string? propertyName = null)
        {
            // Clear entity-level errors when the target property is null or empty
            if (string.IsNullOrEmpty(propertyName))
            {
                ClearAllErrors();
            }
            else
            {
                ClearErrorsForProperty(propertyName!);
            }
        }

        /// <inheritdoc cref="INotifyDataErrorInfo.GetErrors(string)"/>
        [Pure]
        public IEnumerable<ValidationResult> GetErrors(string? propertyName = null)
        {
            // Get entity-level errors when the target property is null or empty
            if (string.IsNullOrEmpty(propertyName))
            {
                // Local function to gather all the entity-level errors
                [Pure]
                [MethodImpl(MethodImplOptions.NoInlining)]
                IEnumerable<ValidationResult> GetAllErrors()
                {
                    return this.errors.Values.SelectMany(static errors => errors);
                }

                return GetAllErrors();
            }

            // Property-level errors, if any
            if (this.errors.TryGetValue(propertyName!, out List<ValidationResult>? errors))
            {
                return errors;
            }

            // The INotifyDataErrorInfo.GetErrors method doesn't specify exactly what to
            // return when the input property name is invalid, but given that the return
            // type is marked as a non-nullable reference type, here we're returning an
            // empty array to respect the contract. This also matches the behavior of
            // this method whenever errors for a valid properties are retrieved.
            return Array.Empty<ValidationResult>();
        }

        /// <inheritdoc/>
        [Pure]
        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => GetErrors(propertyName);

        /// <summary>
        /// Validates all the properties in the current instance and updates all the tracked errors.
        /// If any changes are detected, the <see cref="ErrorsChanged"/> event will be raised.
        /// </summary>
        /// <remarks>
        /// Only public instance properties (excluding custom indexers) that have at least one
        /// <see cref="ValidationAttribute"/> applied to them will be validated. All other
        /// members in the current instance will be ignored. None of the processed properties
        /// will be modified - they will only be used to retrieve their values and validate them.
        /// </remarks>
        protected void ValidateAllProperties()
        {
            static Action<object> GetValidationAction(Type type)
            {
                // MyViewModel inst0 = (MyViewModel)arg0;
                ParameterExpression arg0 = Expression.Parameter(typeof(object));
                UnaryExpression inst0 = Expression.Convert(arg0, type);

                // Get a reference to ValidateProperty(object, string)
                MethodInfo validateMethod = typeof(ObservableValidator).GetMethod(nameof(ValidateProperty), BindingFlags.Instance | BindingFlags.NonPublic)!;

                // We want a single compiled LINQ expression that validates all properties in the
                // actual type of the executing viewmodel at once. We do this by creating a block
                // expression with the unrolled invocations of all properties to validate.
                // Essentially, the body will contain the following code:
                // ===============================================================================
                // {
                //     inst0.ValidateProperty(inst0.Property0, nameof(MyViewModel.Property0));
                //     inst0.ValidateProperty(inst0.Property1, nameof(MyViewModel.Property1));
                //     ...
                // }
                // ===============================================================================
                // We also add an explicit object conversion to represent boxing, if a given property
                // is a value type. It will just be a no-op if the value is a reference type already.
                // Note that this generated code is technically accessing a protected method from
                // ObservableValidator externally, but that is fine because IL doesn't really have
                // a concept of member visibility, that's purely a C# build-time feature.
                BlockExpression body = Expression.Block(
                    from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    where property.GetIndexParameters().Length == 0 &&
                          property.GetCustomAttributes<ValidationAttribute>(true).Any()
                    let getter = property.GetMethod
                    where getter is not null
                    select Expression.Call(inst0, validateMethod, new Expression[]
                    {
                        Expression.Convert(Expression.Call(inst0, getter), typeof(object)),
                        Expression.Constant(property.Name)
                    }));

                return Expression.Lambda<Action<object>>(body, arg0).Compile();
            }

            // Get or compute the cached list of properties to validate. Here we're using a static lambda to ensure the
            // delegate is cached by the C# compiler, see the related issue at https://github.com/dotnet/roslyn/issues/5835.
            EntityValidatorMap.GetValue(GetType(), static t => GetValidationAction(t))(this);
        }

        /// <summary>
        /// Validates a property with a specified name and a given input value.
        /// If any changes are detected, the <see cref="ErrorsChanged"/> event will be raised.
        /// </summary>
        /// <param name="value">The value to test for the specified property.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        protected void ValidateProperty(object? value, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
            {
                ThrowArgumentNullExceptionForNullPropertyName();
            }

            // Check if the property had already been previously validated, and if so retrieve
            // the reusable list of validation errors from the errors dictionary. This list is
            // used to add new validation errors below, if any are produced by the validator.
            // If the property isn't present in the dictionary, add it now to avoid allocations.
            if (!this.errors.TryGetValue(propertyName!, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new();

                this.errors.Add(propertyName!, propertyErrors);
            }

            bool errorsChanged = false;

            // Clear the errors for the specified property, if any
            if (propertyErrors.Count > 0)
            {
                propertyErrors.Clear();

                errorsChanged = true;
            }

            // Validate the property, by adding new errors to the existing list
            this.validationContext.MemberName = propertyName;
            this.validationContext.DisplayName = GetDisplayNameForProperty(propertyName!);

            bool isValid = Validator.TryValidateProperty(value, this.validationContext, propertyErrors);

            // Update the shared counter for the number of errors, and raise the
            // property changed event if necessary. We decrement the number of total
            // errors if the current property is valid but it wasn't so before this
            // validation, and we increment it if the validation failed after being
            // correct before. The property changed event is raised whenever the
            // number of total errors is either decremented to 0, or incremented to 1.
            if (isValid)
            {
                if (errorsChanged)
                {
                    this.totalErrors--;

                    if (this.totalErrors == 0)
                    {
                        OnPropertyChanged(HasErrorsChangedEventArgs);
                    }
                }
            }
            else if (!errorsChanged)
            {
                this.totalErrors++;

                if (this.totalErrors == 1)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }
            }

            // Only raise the event once if needed. This happens either when the target property
            // had existing errors and is now valid, or if the validation has failed and there are
            // new errors to broadcast, regardless of the previous validation state for the property.
            if (errorsChanged || !isValid)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Tries to validate a property with a specified name and a given input value, and returns
        /// the computed errors, if any. If the property is valid, it is assumed that its value is
        /// about to be set in the current object. Otherwise, no observable local state is modified.
        /// </summary>
        /// <param name="value">The value to test for the specified property.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        private bool TryValidateProperty(object? value, string? propertyName, out IReadOnlyCollection<ValidationResult> errors)
        {
            if (propertyName is null)
            {
                ThrowArgumentNullExceptionForNullPropertyName();
            }

            // Add the cached errors list for later use.
            if (!this.errors.TryGetValue(propertyName!, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new();

                this.errors.Add(propertyName!, propertyErrors);
            }

            bool hasErrors = propertyErrors.Count > 0;

            List<ValidationResult> localErrors = new();

            // Validate the property, by adding new errors to the local list
            this.validationContext.MemberName = propertyName;
            this.validationContext.DisplayName = GetDisplayNameForProperty(propertyName!);

            bool isValid = Validator.TryValidateProperty(value, this.validationContext, localErrors);

            // We only modify the state if the property is valid and it wasn't so before. In this case, we
            // clear the cached list of errors (which is visible to consumers) and raise the necessary events.
            if (isValid && hasErrors)
            {
                propertyErrors.Clear();

                this.totalErrors--;

                if (this.totalErrors == 0)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            errors = localErrors;

            return isValid;
        }

        /// <summary>
        /// Clears all the current errors for the entire entity.
        /// </summary>
        private void ClearAllErrors()
        {
            if (this.totalErrors == 0)
            {
                return;
            }

            // Clear the errors for all properties with at least one error, and raise the
            // ErrorsChanged event for those properties. Other properties will be ignored.
            foreach (var propertyInfo in this.errors)
            {
                bool hasErrors = propertyInfo.Value.Count > 0;

                propertyInfo.Value.Clear();

                if (hasErrors)
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyInfo.Key));
                }
            }

            this.totalErrors = 0;

            OnPropertyChanged(HasErrorsChangedEventArgs);
        }

        /// <summary>
        /// Clears all the current errors for a target property.
        /// </summary>
        /// <param name="propertyName">The name of the property to clear errors for.</param>
        private void ClearErrorsForProperty(string propertyName)
        {
            if (!this.errors.TryGetValue(propertyName!, out List<ValidationResult>? propertyErrors) ||
                propertyErrors.Count == 0)
            {
                return;
            }

            propertyErrors.Clear();

            this.totalErrors--;

            if (this.totalErrors == 0)
            {
                OnPropertyChanged(HasErrorsChangedEventArgs);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the display name for a given property. It could be a custom name or just the property name.
        /// </summary>
        /// <param name="propertyName">The target property name being validated.</param>
        /// <returns>The display name for the property.</returns>
        private string GetDisplayNameForProperty(string propertyName)
        {
            static Dictionary<string, string> GetDisplayNames(Type type)
            {
                Dictionary<string, string> displayNames = new();

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.GetCustomAttribute<DisplayAttribute>() is DisplayAttribute attribute &&
                        attribute.GetName() is string displayName)
                    {
                        displayNames.Add(property.Name, displayName);
                    }
                }

                return displayNames;
            }

            // This method replicates the logic of DisplayName and GetDisplayName from the
            // ValidationContext class. See the original source in the BCL for more details.
            DisplayNamesMap.GetValue(GetType(), static t => GetDisplayNames(t)).TryGetValue(propertyName, out string? displayName);

            return displayName ?? propertyName;
        }

#pragma warning disable SA1204
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> when a property name given as input is <see langword="null"/>.
        /// </summary>
        private static void ThrowArgumentNullExceptionForNullPropertyName()
        {
            throw new ArgumentNullException("propertyName", "The input property name cannot be null when validating a property");
        }
#pragma warning restore SA1204
    }
}