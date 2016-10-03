using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurpassApiSdk;

namespace SurpassAPI
{
    public abstract class SurpassHelper
    {
        protected readonly SurpassApiClient m_surpassApiClient;
        public SurpassHelper(SurpassApiClient client)
        {
            m_surpassApiClient = client;
        }
    }
}
