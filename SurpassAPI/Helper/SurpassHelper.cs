using SurpassApiSdk;

namespace SurpassAPI.Helper
{
    public abstract class SurpassHelper
    {
        protected readonly SurpassApiClient m_surpassApiClient;

        protected SurpassHelper(SurpassApiClient client)
        {
            m_surpassApiClient = client;
        }
    }
}
