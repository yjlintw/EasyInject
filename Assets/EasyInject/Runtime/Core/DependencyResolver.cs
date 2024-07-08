using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyInject
{
    public static class DependencyResolver
    {
        public static void ResolveDependencies(MonoBehaviour monoBehaviour)
        {
            var fields = monoBehaviour.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);

            foreach (var field in fields)
            {
                var injectAttribute = (InjectAttribute)field.GetCustomAttributes(typeof(InjectAttribute), true).First();
                var fieldType = field.FieldType;
                Component dependency = null;

                switch (injectAttribute.Scope)
                {
                    case SearchScope.Self:
                        dependency = monoBehaviour.GetComponent(fieldType);
                        if (!CheckFilter(dependency, injectAttribute, fieldType))
                        {
                            dependency = null;
                        }
                        break;
                    case SearchScope.Children:
                        dependency = FindInChildren(monoBehaviour.gameObject, fieldType, injectAttribute);
                        break;
                    case SearchScope.Parent:
                        dependency = FindInParent(monoBehaviour.gameObject, fieldType, injectAttribute);
                        break;
                    case SearchScope.Scene:
                        dependency = FindDependencyInScene(fieldType, injectAttribute);
                        break;
                }

                if (dependency != null)
                {
                    field.SetValue(monoBehaviour, dependency);
                }
                else
                {
                    Debug.LogWarning($"No object of type {fieldType} found in the scope {injectAttribute.Scope} with filters {string.Join(", ", injectAttribute.FilterTypes.Select(t => t.Name))} and properties {string.Join(", ", injectAttribute.PropertyNames)} to inject into {field.Name}.");
                }
            }
        }

        private static Component FindInParent(GameObject gameObject, Type fieldType, InjectAttribute injectAttribute)
        {
            Component[] components = gameObject.GetComponentsInParent(fieldType, true);
            foreach (var component in components)
            {
                if (CheckFilter(component, injectAttribute, fieldType))
                {
                    return component;
                }
            }

            return null;
        }
        
        private static Component FindInChildren(GameObject gameObject, Type fieldType, InjectAttribute injectAttribute)
        {
            Component[] components = gameObject.GetComponentsInChildren(fieldType, true);

            foreach (var component in components)
            {
                if (CheckFilter(component, injectAttribute, fieldType))
                {
                    return component;
                }
            }

            return null;
        }

        private static Component FindDependencyInScene(Type fieldType, InjectAttribute injectAttribute)
        {
            Component[] dependencies = UnityEngine.Object.FindObjectsOfType(fieldType) as Component[];

            if (dependencies == null || dependencies.Length == 0)
            {
                return null;
            }

            // Filter the dependencies based on injectAttribute filters and property checks
            foreach (var dependency in dependencies)
            {
                if (CheckFilter(dependency, injectAttribute, fieldType))
                {
                    return dependency;
                }
            }

            return null;
        }

        private static bool CheckFilter(Component dependency, InjectAttribute injectAttribute, Type fieldType)
        {
            if (dependency == null)
            {
                return false;
            }

            if (injectAttribute.FilterTypes.Length > 0)
            {
                foreach (var filterType in injectAttribute.FilterTypes)
                {
                    if (!IsComponentPresent(dependency.gameObject, filterType))
                    {
                        return false;
                    }
                }
            }

            if (injectAttribute.PropertyNames.Length > 0)
            {
                for (int i = 0; i < injectAttribute.PropertyNames.Length; i++)
                {
                    var propertyName = injectAttribute.PropertyNames[i];
                    var expectedValue = injectAttribute.ExpectedValues[i];

                    var property = fieldType.GetProperty(propertyName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var memberField = fieldType.GetField(propertyName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (property != null)
                    {
                        var propertyValue = property.GetValue(dependency);
                        // Convert property value and expected value to string for comparison
                        if (!string.Equals(Convert.ToString(propertyValue), Convert.ToString(expectedValue)))
                        {
                            return false;
                        }
                    }
                    else if (memberField != null)
                    {
                        var memberFieldValue = memberField.GetValue(dependency);
                        // Convert field value and expected value to string for comparison
                        if (!string.Equals(Convert.ToString(memberFieldValue), Convert.ToString(expectedValue)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Property or field {propertyName} not found on type {fieldType}.");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsComponentPresent(GameObject gameObject, Type componentType)
        {
            return gameObject.GetComponent(componentType) != null;
        }
    }
}
