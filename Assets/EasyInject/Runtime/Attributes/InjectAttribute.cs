using System;
using UnityEngine;

namespace EasyInject
{
    public enum SearchScope
    {
        Self,
        Children,
        Parent,
        Scene
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : PropertyAttribute
    {
        public SearchScope Scope { get; }
        public Type[] FilterTypes { get; }
        public string[] PropertyNames { get; }
        public object[] ExpectedValues { get; }

        public InjectAttribute(SearchScope scope, Type[] filterTypes = null, string[] propertyNames = null, object[] expectedValues = null)
        {
            Scope = scope;
            FilterTypes = filterTypes ?? new Type[0];
            PropertyNames = propertyNames ?? new string[0];
            ExpectedValues = expectedValues ?? new object[0];
        }
    }
}