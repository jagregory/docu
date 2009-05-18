using StructureMap;

namespace Docu
{
    public static class ContainerBootstrapper
    {
        public static IContainer BootstrapStructureMap()
        {
            return new Container(x => x.AddRegistry(new DefaultRegistry()));
        }
    }
}