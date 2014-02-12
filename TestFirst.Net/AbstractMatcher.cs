using System;

namespace TestFirst.Net
{
    public abstract class AbstractMatcher<T>:IMatcher<T>
    {
        private readonly Nulls m_isNullable;

        private enum Nulls
        {
            NonNullableType, Allowed, NotAllowed
        }

        protected AbstractMatcher()
        {
            m_isNullable = IsNullableType(typeof (T))?Nulls.Allowed : Nulls.NonNullableType;
        }

        protected AbstractMatcher(bool allowNull)
        {
            if (IsNullableType(typeof(T)))
            {
                m_isNullable = allowNull ? Nulls.Allowed : Nulls.NotAllowed;
            }
            m_isNullable = Nulls.NonNullableType;
        }

        /// <summary>
        /// Does nothing, subclasses should override if they choose. No need to call this from sub classes
        /// </summary>
        /// <param name="description"></param>
        public virtual void DescribeTo(IDescription description)
        {
            //do nothing
        }

        /// <summary>
        /// Calls <see cref="Matches(object,IMatchDiagnostics)"/> with a <see cref="NullMatchDiagnostics"/>
        /// </summary>
        public bool Matches(object actual)
        {
            return Matches(actual, NullMatchDiagnostics.Instance);
        }

        /// <summary>
        /// Performs a type check to ensure the object is of the expected type. 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public bool Matches(object actual, IMatchDiagnostics diagnostics)
        {
            if (actual == null)
            {
                switch(m_isNullable)
                {
                    case Nulls.Allowed:
                        return Matches((T)actual, diagnostics);
                    case Nulls.NotAllowed:
                        diagnostics.MisMatched("Expected non null");
                        return false;
                    case Nulls.NonNullableType:
                        diagnostics.MisMatched("Wrong type, expected type {0} which is non nullable, but got null instead", typeof(T).FullName);
                        return false;
                }
            }
            if (typeof(T).IsInstanceOfType(actual))
            {
                return Matches((T)actual, diagnostics);
            }
            
            diagnostics.MisMatched(Description.With()
                .Text("Wrong type, expected {0} but got {1}", typeof (T).FullName, actual.GetType().FullName)
                .Value("actual",actual.GetType().FullName)
            );
            return false;
        }

        private static bool IsNullableType(Type t)
        {
            if (!t.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(t) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public override String ToString()
        {
            var desc = new Description();
            DescribeTo(desc);
            return desc.ToString();
        }

        /// <summary>
        /// Sub classes must override this
        /// </summary>
        public abstract bool Matches(T actual, IMatchDiagnostics diag);
    }
}
