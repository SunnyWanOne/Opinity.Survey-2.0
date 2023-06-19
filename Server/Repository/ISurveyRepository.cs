using System.Collections.Generic;
using System.Threading.Tasks;
using Opinity.Survey.Models;
using Opinity.Survey.Server.Repository;
using Radzen;

namespace Opinity.Survey.Repository
{
    public interface ISurveyRepository
    {
        Task<List<OqtaneSurvey>> GetAllSurveysAsync();
        List<OqtaneSurvey> GetAllSurveysByModule(int ModuleId);
        OqtaneSurvey GetSurvey(int Id);
        OqtaneSurvey CreateSurvey(Models.Survey NewSurvey);
        OqtaneSurvey UpdateSurvey(Models.Survey objExistingSurvey);
        bool DeleteSurvey(int id);
        List<OqtaneSurveyItem> GetAllSurveyItems(int ModuleId);
        OqtaneSurveyItem GetSurveyItem(int SurveyItemId);
        OqtaneSurveyItem CreateSurveyItem(Models.SurveyItem NewSurveyItem);
        OqtaneSurveyItem UpdateSurveyItem(Models.SurveyItem objExistingSurveyItem);
        bool DeleteSurveyItem(int Id);
        bool CreateSurveyAnswers(Models.Survey paramDTOSurvey);
        List<SurveyItem> SurveyResultsData(int SelectedSurveyId, LoadDataArgs args);
    }
}
