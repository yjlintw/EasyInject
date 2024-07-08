using System;
using EasyInject;
using UnityEngine;

namespace EasyInject.Examples
{
    public class ExampleUsage : DIBehaviour
    {
        [Inject(SearchScope.Scene, new Type[] { typeof(AudioSource) })]
        public ExampleComponent exCompInScene;

        [Inject(SearchScope.Parent, new Type[] {}, new string[] {"id"}, new object[] {0})]
        public ExampleComponent exCompInParent;

        [Inject(SearchScope.Children, new Type[] { typeof(Collider) })]
        public ExampleComponent exCompInChildren;
        
        [Inject(SearchScope.Self)]
        public ExampleComponent exCompInSelf;
    }
}