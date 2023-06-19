using Oqtane.Models;
using Oqtane.Modules;

namespace Opinity.Survey
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Survey",
            Description = "Survey",
            Version = "1.0.0",
            ServerManagerType = "Opinity.Survey.Manager.SurveyManager, Opinity.Survey.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "Opinity.Survey.Shared.Oqtane,Radzen.Blazor,System.Linq.Dynamic.Core",
            PackageName = "Opinity.Survey" 
        };
    }
}
