# SurpassApi

Welcome to the Surpass API.

You can download the SDK from nuget..... Information to follow

Here's some sample code to create a centre
```cs
var mySurpassClient = new SurpassApiClient(SurpassUrl, SurpassUsername, SurpassPassword);
var myCentreClient = new CentreHelper(mySurpassClient);
var mySubjectClient = new SubjectHelper(mySurpassClient);

//Create a sample centre
CentreCreateUpdateResource myCentre = new CentreCreateUpdateResource
{
    Name = "Shipley Centre",
    Reference = "Shipley001"
};
var myCreatedCentre = myCentreClient.CreateOrUpdateCentre(myCentre);
//Create a sample subject with sample centre as primary centre
SubjectCreateResource mySubject = new SubjectCreateResource
{
    Name = "Surpass Subject",
    Reference = "Surpass0001",
    PrimaryCentre = myCreatedCentre
};
var myCreatedSubject = mySubjectClient.CreateOrUpdateSubject(mySubject);
```