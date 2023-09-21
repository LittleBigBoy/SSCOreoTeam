namespace SSCOreoWebapp.Models
{
    public class ServiceProfileModel
    {
        public string Service { get; set; }
        public string AvgAnnualReturn { get; set; }
        public string AnnualReturnVolatility { get; set; }
        public string Liquidity { get; set; }
        public string TotalValue { get; set; }
        public double AvgAnnualFeeRate { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalServiceCost { get; set; }
        public double ProfitMargin { get; set; }
        public double Score { get; set; }
        public double AdjustedScore { get; set; }
    }
}
