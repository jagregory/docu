using StructureMap;

namespace DrDoc
{
    internal static class ContainerBootstrapper
    {
        public static void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new DefaultRegistry());
            });
        }
    }
}