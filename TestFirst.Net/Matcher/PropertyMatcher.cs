using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net.Matcher
{
    /// <summary>
    /// A matcher which will look up properties to match dynamically. Matchers are registered dynamically so there is no need to
    /// create a bunch of private fields and matchers to match against.Example:
    /// <para>
    /// AFoo:PropertyMatcher&lt;Foo&gt;
    /// {
    ///     // to enable refactor safe property names
    ///     private static readonly Foo PropertyNames = null;
    /// </para>
    /// <para>
    ///     public static AFoo With()
    ///     {
    ///         return new MyFooMatcher();
    ///     }
    /// </para>
    /// <para>
    ///     public AFoo MyBar(string val)
    ///     {
    ///         MyBar(AString.EqualTo(val));
    ///         return this;
    ///     }
    /// </para>
    /// <para>
    ///     public AFoo MyBar(IMatcher&lt;string&gt; matcher)
    ///     {
    ///         WithProperty(() => PropertyNames.MyBar, matcher);
    ///         return this;
    ///     }
    /// </para>
    /// <para>
    ///     public AFoo SomeCustomMethod(IMatcher&lt;string&gt; matcher)
    ///     {
    ///         WithMatcher("GetIt()", (poco)=> poco.GetIt(), matcher);
    ///         return this;
    ///     } 
    /// }
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of object to match</typeparam>
    public class PropertyMatcher<T> : ObjectMatcher<T>
    {
        // Matchers for a particular property. Run these the order they were added as callers may want to make certain assertions before others
        // to ease diagnostics
        private readonly IList<TypeSafePropertyMatcher> m_propertyMatchers = new List<TypeSafePropertyMatcher>();

        public override bool Matches(T actual, IMatchDiagnostics diagnostics)
        {
            if (!base.Matches(actual, diagnostics))
            {
                return false;
            }
            foreach (var matcher in m_propertyMatchers)
            {
                PropertyInfo property = GetPropertyForTypeNamed(actual.GetType(), matcher.PropertyName);
                if (property == null)
                {
                    diagnostics.MisMatched(
                        Description.With()
                            .Value("propertyExists", false));
                    return false;
                }
                var propertyValue = property.GetValue(actual, null);
                if (!diagnostics.TryMatch(propertyValue, matcher))
                {
                    return false;
                }
            }
            return true;
        }

        public override void DescribeTo(IDescription desc)
        {
            base.DescribeTo(desc);
            desc.Children(m_propertyMatchers);
        }

        /// <summary>
        /// Add a property to match against using an expression to extract the property name. This should be used in preference
        /// to passing in a property name as a string, as this will work across property renames (and find usages etc works too). 
        /// <example>
        /// <para>
        /// The caller will likely need to do something along these lines:
        /// <code>
        /// // this makes the compiler think you have an instance, but you don't actually need one
        /// private static readonly MyDto PropertyNames = null;
        /// ...
        /// WithProperty(() => PropertyNames.MyPropertyName, matcher)
        /// </code>
        /// </para>
        /// <para>
        /// The expression 
        /// <code>'() => PropertyNames.MyPropertyName'</code>
        /// returns a function which returns an expression to obtain
        /// the property node. This is then used to extract the property name
        /// </para>
        /// </example>
        /// <para>
        /// This 'With' call will also be included in searches to find all usages of the given property. If the property is renamed
        /// then this call will be updated too. Unlike magic strings which require the user refactoring the code to remember to search
        /// through all the code base for the given string and rename (if they remember)
        /// </para>
        /// <para>
        /// Possibly slightly unusual and surprising but after a while your code becomes obvious and refactor friendly once
        /// you understand the pattern
        /// </para>
        /// </summary>
        /// <typeparam name="TPropertyType">The type of property to match</typeparam>
        /// <param name="expression">The expression which names the property to match</param>
        /// <param name="propertyMatcher">The matcher to use</param>
        /// <returns>A property matcher</returns>
        protected internal PropertyMatcher<T> WithProperty<TPropertyType>(Expression<Func<TPropertyType>> expression, IMatcher<TPropertyType> propertyMatcher)
        {
            string propName = InternalExtractPropertyNameFrom(expression);
            WithProperty(propName, propertyMatcher);
            return this;
        }

        /// <summary>
        /// Add a property to match against
        /// </summary>
        /// <typeparam name="TPropertyType">the type of the property</typeparam>
        /// <param name="propertyName">name of the property to match</param>
        /// <param name="fieldMatcher">the matcher to use for the given property</param>
        /// <returns>A property matcher</returns>
        protected internal PropertyMatcher<T> WithProperty<TPropertyType>(string propertyName, IMatcher<TPropertyType> fieldMatcher)
        {
            if (PropertyExists(propertyName))
            {
                m_propertyMatchers.Add(TypeSafePropertyMatcher.NewWithNameAndMatcher(typeof(T), propertyName, fieldMatcher));
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                    "Property with name '{0}' does not exist on type {1}, expect one of [{2}]", 
                        propertyName,
                        typeof(T).FullName,
                        string.Join(",", GetPropertyNamesForT())));
            }
            return this;
        }

        private static string InternalExtractPropertyNameFrom<TPropertyType>(Expression<Func<TPropertyType>> expression)
        {
            MemberExpression body;
            try
            {
                body = expression.Body as MemberExpression;
                if (body == null)
                {
                    var ubody = (UnaryExpression)expression.Body;
                    body = ubody.Operand as MemberExpression;
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid property matcher expression, check the property and matcher type are correct. Check for invalid casts. Expression is:" + expression, e);
            }

            if (body == null)
            {
                throw new ArgumentException("Expression is not a valid or recognised property reference");
            }

            return body.Member.Name;
        }

        private bool PropertyExists(string propertyName)
        {
            return GetPropertyForTypeNamed(typeof(T), propertyName) != null;
        }

        private PropertyInfo GetPropertyForTypeNamed(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        } 

        private IEnumerable<string> GetPropertyNamesForT()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(propertry => propertry.Name);
        }

        /// <summary>
        /// performs type checks of the property value to ensure the matcher can match the given type, providing nicer 
        /// error messages if the checks fail
        /// </summary>
        internal class TypeSafePropertyMatcher : ISelfDescribing, IMatcher<object>, IProvidePrettyTypeName
        {
            private readonly Type m_propertyType;
            private readonly bool m_isNullableType;
            private readonly IMatcher m_propertyValueMatcher;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeSafePropertyMatcher"/> class.
            /// </summary>
            /// <param name="containingType">The type containing the property</param>
            /// <param name="propertyName">The name of the property to match</param>
            /// <param name="propertyType">The type of property to match</param>
            /// <param name="propertyValueMatcher">The matcher</param>
            private TypeSafePropertyMatcher(Type containingType, string propertyName, Type propertyType, IMatcher propertyValueMatcher)
            {
                PropertyName = propertyName;
                m_propertyType = propertyType;
                m_propertyValueMatcher = propertyValueMatcher;
                m_isNullableType = IsNullableType(propertyType);
            }

            internal string PropertyName { get; private set; }

            /// <summary>
            /// Calls <see cref="Matches(object, IMatchDiagnostics)"/> with a <see cref="NullMatchDiagnostics"/>
            /// </summary>
            /// <param name="actual">The value to match against</param>
            /// <returns>true if the matcher matches the given value</returns>
            public bool Matches(object actual)
            {
                return Matches(actual, NullMatchDiagnostics.Instance);
            }

            public bool Matches(object actual, IMatchDiagnostics diagnostics)
            {
                if (actual == null)
                {
                    if (m_isNullableType)
                    {
                        // the matcher should be able to handle the nulls, let it deal with it. It may match for null
                        return diagnostics.TryMatch(null, m_propertyValueMatcher);
                    }
                    diagnostics.MisMatched(Description.With()
                        .Value("expected type to be nullable")
                        .Value("for type", m_propertyType.FullName)
                        .Value("expected", "not null"));
                    return false;
                }

                if (m_propertyType.IsInstanceOfType(actual))
                {
                    return diagnostics.TryMatch(actual, m_propertyValueMatcher);
                }

                diagnostics.MisMatched(
                        Description.With()
                            .Value("Incorrect type")
                            .Value("expected type", m_propertyType.FullName)
                            .Value("actual type", actual.GetType().FullName));

                return false;
            }

            public void DescribeTo(IDescription desc)
            {
                desc.Value("property", PropertyName);
                desc.Child("matches", m_propertyValueMatcher);
            }

            public string GetPrettyTypeName()
            {
                return "PropertyMatcher<" + ProvidePrettyTypeName.GetPrettyTypeNameFor<T>() + ">";
            }

            internal static TypeSafePropertyMatcher NewWithNameAndMatcher<TProperty>(Type containingType, string propertyName, IMatcher<TProperty> propertyValueMatcher)
            {
                return new TypeSafePropertyMatcher(containingType, propertyName, typeof(TProperty), propertyValueMatcher);
            }

            internal static bool IsNullableType(Type t)
            {
                if (!t.IsValueType) return true; // ref-type
                if (Nullable.GetUnderlyingType(t) != null) return true; // Nullable<T>
                return false; // value-type
            }
        }
    }
}