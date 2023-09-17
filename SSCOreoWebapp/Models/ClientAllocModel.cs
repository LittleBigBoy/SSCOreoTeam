namespace SSCOreoWebapp.Models
{
    public class ClientAllocModel
    {
        public string Client { get; set; }
        public int Contribution { get; set; }
        public double SharesHoldingPercentage { get; set; }
        public string Portfolio { get; set; }
        public string Service { get; set; }
        public DateTime AsOf { get; set; }

    }
}
