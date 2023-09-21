namespace SSCOreoWebapp.Models
{
    public class AnswerResponseModel
    {
        public string Tittle { get; set; }
        public List<string> PortfolioNames { get; set; }
        /// <summary>
        /// history / prediction
        /// 
        /// </summary>
        public string PortfolioType { get; set; }
        /// <summary>
        /// return or NAV
        /// </summary>
        public string AssetType { get; set; }
    }
    
}
