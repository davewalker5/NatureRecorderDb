# NatureRecorderDb

[![Build Status](https://github.com/davewalker5/NatureRecorderDb/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/NatureRecorderDb/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/NatureRecorderDb)](https://github.com/davewalker5/NatureRecorderDb/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/NatureRecorderDb/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/NatureRecorderDb?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/NatureRecorderDb.svg?include_prereleases)](https://github.com/davewalker5/NatureRecorderDb/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/NatureRecorderDb/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/NatureRecorderDb/)
[![Language](https://img.shields.io/badge/database-SQLite-blue.svg)](https://github.com/davewalker5/NatureRecorderDb/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/NatureRecorderDb)](https://github.com/davewalker5/NatureRecorderDb/)

## About NatureRecorderDb

NatureRecorderDb implements the entities, business logic and a command-line application for recording wildife sightings. The application maintains details of:

- Locations, with the following details:
  - Unique name
  - Address details
  - Latitude and longitude
- Categories, with the following details:
  - Category name e.g. birds, mammals
- Species, with the following details:
  - The category to which the species belongs
  - Species name
- Species sightings, consisting of:
  - The species, and by implication the category the species belongs to
  - Location
  - Date
  - Number of animals seen (optional)
  - Whether or not they were seen with young

The CLI provides extensive commands for:

- Data entry, editing and removal
- On-screen reporting of the data
- Filtered (or non-filtered) export of data in CSV format

## Getting Started

Please see the [Wiki](https://github.com/davewalker5/NatureRecorderDb/wiki) for details on how to reference and use the Nature Recorder business logic and command line application.

## Authors

- **Dave Walker** - *Initial work* - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/NatureRecorderDb/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Trello

*  [Nature Recorder on Trello](https://trello.com/b/dwPS64rZ)
