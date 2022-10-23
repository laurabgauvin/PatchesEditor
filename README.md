# Patches Editor
This program was written for a specific work environment. All references to that company have been removed from the UI and from the code. In addition, some functionality has been removed from the original program for simplicity and/or privacy reasons.

*Note: this program was meant to be used as an internal program within the QA department of a specific company. As such, the UI is quite minimal and simple, since it was not meant to be used by external users that would be unfamiliar with its purpose or process.*

Its purpose is twofold:
1. To make it easier and faster to write and export patch notes using the specific format used by the company for which this was written. Examples of rules:
   - Lines in the exported patch notes can have a maximum of 100 characters, so this program allows the user to type the patch notes text without having to worry about making/updating line breaks every 100 characters and will instead do them automatically once the patch notes are exported.
   - Each program being patched out as part of one single patch may have different dependencies. This program allows setting the program dependencies using simple toggles for each individual program prior to exporting, no typing required.
2. To automate merging of multiple patch notes together using the specific process used by the company for which this was written.
   - Some sections (background, description of changes) are kept separate and marked by the 'short description' for the corresponding patch notes, while other sections are merged together (header, instructions, programs used). The remaining sections (impact, dependencies) are merged together when they are the same, and kept separate when they are different.

## General Information
This is a desktop program written in C# and WPF (XAML) using MVVM design pattern. It incorporates Material Design UI libraries.

It can import/export patch notes documents in .txt format (formatted patch notes for export) or .patch format (internal JSON data). It can be set as the default program to open .patch format documents and will open the selected file on startup if opened that way.

Patch notes contain the following sections:
- **Header**: Date, Programmer(s), Tester(s), Ticket(s) #
- **Background**: description of the original issue
- **Impact**: impact of applying this patch to the system
- **Dependencies**: may be different for each program being patched out
- **Description of Changes**: description of changes made to each program being patched out
- **Instructions**: instructions on how to apply the patch
- **Programs Used**: list of all programs patched out

See samples in the following directory: [Resources/Samples](/PatchesEditor/Resources/Samples)

## Starting Page
This is the page that displays when the program is opened directly from running the exe. It allows the user to quickly select which function they would like to perform.

![Starting Page Screenshot](/PatchesEditor/Resources/Images/StartupPage.png)
### New
Asks the user to select the folder where the programs to be patched out are located, then automatically generates the starting dependencies, instructions and programs used based on the programs in that folder.
### Open
Asks the user to select an existing patch notes document (.txt or .patch file format) and will continue editing that document.
### Import
Asks the user to select an existing patch notes document (.txt or .patch file format). It will load the data from that document, but the user will have to select a new file name when saving changes (it does not overwrite the original document).
### Merge
Sets the program in merge mode and asks the user to select the first patch notes document (.txt or .patch file format) to load.
### Continue
Goes to the program main page without loading anything.
### Parameters
Goes to the parameters page.

## Main Page
This is the main page of the application where the user enters the patch notes information

Blank page:
![Blank Main Page Screenshot](/PatchesEditor/Resources/Images/MainPage_Empty.png)

Page with some data entered:
![Main Page with Data Screenshot](/PatchesEditor/Resources/Images/MainPage_PatchNotes.png)

### Dependencies
The dependencies section is wrapped in an expander so that it may be fully hidden when it is already filled out, to save some space on the screen.

All programs being patched out have their own section with 3 main dependencies options: 
1. **All**: this program requires all current patches to be applied.
2. **Some**: this program requires some specific programs.
3. **None**: no dependencies for this program.

In the 'Some' case, the arrow on the left side of the box can be toggled in order to select which other programs this programs requires. Each dependency program can be set to one of the following statuses:
1. **Required**: this dependency program is required for the current patch.
2. **Previous**: this dependency program was required in a previous patch.
3. **Not Required**: this program is not a dependency for the main program.

The text displayed below each main program name (in italics with the lighter background) updates automatically based on the selection and is the text that will be exported to the final patch notes for that program.
![Main Page Dependencies Selection Screenshot](/PatchesEditor/Resources/Images/MainPage_Dependencies.png)

### Merge
When the program is in merge mode, an additional bar displays at the top where the user can navigate between all the patch notes that have been added so far, as well as add new ones or remove existing ones.

![Main Page Merge Mode Screenshot](/PatchesEditor/Resources/Images/MainPage_Merge.png)

## Parameters Page
The parameters for this application are saved in JSON format in a text file (with .params extention) to the C:\Patches Editor\ local directory (see [Resources/Samples](/PatchesEditor/Resources/Samples)). They can be modified directly in that text file and will be reloaded next time the application is started, but they can also be modified within the program (and take effect immediately).

![Parameters Page General Tab Screenshot](/PatchesEditor/Resources/Images/ParametersPage_General.png)
![Parameters Page Project Tab Screenshot](/PatchesEditor/Resources/Images/ParametersPage_Project.png)

## Single Patch Process
This is an overview of the process to export patch notes for a single patch:
1. Open the Patches Editor and select 'New'.
2. Select the directory where the programs to be patched out are located.
3. The Patches Editor automatically fills in the following fields:
   - Dependencies: the starting dependencies for each programs are determined either by the parameters (AllDependenciesPrograms and NoneDependenciesPrograms) or by the dependencies listed in the previously patched out patch notes (the previously patched out patch notes are found using the path in the parameter PatchesDirectory). *(Please note that the latter functionality has been commented out for this public version as you would not have any previously patched out patch notes)*
   - Instructions
   - Programs Used
4. The user enters the patch notes information (programmer(s), tester(s), ticket #, background, impact, dependencies, description of changes).
5. Once the patch notes are written and the programs are ready to be patched out, the user gets the previously patched out patch notes for each program (if there are some) and copies them into the directory with the programs.
6. The user clicks 'Export All'.
7. The Patches Editor generates patch notes for each program being patched out and either creates a .txt file for that program (if none exist), or edits the existing document by adding the new patch notes to the top.

## Merged Patch Process
This is an overview of the process to export patch notes when merging patch notes. Prior to this happening, patch notes would have to be written for each patch (either in .txt or .patch format).
1. Open the Patches Editor and select 'Merge'.
2. Select the first patch notes document.
3. The Patches Editor loads the first patch notes and displays a bar at the top where the user can add a new patch.
4. The user uses the + button to add all patch notes being merged together.
5. The Patches Editor saves all the patch data entered every time a new one is added, removed, or the user moves between patches using the arrow buttons. The user can also save manually using the 'Save' button.
6. Once all patches have been added, the user creates a new folder that contains all of the programs being patched out, as well as the previously patched out patch notes for each program (if there are some).
7. The user clicks 'Export All'.
8. The Patches Editor asks the user to select the folder where the final programs are located (folder from step 6).
9. The Patches Editor goes through each program being patched out in each patch notes and performs the merge.
   - First, it creates a dictionary that has each program being patched out as the key and the list of patches containing this program as the value (ordered by ticket #).
   - Then, for each of those key-value pairs, it creates the merged patch text:
     - The date is set to the current day.
     - The list of programmers, testers and tickets are added together from all patches.
     - Each of the backgrounds are marked with the 'short description' and added in order.
     - The impact and dependencies are merged together if they are all the same. If they are different, they are marked with the 'short description' for each patch.
     - Each of the description of changes are marked with the 'short description' and added in order.
     - The instructions are re-generated using the list of all programs from all patch notes together.
     - The list of programs used contains all of the programs from all patch notes together.
10. The Patches Editor generates patch notes for each program being patched out and either creates a .txt file for that program (if none exist), or edits the existing document by adding the new patch notes to the top.
