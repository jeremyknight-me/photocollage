# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

---

## Unreleased (Version 3.0.0)
### Added
* Added option for background transparency slider.
* Added option for maximum rotation. 
* Added ability to run screensaver from desktop.
* Added preview button to option screen to allow desktop viewing. 
### Changed
* Upgraded to .NET Core 3. 
* Changed configuration file to use JSON.
### Removed
* Removed requirement for user to install .NET.
* Removed stand alone desktop version. 

## [Version 2.1.0](../../releases/tag/v2.1)
* Updated .NET to 4.7.1
* Updated file system code to speed up startup

## [Version 2.0.0](../../releases/tag/v2.0)
* Completely re-written in .NET 4.5
* Added multiple monitor support
* Changed from file dialog to folder dialog for folder selection

## Version 1.5.0
* Fixed bug with photo location randomization

## Version 1.4.0
* Added grayscale option.
* Added option to turn off borders.
* First release to include a desktop/stand-alone version.

## Version 1.3.0
* Added an installer (.exe setup file)!
* More code optimization

## Version 1.2.0
* Added configuration setting for randomization of photos
* Added setting for verbose logging. (Only accessible from configuration file.)
* Began adding code to take advantage of multiple core processors (parallel programming).
  
## Version 1.1.0
* Fixed scaling issue on images that are not 96 dpi.
* Fixed video memory issue.
