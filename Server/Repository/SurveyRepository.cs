using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using Opinity.Survey.Models;
using System.Threading.Tasks;
using System;
using Opinity.Survey.Server.Repository;
using Radzen;

namespace Opinity.Survey.Repository
{
    public class SurveyRepository : ISurveyRepository, IService
    {
        private readonly SurveyContext _db;

        public SurveyRepository(SurveyContext context)
        {
            _db = context;
        }

        // Surveys

        public async Task<List<OqtaneSurvey>> GetAllSurveysAsync()
        {
            return await _db.OqtaneSurvey
                .Include(x => x.OqtaneSurveyItem)
                .ThenInclude(x => x.OqtaneSurveyItemOption)
                .AsNoTracking()
                .OrderBy(x => x.SurveyName)
                .ToListAsync();
        }

        public List<OqtaneSurvey> GetAllSurveysByModule(int ModuleId)
        {
            return _db.OqtaneSurvey
                .Where(x => x.ModuleId == ModuleId)
                .Include(x => x.OqtaneSurveyItem)
                .ThenInclude(x => x.OqtaneSurveyItemOption)
                .AsNoTracking()
                .OrderBy(x => x.SurveyName)
                .ToList();
        }

        public OqtaneSurvey GetSurvey(int Id)
        {
            // There is one Survey per Module
            // so we get the survey by ModuleId
            return _db.OqtaneSurvey
                .Include(x => x.OqtaneSurveyItem)
                .ThenInclude(x => x.OqtaneSurveyItemOption)
                .Where(x => x.ModuleId == Id)
                .FirstOrDefault();
        }

        public OqtaneSurvey CreateSurvey(Models.Survey NewSurvey)
        {
            try
            {
                OqtaneSurvey objSurvey = new OqtaneSurvey();

                objSurvey.SurveyId = 0;
                objSurvey.ModuleId = NewSurvey.ModuleId;
                objSurvey.SurveyName = NewSurvey.SurveyName;
                objSurvey.UserId = NewSurvey.UserId;
                objSurvey.CreatedOn = DateTime.Now;

                _db.OqtaneSurvey.Add(objSurvey);
                _db.SaveChanges();

                return objSurvey;
            }
            catch
            {
                DetachAllEntities();
                throw;
            }
        }

        public OqtaneSurvey UpdateSurvey(Models.Survey objExistingSurvey)
        {
            try
            {
                var ExistingSurvey = _db.OqtaneSurvey
                                    .Where(x => x.SurveyId == objExistingSurvey.SurveyId)
                                    .FirstOrDefault();

                ExistingSurvey.SurveyName = objExistingSurvey.SurveyName;

                _db.SaveChanges();

                return ExistingSurvey;
            }
            catch
            {
                DetachAllEntities();
                throw;
            }
        }

