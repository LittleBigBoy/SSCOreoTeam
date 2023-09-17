namespace SSCOreoWebapp.Models
{
    public class PortfolioHistoryModel
    {
        public string Service { get; set; }
        public string Portfolio { get; set; }
        public double NAV { get; set; }
        public double Return { get; set; }
        public DateTime AsOf { get; set; }
    }

    public class Portfolio
    {
        public string Name { get; set; }
        public List<PortfolioData> Datas { get; set; }

    }

    public class PortfolioData
    {
        public double NAV { get; set; }
        public double Return { get; set; }
        public string AsOf { get; set; }
    }
}
