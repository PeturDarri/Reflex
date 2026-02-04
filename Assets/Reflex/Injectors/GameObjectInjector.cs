using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Injectors
{
    public static class GameObjectInjector
    {
        public static void InjectSingle(GameObject gameObject, Container container)
        {
            if (gameObject.TryGetComponent(out GameObjectScope gameObjectScope) && container != gameObjectScope.Container) return;

            if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                AttributeInjector.Inject(monoBehaviour, container);
            }
        }

        public static void InjectObject(GameObject gameObject, Container container)
        {
            if (gameObject.TryGetComponent(out GameObjectScope gameObjectScope) && container != gameObjectScope.Container) return;

            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponents<MonoBehaviour>(monoBehaviours);

            for (var i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    AttributeInjector.Inject(monoBehaviour, container);
                }
            }
        }

        public static void InjectRecursive(GameObject gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            GetMonoBehavioursRecursive(gameObject, container, monoBehaviours);

            for (var i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    AttributeInjector.Inject(monoBehaviour, container);
                }
            }

            return;
        }

        public static void InjectRecursiveMany(List<GameObject> gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);

            for (var i = 0; i < gameObject.Count; i++)
            {
                monoBehaviours.Clear();
                GetMonoBehavioursRecursive(gameObject[i], container, monoBehaviours);

                for (var j = 0; j < monoBehaviours.Count; j++)
                {
                    var monoBehaviour = monoBehaviours[j];

                    if (monoBehaviour != null)
                    {
                        AttributeInjector.Inject(monoBehaviour, container);
                    }
                }
            }
        }
        
        private static readonly List<MonoBehaviour> TempList = new();
        
        private static void GetMonoBehavioursRecursive(GameObject gameObject, Container container, List<MonoBehaviour> monoBehaviours)
        {
            if (gameObject.TryGetComponent(out GameObjectScope gameObjectScope) && container != gameObjectScope.Container) return;

            gameObject.GetComponents(TempList);
            monoBehaviours.AddRange(TempList);
            TempList.Clear();
            
            var transform = gameObject.transform;
            var childCount = transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                GetMonoBehavioursRecursive(child.gameObject, container, monoBehaviours);
            }
        }
    }
}