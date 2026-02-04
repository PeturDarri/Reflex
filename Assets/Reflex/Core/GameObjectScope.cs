using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    [DefaultExecutionOrder(ContainerScope.SceneContainerScopeExecutionOrder + 10)]
    public class GameObjectScope : MonoBehaviour
    {
        public Container Container { get; private set; }

        private void Awake()
        {
            var sceneContainer = gameObject.scene.GetSceneContainer();
            Container = sceneContainer.Scope(InstallBindings);
            GameObjectInjector.InjectRecursive(gameObject, Container);
        }

        private void InstallBindings(ContainerBuilder builder)
        {
            builder.SetName($"{gameObject.name} ({gameObject.GetInstanceID()})");
            
            using var pooledObject = ListPool<IInstaller>.Get(out var installers);
            GetComponentsInChildren(installers);

            foreach (var installer in installers)
            {
                installer.InstallBindings(builder);
            }
        }
    }
}