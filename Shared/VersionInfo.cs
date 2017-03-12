using System.Reflection;
using System.Runtime.CompilerServices;

// Les informations générales relatives à un assembly dépendent de 
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyCompany("OFI-AM")]
[assembly: AssemblyProduct("CodeTools")]
[assembly: AssemblyCopyright("Copyright © OFI-AM 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if(DEBUG)
    [assembly: AssemblyConfiguration("debug")]
    [assembly: AssemblyInformationalVersion("2.0.0-pre")]
#else
    [assembly: AssemblyConfiguration("release")]
    [assembly: AssemblyInformationalVersion("2.0.0")]
#endif

	 [assembly: InternalsVisibleTo("CodeTools.VisualStudio.Tools.Test")]