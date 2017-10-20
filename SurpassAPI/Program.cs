using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Minimod.PrettyPrint;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Base;
using SurpassApiSdk.DataContracts.Candidate;
using SurpassApiSdk.DataContracts.Centre;
using SurpassApiSdk.DataContracts.Folder;
using SurpassApiSdk.DataContracts.Item;
using SurpassApiSdk.DataContracts.ItemTagValue;
using SurpassApiSdk.DataContracts.Subject;
using SurpassApiSdk.DataContracts.TagGroup;
using SurpassApiSdk.DataContracts.TagValue;
using SurpassApiSdk.DataContracts.TestSchedule;
using SurpassAPI.Helper;
using TagGroupResource = SurpassApiSdk.DataContracts.ItemTagValue.TagGroupResource;
using SurpassApiSdk.DataContracts.Item.Details;
using SurpassApiSdk.DataContracts.Item.Details.Items;
using SurpassAPI.Models;
using ET.FakeText;

namespace SurpassAPI
{
    class Program
    {
        public static string SurpassUrl { get; set; }
        public static string SurpassUsername { get; set; }
        public static string SurpassPassword { get; set; }
        const String _SubjectReference = "ShipleySubject";
        const String _SubjectName = "Shipley Subject";
        const String _CentreReference = "Shipley0001";
        const String _CentreName = "Shipley Centre 0001";

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once InconsistentNaming
        static void Main(string[] args)
        {

            SurpassUrl = Properties.Settings.Default.SurpassUrl ?? @"https://instanceName.surpass.com/";
            SurpassUsername = Properties.Settings.Default.SurpassUsername ?? @"ThisIsNotaUsername";
            SurpassPassword = Properties.Settings.Default.SurpassPassword ?? @"ThisIsNotaPassword";
            var mySurpassClient = new SurpassApiClient(SurpassUrl, SurpassUsername, SurpassPassword);
            //Uncomment the below to run sample code

            runSampleSurpassPopulation(mySurpassClient);
            var myFolderName = "Import Folder " + DateTime.UtcNow.ToLongDateString();
            importAllSampleContent(mySurpassClient, _SubjectReference, myFolderName);
            addImageToMediaLibrary(mySurpassClient, _SubjectReference, "image.jpg");
            scheduleTestForToday(mySurpassClient, "Exam01", _CentreReference, "candidateRef01");
            getResultForExam(mySurpassClient, "TF8B3HHF");

        }

        public static void importAllSampleContent(SurpassApiClient surpassClient, string subjectRef, String folderName)
        {
            var myPathToMultipleChoiceCsv = AppDomain.CurrentDomain.BaseDirectory + @"resources\Music.txt";
            importMultipleChoiceContentFromCsv(surpassClient, _SubjectReference, folderName, myPathToMultipleChoiceCsv, true, "Music");
            myPathToMultipleChoiceCsv = AppDomain.CurrentDomain.BaseDirectory + @"resources\Films.txt";
            importMultipleChoiceContentFromCsv(surpassClient, _SubjectReference, folderName, myPathToMultipleChoiceCsv, true, "Films");
            myPathToMultipleChoiceCsv = AppDomain.CurrentDomain.BaseDirectory + @"resources\Science And Nature.txt";
            importMultipleChoiceContentFromCsv(surpassClient, _SubjectReference, folderName, myPathToMultipleChoiceCsv, true, "Science And Nature");
            myPathToMultipleChoiceCsv = AppDomain.CurrentDomain.BaseDirectory + @"resources\SampleMCQs.txt";
            importMultipleChoiceContentFromCsv(surpassClient, _SubjectReference, folderName, myPathToMultipleChoiceCsv);
        }
        /// <summary>
        /// Sample upload - uses lorempixel to get a random image
        /// </summary>
        /// <param name="surpassClient"></param>
        /// <param name="subjectReference"></param>
        /// <param name="imageName"></param>
        private static long? addImageToMediaLibrary(SurpassApiClient surpassClient, string subjectReference, string imageName)
        {
            var myMediaHelper = new MediaHelper(surpassClient);
            //Ensure we only use safe characters
            Regex rgx = new Regex("[^a-zA-Z0-9 -.]");
            imageName = rgx.Replace(imageName, "");
            return myMediaHelper.Post(subjectReference, downloadImage(@"http://lorempixel.com/400/200/"), imageName);
        }

