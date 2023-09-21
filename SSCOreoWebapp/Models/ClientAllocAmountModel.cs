namespace SSCOreoWebapp.Models
{
    public class ClientAllocAmountModel
    {
        public string Client { get; set; }
        public string Service { get; set; }
        public double AllocAmount { get; set; }
    }

    public class ClientServiceAmountMapping
    {
        public string Service { get; set; }
        public double AllocAmount { get; set; }
    }
}
