using System;
using System.Text;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.TestSchedule;
using SurpassApiSdk.Exceptions;
using SurpassApiSdk.Models;

namespace SurpassAPI.Helper
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
