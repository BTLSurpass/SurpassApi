using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Base;
using SurpassApiSdk.DataContracts.TestSchedule;
using SurpassApiSdk.Exceptions;
using SurpassApiSdk.Models;

namespace SurpassAPI
{
    public class TestScheduleHelper: SurpassHelper
    {
        public TestScheduleHelper(SurpassApiClient client) : base(client)
        {

        }
        public TestSchedulePostResponseModel CreateTestSchedule(TestScheduleResource testSchedule)
        {
            var testScheduleController = m_surpassApiClient.TestSchedule;
            try
            {
                TestSchedulePostResponseModel myResponse = testScheduleController.Post(testSchedule);
                return myResponse;
            }
            catch (ResourceException ex)
            {
                var myErrors = new StringBuilder();
                foreach (ApiHttpError error in ex.Errors)
                {
                    myErrors.AppendLine(error.Message);
                }
                throw new Exception(myErrors.ToString());
            }

        }

    }
}
