// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public class Test_ObservableValidator
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_HasErrors()
        {
            var model = new Person();
            var args = new List<PropertyChangedEventArgs>();

            model.PropertyChanged += (s, e) => args.Add(e);

            Assert.IsFalse(model.HasErrors);

            model.Name = "No";

            // Verify that errors were correctly reported as changed, and that all the relevant
            // properties were broadcast as well (both the changed property and HasErrors). We need
            // this last one to raise notifications too so that users can bind to that in the UI.
            Assert.IsTrue(model.HasErrors);
            Assert.AreEqual(args.Count, 2);
            Assert.AreEqual(args[0].PropertyName, nameof(Person.Name));
            Assert.AreEqual(args[1].PropertyName, nameof(INotifyDataErrorInfo.HasErrors));

            model.Name = "Valid";

            Assert.IsFalse(model.HasErrors);
            Assert.AreEqual(args.Count, 4);
            Assert.AreEqual(args[2].PropertyName, nameof(Person.Name));
            Assert.AreEqual(args[3].PropertyName, nameof(INotifyDataErrorInfo.HasErrors));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_ErrorsChanged()
        {
            var model = new Person();

            List<(object Sender, DataErrorsChangedEventArgs Args)> errors = new List<(object, DataErrorsChangedEventArgs)>();

            model.ErrorsChanged += (s, e) => errors.Add((s, e));

            model.Name = "Foo";

            Assert.AreEqual(errors.Count, 1);
            Assert.AreSame(errors[0].Sender, model);
            Assert.AreEqual(errors[0].Args.PropertyName, nameof(Person.Name));

            errors.Clear();

            model.Name = "Bar";

            Assert.AreEqual(errors.Count, 1);
            Assert.AreSame(errors[0].Sender, model);
            Assert.AreEqual(errors[0].Args.PropertyName, nameof(Person.Name));

            errors.Clear();

            model.Name = "Valid";

            Assert.AreEqual(errors.Count, 1);
            Assert.AreSame(errors[0].Sender, model);
            Assert.AreEqual(errors[0].Args.PropertyName, nameof(Person.Name));

            errors.Clear();

            model.Name = "This is fine";

            Assert.AreEqual(errors.Count, 0);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_GetErrors()
        {
            var model = new Person();

            Assert.AreEqual(model.GetErrors(null).Count(), 0);
            Assert.AreEqual(model.GetErrors(string.Empty).Count(), 0);
            Assert.AreEqual(model.GetErrors("ThereIsntAPropertyWithThisName").Count(), 0);
            Assert.AreEqual(model.GetErrors(nameof(Person.Name)).Count(), 0);

            model.Name = "Foo";

            var errors = model.GetErrors(nameof(Person.Name)).ToArray();

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].MemberNames.First(), nameof(Person.Name));

            Assert.AreEqual(model.GetErrors("ThereIsntAPropertyWithThisName").Count(), 0);

            errors = model.GetErrors(null).ToArray();

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].MemberNames.First(), nameof(Person.Name));

            errors = model.GetErrors(string.Empty).ToArray();

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].MemberNames.First(), nameof(Person.Name));

            model.Age = -1;

            errors = model.GetErrors(null).ToArray();

            Assert.AreEqual(errors.Length, 2);
            Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Name))));
            Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Age))));

            model.Age = 26;

            errors = model.GetErrors(null).ToArray();

            Assert.AreEqual(errors.Length, 1);
            Assert.IsTrue(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Name))));
            Assert.IsFalse(errors.Any(e => e.MemberNames.First().Equals(nameof(Person.Age))));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow("", false)]
        [DataRow("No", false)]
        [DataRow("This text is really, really too long for the target property", false)]
        [DataRow("1234", true)]
        [DataRow("01234567890123456789", true)]
        [DataRow("Hello world", true)]
        public void Test_ObservableValidator_ValidateReturn(string value, bool isValid)
        {
            var model = new Person { Name = value };

            Assert.AreEqual(model.HasErrors, !isValid);

            if (isValid)
            {
                Assert.IsTrue(!model.GetErrors(nameof(Person.Name)).Any());
            }
            else
            {
                Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());
            }
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_TrySetProperty()
        {
            var model = new Person();
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            // Set a correct value, this should update the property
            Assert.IsTrue(model.TrySetName("Hello", out var errors));
            Assert.IsTrue(errors.Count == 0);
            Assert.IsTrue(events.Count == 0);
            Assert.AreEqual(model.Name, "Hello");
            Assert.IsFalse(model.HasErrors);

            // Invalid value #1, this should be ignored
            Assert.IsFalse(model.TrySetName(null, out errors));
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(events.Count == 0);
            Assert.AreEqual(model.Name, "Hello");
            Assert.IsFalse(model.HasErrors);

            // Invalid value #2, same as above
            Assert.IsFalse(model.TrySetName("This string is too long for the target property in this model and should fail", out errors));
            Assert.IsTrue(errors.Count > 0);
            Assert.IsTrue(events.Count == 0);
            Assert.AreEqual(model.Name, "Hello");
            Assert.IsFalse(model.HasErrors);

            // Correct value, this should update the property
            Assert.IsTrue(model.TrySetName("Hello world", out errors));
            Assert.IsTrue(errors.Count == 0);
            Assert.IsTrue(events.Count == 0);
            Assert.AreEqual(model.Name, "Hello world");
            Assert.IsFalse(model.HasErrors);

            // Actually set an invalid value to show some errors
            model.Name = "No";

            // Errors should now be present
            Assert.IsTrue(model.HasErrors);
            Assert.IsTrue(events.Count == 1);
            Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());
            Assert.IsTrue(model.HasErrors);

            // Trying to set a correct property should clear the errors
            Assert.IsTrue(model.TrySetName("This is fine", out errors));
            Assert.IsTrue(errors.Count == 0);
            Assert.IsTrue(events.Count == 2);
            Assert.IsFalse(model.HasErrors);
            Assert.AreEqual(model.Name, "This is fine");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_ValidateProperty()
        {
            var model = new ComparableModel();
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            // Set a correct value for both properties, first A then B
            model.A = 42;
            model.B = 30;

            Assert.AreEqual(events.Count, 0);
            Assert.IsFalse(model.HasErrors);

            // Make B greater than A, hence invalidating A
            model.B = 50;

            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events.Last().PropertyName, nameof(ComparableModel.A));
            Assert.IsTrue(model.HasErrors);

            events.Clear();

            // Make A greater than B, hence making it valid again
            model.A = 51;

            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events.Last().PropertyName, nameof(ComparableModel.A));
            Assert.AreEqual(model.GetErrors(nameof(ComparableModel.A)).Count(), 0);
            Assert.IsFalse(model.HasErrors);

            events.Clear();

            // Make A smaller than B, hence invalidating it
            model.A = 49;

            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events.Last().PropertyName, nameof(ComparableModel.A));
            Assert.AreEqual(model.GetErrors(nameof(ComparableModel.A)).Count(), 1);
            Assert.IsTrue(model.HasErrors);

            events.Clear();

            // Lower B, hence making A valid again
            model.B = 20;

            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events.Last().PropertyName, nameof(ComparableModel.A));
            Assert.AreEqual(model.GetErrors(nameof(ComparableModel.A)).Count(), 0);
            Assert.IsFalse(model.HasErrors);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_ClearErrors()
        {
            var model = new Person();
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            model.Age = -2;

            Assert.IsTrue(model.HasErrors);
            Assert.IsTrue(model.GetErrors(nameof(Person.Age)).Any());

            model.ClearErrors(nameof(Person.Age));

            Assert.IsFalse(model.HasErrors);
            Assert.IsTrue(events.Count == 2);

            model.Age = 200;
            model.Name = "Bo";
            events.Clear();

            Assert.IsTrue(model.HasErrors);
            Assert.IsTrue(model.GetErrors(nameof(Person.Age)).Any());
            Assert.IsTrue(model.GetErrors(nameof(Person.Name)).Any());

            model.ClearErrors(null);
            Assert.IsFalse(model.HasErrors);
            Assert.IsFalse(model.GetErrors(nameof(Person.Age)).Any());
            Assert.IsFalse(model.GetErrors(nameof(Person.Name)).Any());
            Assert.IsTrue(events.Count == 2);
            Assert.IsTrue(events[0].PropertyName == nameof(Person.Age));
            Assert.IsTrue(events[1].PropertyName == nameof(Person.Name));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_ValidateAllProperties()
        {
            var model = new PersonWithDeferredValidation();
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            model.ValidateAllProperties();

            Assert.IsTrue(model.HasErrors);
            Assert.IsTrue(events.Count == 2);

            // Note: we can't use an index here because the order used to return properties
            // from reflection APIs is an implementation detail and might change at any time.
            Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
            Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

            events.Clear();

            model.Name = "James";
            model.Age = 42;

            model.ValidateAllProperties();

            Assert.IsFalse(model.HasErrors);
            Assert.IsTrue(events.Count == 2);
            Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Name)));
            Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));

            events.Clear();

            model.Age = -10;

            model.ValidateAllProperties();
            Assert.IsTrue(model.HasErrors);
            Assert.IsTrue(events.Count == 1);
            Assert.IsTrue(events.Any(e => e.PropertyName == nameof(Person.Age)));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_CustomValidation()
        {
            var items = new Dictionary<object, object> { [nameof(CustomValidationModel.A)] = 42 };
            var model = new CustomValidationModel(items);
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            model.A = 10;

            Assert.IsFalse(model.HasErrors);
            Assert.AreEqual(events.Count, 0);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_CustomValidationWithInjectedService()
        {
            var model = new ValidationWithServiceModel(new FancyService());
            var events = new List<DataErrorsChangedEventArgs>();

            model.ErrorsChanged += (s, e) => events.Add(e);

            model.Name = "This is a valid name";

            Assert.IsFalse(model.HasErrors);
            Assert.AreEqual(events.Count, 0);

            model.Name = "This is invalid238!!";

            Assert.IsTrue(model.HasErrors);
            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events[0].PropertyName, nameof(ValidationWithServiceModel.Name));
            Assert.AreEqual(model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray().Length, 1);

            model.Name = "This is valid but it is too long so the validation will fail anyway";

            Assert.IsTrue(model.HasErrors);
            Assert.AreEqual(events.Count, 2);
            Assert.AreEqual(events[1].PropertyName, nameof(ValidationWithServiceModel.Name));
            Assert.AreEqual(model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray().Length, 1);

            model.Name = "This is both too long and it also contains invalid characters, a real disaster!";

            Assert.IsTrue(model.HasErrors);
            Assert.AreEqual(events.Count, 3);
            Assert.AreEqual(events[2].PropertyName, nameof(ValidationWithServiceModel.Name));
            Assert.AreEqual(model.GetErrors(nameof(ValidationWithServiceModel.Name)).ToArray().Length, 2);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableValidator_ValidationWithFormattedDisplayName()
        {
            var model = new ValidationWithDisplayName();

            Assert.IsTrue(model.HasErrors);

            // We need to order because there is no guaranteed order on the members of a type
            ValidationResult[] allErrors = model.GetErrors().OrderBy(error => error.ErrorMessage).ToArray();

            Assert.AreEqual(allErrors.Length, 2);

            Assert.AreEqual(allErrors[0].MemberNames.Count(), 1);
            Assert.AreEqual(allErrors[0].MemberNames.Single(), nameof(ValidationWithDisplayName.StringMayNotBeEmpty));
            Assert.AreEqual(allErrors[0].ErrorMessage, $"FIRST: {nameof(ValidationWithDisplayName.StringMayNotBeEmpty)}.");

            Assert.AreEqual(allErrors[1].MemberNames.Count(), 1);
            Assert.AreEqual(allErrors[1].MemberNames.Single(), nameof(ValidationWithDisplayName.AnotherRequiredField));
            Assert.AreEqual(allErrors[1].ErrorMessage, $"SECOND: {nameof(ValidationWithDisplayName.AnotherRequiredField)}.");
        }

        public class Person : ObservableValidator
        {
            private string name;

            [MinLength(4)]
            [MaxLength(20)]
            [Required]
            public string Name
            {
                get => this.name;
                set => SetProperty(ref this.name, value, true);
            }

            public bool TrySetName(string value, out IReadOnlyCollection<ValidationResult> errors)
            {
                return TrySetProperty(ref name, value, out errors, nameof(Name));
            }

            private int age;

            [Range(0, 100)]
            public int Age
            {
                get => this.age;
                set => SetProperty(ref this.age, value, true);
            }

            public new void ClearErrors(string propertyName)
            {
                base.ClearErrors(propertyName);
            }
        }

        public class PersonWithDeferredValidation : ObservableValidator
        {
            [MinLength(4)]
            [MaxLength(20)]
            [Required]
            public string Name { get; set; }

            [Range(18, 100)]
            public int Age { get; set; }

            // Extra property with no validation
            public float Foo { get; set; } = float.NaN;

            public new void ValidateAllProperties()
            {
                base.ValidateAllProperties();
            }
        }

        /// <summary>
        /// Test model for linked properties, to test <see cref="ObservableValidator.ValidateProperty(object?, string?)"/> instance.
        /// See https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3665 for the original request for this feature.
        /// </summary>
        public class ComparableModel : ObservableValidator
        {
            private int a;

            [Range(10, 100)]
            [GreaterThan(nameof(B))]
            public int A
            {
                get => this.a;
                set => SetProperty(ref this.a, value, true);
            }

            private int b;

            [Range(20, 80)]
            public int B
            {
                get => this.b;
                set
                {
                    SetProperty(ref this.b, value, true);
                    ValidateProperty(A, nameof(A));
                }
            }
        }

        public sealed class GreaterThanAttribute : ValidationAttribute
        {
            public GreaterThanAttribute(string propertyName)
            {
                PropertyName = propertyName;
            }

            public string PropertyName { get; }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                object
                    instance = validationContext.ObjectInstance,
                    otherValue = instance.GetType().GetProperty(PropertyName).GetValue(instance);

                if (((IComparable)value).CompareTo(otherValue) > 0)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("The current value is smaller than the other one");
            }
        }

        /// <summary>
        /// Test model for custom validation properties.
        /// See https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3729 for the original request for this feature.
        /// </summary>
        public class CustomValidationModel : ObservableValidator
        {
            public CustomValidationModel(IDictionary<object, object> items)
                : base(items)
            {
            }

            private int a;

            [CustomValidation(typeof(CustomValidationModel), nameof(ValidateA))]
            public int A
            {
                get => this.a;
                set => SetProperty(ref this.a, value, true);
            }

            public static ValidationResult ValidateA(int x, ValidationContext context)
            {
                Assert.AreEqual(context.MemberName, nameof(A));

                if ((int)context.Items[nameof(A)] == 42)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Missing the magic number");
            }
        }

        public interface IFancyService
        {
            bool Validate(string name);
        }

        public class FancyService : IFancyService
        {
            public bool Validate(string name)
            {
                return Regex.IsMatch(name, @"^[A-Za-z ]+$");
            }
        }

        /// <summary>
        /// Test model for custom validation with an injected service.
        /// See https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3750 for the original request for this feature.
        /// </summary>
        public class ValidationWithServiceModel : ObservableValidator
        {
            private readonly IFancyService service;

            public ValidationWithServiceModel(IFancyService service)
            {
                this.service = service;
            }

            private string name;

            [MaxLength(25, ErrorMessage = "The name is too long")]
            [CustomValidation(typeof(ValidationWithServiceModel), nameof(ValidateName))]
            public string Name
            {
                get => this.name;
                set => SetProperty(ref this.name, value, true);
            }

            public static ValidationResult ValidateName(string name, ValidationContext context)
            {
                bool isValid = ((ValidationWithServiceModel)context.ObjectInstance).service.Validate(name);

                if (isValid)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("The name contains invalid characters");
            }
        }

        /// <summary>
        /// Test model for validation with a formatted display name string on each property.
        /// This is issue #1 from https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3763.
        /// </summary>
        public class ValidationWithDisplayName : ObservableValidator
        {
            public ValidationWithDisplayName()
            {
                ValidateAllProperties();
            }

            private string stringMayNotBeEmpty;

            [Required(AllowEmptyStrings = false, ErrorMessage = "FIRST: {0}.")]
            public string StringMayNotBeEmpty
            {
                get => this.stringMayNotBeEmpty;
                set => SetProperty(ref this.stringMayNotBeEmpty, value, true);
            }

            private string anotherRequiredField;

            [Required(AllowEmptyStrings = false, ErrorMessage = "SECOND: {0}.")]
            public string AnotherRequiredField
            {
                get => this.anotherRequiredField;
                set => SetProperty(ref this.anotherRequiredField, value, true);
            }
        }
    }
}
