using CsvHelper;
using Microsoft.Extensions.Caching.Memory;
using SSCOreoWebapp.Models;
using SSCOreoWebapp.Service.Interface;
using System.Formats.Asn1;
using System.Globalization;

namespace SSCOreoWebapp.Service.Service
{
    public class CsvReadService:ICsvReadService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public CsvReadService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache= memoryCache;
        }

        public IEnumerable<T> GetCsvData<T>(string cacheKey, string path)
        {
            //ClientAllocAllContent
            try
            {
                return _memoryCache.GetOrCreate(cacheKey, (t) =>
                {
                    using var reader = new StreamReader(path);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var records = csv.GetRecords<T>();
                    return records.ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

         

        public async Task<Dictionary<string, Dictionary<string, List<PortfolioData>>>> GetCsvData()
        {
            try
            {
                var result = new Dictionary<string, Dictionary<string, List<PortfolioData>>>();
                using var reader = new StreamReader(_configuration["AppSettings:PortfolioHistoryFilePath"]);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<PortfolioHistoryModel>();
                    if (record == null) continue;
                    var data = new PortfolioData()
                        { NAV = record.NAV, Return = record.Return, AsOf = record.AsOf.ToString("yyyy-MM-dd") };
                    if (result.ContainsKey(record.Service))
                    {
                        var portfolio = result[record.Service];
                        if (portfolio.ContainsKey(record.Portfolio))
                        {
                            portfolio[record.Portfolio].Add(data);
                        }
                        else
                        {
                            portfolio.Add(record.Portfolio, new List<PortfolioData>() { data });
                        }
                    }
                    else
                    {
                        result.Add(record.Service,
                            new Dictionary<string, List<PortfolioData>>
                                { { record.Portfolio, new List<PortfolioData>() { data } } });
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                return null;
                Console.WriteLine(e);
            }

        }
    }
}
