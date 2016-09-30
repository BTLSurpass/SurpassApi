# SurpassApi

Welcome to the Surpass API.

You can download the SDK from nuget blah, blah, blah
Here's some sample code to create a centre
'''
CentreCreateUpdateResource myCentre = new CentreCreateUpdateResource
            {
                Name = "BTL Centre",
                Reference = "BTL00001"
            };
var myCreatedCentre = m_helper.CreateOrUpdateCentre(myCentre);
SubjectCreateResource mySubject = new SubjectCreateResource
            {
                Name = "BTL Subject",
                Reference = "BTLSubject00001",
                PrimaryCentre = myCreatedCentre
            };
var myCreatedSubject = m_helper.CreateOrUpdateSubject(mySubject);
'''
