# Rezultate Vot 

[![GitHub contributors](https://img.shields.io/github/contributors/code4romania/rezultate-vot.svg?style=for-the-badge)](https://github.com/code4romania/rezultate-vot/graphs/contributors) [![GitHub last commit](https://img.shields.io/github/last-commit/code4romania/rezultate-vot.svg?style=for-the-badge)](https://github.com/code4romania/rezultate-vot/commits/master) [![License: MPL 2.0](https://img.shields.io/badge/license-MPL%202.0-brightgreen.svg?style=for-the-badge)](https://opensource.org/licenses/MPL-2.0)

The project aims to be an aggregator for election information in Romania. The project will contain historic information for elections in Romania, will contain real time results for each election, as it will parse results published by BEC (Biroul Electoral Central) and it will also contain information about the budgets spent during the electoral campaign.

[See the project live](https://rezultatevot.ro)

The partial results published by BEC (Biroul Electoral Central) are often raw results that are not easily interpreted and give no meaningful information to regular users. The aim of the project is to aggregate the raw data and provide timely updates on the progression of voting results in Romanian elections.

[Contributing](#contributing) | [Built with](#built-with) | [Repos and projects](#repos-and-projects) | [Deployment](#deployment) | [Feedback](#feedback) | [License](#license) | [About Code4Ro](#about-code4ro)

## Contributing

This project is built by amazing volunteers and you can be one of them! Here's a list of ways in [which you can contribute to this project](.github/CONTRIBUTING.MD).

You can also list any pending features and planned improvements for the project here.

## Built With

### Programming languages

C# 7

### Platforms

.NET Core 2.1

### Package managers

NuGet

### Database technology & provider

* Amazon S3 Storage
* Amazon DynamoDB

## Repos and projects

TBD

## Deployment

#### Requirements

##### Prerequisites
* .NET Core 2.2 (backend)
* NodeJS (frontend)
* Docker (for running localstack on local machine)

##### Run the project

- `cd localstack`
- `.\setup.cmd`
  - this will use the docker-compose file to pull and run the localstack image
  - after that it will add two settings called `electionsConfig` which is the list of elections and `intervalInSeconds` which is the background task run interval
- `cd src\ElectionResults.WebApi`
- `dotnet run`

#### Configuration

In appsettings.\{environment\}.json you'll find the following settings:
##### Settings section
- **bucketName**: "code4-presidential-2019"
  - the name of the bucket where CSV files are downloaded
- **tableName**: "electionresults"
  - the name of the DynamoDB Table where the JSON statistics are stored
##### AWS.Logging section
- specifies how much information should appear in the AWS logs

##### config.json
If you run the project on your local machine, you can also modify the file `localstack\config.json` before running `localstack\setup.cmd`

## CSV URLs and mappings
- PUT request on /api/settings/election-config with a JSON representation of a list of Election objects. This will overwrite the json from AWS Parameter Store.
- Each Election object has:
  - a list of candidates where information about them can be provided(candidate picture, name, CSV column id, etc.)
  - a list of BEC URLs, each file has the type of results(provisional, partial or final), location(Romania or Diaspora), URL
- An initial configuration is provided in the file `localstack\config.json`

#### Run unit tests
- `cd tests\ElectionResults.Tests`
- `dotnet test`


## Feedback

* Request a new feature on GitHub.
* Vote for popular feature requests.
* File a bug in GitHub Issues.
* Email us with other feedback contact@code4.ro

## License 

This project is licensed under the MPL 2.0 License - see the [LICENSE](LICENSE) file for details

## About Code4Ro

Started in 2016, Code for Romania is a civic tech NGO, official member of the Code for All network. We have a community of over 500 volunteers (developers, ux/ui, communications, data scientists, graphic designers, devops, it security and more) who work pro-bono for developing digital solutions to solve social problems. #techforsocialgood. If you want to learn more details about our projects [visit our site](https://www.code4.ro/en/) or if you want to talk to one of our staff members, please e-mail us at contact@code4.ro.

Last, but not least, we rely on donations to ensure the infrastructure, logistics and management of our community that is widely spread across 11 timezones, coding for social change to make Romania and the world a better place. If you want to support us, [you can do it here](https://code4.ro/en/donate/).
