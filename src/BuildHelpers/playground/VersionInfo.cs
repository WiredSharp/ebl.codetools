using System.Reflection;
using System.Runtime.CompilerServices;

// Les informations g�n�rales relatives � un assembly d�pendent de 
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associ�es � un assembly.
[assembly: AssemblyCompany("OFI-AM")]
[assembly: AssemblyProduct("CodeTools")]
[assembly: AssemblyCopyright("Copyright � OFI-AM 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if(DEBUG)
    [assembly: AssemblyConfiguration("debug")]
    [assembly: AssemblyInformationalVersion("2.0.17210")]
#else
    [assembly: AssemblyConfiguration("release")]
    [assembly: AssemblyInformationalVersion("2.0.17210")]
#endif

	 [assembly: InternalsVisibleTo("CodeTools.VisualStudio.Tools.Test")]