        private static byte[] downloadImage(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                return data;
            }
        }

        private static void addTagValue(SurpassApiClient surpassClient, TagGroupDetailedResource tagGroup, String value)
        {
            var myTagValueHelper = new TagValueHelper(surpassClient);
            if (myTagValueHelper.Get((int)tagGroup.Id.Value, value) == null)
            {
                var myTagValue = myTagValueHelper.CreateTag(tagGroup.Id.Value, value);
            }


        }
        private static TagGroupDetailedResource createOrGetTagGroup(SurpassApiClient surpassClient, String subjectRef, String name, TagGroupTagTypeValueKey tagType = TagGroupTagTypeValueKey.Text, bool allowDecimals = false)
        {
            var mySubjectHelper = new SubjectHelper(surpassClient);
            var mySubject = mySubjectHelper.GetSubject(subjectRef);
            if (mySubject == null) return null;
            var myTagGroupHelper = new TagGroupHelper(surpassClient);
            var myTagGroupDetailedResource = myTagGroupHelper.GetTagGroup(name, subjectRef);
            if (myTagGroupDetailedResource == null)
            {
                var myTagGroup = new TagGroupInputResource
                {
                    Name = name,
                    Subject = new ItemSubjectResource { Id = (int)mySubject.Id.Value },
                    AuthorCreation = true,
                    AllowMultipleTags = !(tagType == TagGroupTagTypeValueKey.Numeric),
                    TagTypeValue = tagType,
                    
                    //NumericTagProperties = new TagGroupRestrictionsResource(),
                };
                if (tagType == TagGroupTagTypeValueKey.Numeric)
                {
                    myTagGroup.NumericTagProperties = new TagGroupRestrictionsResource()
                    {
                        AllowDecimalPlaces = allowDecimals,
                        
                    };
                }
                myTagGroupDetailedResource = myTagGroupHelper.CreateTagGroup(myTagGroup);
            }
            Console.WriteLine(myTagGroupDetailedResource.PrettyPrint());
            return myTagGroupDetailedResource;
        }

        /// <summary>
        /// Sample method to get a result from Surpass
        /// It is advised to cache the centre & subjects if you are calling for many results in a loop
        /// </summary>
        /// <param name="surpassClient">Surpass API client</param>
        /// <param name="keycode">keycode for exam</param>
        static void getResultForExam(SurpassApiClient surpassClient, String keycode)
        {
            var myHelper = new ResultHelper(surpassClient);
            var myResult = myHelper.GetResult(keycode);
            var myCandidateHelper = new CandidateHelper(surpassClient);
            var myCandidate = myCandidateHelper.GetCandidate(myResult.Candidate.Reference);
            var mySubjectHelper = new SubjectHelper(surpassClient);
            var mySubject = mySubjectHelper.GetSubject(myResult.Subject.Reference);
            var mycentreHelper = new CentreHelper(surpassClient);
            var myCentre = mycentreHelper.GetCentre(myResult.Centre.Reference);

            var mySections = myResult.Sections;
            Console.WriteLine("Result for candidate '{0} {1}'. Exam taken at centre '{2}' in Subject '{3}' on '{4}'. Grade acheived was '{5}'. The exam had '{6}' sections."
                , myCandidate.FirstName, myCandidate.LastName, myCentre.Name, mySubject.Name, myResult.SubmittedDate, myResult.Grade, myResult.Sections.Count());
            Console.WriteLine(myResult.PrettyPrint());

        }

