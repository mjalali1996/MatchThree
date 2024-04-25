using Game.Models;
using UnityEngine;
using Zenject;

namespace Project
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private LevelsContainer _levelsContainer;
        
        public override void InstallBindings()
        {
            Container.Bind<LevelsContainer>().FromInstance(_levelsContainer).AsSingle();
        }
    }
}
