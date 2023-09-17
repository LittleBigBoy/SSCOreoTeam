namespace SSCOreoWebapp.Models
{
    public class ClientPortfoliosResponse
    {
        public string PortfolioName { get; set; }
        public double PortfolioData { get; set; }
        public string Percentage { get; set; }
        public List<PortfolioData> Data { get; set; }
    }
}