        /// <summary>
        /// Example demonstrating how to import content to create multiple choice questions
        /// </summary>
        /// <param name="surpassClient">Surpass API client</param>
        /// <param name="subjectReference">A unique identifier for the subject</param>
        /// <param name="folderName">Folder name</param>
        /// <param name="pathToMultipleChoiceCsv"></param>
        /// <param name="createImageForeachItem"></param>
        /// <param name="tagValue"></param>
        static void importMultipleChoiceContentFromCsv(SurpassApiClient surpassClient, String subjectReference, String folderName, string pathToMultipleChoiceCsv, bool createImageForeachItem = false, String tagValue = "")
        {
            //Create a link between the item and the subject - assumes that they already exist
            ItemSubjectResource myItemSubjectResource = new ItemSubjectResource
            {
                Reference = subjectReference
            };
            FolderInputResource myFolderInputResource = new FolderInputResource
            {
                Name = folderName,
                Subject = myItemSubjectResource
            };
            var myFolderHelper = new FolderHelper(surpassClient);
            var myFolder = myFolderHelper.GetOrCreateFolder(myFolderInputResource, subjectReference);
            TagGroupDetailedResource myTagGroup = null;
            TagValueResource myTagValue = null;
            if (tagValue != string.Empty)
            {
                myTagGroup = createOrGetTagGroup(surpassClient, subjectReference, "CustomProperty");
                var myTagHelper = new TagValueHelper(surpassClient);

                myTagValue = myTagHelper.Get((int)myTagGroup.Id, tagValue);
                if (myTagValue == null)
                {
                    myTagValue = myTagHelper.CreateTag((int)myTagGroup.Id, tagValue);
                }
            }
            importContent(surpassClient, pathToMultipleChoiceCsv, myFolder, myItemSubjectResource, createImageForeachItem, myTagGroup, myTagValue);
        }

        private static ItemInputResource createMCQItem(SurpassApiClient surpassClient, ItemSubjectResource itemSubjectResource, FolderResource folder, String myQuestionStem, double mySeededPValue, int mySeededUsageCount, List<ItemOptionUpdateResource> myAnswerOptions, bool createImageForeachItem)
        {
            
            ItemInputResource myQuestion = new ItemInputResource
            {
                Subject = itemSubjectResource,
                Name = myQuestionStem.Substring(0, Math.Min(myQuestionStem.Length, 50)).Replace('?', new char()),
                QuestionText = myQuestionStem,
                Status = "Draft",
                Mark = 1,
                MarkingType = MarkingTypeKey.Computer,
                SeedPValue = mySeededPValue,
                SeedUsageCount = mySeededUsageCount,
                
                Folder = new ItemFolderResource
                {
                    Id = (int)folder.Id
                },

                MultipleChoiceQuestions = new List<MultipleChoiceItemUpdateResource>
                        {
                            new MultipleChoiceItemUpdateResource
                            {
                                MarkType = MarkTypeKey.Standard,
                                Randomise = true,

                                //OptionList = myAnswerOptions

                                OptionList = new ItemOptionListUpdateResource
                                {
                                    Options = myAnswerOptions
                                }

                            }

                        }
            };
            if (mySeededUsageCount == 0)
            {
                myQuestion.SeedPValue = null;
                
            }

            if (createImageForeachItem)
            {

                Random r = new Random();
                var imageId = addImageToMediaLibrary(surpassClient, itemSubjectResource.Reference, r.Next(1, 10000) + ".jpg");
                var myList = new List<MediaItemDetailResource>();
                myList.Add(new MediaItemDetailResource
                {
                    Id = (int)imageId
                });
                myQuestion.MediaItems = myList;
            }
            return myQuestion;
        }

