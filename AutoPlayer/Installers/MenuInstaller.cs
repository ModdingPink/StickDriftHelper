using AutoPlayer.UI;
using Zenject;

namespace AutoPlayer.Installers
{
    class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<StickDriftMenu>().FromNewComponentOnRoot().AsSingle();
        }
    }
}
