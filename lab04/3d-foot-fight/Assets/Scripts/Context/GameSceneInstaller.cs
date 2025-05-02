using UnityEngine;
using Zenject;

namespace Context
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Header("Services")] [SerializeField] private ParticleSpawner particleSpawner;

        public override void InstallBindings()
        {
            Container.Bind<ParticleSpawner>()
                .FromInstance(particleSpawner)
                .AsSingle();
        }
    }
}