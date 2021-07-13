// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#nullable enable

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_ObservablePropertyAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservablePropertyAttribute_Events()
        {
            var model = new SampleModel();

            (PropertyChangingEventArgs, int) changing = default;
            (PropertyChangedEventArgs, int) changed = default;

            model.PropertyChanging += (s, e) =>
            {
                Assert.IsNull(changing.Item1);
                Assert.IsNull(changed.Item1);
                Assert.AreSame(model, s);
                Assert.IsNotNull(s);
                Assert.IsNotNull(e);

                changing = (e, model.Data);
            };

            model.PropertyChanged += (s, e) =>
            {
                Assert.IsNotNull(changing.Item1);
                Assert.IsNull(changed.Item1);
                Assert.AreSame(model, s);
                Assert.IsNotNull(s);
                Assert.IsNotNull(e);

                changed = (e, model.Data);
            };

            model.Data = 42;

            Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Data));
            Assert.AreEqual(changing.Item2, 0);
            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
            Assert.AreEqual(changed.Item2, 42);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_AlsoNotifyChangeForAttribute_Events()
        {
            var model = new DependentPropertyModel();

            List<string?> propertyNames = new();

            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

            model.Name = "Bob";
            model.Surname = "Ross";

            CollectionAssert.AreEqual(new[] { nameof(model.Name), nameof(model.FullName), nameof(model.Surname), nameof(model.FullName) }, propertyNames);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ValidationAttributes()
        {
            var nameProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Name))!;

            Assert.IsNotNull(nameProperty.GetCustomAttribute<RequiredAttribute>());
            Assert.IsNotNull(nameProperty.GetCustomAttribute<MinLengthAttribute>());
            Assert.AreEqual(nameProperty.GetCustomAttribute<MinLengthAttribute>()!.Length, 1);
            Assert.IsNotNull(nameProperty.GetCustomAttribute<MaxLengthAttribute>());
            Assert.AreEqual(nameProperty.GetCustomAttribute<MaxLengthAttribute>()!.Length, 100);

            var ageProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Age))!;

            Assert.IsNotNull(ageProperty.GetCustomAttribute<RangeAttribute>());
            Assert.AreEqual(ageProperty.GetCustomAttribute<RangeAttribute>()!.Minimum, 0);
            Assert.AreEqual(ageProperty.GetCustomAttribute<RangeAttribute>()!.Maximum, 120);

            var emailProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.Email))!;

            Assert.IsNotNull(emailProperty.GetCustomAttribute<EmailAddressAttribute>());

            var comboProperty = typeof(MyFormViewModel).GetProperty(nameof(MyFormViewModel.IfThisWorksThenThatsGreat))!;

            TestValidationAttribute testAttribute = comboProperty.GetCustomAttribute<TestValidationAttribute>()!;

            Assert.IsNotNull(testAttribute);
            Assert.IsNull(testAttribute.O);
            Assert.AreEqual(testAttribute.T, typeof(SampleModel));
            Assert.AreEqual(testAttribute.Flag, true);
            Assert.AreEqual(testAttribute.D, 6.28);
            CollectionAssert.AreEqual(testAttribute.Names, new[] { "Bob", "Ross" });

            object[] nestedArray = (object[])testAttribute.NestedArray;

            Assert.AreEqual(nestedArray.Length, 3);
            Assert.AreEqual(nestedArray[0], 1);
            Assert.AreEqual(nestedArray[1], "Hello");
            Assert.IsTrue(nestedArray[2] is int[]);
            CollectionAssert.AreEqual((int[])nestedArray[2], new[] { 2, 3, 4 });

            Assert.AreEqual(testAttribute.Animal, Animal.Llama);
        }

        public partial class SampleModel : ObservableObject
        {
            /// <summary>
            /// This is a sample data field within <see cref="SampleModel"/> of type <see cref="int"/>.
            /// </summary>
            [ObservableProperty]
            private int data;
        }

        [INotifyPropertyChanged]
        public sealed partial class DependentPropertyModel
        {
            [ObservableProperty]
            [AlsoNotifyChangeFor(nameof(FullName))]
            private string? name;

            [ObservableProperty]
            [AlsoNotifyChangeFor(nameof(FullName))]
            private string? surname;

            public string FullName => $"{Name} {Surname}";
        }

        public partial class MyFormViewModel : ObservableValidator
        {
            [ObservableProperty]
            [Required]
            [MinLength(1)]
            [MaxLength(100)]
            private string? name;

            [ObservableProperty]
            [Range(0, 120)]
            private int age;

            [ObservableProperty]
            [EmailAddress]
            private string? email;

            [ObservableProperty]
            [TestValidation(null, typeof(SampleModel), true, 6.28, new[] { "Bob", "Ross" }, NestedArray = new object[] { 1, "Hello", new int[] { 2, 3, 4 } }, Animal = Animal.Llama)]
            private int ifThisWorksThenThatsGreat;
        }

        private sealed class TestValidationAttribute : ValidationAttribute
        {
            public TestValidationAttribute(object? o, Type t, bool flag, double d, string[] names)
            {
                O = o;
                T = t;
                Flag = flag;
                D = d;
                Names = names;
            }

            public object? O { get; }

            public Type T { get; }

            public bool Flag { get; }

            public double D { get; }

            public string[] Names { get; }

            public object NestedArray { get; set; }

            public Animal Animal { get; set; }
        }

        public enum Animal
        {
            Cat,
            Dog,
            Llama
        }
    }
}
