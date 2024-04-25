using Game.Views;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private Stone _stonePrefab;

        public override void InstallBindings()
        {
            Container.Bind<BoardView>().FromComponentInHierarchy().AsSingle();
            
            Container.BindFactory<CellView, CellView.Factory>().FromPoolableMemoryPool<CellView, CellView.Pool>(m =>
                m.WithInitialSize(64).FromComponentInNewPrefab(_cellPrefab).UnderTransformGroup("Cell Pool"));
            
            Container.BindFactory<Stone, Stone.Factory>().FromPoolableMemoryPool<Stone, Stone.Pool>(m =>
                m.WithInitialSize(64).FromComponentInNewPrefab(_stonePrefab).UnderTransformGroup("Stone Pool"));
        }
    }
}