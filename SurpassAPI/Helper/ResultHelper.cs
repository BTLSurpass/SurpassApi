using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Candidate;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.DataContracts.Result;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class ResultHelper: SurpassHelper
    {
        public ResultHelper(SurpassApiClient client) : base(client)
        {

        }

        public ResultDetailedResource GetResult(String keycode)
        {
            try
            {
                var resultController = m_surpassApiClient.Result;
                TimeZonePageResponse<ResultDetailedResource> myResponse = resultController.GetByKeycode(keycode, false);
                return myResponse.Response.FirstOrDefault();
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }
    }
}
