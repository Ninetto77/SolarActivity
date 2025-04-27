using UnityEngine;
using Zenject;

public class GallaryInstaller : MonoInstaller
{
    [SerializeField] private SolarActivityGallery gallery;
    public override void InstallBindings()
    {
        Container.Bind<SolarActivityGallery>().FromInstance(gallery).AsSingle().NonLazy();
    }
}