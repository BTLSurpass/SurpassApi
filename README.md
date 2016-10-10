# SurpassApi

##Welcome to the Surpass API SDK sample project.

This application is available to help C# developers work with the Surpass API via the Surpass SDK.

You can download the SDK from [nuget](https://www.nuget.org/packages/Surpass.API.SDK/)

Install using the package manager console
**Install-Package Surpass.API.SDK -Pre**

*Example code to create a centre & subject*
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