using LM;
using UnityEngine;
using Zenject;

namespace Context
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Header("Services")]
        [SerializeField] private ParticleSpawner particleSpawner;

        [SerializeField] private SoundManager soundManager;

        public override void InstallBindings()
        {
            Container.Bind<ParticleSpawner>()
                .FromInstance(particleSpawner)
                .AsSingle();

            Container.Bind<SoundManager>()
                .FromInstance(soundManager)
                .AsSingle();
        }
    }
}