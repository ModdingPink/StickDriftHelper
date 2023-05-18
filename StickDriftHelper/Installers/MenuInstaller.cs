using StickDriftHelper.UI;
using Zenject;

namespace StickDriftHelper.Installers
{
    class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<StickDriftMenu>().FromNewComponentOnRoot().AsSingle();
        }
    }
}
