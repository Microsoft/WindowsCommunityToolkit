// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using CommunityToolkit.WinUI.Design.Types;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace CommunityToolkit.WinUI.Design.Common
{
    public abstract class MetadataRegistrationBase : IProvideAttributeTable
    {
        private AttributeTable mainMetadataTable;

        internal MetadataRegistrationBase() { }

        /// <summary>
        /// Build design time metadata attribute table.
        /// </summary>
        /// <returns>Custom attribute table.</returns>
        protected virtual AttributeTable BuildAttributeTable()
        {
            var builder = new AttributeTableBuilder();

            AddDescriptions(builder);
            AddAttributes(builder);
            AddTables(builder, this);

            mainMetadataTable = builder.CreateTable();
            return mainMetadataTable;
        }

        #region IProvideAttributeTable Members

        /// <summary>
        /// Gets the AttributeTable for design time metadata.
        /// </summary>
        public AttributeTable AttributeTable => BuildAttributeTable();

        #endregion

        /// <summary>
        /// Find all AttributeTableBuilder subclasses in the assembly
        /// and add their attributes to the assembly attribute table.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design time dll should not fail!")]
        private void AddTables(AttributeTableBuilder builder, object parent)
        {
            Debug.Assert(builder != null, "AddTables is called with null parameter!");

            Assembly asm = parent.GetType().Assembly;
            foreach (Type t in asm.GetTypes())
            {
                if (t.IsSubclassOf(typeof(AttributeTableBuilder)))
                {
                    try
                    {
                        var atb = (AttributeTableBuilder)Activator.CreateInstance(t);
                        builder.AddTable(atb.CreateTable());
                    }
                    catch (Exception)
                    {
                        //error loading design assembly
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the case sensitive resource name of the embedded XML file.
        /// </summary>
        protected string XmlResourceName { get; set; }

        /// <summary>
        /// Gets or sets the FullName of the corresponding run time assembly.
        /// </summary>
        protected string AssemblyFullName { get; set; }

        /// <summary>
        /// Create description attribute from run time assembly XML file.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design time dll should not fail.")]
        private void AddDescriptions(AttributeTableBuilder builder)
        {
            Debug.Assert(builder != null, "AddDescriptions is called with null parameter!");

            if (string.IsNullOrEmpty(XmlResourceName) || string.IsNullOrEmpty(AssemblyFullName)) return;

            XDocument xDoc;
            try
            {
                xDoc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(XmlResourceName));
            }
            catch
            {
                return;
            }

            if (xDoc == null) return;

            foreach (XElement member in xDoc.Descendants("member"))
            {
                try
                {
                    string name = (string)member.Attribute("name");

                    if (name == null) continue;

                    bool isType = name.StartsWith("T:", StringComparison.OrdinalIgnoreCase);
                    bool isProperty = name.StartsWith("P:", StringComparison.OrdinalIgnoreCase);

                    if (isType || isProperty)
                    {
                        int lastDot = name.Length;
                        string typeName;

                        if (isType)
                        {
                            typeName = name.Substring(2); // skip leading "T:"
                        }
                        else
                        {
                            lastDot = name.LastIndexOf('.');
                            typeName = name.Substring(2, lastDot - 2);
                        }

                        var type = Type.GetType(typeName + ", " + AssemblyFullName);

                        if (type != null && type.IsPublic && type.IsClass && type.IsSubclassOf(PlatformTypes.DependencyObject))
                        {
                            string desc = ParseDescription(member);

                            if (desc == null) continue;

                            desc = string.Join(" ", desc.Trim().Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));

                            if (isType)
                            {
                                if (IsBrowsable(type))
                                {
                                    builder.AddCustomAttributes(typeName, new DescriptionAttribute(desc));
                                }
                                else //Hide from intellisense
                                {
                                    builder.AddCustomAttributes(typeName,
                                        new BrowsableAttribute(false),
                                        new ToolboxBrowsableAttribute(false),
                                        new ToolboxItemAttribute(false));
                                }
                            }
                            else
                            {
                                var propertyName = name.Substring(lastDot + 1);
                                PropertyInfo pi = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                                if (pi != null)
                                {
                                    if (IsBrowsable(type))
                                    {
                                        builder.AddCustomAttributes(typeName, propertyName, new DescriptionAttribute(desc));
                                    }
                                    else //Hide from intellisense
                                    {
                                        builder.AddCustomAttributes(typeName, new BrowsableAttribute(false));
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private static bool IsBrowsable(MemberInfo typeOrMember)
        {
            EditorBrowsableAttribute attribute;
            try
            {
                attribute = typeOrMember.GetCustomAttribute<EditorBrowsableAttribute>(false);
            }
            catch
            {
                return true; // If there is no [EditorBrowsable] attribute present, we'll show it by default.
            }
            return attribute.State != EditorBrowsableState.Never;
        }

        /// <summary>
        /// Create description string from XML doc summary tag.
        /// </summary>
        /// <param name="member">A single node of the XML doc.</param>
        /// <returns>Description string.</returns>
        private static string ParseDescription(XElement member)
        {
            string desc = null;
            XElement memberDesc = member.Descendants("summary").FirstOrDefault();

            if (memberDesc != null)
            {
                IEnumerable<XNode> nodes = memberDesc.DescendantNodes();

                if (nodes != null)
                {
                    foreach (XNode node in nodes)
                    {
                        if (node.NodeType == System.Xml.XmlNodeType.Text)
                        {
                            desc += node.ToString();
                        }
                        else
                        {
                            string s = node.ToString();
                            int i = s.LastIndexOf('.');
                            int j = s.LastIndexOf('"');

                            if ((i != -1 || j != -1) && j - i - 1 > 0)
                            {
                                try
                                {
                                    desc += s.Substring(i + 1, j - i - 1);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            return desc;
        }

        /// <summary>
        /// Provide a place to add custom attributes without creating a AttributeTableBuilder subclass.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        protected virtual void AddAttributes(AttributeTableBuilder builder)
        {
        }
    }
}