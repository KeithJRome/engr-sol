# Take-home Engineering Challenge

With this engineering challenge, I considered three overall approaches to the solution:

| Approach | Pros | Cons |
| --- | --- | --- |
| Console app | can definitely write this quickly | won't be as fun to use, will be a very plain app |
| Web app w/ Google/Bing Maps | should look very nice, can setup deployment to test env | haven't used these APIs in javascript before and will require a bit of research to get keys (which also need to be protected), etc |
| Mobile app w/ Xamarin Maps | I've done this before, and know it will work. It also seems to be the most natural environment to have a food truck finding app | Definitely not enough time to deploy to stores, so anyone wishing to test will need to enroll in beta channel or install a fair amount of dev tooling |

## Reasoning

The code challenge instructions are clear that the solution should not be overengineered ("quality over quantity"). It is also clear about focusing on being a production-ready solution versus having a ton of features. Given more time to dedicate to the task, the web application would certainly be the ideal choice - it would show very nicely, and could be published directly to a demo URL from a CI/CD pipeline.

The mobile app approach would also be a desireable choice, however past experience with the mobile toolchains has taught me a hard lesson: the dev environment is very fragile and Murphy very much has the upper hand here.

That leaves the console app as the best choice IMO, so I can focus on completion and quality rather than showmanship. Also, the instructions claim that the San Francisco team has no problem with a command-line interface!

## Plan

Create a .NET Core console application with the following features:

* Download and parse the current vendor list
* Allow the user to enter their current lat/lng
* Ask for truck / cart preference
* Ask for any keywords to filter by
* Compute distances
* Display nearest 5 vendors
* Display total number of vendors operating that day

Optional features (time permitting):

* Allow the user to select the current date or enter another day
* Allow the user to enter their current address, and use Bing API to resolve lat/lng

## Development/Build Setup

* .NET Core SDK (Windows, Linux, MacOS) is the only required tooling: <https://dotnet.microsoft.com/download>

## Dev log notes

* For loading/parsing of the source data, this could be done a number of ways (including writing my own parser). However, the requirements here are fairly basic and the source data fairly clean. I decided to use the CsvHelper nuget package to perform this task. CsvHelper allows me to project the records into strongly-typed .NET objects.
* One complication discovered while testing is that when allowing the enumeration of records parsed by CsvHelper to be deferred, if the underlying stream has been disposed then the enumeration would throw. If this were high-throughput then I might let this get more complicated, but for this situation, I decided to simply force the enumeration before exiting the `using` block.
* To compute distances, rather than rely on a geolocation API (or batch API), I'm using a well-known formula to compute them quickly. It's not perfectly accurate, but should be "good enough" for relatively short distances such as these.
* Some permits do not include valid lat/lng data and so cannot be included in distance calculations. One way around this would be to perform address lookups using the Bing/Google APIs in order to resolve possibly usable coordinates.
* GeoLocation data is not available in .NET Core console apps, so there isn't a good way to find the user's current location. This is a downside due to the choice of application type (a mobile or web application would not have this limitation). I picked an arbitrary street corner in San Francisco as the default values to ease testing.
* Added some filtering features that weren't part of the original requirements (vendor type and keyword filters).
* Performed some refactoring to unclutter the main program class and provide a more clear seperation of concerns between code classes.
* Moved services behind interfaces to enable easier composition and testing.
* Created CI Build pipeline in personal devops account.