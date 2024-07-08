using UnityEngine;

namespace EasyInject
{
    [ExecuteInEditMode]
    public class DIBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DependencyResolver.ResolveDependencies(this);
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                DependencyResolver.ResolveDependencies(this);
            }
        }
    }
}