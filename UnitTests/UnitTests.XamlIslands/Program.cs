// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Forms.UI.XamlHost;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using UnitTests.XamlIslands.UWPApp;

namespace UnitTests.XamlIslands
{
    class Program
    {
        internal static App AppInstance;

        [STAThread]
        public static void Main()
        {
            using (AppInstance = new App())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var mainFormInstance = new MainForm();
                Application.Run(mainFormInstance);
            }
        }

        public class MainForm : Form
        {
            public WindowsXamlHost xamlHost = new WindowsXamlHost();

            public MainForm()
            {
                SuspendLayout();
                xamlHost.AutoSizeMode = AutoSizeMode.GrowOnly;
                xamlHost.Location = new System.Drawing.Point(0, 0);
                xamlHost.Name = "xamlHost";
                xamlHost.Size = new System.Drawing.Size(800, 800);
                xamlHost.TabIndex = 0;
                xamlHost.Text = "xamlHost";
                xamlHost.Dock = DockStyle.Fill;
                xamlHost.ChildChanged += XamlHost_ChildChanged;
                xamlHost.Child = new Frame();

                AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                WindowState = FormWindowState.Maximized;
                ClientSize = new System.Drawing.Size(800, 600);
                Controls.Add(xamlHost);
                Name = "MainForm";
                Text = "Xaml Islands";
                ResumeLayout(false);
            }

            private void XamlHost_ChildChanged(object sender, EventArgs e)
            {
                if (xamlHost.Child is Frame frame)
                {
                    frame.Navigate(typeof(TestsPage));
                }
            }
        }
    }
}