namespace SSCOreoWebapp.Models
{
    public class ClientProfileModel
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
    }
}
