namespace SaudePedraBela.Models
{
    public class DropboxSettings
    {
        
        public DropboxSettings()
        { 
        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
    }
}
