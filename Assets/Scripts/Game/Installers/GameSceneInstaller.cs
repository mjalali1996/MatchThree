using Containers;
using Game.Presenters;
using Game.Views;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private CellView _cellPrefab;

        [FormerlySerializedAs("_stonePrefab")] [SerializeField]
        private StoneView _stoneViewPrefab;

        [SerializeField] private SpriteContainer _stoneSpriteContainer;

        public override void InstallBindings()
        {
            Container.Bind<GameView>().FromComponentInHierarchy().WhenInjectedInto<GamePresenter>();
            Container.Bind<BoardPresenter>().FromComponentInHierarchy().WhenInjectedInto<GamePresenter>();
            Container.Bind<IBoardView>().To<BoardView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SpriteContainer>().FromInstance(_stoneSpriteContainer).WhenInjectedInto<BoardView>();

            Container.BindFactory<CellView, CellView.Factory>().FromMonoPoolableMemoryPool(m =>
                m.WithInitialSize(64).FromComponentInNewPrefab(_cellPrefab).UnderTransformGroup("Cells Pool"));

            Container.BindFactory<StoneView, StoneView.Factory>().FromMonoPoolableMemoryPool(m =>
                m.WithInitialSize(64).FromComponentInNewPrefab(_stoneViewPrefab).UnderTransformGroup("Stones Pool"));
        }
    }
}