        public bool DeleteSurvey(int id)
        {
            var ExistingSurvey =
                _db.OqtaneSurvey
                .Where(x => x.ModuleId == id)
                .FirstOrDefault();

            if (ExistingSurvey != null)
            {
                _db.OqtaneSurvey.Remove(ExistingSurvey);
                _db.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        // Survey Items

        public List<OqtaneSurveyItem> GetAllSurveyItems(int ModuleId)
        {
            return _db.OqtaneSurveyItem
                .AsNoTracking()
                .Where(x => x.SurveyNavigation.ModuleId == ModuleId)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public OqtaneSurveyItem GetSurveyItem(int SurveyItemId)
        {
            return _db.OqtaneSurveyItem
                .Where(x => x.Id == SurveyItemId)
                .Include(x => x.OqtaneSurveyItemOption)
                .FirstOrDefault();
        }

        public OqtaneSurveyItem CreateSurveyItem(Models.SurveyItem NewSurveyItem)
        {
            try
            {
                OqtaneSurveyItem objSurveyItem = new OqtaneSurveyItem();

                objSurveyItem.OqtaneSurveyAnswer = new List<OqtaneSurveyAnswer>();

                objSurveyItem.SurveyNavigation =
                    _db.OqtaneSurvey
                    .Where(x => x.ModuleId == NewSurveyItem.ModuleId)
                    .FirstOrDefault();

                objSurveyItem.Id = 0;
                objSurveyItem.ItemLabel = NewSurveyItem.ItemLabel;
                objSurveyItem.ItemType = NewSurveyItem.ItemType;
                objSurveyItem.ItemValue = NewSurveyItem.ItemValue;
                objSurveyItem.Required = NewSurveyItem.Required;
                objSurveyItem.Position = 0;

                if (NewSurveyItem.SurveyItemOption != null)
                {
                    objSurveyItem.OqtaneSurveyItemOption = ConvertToOqtaneSurveyItems(NewSurveyItem.SurveyItemOption);
                }

                _db.OqtaneSurveyItem.Add(objSurveyItem);
                _db.SaveChanges();

                // Set position
                int CountOfSurveyItems =
                    _db.OqtaneSurveyItem
                    .Where(x => x.SurveyNavigation.ModuleId == NewSurveyItem.ModuleId)
                    .Count();

                objSurveyItem.Position = CountOfSurveyItems;
                _db.SaveChanges();

                return objSurveyItem;
            }
            catch
            {
                DetachAllEntities();
                throw;
            }
        }

        public OqtaneSurveyItem UpdateSurveyItem(Models.SurveyItem objExistingSurveyItem)
        {
            try
            {
                var ExistingSurveyItem = _db.OqtaneSurveyItem
                    .Where(x => x.Id == objExistingSurveyItem.Id)
                    .Include(x => x.OqtaneSurveyItemOption)
                    .FirstOrDefault();

                ExistingSurveyItem.ItemLabel = objExistingSurveyItem.ItemLabel;
                ExistingSurveyItem.ItemType = objExistingSurveyItem.ItemType;
                ExistingSurveyItem.ItemValue = objExistingSurveyItem.ItemValue;
                ExistingSurveyItem.Required = objExistingSurveyItem.Required;

                ExistingSurveyItem.OqtaneSurveyItemOption = ConvertToOqtaneSurveyItems(objExistingSurveyItem.SurveyItemOption);

                _db.SaveChanges();

                return ExistingSurveyItem;
            }
            catch
            {
                DetachAllEntities();
                throw;
            }
        }

        public bool DeleteSurveyItem(int Id)
        {
            var ExistingSurveyItem =
                _db.OqtaneSurveyItem
                .Where(x => x.Id == Id)
                .FirstOrDefault();

            if (ExistingSurveyItem != null)
            {
                _db.OqtaneSurveyItem.Remove(ExistingSurveyItem);
                _db.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }
    
        // Survey Answers

        public bool CreateSurveyAnswers(Models.Survey paramDTOSurvey)
        {
            try
            {
                List<SurveyAnswer> SurveyAnswers = new List<SurveyAnswer>();

                foreach (var SurveyItem in paramDTOSurvey.SurveyItem)
                {
                    // Delete possible existing answer
                    List<OqtaneSurveyAnswer> ExistingAnswers;

                    if (paramDTOSurvey.UserId != null)
                    {                        
                        ExistingAnswers = _db.OqtaneSurveyAnswer
                            .Where(x => x.SurveyItemId == SurveyItem.Id)
                            .Where(x => x.UserId == paramDTOSurvey.UserId)
                            .ToList();
                    }
                    else
                    {
                        ExistingAnswers = _db.OqtaneSurveyAnswer
                            .Where(x => x.SurveyItemId == SurveyItem.Id)
                            .Where(x => x.AnonymousCookie == paramDTOSurvey.AnonymousCookie)
                            .ToList();
                    }

                    if (ExistingAnswers != null)
                    {
                        _db.OqtaneSurveyAnswer.RemoveRange(ExistingAnswers);
                        _db.SaveChanges();
                    }

                    // Save Answer

                    if (SurveyItem.ItemType != "Multi-Select Dropdown")
                    {
                        OqtaneSurveyAnswer NewSurveyAnswer = new OqtaneSurveyAnswer();

                        NewSurveyAnswer.AnswerValue = SurveyItem.AnswerValueString;

                        if (SurveyItem.AnswerValueDateTime != null)
                        {
                            NewSurveyAnswer.AnswerValueDateTime = Convert.ToDateTime(SurveyItem.AnswerValueDateTime);
                        }

                        NewSurveyAnswer.SurveyItemId = SurveyItem.Id;

                        if (paramDTOSurvey.UserId != null)
                        {
                            NewSurveyAnswer.UserId = paramDTOSurvey.UserId;
                        }
                        else
                        {
                            NewSurveyAnswer.AnonymousCookie = paramDTOSurvey.AnonymousCookie;
                        }

                        _db.OqtaneSurveyAnswer.Add(NewSurveyAnswer);
                        _db.SaveChanges();
                    }

                    if (SurveyItem.AnswerValueList != null)
                    {
                        foreach (var item in SurveyItem.AnswerValueList)
                        {
                            OqtaneSurveyAnswer NewSurveyAnswerValueList = new OqtaneSurveyAnswer();

                            NewSurveyAnswerValueList.AnswerValue = item;
                            NewSurveyAnswerValueList.SurveyItemId = SurveyItem.Id;

                            if (paramDTOSurvey.UserId != null)
                            {
                                NewSurveyAnswerValueList.UserId = paramDTOSurvey.UserId;
                            }
                            else
                            {
                                NewSurveyAnswerValueList.AnonymousCookie = paramDTOSurvey.AnonymousCookie;
                            }

                            _db.OqtaneSurveyAnswer.Add(NewSurveyAnswerValueList);
                            _db.SaveChanges();
                        }
                    }
                }

                return true;
            }
            catch
            {
                DetachAllEntities();
                throw;
            }
        }


        public List<SurveyItem> SurveyResultsData(int SelectedSurveyId, LoadDataArgs args)
        {
            // Get Survey Items
            // Don't include "Text Area"

            var query = _db.OqtaneSurveyItem
                .Where(x => x.Survey == SelectedSurveyId)
                .Where(x => x.ItemType != "Text Area")
                .Include(x => x.OqtaneSurveyItemOption)
                .OrderBy(x => x.Position).AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            var Results = query.Skip(args.Skip.Value).Take(args.Top.Value).ToList();

            List<SurveyItem> SurveyResultsCollection = new List<SurveyItem>();

            foreach (var SurveyItem in Results)
            {
                SurveyItem NewDTOSurveyItem = new SurveyItem();

                NewDTOSurveyItem.Id = SurveyItem.Id;
                NewDTOSurveyItem.ItemLabel = SurveyItem.ItemLabel;
                NewDTOSurveyItem.ItemType = SurveyItem.ItemType;
                NewDTOSurveyItem.Position = SurveyItem.Position;
                NewDTOSurveyItem.Required = SurveyItem.Required;

                List<AnswerResponse> ColAnswerResponse = new List<AnswerResponse>();

                if ((SurveyItem.ItemType == "Date") || (SurveyItem.ItemType == "Date Time"))
                {
                    var TempColAnswerResponse = _db.OqtaneSurveyAnswer
                        .Where(x => x.SurveyItemId == SurveyItem.Id)
                        .GroupBy(n => n.AnswerValueDateTime)
                        .Select(n => new AnswerResponse
                        {
                            OptionLabel = n.Key.Value.ToString(),
                            Responses = n.Count()
                        }
                        ).OrderBy(n => n.OptionLabel).ToList();

                    foreach (var item in TempColAnswerResponse)
                    {
                        string ShortDate = item.OptionLabel;

                        try
                        {
                            DateTime dtTempDate = Convert.ToDateTime(item.OptionLabel);
                            ShortDate = dtTempDate.ToShortDateString();
                        }
                        catch
                        {
                            // use original string
                        }

                        ColAnswerResponse.Add(
                            new AnswerResponse
                            {
                                OptionLabel = ShortDate,
                                Responses = item.Responses
                            }
                            );
                    }
                }
                else
                {
                    ColAnswerResponse = _db.OqtaneSurveyAnswer
                        .Where(x => x.SurveyItemId == SurveyItem.Id)
                        .GroupBy(n => n.AnswerValue)
                        .Select(n => new AnswerResponse
                        {
                            OptionLabel = n.Key,
                            Responses = n.Count()
                        }
                        ).OrderBy(n => n.OptionLabel).ToList();
                }

                if (ColAnswerResponse.Count > 10)
                {
                    // Only take top 10 
                    NewDTOSurveyItem.AnswerResponses = ColAnswerResponse
                        .OrderByDescending(x => x.Responses)
                        .Take(10).ToList();

                    // Put remaining items in "Other"
                    var ColOtherItems = ColAnswerResponse.OrderByDescending(x => x.Responses).Skip(10).ToList();
                    var SumOfOther = ColOtherItems.Sum(x => x.Responses);
                    NewDTOSurveyItem.AnswerResponses.Add(new AnswerResponse() { OptionLabel = "Other", Responses = SumOfOther });
                }
                else
                {
                    NewDTOSurveyItem.AnswerResponses = ColAnswerResponse;
                }

                SurveyResultsCollection.Add(NewDTOSurveyItem);
            }

            int SurveyResultsCount = _db.OqtaneSurveyItem
                .Where(x => x.Survey == SelectedSurveyId)
                .Where(x => x.ItemType != "Text Area")
                .Count();

            return SurveyResultsCollection;
        }

        // Utility       

        public async Task ExecuteSqlRaw(string sql)
        {
            await _db.Database.ExecuteSqlRawAsync(sql);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = _db.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();
            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        private List<OqtaneSurveyItemOption> ConvertToOqtaneSurveyItems(IEnumerable<Models.SurveyItemOption> colSurveyItemOptions)
        {
            List<OqtaneSurveyItemOption> colOqtaneSurveyItemOptionCollection = new List<OqtaneSurveyItemOption>();

            foreach (var objSurveyItemOption in colSurveyItemOptions)
            {
                // Convert to OqtaneSurveyItemOption
                OqtaneSurveyItemOption objAddOqtaneSurveyItemOption = ConvertToOqtaneSurveyItemOption(objSurveyItemOption);

                // Add to Collection
                colOqtaneSurveyItemOptionCollection.Add(objAddOqtaneSurveyItemOption);
            }

            return colOqtaneSurveyItemOptionCollection;
        }

        private OqtaneSurveyItemOption ConvertToOqtaneSurveyItemOption(SurveyItemOption objSurveyItemOption)
        {
            if (objSurveyItemOption == null)
            {
                return new OqtaneSurveyItemOption();
            }

            // Create new Object
            OqtaneSurveyItemOption objOqtaneSurveyItemOption = new OqtaneSurveyItemOption();

            objOqtaneSurveyItemOption.Id = objSurveyItemOption.Id;
            objOqtaneSurveyItemOption.OptionLabel = objSurveyItemOption.OptionLabel;

            return objOqtaneSurveyItemOption;
        }
    }
}
