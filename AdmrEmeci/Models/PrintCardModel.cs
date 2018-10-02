namespace AdmrEmeci.Models
{
    public class PrintCardModel
    {
        public string UrlImage { get; set; }
        public string EmeciSelected { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
    }
}