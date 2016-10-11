using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Base;
using SurpassApiSdk.DataContracts.Candidate;
using SurpassApiSdk.DataContracts.Centre;
using SurpassApiSdk.DataContracts.Folder;
using SurpassApiSdk.DataContracts.Item;
using SurpassApiSdk.DataContracts.Subject;
using SurpassApiSdk.DataContracts.TestSchedule;
using SurpassAPI.Helper;

namespace SurpassAPI
{
    class Program
    {
        public static string SurpassUrl { get; set; }
        public static string SurpassUsername { get; set; }
        public static string SurpassPassword { get; set; }
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once InconsistentNaming
        static void Main(string[] args)
        {
            SurpassUrl = @"https://instanceName.surpass.com/";
            SurpassUsername = @"ThisIsNotaUsername";
            SurpassPassword = @"ThisIsNotaPassword";
            var mySurpassClient = new SurpassApiClient(SurpassUrl, SurpassUsername, SurpassPassword);
            //Uncomment the blow to run

            //runSampleSurpassPopulation(mySurpassClient);
            //scheduleTestForToday(mySurpassClient, "Exam01", "Shipley001", "candidateRef01");
            //createSampleMultipleChoiceItem(mySurpassClient);
        }

        static void createSampleMultipleChoiceItem(SurpassApiClient surpassClient)
        {
            var myItemHelper = new ItemHelper(surpassClient);
            //Create a link between the item and the subject

            ItemSubjectResource myItemSubjectResource = new ItemSubjectResource
            {
                Reference = "Surpass0001"
            };
            FolderInputResource myFolderInputResource = new FolderInputResource
            {
                Name = "Sample Folder " + DateTime.UtcNow.ToLongDateString(),
                Subject = myItemSubjectResource
            };
            var myFolderHelper = new FolderHelper(surpassClient);
            FolderResource myFolder = myFolderHelper.GetFolderByName(myFolderInputResource);
            if (myFolder == null)
            {
                myFolder = myFolderHelper.CreateFolder(myFolderInputResource);
            }



            ItemInputResource myQuestion = new ItemInputResource
            {
                Subject = myItemSubjectResource,
                Name = "Sample Question",
                QuestionText = "What is the capital city of England?",
                Status = ItemStatusKey.Draft,
                Mark = 1,
                MarkingType = MarkingTypeKey.Computer,
                Folder = new ItemFolderResource
                {
                    Id = (int)myFolder.Id
                },
                MultipleResponseQuestions = new List<MulptipleResponseItemUpdateResource>
                {
                    new MulptipleResponseItemUpdateResource
                    {
                        MarkType = MarkTypeKey.Standard,
                        Randomise = true,
                        PartialMarks = true,
                        MaxSelections = 1,
                        AddLabelsToOptions = false,
                        OptionList = new ItemOptionListResource
                        {
                            Options = new List<ItemOptionResource>
                                {
                                    new ItemOptionResource
                                    {
                                        Text = "Shipley",
                                        Correct = false,
                                        Id = 1,
                                        ContentType = ContentTypeKey.RichText
                                    },
                                    new ItemOptionResource
                                    {
                                        Text = "London",
                                        Correct = true,
                                        Id = 2,
                                        ContentType = ContentTypeKey.RichText
                                    },
                                    new ItemOptionResource
                                    {
                                        Text = "Paris",
                                        Correct = false,
                                        Id = 3,
                                        ContentType = ContentTypeKey.RichText
                                    }
                                }
                        }

                    }

                }
            };
            var myCreatedItem = myItemHelper.CreateItem(myQuestion);
            Debug.WriteLine("Created Item: {0}, version: {1}", myCreatedItem.Id, myCreatedItem.ItemVersion);
        }
        static void scheduleTestForToday(SurpassApiClient surpassClient, string examReference, string centreReference, string candidateReference)
        {
            var myTestScheduleHelper = new TestScheduleHelper(surpassClient);
            var mySchedule = new TestScheduleResource
            {
                Test = new Resource
                {
                    Reference = examReference
                },
                Centre = new Resource
                {
                    Reference = centreReference
                },
                Candidate = new Resource
                {
                    Reference = candidateReference
                },
                StartDate = DateTime.Now.ToShortDateString(),
                StartTime = "0900",
                EndDate = DateTime.Now.AddDays(1).ToShortDateString(),
                EndTime = "1600",


                RequiresInvigilation = true,
                AllowMultipleOpenSessions = false
            };
            try
            {
                TestSchedulePostResponseModel myScheduleResponse = myTestScheduleHelper.CreateTestSchedule(mySchedule);
                Debug.WriteLine("Created test with keycode: {0} and PIN: {1}", myScheduleResponse.Keycode, myScheduleResponse.Pin);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating testschedule: {0}", ex.Message);
            }
        }

