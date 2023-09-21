namespace SSCOreoWebapp.Models
{
    public class ClientAllocAmountModel
    {
        public string Client { get; set; }
        public string RiskTolerance { get; set; }
        public string Region { get; set; }
        public double TotalInvestment { get; set; }
        public double YearsOfInvestmentExperience { get; set; }
        public double AnnualFeeRate { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalServiceCost { get; set; }
        public double NetIncome { get; set; }
        public double ProfitMargin { get; set; }
        public string Service { get; set; }
        public double AvgAnnualReturn { get; set; }
        public double AnnualReturnVolatility { get; set; }
        public string Liquidity { get; set; }
        public double TotalValue { get; set; }
        public double ServiceTotalServiceFee { get; set; }
        public double ServiceTotalServiceCost { get; set; }
        public double AvgAnnualFeeRate { get; set; }
        public double Score { get; set; }
        public double AdjustedScore { get; set; }
        public double AllocPercent { get; set; }
        
    }

    public class ClientServiceAmountMapping
    {
        public string Service { get; set; }
        public double TotalInvestment { get; set; }
        public double AllocPercent { get; set; }
    }
}