        private static void importContent(SurpassApiClient surpassClient, String pathToCsv, FolderResource folder, ItemSubjectResource itemSubjectResource, bool createImageForeachItem = false, TagGroupDetailedResource tagGroup = null, TagValueResource tagValue = null)
        {
            //Read the text file
            //We are using a simplified format of QuestionText|CorrectAnswer|Incorrect answers (multiple) - seperated by a '|' character
            //Assuming all questions are computer marked and worth one mark each

            var myItemHelper = new ItemHelper(surpassClient);
            using (StreamReader myStreamReader = new StreamReader(pathToCsv))
            {
                string myLine;
                //Ignore the first line as this only contains the headers
                myStreamReader.ReadLine();
                while ((myLine = myStreamReader.ReadLine()) != null)
                {

                    var myQuestionData = myLine.Split('|');
                    string myQuestionStem = myQuestionData[0];
                    double mySeededPValue = Convert.ToDouble(myQuestionData[1]);
                    int mySeededUsageCount = Convert.ToInt32(myQuestionData[2]);
                    string myCorrectAnswer = myQuestionData[3];
                    var myIncorrectAnswers = new List<String>();
                    for (int i = 4; i < myQuestionData.Length; i++)
                    {
                        myIncorrectAnswers.Add(myQuestionData[i]);
                    }
                    //Each answer option must have a unique id
                    var myAnswerId = 1;
                    List<ItemOptionUpdateResource> myAnswerOptions = new List<ItemOptionUpdateResource>
                    {
                        //Add the correct answer (we will randomise for presentation order later)
                        new ItemOptionUpdateResource
                        {
                            Text = myCorrectAnswer,
                            Correct = true,
                            Id = myAnswerId,
                            ContentType = ContentTypeKey.RichText
                        }
                    };
                    //Add each of the incorrect answers
                    foreach (String myIncorrectAnswer in myIncorrectAnswers)
                    {
                        myAnswerId++;
                        myAnswerOptions.Add(new ItemOptionUpdateResource
                        {
                            Text = myIncorrectAnswer,
                            Correct = false,
                            Id = myAnswerId,
                            ContentType = ContentTypeKey.RichText
                        });
                    }


                    var myQuestion = createMCQItem(surpassClient, itemSubjectResource, folder, myQuestionStem, mySeededPValue, mySeededUsageCount, myAnswerOptions, createImageForeachItem);

                    var myCreatedItem = myItemHelper.CreateItem(myQuestion);
                    Debug.WriteLine("Created Item: {0}, version: {1}", myCreatedItem.Id, myCreatedItem.ItemVersion);
                    if (myCreatedItem != null)
                    {
                        if ((tagGroup != null) && (tagValue != null))
                        {

                            var myItemTagValueInputResource = new ItemTagValueInputResource
                            {
                                TagValue = new SubjectTagValueResource { Id = (long)tagValue.Id },
                                TagGroup = new TagGroupResource { Id = tagGroup.Id.Value },
                                Item = new SubjectItemTagValueResource { Id = myCreatedItem.Id.Value }
                            };
                            var myItemTagValueHelper = new ItemTagValueHelper(surpassClient);
                            myItemTagValueHelper.Post(myItemTagValueInputResource);
                        }
                    }
                    

                }
            }
        }
        
        
        private static void setTagValue(SurpassApiClient surpassClient, TagGroupDetailedResource tagGroup, double value, ItemResource surpassItem)
        {
            setTagValue(surpassClient, tagGroup, Convert.ToString(value), surpassItem);
        }
        private static void setTagValue(SurpassApiClient surpassClient, TagGroupDetailedResource tagGroup, String value, ItemResource surpassItem)
        {
            var myTagValueHelper = new TagValueHelper(surpassClient);
            TagValueResource mySetCaseTagValue = myTagValueHelper.Get(Convert.ToInt32(tagGroup.Id), value);
            if (mySetCaseTagValue == null)
            {
                mySetCaseTagValue = myTagValueHelper.CreateTag(Convert.ToInt32(tagGroup.Id), value);
            }
            var myItemTagValueInputResource = new ItemTagValueInputResource
            {
                TagValue = new SubjectTagValueResource { Id = (long)mySetCaseTagValue.Id.Value },
                TagGroup = new TagGroupResource { Id = tagGroup.Id.Value },
                Item = new SubjectItemTagValueResource { Id = surpassItem.Id.Value }
            };
            var myItemTagValueHelper = new ItemTagValueHelper(surpassClient);
            myItemTagValueHelper.Post(myItemTagValueInputResource);
        }

        /// <summary>
        /// Creates one Multiple choice item
        /// </summary>
        /// <param name="surpassClient">Surpass API client</param>
        /// <param name="subjectReference">A unique identifier for the subject</param>
        //static void createSampleMultipleChoiceItem(SurpassApiClient surpassClient, String subjectReference)
        //{
        //    var myItemHelper = new ItemHelper(surpassClient);
        //    //Create a link between the item and the subject

        //    ItemSubjectResource myItemSubjectResource = new ItemSubjectResource
        //    {
        //        Reference = subjectReference
        //    };
        //    FolderInputResource myFolderInputResource = new FolderInputResource
        //    {
        //        Name = "Import Folder " + DateTime.UtcNow.ToLongDateString(),
        //        Subject = myItemSubjectResource
        //    };
        //    var myFolderHelper = new FolderHelper(surpassClient);
        //    var myFolder = myFolderHelper.GetOrCreateFolder(myFolderInputResource);

