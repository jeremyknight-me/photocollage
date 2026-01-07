# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

---

## Unreleased 

### Changed
 * Upgraded to .NET 10.
 * Optimized photo loading based on benchmark testing.

### Added
 * Added option for full-screen
 * Added extra options for transition speed
 * Added option to stretch or center full-screen display
 * Added option to fix rotation based on EXIF metadata for full-screen display

## [Version 4.0.0](../../releases/tag/v4) - 09 November 2021
### Changed
* Upgraded to .NET 6. 

## [Version 3.0.0](../../releases/tag/v3.0.0) - 29 January 2021
### Added
* Added option for background transparency slider.
* Added option for maximum photo size.
* Added option for maximum rotation (no UI). 
* Added preview button to option screen to allow desktop viewing. 
### Changed
* Upgraded to .NET 5. 
* Changed configuration file to use JSON.
* Changed maximum number of photos to 100 on slider.
### Removed
* Removed stand alone desktop version. 

## [Version 2.1.0](../../releases/tag/v2.1) - 19 July 2018
* Updated .NET to 4.7.1
* Updated file system code to speed up startup

## [Version 2.0.0](../../releases/tag/v2.0) - 8 January 2014
* Completely re-written in .NET 4.5
* Added multiple monitor support
* Changed from file dialog to folder dialog for folder selection

## Version 1.5.0 - 16 October 2013
* Fixed bug with photo location randomization

## Version 1.4.0 - 13 February 2013
* Added grayscale option.
* Added option to turn off borders.
* First release to include a desktop/stand-alone version.

## Version 1.3.0 - 21 February 2012
* Added an installer (.exe setup file)!
* More code optimization

## Version 1.2.0 - 29 December 2011
* Added configuration setting for randomization of photos
* Added setting for verbose logging. (Only accessible from configuration file.)
* Began adding code to take advantage of multiple core processors (parallel programming).
  
## Version 1.1.0 - 22 December 2011
* Fixed scaling issue on images that are not 96 dpi.
* Fixed video memory issue.
