using UnityEngine;

namespace EasyInject.Examples
{
    public class ExampleComponent : MonoBehaviour, IFilterA
    {
        public string exampleProperty = "Hello, World!";
        public float id = 0;
    }
}