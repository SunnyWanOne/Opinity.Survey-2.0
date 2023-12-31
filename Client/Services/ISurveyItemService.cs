﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Opinity.Survey.Models;

namespace Opinity.Survey.Services
{
    public interface ISurveyItemService
    {
        Task<List<Models.SurveyItem>> GetSurveyItemsAsync(int ModuleId);

        Task<Models.SurveyItem> GetSurveyItemAsync(int ModuleId);

        Task<Models.SurveyItem> AddSurveyItemAsync(Models.SurveyItem SurveyItem);

        Task<Models.SurveyItem> MoveSurveyItemAsync(string MoveType, Models.SurveyItem SurveyItem);

        Task<Models.SurveyItem> UpdateSurveyItemAsync(Models.SurveyItem SurveyItem);

        Task DeleteSurveyItemAsync(Models.SurveyItem SurveyItem);
    }
}
