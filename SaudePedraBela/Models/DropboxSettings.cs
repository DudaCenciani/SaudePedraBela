namespace SaudePedraBela.Models
{
    public class DropboxSettings
    {
        public DropboxSettings(string AcToken, string RfToken, string KeyN, string KeyS)
        {
            AccessToken = AcToken;
            RefreshToken = RfToken;
            AppKey = KeyN;
            AppSecret = KeyS;

        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
    }
}
