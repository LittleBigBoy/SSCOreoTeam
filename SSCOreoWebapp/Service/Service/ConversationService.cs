using Azure.Core;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Identity;
using SSCOreoWebapp.Service.Interface;
using System.Text.Json;
using SSCOreoWebapp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SSCOreoWebapp.Service.Service
{
   
    public class ConversationService : IConversationService
    {
        private readonly IConfiguration _configuration;
        private readonly ICsvReadService _csvReadService;
        private readonly IPortfolioService _portfolioService;
        public ConversationService(IConfiguration configuration, ICsvReadService csvReadService, IPortfolioService portfolioService)
        {
            _configuration = configuration;
            _csvReadService = csvReadService;
            _portfolioService = portfolioService;
        }

        public async Task<AnswerResponseModel> GetAnswerAsync(string question)
        {
            
            var endpoint = new Uri(_configuration["AppSettings:LanguageConversationEndpoint"]);
            var credential = new AzureKeyCredential(_configuration["AppSettings:LanguageConversationKey"]);

            var client = new ConversationAnalysisClient(endpoint, credential);

            var projectName = _configuration["AppSettings:LanguageConversationProjectName"];
            var deploymentName = _configuration["AppSettings:LanguageConversationDeployName"];

            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        text = question,
                        id = "1",
                        participantId = "1",
                    }
                },
                parameters = new
                {
                    projectName,
                    deploymentName,
                    verbose = true,

                    // Use Utf16CodeUnit for strings in .NET.
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };

            var response = await client.AnalyzeConversationAsync(RequestContent.Create(data));

            using var result = await JsonDocument.ParseAsync(response.ContentStream);
            var conversationalTaskResult = result.RootElement;
            var conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");
            var entities = conversationPrediction.GetProperty("entities")
                .EnumerateArray()
                .Select(entity => new ConversationPredictionModel()
                {
                    Category = entity.GetProperty("category").GetString(),
                    Text = entity.GetProperty("text").GetString(),
                    Offset = entity.GetProperty("offset").GetInt32(),
                    Length = entity.GetProperty("length").GetInt32(),
                    ConfidenceScore = entity.GetProperty("confidenceScore").GetSingle()
                })
                .ToList();
            var answerResponse = new AnswerResponseModel();
            var portfolioAsset = entities.FirstOrDefault(p => p.Category == "AssetPort")?.Text;
            var bondAsset = entities.FirstOrDefault(p => p.Category == "AssetBond")?.Text;
            var stockAsset = entities.FirstOrDefault(p => p.Category == "AssetStock")?.Text;
            var assetAttribute = entities.FirstOrDefault(p => p.Category == "AssetAttribute")?.Text;
            var compareAction = entities.FirstOrDefault(p => p.Category == "CompareAction")?.Text;
            var compareWith = entities.FirstOrDefault(p => p.Category == "CompareWith")?.Text;
            var compareScope = entities.FirstOrDefault(p => p.Category == "CompareScope")?.Text;
            var portfolioStringList = new List<string>();
            if (!string.IsNullOrWhiteSpace(bondAsset))
            {
                answerResponse.Tittle = "Bond is not supported now";
                return answerResponse;
            }
            else if (!string.IsNullOrWhiteSpace(stockAsset))
            {
                answerResponse.Tittle = "Stock is not supported now";
                return answerResponse;
            }

            if (portfolioAsset == null || !portfolioAsset.Contains("portfolio", StringComparison.OrdinalIgnoreCase))
            {
                answerResponse.Tittle = "I am learning this question now, please change another one.";
                return answerResponse;
            }
            {
                var clientAllocPath = _configuration["AppSettings:ClientAllocFilePath"];
                var clientAllocSourceData = _csvReadService.GetCsvData<ClientAllocModel>("ClientAllocFileData", clientAllocPath);
                var portfolios = clientAllocSourceData.Select(p => p.Portfolio).Distinct();
                
                if (compareScope != null && (compareScope.Contains("in future", StringComparison.OrdinalIgnoreCase) ||
                                             compareScope.Contains("predicted", StringComparison.OrdinalIgnoreCase)))
                {
                    // predicted
                }
                else
                {
                    answerResponse.Tittle = "Below portfolios are found for the description";
                    foreach (var portfolio in portfolios)
                    {
                        var portfolioData = _portfolioService.GetPortfolioData(portfolio).MaxBy(p => p.AsOf);
                        if (assetAttribute.Contains("return", StringComparison.OrdinalIgnoreCase))
                        {
                            answerResponse.AssetType = "return";
                            var returnValue = portfolioData.Return;
                            if (CheckValue(compareAction, compareWith, returnValue))
                                portfolioStringList.Add(portfolio);

                        }
                        else if (assetAttribute.Contains("NAV", StringComparison.OrdinalIgnoreCase))
                        {
                            answerResponse.AssetType = "NAV";
                            var navValue = portfolioData.NAV;
                            if (CheckValue(compareAction, compareWith, navValue))
                                portfolioStringList.Add(portfolio);
                        }
                    }

                    answerResponse.PortfolioNames = portfolioStringList;
                    answerResponse.PortfolioType = "History";
                }
            }
            return answerResponse;
        }

        private bool CheckValue(string compareAction,string compareWith, double compareVale)
        {
            var toValue = 0d;
            if (compareWith.Contains("%"))
            {
                if (double.TryParse(compareWith.Substring(0, compareWith.IndexOf('%')), out toValue))
                {
                    toValue = toValue / 100;
                }
            }
            else
            {
                double.TryParse(compareWith, out toValue);
            }

            return compareAction.ToLower() switch
            {
                "greater than" => compareVale > toValue,
                "less than" => compareVale < toValue,
                "equal to" => Math.Abs(compareVale - toValue) < 0,
                _ => false
            };
        }

    }
}
