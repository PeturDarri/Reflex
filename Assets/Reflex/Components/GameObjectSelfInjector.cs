using System;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Components
{
    [DefaultExecutionOrder(ContainerScope.SceneContainerScopeExecutionOrder + 100)] // +100 instead of +1 to leave room for other user custom components
    internal sealed class GameObjectSelfInjector : MonoBehaviour
    {
        [SerializeField] private InjectionStrategy _injectionStrategy = InjectionStrategy.Recursive;

        private void Awake()
        {
            var gameObjectScope = gameObject.GetComponentInParent<GameObjectScope>();
            var container = gameObjectScope != null ? gameObjectScope.Container : gameObject.scene.GetSceneContainer();

            switch (_injectionStrategy)
            {
                case InjectionStrategy.Single:
                    GameObjectInjector.InjectSingle(gameObject, container);
                    break;
                case InjectionStrategy.Object:
                    GameObjectInjector.InjectObject(gameObject, container);
                    break;
                case InjectionStrategy.Recursive:
                    GameObjectInjector.InjectRecursive(gameObject, container);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_injectionStrategy.ToString());
            }
        }

        private enum InjectionStrategy
        {
            Single,
            Object,
            Recursive
        }
    }
}