        static void runSampleSurpassPopulation(SurpassApiClient surpassClient)
        {
            var myCentreClient = new CentreHelper(surpassClient);
            var mySubjectClient = new SubjectHelper(surpassClient);
            var myCandidateHelper = new CandidateHelper(surpassClient);
            //Create a sample centre
            CentreCreateUpdateResource myCentre = new CentreCreateUpdateResource
            {
                Name = "Shipley Centre",
                Reference = "Shipley001"
            };
            var myCreatedCentre = myCentreClient.CreateOrUpdateCentre(myCentre);
            Debug.WriteLine("Created centre {0}", myCentre.Reference);
            //Create a sample subject with sample centre as primary centre
            SubjectCreateResource mySubject = new SubjectCreateResource
            {
                Name = "Surpass Subject",
                Reference = "Surpass0001",
                PrimaryCentre = myCreatedCentre
            };
            var myCreatedSubject = mySubjectClient.CreateOrUpdateSubject(mySubject);
            Debug.WriteLine("Created subject {0}", myCreatedSubject.Reference);
            //Create candidates from a list
            var myListOfSubjects = new List<SubjectResource> { mySubjectClient.Convert(myCreatedSubject) };
            var myListOfCentres = new List<CentreResource> { myCentreClient.Convert(myCreatedCentre) };
            var mySampleCandidates = createSampleCandidates(myListOfCentres, myListOfSubjects);
            foreach (var candidate in mySampleCandidates)
            {
                try
                {
                    var mySurpassCandidate = myCandidateHelper.CreateOrUpdateCandidate(candidate);
                    Debug.WriteLine("Created candidate {0}", mySurpassCandidate.Reference);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to create candidate {0} - {1}", candidate.Reference, ex.Message);
                }
            }
        }

        static List<CandidateCreateResource> createSampleCandidates(List<CentreResource> centres, List<SubjectResource> subjects)
        {
            var myActorsFileName = AppDomain.CurrentDomain.BaseDirectory + @"resources\FamousActors.txt";
            var mySampleCandidates = createCandidatesFromTextFile(myActorsFileName, centres, subjects);
            return mySampleCandidates;
        }

        static List<CandidateCreateResource> createCandidatesFromTextFile(string textFileName, IEnumerable<CentreResource> centres, IEnumerable<SubjectResource> subjects)
        {
            List<CandidateCreateResource> myList = new List<CandidateCreateResource>();
            using (StreamReader myStreamReader = new StreamReader(textFileName))
            {
                string myLine;
                while ((myLine = myStreamReader.ReadLine()) != null)
                {
                    var myStrings = myLine.Split('|');
                    var myCandidateFullName = myStrings[1];
                    var myNames = myCandidateFullName.Split(' ');
                    var myCandidateDateOfBirth = myStrings[3];

                    CandidateCreateResource myCandidate = new CandidateCreateResource
                    {
                        FirstName = myNames[0],
                        LastName = myNames[1],
                        DateOfBirth = DateTime.Parse(myCandidateDateOfBirth),
                        Reference = myCandidateFullName.Replace(" ", String.Empty) + myCandidateDateOfBirth.Replace(" ", String.Empty),
                        Subjects = subjects,
                        Centres = centres
                    };
                    myList.Add(myCandidate);
                }
            }
            return myList;

            //throw new NotImplementedException();
        }
    }
}
