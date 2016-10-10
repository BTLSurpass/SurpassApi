using SurpassApiSdk;

namespace SurpassAPI.Helper
{
    public class SurpassApiHelper
    {
        private SurpassApiClient m_surpassApiClient;
        public SurpassApiHelper(SurpassApiClient client)
        {
            m_surpassApiClient = client;
        }
        public SurpassApiHelper(string url, string userName, string password)
        {
            m_surpassApiClient = new SurpassApiClient(url, userName, password);
        }
    }
}
