// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Reflection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_INotifyPropertyChangedAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_INotifyPropertyChanged_Events()
        {
            var model = new SampleModel();

            (PropertyChangedEventArgs, int) changed = default;

            model.PropertyChanged += (s, e) =>
            {
                Assert.IsNull(changed.Item1);
                Assert.AreSame(model, s);
                Assert.IsNotNull(s);
                Assert.IsNotNull(e);

                changed = (e, model.Data);
            };

            model.Data = 42;

            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
            Assert.AreEqual(changed.Item2, 42);
        }

        [INotifyPropertyChanged]
        public partial class SampleModel
        {
            private int data;

            public int Data
            {
                get => data;
                set => SetProperty(ref data, value);
            }
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_INotifyPropertyChanged_WithoutHelpers()
        {
            Assert.IsTrue(typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(SampleModelWithoutHelpers)));
            Assert.IsFalse(typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(SampleModelWithoutHelpers)));

            // This just needs to check that it compiles
            _ = nameof(SampleModelWithoutHelpers.PropertyChanged);

            var methods = typeof(SampleModelWithoutHelpers).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Assert.AreEqual(methods.Length, 2);
            Assert.AreEqual(methods[0].Name, "OnPropertyChanged");
            Assert.AreEqual(methods[1].Name, "OnPropertyChanged");

            var types = typeof(SampleModelWithoutHelpers).GetNestedTypes(BindingFlags.NonPublic);

            Assert.AreEqual(types.Length, 0);
        }

        [INotifyPropertyChanged(IncludeAdditionalHelperMethods = false)]
        public partial class SampleModelWithoutHelpers
        {
        }
    }
}