        //    ItemInputResource myQuestion = new ItemInputResource
        //    {
        //        Subject = myItemSubjectResource,
        //        Name = "Sample Question",
        //        QuestionText = "What is the capital city of England?",
        //        Status = "Draft",
        //        Mark = 1,
        //        MarkingType = MarkingTypeKey.Computer,
        //        Folder = new ItemFolderResource
        //        {
        //            // ReSharper disable once PossibleInvalidOperationException
        //            Id = (int)myFolder.Id
        //        },
        //        MultipleResponseQuestions = new List<MulptipleResponseItemUpdateResource>
        //        {
        //            new MulptipleResponseItemUpdateResource
        //            {
        //                MarkType = MarkTypeKey.Standard,
        //                Randomise = true,
        //                PartialMarks = true,
        //                MaxSelections = 1,
        //                AddLabelsToOptions = false,
        //                OptionList = new ItemOptionListResource
        //                {
        //                    Options = new List<ItemOptionResource>
        //                        {
        //                            new ItemOptionResource
        //                            {
        //                                Text = "Shipley",
        //                                Correct = false,
        //                                Id = 1,
        //                                ContentType = ContentTypeKey.RichText
        //                            },
        //                            new ItemOptionResource
        //                            {
        //                                Text = "London",
        //                                Correct = true,
        //                                Id = 2,
        //                                ContentType = ContentTypeKey.RichText
        //                            },
        //                            new ItemOptionResource
        //                            {
        //                                Text = "Paris",
        //                                Correct = false,
        //                                Id = 3,
        //                                ContentType = ContentTypeKey.RichText
        //                            }
        //                        }
        //                }

        //            }

        //        }
        //    };
        //    var myCreatedItem = myItemHelper.CreateItem(myQuestion);
        //    Debug.WriteLine("Created Item: {0}, version: {1}", myCreatedItem.Id, myCreatedItem.ItemVersion);
        //    Console.WriteLine(myCreatedItem.PrettyPrint());
        //}
        /// <summary>
        /// Schedule a test for a candidate (assumes test is already created in Surpass)
        /// </summary>
        /// <param name="surpassClient">Surpass API client</param>
        /// <param name="testReference">An identifier for the test</param>
        /// <param name="centreReference">A unique identifier for the centre</param>
        /// <param name="candidateReference">A unique identifier for the candidate</param>
        private static void scheduleTestForToday(SurpassApiClient surpassClient, string testReference, string centreReference, string candidateReference)
        {
            var myTestScheduleHelper = new TestScheduleHelper(surpassClient);
            var mySchedule = new TestScheduleResource
            {
                Test = new Resource
                {
                    Reference = testReference
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
                Console.WriteLine(myScheduleResponse.PrettyPrint());

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating testschedule: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Demonstrates how to setup a centre and subject in Surpass
        /// </summary>
        /// <param name="surpassClient">Surpass API client</param>
        static void runSampleSurpassPopulation(SurpassApiClient surpassClient)
        {
            var myCentreClient = new CentreHelper(surpassClient);
            var mySubjectClient = new SubjectHelper(surpassClient);
            var myCandidateHelper = new CandidateHelper(surpassClient);
            //Create a sample centre
            CentreCreateResource myCentre = new CentreCreateResource
            {
                Name = _CentreName,
                Reference = _CentreReference
            };
            var myCreatedCentre = myCentreClient.CreateOrUpdateCentre(myCentre);
            Debug.WriteLine("Created centre {0}", myCentre.Reference);
            //Create a sample subject with sample centre as primary centre
            SubjectCreateResource mySubject = new SubjectCreateResource
            {
                Name = _SubjectName,
                Reference = _SubjectReference,
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

        /// <summary>
        /// Creates candidates and assigns each to a list on centres & subjects in Surpass
        /// </summary>
        /// <param name="centres">List of centres</param>
        /// <param name="subjects">List of subjects</param>
        /// <returns></returns>
        static List<CandidateCreateResource> createSampleCandidates(List<CentreResource> centres, List<SubjectResource> subjects)
        {
            var myActorsFileName = AppDomain.CurrentDomain.BaseDirectory + @"resources\FamousActors.txt";
            var mySampleCandidates = createCandidatesFromTextFile(myActorsFileName, centres, subjects);
            return mySampleCandidates;
        }

        private static List<CandidateCreateResource> createCandidatesFromTextFile(string textFileName, IEnumerable<CentreResource> centres, IEnumerable<SubjectResource> subjects)
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
