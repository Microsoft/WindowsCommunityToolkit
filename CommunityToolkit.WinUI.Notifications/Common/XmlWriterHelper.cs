// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommunityToolkit.WinUI.Notifications
{
    internal static class XmlWriterHelper
    {
        public static void Write(global::System.Xml.XmlWriter writer, object element)
        {
            NotificationXmlElementAttribute elAttr = GetElementAttribute(element.GetType());

            // If it isn't an element attribute, don't write anything
            if (elAttr == null)
            {
                return;
            }

            writer.WriteStartElement(elAttr.Name);

            IEnumerable<PropertyInfo> properties = GetProperties(element.GetType());

            List<object> elements = new List<object>();
            object content = null;

            // Write the attributes first
            foreach (PropertyInfo p in properties)
            {
                IEnumerable<Attribute> attributes = GetCustomAttributes(p);

                NotificationXmlAttributeAttribute attr = attributes.OfType<NotificationXmlAttributeAttribute>().FirstOrDefault();

                object propertyValue = GetPropertyValue(p, element);

                // If it's the additional properties item
                if (p.Name == nameof(IElement_AdditionalProperties.AdditionalProperties) && element is IElement_AdditionalProperties && p.PropertyType == typeof(IDictionary<string, string>))
                {
                    if (propertyValue != null)
                    {
                        foreach (var additionalProp in propertyValue as IDictionary<string, string>)
                        {
                            writer.WriteAttributeString(additionalProp.Key, additionalProp.Value);
                        }
                    }
                }

                // If it's an attribute
                else if (attr != null)
                {
                    object defaultValue = attr.DefaultValue;

                    // If the value is not the default value (and it's not null) we'll write it
                    if (!object.Equals(propertyValue, defaultValue) && propertyValue != null)
                    {
                        writer.WriteAttributeString(attr.Name, PropertyValueToString(propertyValue));
                    }
                }

                // If it's a content attribute
                else if (attributes.OfType<NotificationXmlContentAttribute>().Any())
                {
                    content = propertyValue;
                }

                // Otherwise it's an element or collection of elements
                else
                {
                    if (propertyValue != null)
                    {
                        elements.Add(propertyValue);
                    }
                }
            }

            // Then write children
            foreach (object el in elements)
            {
                // If it's a collection of children
                if (el is IEnumerable)
                {
                    foreach (object child in el as IEnumerable)
                    {
                        Write(writer, child);
                    }

                    continue;
                }

                // Otherwise just write the single element
                Write(writer, el);
            }

            // Then write any content if there is content
            if (content != null)
            {
                string contentString = content.ToString();
                if (!string.IsNullOrWhiteSpace(contentString))
                {
                    writer.WriteString(contentString);
                }
            }

            writer.WriteEndElement();
        }

        private static object GetPropertyValue(PropertyInfo propertyInfo, object obj)
        {
#if NETFX_CORE
            return propertyInfo.GetValue(obj);
#else
            return propertyInfo.GetValue(obj, null);
#endif
        }

        private static string PropertyValueToString(object propertyValue)
        {
            Type type = propertyValue.GetType();

            if (IsEnum(type))
            {
                EnumStringAttribute enumStringAttr = GetEnumStringAttribute(propertyValue as Enum);

                if (enumStringAttr != null)
                {
                    return enumStringAttr.String;
                }
            }
            else if (propertyValue is bool)
            {
                if ((bool)propertyValue)
                {
                    return "true";
                }

                return "false";
            }
            else if (propertyValue is DateTimeOffset?)
            {
                DateTimeOffset? dateTime = propertyValue as DateTimeOffset?;
                if (dateTime.HasValue)
                {
                    // ISO 8601 format
                    return global::System.Xml.XmlConvert.ToString(dateTime.Value);
                }
                else
                {
                    return null;
                }
            }

            return propertyValue.ToString();
        }

        private static EnumStringAttribute GetEnumStringAttribute(Enum enumValue)
        {
#if NETFX_CORE
            return enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<EnumStringAttribute>();
#else
            MemberInfo[] memberInfo = enumValue.GetType().GetMember(enumValue.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(EnumStringAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return attrs[0] as EnumStringAttribute;
            }

            return null;
#endif
        }

        private static bool IsEnum(Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredProperties;
#else
            return type.GetProperties();
#endif
        }

        private static NotificationXmlElementAttribute GetElementAttribute(Type type)
        {
            return GetCustomAttributes(type).OfType<NotificationXmlElementAttribute>().FirstOrDefault();
        }

        private static IEnumerable<Attribute> GetCustomAttributes(Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttributes();
#else
            return type.GetCustomAttributes(true).OfType<Attribute>();
#endif
        }

        private static IEnumerable<Attribute> GetCustomAttributes(PropertyInfo propertyInfo)
        {
#if NETFX_CORE
            return propertyInfo.GetCustomAttributes();
#else
            return propertyInfo.GetCustomAttributes(true).OfType<Attribute>();
#endif
        }

        /// <summary>
        /// Gets the provided binding value, if it exists. Otherwise, falls back to the absolute value.
        /// </summary>
        /// <typeparam name="T">The type of the enum of the class properties.</typeparam>
        /// <param name="bindings">The collection of data-bound values.</param>
        /// <param name="bindableProperty">The property to obtain.</param>
        /// <param name="absoluteValue">The absolute value, if any.</param>
        /// <returns>The provided binding value, if it exists. Otherwise, falls back to the absolute value.</returns>
        internal static string GetBindingOrAbsoluteXmlValue<T>(IDictionary<T, string> bindings, T bindableProperty, string absoluteValue)
        {
            // If a binding is provided, use the binding value
            string bindingValue;
            if (bindings.TryGetValue(bindableProperty, out bindingValue))
            {
                return "{" + bindingValue + "}";
            }

            // Otherwise fallback to the absolute value
            return absoluteValue;
        }
    }
}