using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class Level1Installer : SceneInstallerBase
{
    public override void RegisterServices()
    {
        Services.Register(new PlayerInputService());
    }
}
}