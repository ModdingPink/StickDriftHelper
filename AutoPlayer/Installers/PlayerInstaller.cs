using AutoPlayer.Handlers;
using AutoPlayer.UI;
using Zenject;

namespace AutoPlayer.Installers
{
    class PlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<AutoPlayHandler>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
