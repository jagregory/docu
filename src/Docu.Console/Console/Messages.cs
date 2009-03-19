namespace Docu.Console
{
    public static class Messages
    {
        public static readonly IScreenMessage Done = new DoneMessage();
        public static readonly IScreenMessage Help = new HelpMessage();
        public static readonly IScreenMessage NoAssembliesSpecified = new NoAssembliesFoundMessage();
        public static readonly IScreenMessage NoXmlsFound = new NoXmlsFoundMessage();
        public static readonly IScreenMessage ProcessingArguments = new ProcessingArgumentsMessage();
        public static readonly IScreenMessage Splash = new SplashMessage();
        public static readonly IScreenMessage Start = new StartMessage();
        public static readonly IScreenMessage BadFile = new BadFileMessage();
    }
}