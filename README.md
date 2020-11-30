# Readme 

_AR Interactive Tour Guide_ is an augmented reality app for guided tours in the exhibition space. It was developed for a big scale object of the Oceania Collection of the Ethnologisches Museum Berlin in the Humboldt Forum Museum. This repository contains a Unity Project developed for and tested on iPads running iOS 13.1.1 and up. __The propietary software library [Fingers](https://assetstore.unity.com/packages/tools/input-management/fingers-touch-gestures-for-unity-41076)__ must be added through Unity store in the project hierarchy at _/ar_interactive_tour_guide_unity/Assets/Fingers_. All other third party libraries contain their own permissive open source licences.

## Table Of Contents


1. [Readme](#readme)
2. [Table of Contents](#table-of-contents)
3. [Brief](#brief)
4. [Technical Documentation of the Programming](#technical-documentation-of-the-programming)
5. [Installation](#installation)
6. [Use](#use)
7. [Credits](#credits)
8. [License](#license)

### Introduction
This _Manual for How to Use the AR Interactive Tour Guide Tool_ has been developed to help understand how to use the digital tool and how to improve it.

AR Interactive Tour Guide has been developed by museum4punkt0 along with NEEEU Spaces Gmbh for the Ethnologisches Museum Berlin/Staatliche Museen zu Berlin-Preußischer Kulturbesitz. The project museum4punkt0 is funded by the Federal Government Commissioner for Culture and the Media in accordance with a resolution issued by the German Bundestag (Parliament of the Federal Republic of Germany). The object which is contextualized is a ceremonial house of the Abelam culture (Dt. Kulthaus der Abelam), Oceania Collections of the Ethnologisches Museum. It will be displayed in one of the main exhibition rooms in the Humboldt Forum. 

The design challenge that was initially set was:

__How can augmented reality improve the guided visitor experience at the Ethnologisches Museum Berlin?__

After a first stage of research, a concept was defined in which the visitors and the guide would have tablets. The guide is able to use the main tablet to trigger videos, images and interactable AR elements in the tablets of the visitors to contextualise the narration and the object. At certain moments of the guided tour visitors can freely explore augmented objects.

![Cult house of the Abelam][211]

### Description 
The tool is designed to support the guide during the tour designed for the ceremonial house of the Abelam cultureat the Ethnologisches Museum in the Humboldt Forum in Berlin. The guide and visitors have iPads that are connected using a local network over WiFi. The guide’s iPad controls what the rest of the iPads can see, and when they are interactable. Visitors' can explore freely objects in 3D in some parts of the tour.

Some of the functionalities that are triggered are:

* AR content synchronisation
* AR content selection
* AR content focus and highlighting
* Coordinated gameplay
* Image and text display
* Video content reproduction and synchronization image reproduction

This manual provides an explanation on how to set up the digital tool and how to change the content in order to be adapted for other scenarios.

![AR Interactive Tour Guide at work][212]

## Brief 
AR Interactive Tour Guide is an augmented reality project of the Staatliche Museen zu Berlin - Preußischer Kulturbesitz in the context of museum4punkt0 for the Ethnologisches Museum Berlin in the Humboldt Forum.It is a storytelling tool  to support guided tours by allowing the mediating guide to play with high flexibility audiovisual content, games, 3D object manipulation and augmented reality in the tablets of visitors at the exhibition space. The guide and visitors have iPads that are connected using a local network over WiFi. The guide’s iPad controls what the rest of the iPads can see, and when they are interactable. When allowed by the guide’s iPad at certain moments of the storytelling, visitors can freely explore augmented objects such as sculptures.

The central object of the storytelling tool is the ceremonial house of the Abelam culture (German: Kulthaus der Abelam), a main, large size object of the Oceania Collections original from Papua New Guinea. By means of the storytelling tool, the mediating guide can expand his/her storytelling with contextual material such as maps, images and videos from anthropological field researches, reconstructions of previously object displays. The storytelling tool enables the mediating guide and the visitors to zoom in into displayed and digital augmented objects and to highlight their key elements. 

AR Interactive Tour Guide opens new ways of interaction and engagement for mediators and visitor groups and reinforce the interpersonal communication at the exhibition space.  

AR Interactive Tour Guide is part of  museum4punkt0 - Digital Strategies for the Museum of the Future. It was developed with the team xstream Digital (Staatliche Museen zu Berlin, focus: museums in the Humboldt Forum) in collaboration with NEEEU Spaces GmbH. Further information: [www.museum4punkt0.de](https://www.museum4punkt0.de) and [www.neu.io](https://www.neu.io).

The project museum4punkt0 is funded by the Federal Government Commissioner for Culture and the Media in accordance with a resolution issued by the German Bundestag (Parliament of the Federal Republic of Germany).


## Technical Documentation of the Programming

**This document offers technical details in this repository and act as an intro guide to the code. It is aimed at anyone trying to understand what the code is doing. The mediating guides using the app do not need to read or understand this document. The app will compile but will only show a blank screen unless the [server](#data-loading) containing the .csv files is properly adressed and the [external data folder](#how-to-work-with-the-external-data-folder) is properly imported. As mentioned in the introduction, the proprietary software library [Fingers](https://assetstore.unity.com/packages/tools/input-management/fingers-touch-gestures-for-unity-41076) is required to build the project.**

### General Structure and Remarks

### UI Framework
We are using a [UI Framework] (https://github.com/yankooliveira/uiframework/blob/master/MANUAL.md) to drive the flow of the application, in combination with a [typesafe, lightweight messaging library](https://github.com/yankooliveira/signals). You will be able to understand the code without understanding the details of these. You need to know:
-   The central screen list on the App GameObject defines which screens there are. For clarity, we kept this manually written instead of generated.
-   The static `App.uiFrame.OpenWindow(string windowName)` will hide the current window, add it to a history stack, and open the window with the given name.
-   `App.uiFrame.CloseCurrentWindow()` will reopen the last window on the stack

#### Synchronisation
 We are using [Mirror](https://github.com/vis2k/Mirror) for synchronising visitor and guide devices. Mirror is the community-developed continuation of UNet. About Mirror you need to know:

- The setup of Host and Clients is relatively straight forward. The guide device is setup as a Host, starting a server and a client connected to the server. Visitor devices discover hosts and connect to them as well.
- A GameObject can be synchronised by adding any component inheriting from NetworkBehaviour (as opposed to MonoBehaviour). 
- Any such GameObject must contain a NetworkIdentity component.
- Using several mechanisms, the state of a NetworkBehaviour [can be synchronised](https://mirror-networking.com/docs/Guides/Sync/) from the host to the clients. 
- A NetworkBehaviour must either be in the scene hierarchy from application startup or be spawned (as opposed to simply instantiated from a prefab) in order to be synchronised. NetworkBehaviour GameObjects will be deactivated until a network connection is established.  

Most of the unwanted complexity in the code is coming from the combination of UIFramework and Mirror. This is because UIFramework instantiates prefabs of its windows, meaning they cannot be NetworkBehaviours. Instead of changing that, we tried to separate out NetworkBehaviours so they do not need to be part of any windows, semi-successfully. 

There are two parts containing NetworkBehaviours:
- A central GuideSync NetworkBehaviour. If any window wants to synchronise anything, it goes through GuideSync. Note that this goes against the decentralisation premise of the Mirror library.
- GameObjects in the AR scenes: this is how Mirror is supposed to be used (however it goes against the code patterns of UIFramework to have these things lying around in the Scene hierarchy). 

A better integration of Mirror with UIFramework would be a laborious but rewarding rework of the code.

#### Data Loading
We are loading data into the Application from three UTF-8 encoded CSV files exported from Excel. These encode a three-level tree file/folder structure. See the excel sheet documentation.
The data has to be on a web space that can be reached via the URL that is entered in guide mode.

#### Coroutines
We are following a certain style guide to use unity coroutines.
- `App.cs` contains extension methods to execute coroutines in succession (`Then` and `Do`).
- Classes that define asynchronous behaviours simply expose IEnumerators. These may be nested (i.e. yield other IEnumerators at some point). These classes then do not need to be MonoBehaviours.
- Since IEnumerators cannot return results in Unity Coroutines, if an `IEnumerator Foo` needs to return a result T, we publicly define a field `T fooResult`. `Foo` will clear `fooResult` and reassign it.

DataImporterFromCSV is the best example for this pattern.

### App Startup Flow 
- Finds current network settings. Note these will likely be invalid on first startup. These contain the URL to the CSV files for the folder structure (also, less importantly, two ports that must be open and distinct that will be used for communication between guide and visitor devices).

  ![Network Settings Screen][411]

- Gets the three CSV files from the given URL
- Loads them into our File/Folder structure as given in `DataImporterFromCSV`. See remarks above on how we use coroutines. This returns a Folder of our File/Folder tree data structure, filled with all folders and files specified in the CSV.
- Checks whether the data from the CSV has changed since the last load. If it has, it throws away all local edits of this tour (tours can be locally edited, that is, the order of folders can be changed and folders can be hidden).
- Saves the data loaded from the CSV as the default tour
- Caches data. The Tour contains images and videos, specified by their URL. Cache will download these into a cache directory and download (video) or generate (images) thumbnails for each.
- Opens the ModeSelectionWindow, which has buttons to open the Guide and Visitor Setup Windows and the NetworkSettings Window. The function of these is better explained in code than prose.

For what remains, the Content Window drives the main user experience. Two different prefabs of the ContentWindowController are opened, depending on whether a Guide or a Visitor is opening them (ContentWindowGuide and ContentWindowVisitor respectively). It opens Files (synced over GuideSync), enables/disables the AR manager, and contains the Tour Explorer - a File Explorer for our File/Folder tree datastructure.

(###How-to-work-with-the-external-data-folder)
There’s a folder in the Unity Assets folder that is not checked in to git, as it contains large files. Instead, the data is stored on the server. Working with this setup needs some considerations for future developments of the tool.

The files are stored in the Assets/GDrive/ folder. Whenever there is a modification in that folder, the procedure must be:

1. Add the files in your Unity project to Assets/GDrive/
2. Open your unity project so Unity creates .meta files
3. Set the import settings you want in Unity - that is, whatever the inspector shows when you select the file in Unity
4. Upload the files to the server, including the generated meta files and any meta files for new folders you made

Whenever you add, remove, rename or move a file in the folder, and whenever you change the import settings for the asset in Unity, make sure to upload the file and its metafile to the server.



### How to build the AR scene with 3D models and Highlight Textures
In order to change the 3D elements in the tool, it is necessary to use Unity 2019.3.9f1. Once the project is open, please follow the next steps:

**Step 1**: prepare the 3D Models by building a prefab containing the 3DModel and the POIs

- Use **POIGettable** prefab
    1. Prepare the 3d Model and ensure that the pivot of the parent is in the centre and the model is facing in negative Z
    
          ![lorem ipsum][412]

   	2. Set the material to *Rendering Mode: 'Fade'* and enable *Emission*
    
          ![lorem ipsum][413] 
       
	3. Build a prefab of the 3D model by dragging it into *Assets/Prefabs/prepared_3DModels*
    
          ![lorem ipsum][414]
       
	4. Make a copy of POIGettable and place the 3D model prefab under the *model* child of the POIGettable
	5. Place one or multiple **POISub** prefabs under the *labels* child and position them correctly (the yellow sphere shows the origin position).
	6. Edit the descriptions
	    - on the **POIGettable** POI script edit the 6 text fields
		- on the **POISub** only the title fields are relevant
        
             ![lorem ipsum][415]
          
- Assigning Highlight Textures:
    1. Create the highlight texture by creating a mask over the area to highlight.
    
          ![lorem ipsum][416]
       
	2. Import the highlight texture.
	3. Drag the highlight-texture on the **HighlightTexture script** of a **POISub**.
	4. If the 3d Model consists of multiple models (eg Puti) you have to assign also the renderer on the **HighlightTexture script**. (if there is only one model it will be found by the script)
	5. Use the *Preview Highlight Texture* button to show which texture is assigned.
    
          ![lorem ipsum][417]
       
- Build a prefab variant of the POIGettable by dragging it in the  *Assets/Prefabs /AR Prefabs /POIGettables* folder to have it ready when building the final 3d Scene.

**Step 2**: Build the scene
- place the prepared prefabs in *Main Scene* under: *[AR]/AR Content/AR Scene (#)/ Content*
- save only the ARScene prefab. There is no need to save *Main Scene*.

**Step 3**: How to test it
To test the scene in the most deploy like state, start the app and navigate to the desired AR scene like this:

1. On the start screen select *Guide*.
2. Select a tour (or create a new one).
3. Navigate to the AR scene via the *Next* button or use the *UI Toggle* on the upper right and select *Start AR Exploration*. If there is none, please check your loaded .csv files.
4. Open *Marker* in the hierarchy: *[AR]/AR Content/AR Scene (#)/Marker* and click on *Marker Found* field button on the *Marker script*.

      ![lorem ipsum][418]

5. You can now select the objects and test your POIs and highlight textures

      ![lorem ipsum][419]

#### Compiling to Devices
In order to compile for iPads, choose: Build and choose iOS as building platform. Then press *Build and Run*.

![Unity build window][420]



## Installation
### First Time Setup
This is a guide on how to configure the app before use. Guides will not need this setup in order to use the app.

This app will function in any IOS device with iOS 13, but it is recommended to use iPads Pro 2020. 

The app needs to be installed in all the iPads, and they have to be connected to the same WiFi.

Opening the app for the first time, it will show you a warning saying it cannot find the CSV files to download tour data from. This is simply because we haven’t told it where to get this data from. You will then get to this screen:

IMAGE 1

![alt text][511]

Tap the button "NETZWERK" on the upper left corner of the screen.

![alt text][512]

It will get you to the Network Settings. Here, you can tell the app where to look for the CSV files containing the tour data. These settings will be saved locally on the device. Enter the URL you uploaded the CSV files to. The other two settings will generally stay as they are, the app uses these ports to communicate in your local network.

Hit “save”/”speichern" and wait for the app to load. If it has to cache a lot of data - that is, if you have big images and videos to load, this may well take over ten minutes. A highly stable WiFi connection may be necessary. Once this is done, the devices are set up and ready to go! Please follow the next instructions about how to use the digital tool as a guide.  


## Use
### How to Use the Tool as a Guide
This chapter assumes the First Time Setup has already been done.

As a guide, you will need to set up one device as your *guide device* and all others as *visitor devices*. This choice can be made on the first screen after the loading screen: 

![start screen][611]

Setting up the Guide device, you should create a profile with your own name. These profiles are stored on the device and saved until the tour data (from the excel sheet) changes. 

![start screen][612]

Clicking on the edit button next to your profile name will allow you to reorder and hide specific chapters for your tour. They will be saved under your profile only and will not affect other profiles. 

![start screen][613]

Clicking on your profile name will start the tour. You will at any point see the same content as the visitors - if you are seeing a video, so will every visitor. You can go through the content one at a time by clicking the NEXT/NÄCHSTES and PREVIOUS/ZURÜCK buttons in the footer. The next item (image, text, video, etc.) will be displayed, not the next chapter or theme.

![start screen][614]

In a started tour, there is the toggle UI button in the top right corner. It will show a menu of all the content in your tour. 

![start screen][615]

In there, you will find all photos, videos, text slides and AR experiences, organised in themes and chapters. You can open any of them by tapping on it. If there are several images in a chapter, you can skip items by tapping in a different one. If you want to go directly to an item in a different chapter or theme, you can freely navigate while a content is displayed for the visitors. This allows you to improvise, come backwards or forwards and to skip items when exploring the contents with the visitors.   
 

![start screen][616]

#### Using the Video
In order to play the video, please press the play button. When dragging the video timeline forward and backward, the timeline of the videos in the visitor devices will be controlled too.

#### *Next* and *Previous* Buttons
The NEXT/NÄCHSTES and PREVIOUS/ZURÜCK buttons go through each item in the order established in the app. Items are every single piece of content (text, images, videos, AR experiences, game). Please note that if an item is chosen manually by the use of the UI, after pressing NEXT/NÄCHSTES or PREVIOUS/ZURÜCK, the active item will ignore the one chosen manually.

#### Using AR Elements
Clicking on an AR experience, you will need to scan a marker to start the experience. 

![start screen][617]

##### Choosing between guided and free exploration
While the device is in guided exploration, the guide controls what the visitor sees. They will see the same as the guide, but without the UI overlay. In order to allow visitors to have control, the guide has to press the Button which says "Free Exploration/Freies Entdecken”. Each visitor will have to scan the marker to access the AR elements.

##### Ending the AR experiences
Pressing the END/BEENDEN button placed at the top left corner will end the AR experience.

![end AR scene][618]


##### Giebelwand AR Experience
For the AR experience of the Giebelwand, the guide can tap in the labels to access further information about each point of interest. Visitors can access themselves after the guide presses the “Free Exploration/Freies Entdecken” button.

##### Chambers AR Experience
For the interior of the house, it is possible to zoom in digital objects.
The chosen objects allow for zooming, spinning and showcasing points of interest. After pressing the Labels button, some parts of the objects are marked with dots (points of interest). By pressing in the points of interest, the area highlights and further information appears.

![start screen][619]

#### Construction Game
It is a simple, non-competitive click game, in which a series of drawings of parts used for the construction are presented. The visitors have to guess which are the ones representing the first and last steps of the construction.
If the drawing of the piece selected is the right one representing the step, it will highlight in green and be added to the drawing in the left of the screen.
If the piece selected is wrong, it will just highlight in red and do nothing.

Once the steps are finished, a message of congratulations appears.

![start screen][620]

#### How to Finish using the app
To Finish using the app, simply open the User Interface clicking in the top right button SHOW UI/UI AUSBLENDEN to open the UI. If you see the chapter menu, click on the top left corner button PREVIOUS/ZURÜCK to exit to the tour menu and click on the top left corner button END TOUR/TOUR BEENDEN to return to the guide tour editor window.

### How to Use the Tool as a Visitor
When using the tool as a visitor, the experience is mostly passive.
The guide will control what is seen in the screen, and the visitors can’t interact except in certain conditions:

- Free exploration/Freies Erkunden in AR mode
- Game

#### Connecting to a guide
When beginning the experience, after choosing visitor mode, a list of the potential guides will show up. By clicking in one of them, the device gets linked. Please, regard that the guide should be already inside the tour in order to be visible in the visitors’ devices.

![start screen][623]

#### Passive mode
Through most of the experience, there is not UI, and what is seen in the visitor device is triggered by the guide device. When the visitor tries to interact with the application, an image overlay stating: "Hallo! Gerade ist keine Interaktion möglich. Sie können stattdessen dem Guide zuhören!/Hello, hello. No interaction is possible now. You can listen to the guide instead!".

![Non-interactivce screen][623_1]

![start screen][624]

#### AR Mode
When the guide turns on AR Mode, a screen prompting the visitor to scan an image will prompt. Once it has been successfully scanned by the device, the frame will disappear and it will be possible to see the AR mode.

![start screen][625]

#### Free AR Exploration Mode
When Free exploration/Freies Entdecken is chosen in AR Mode by the guiding device, a message will prompt informing that it is possible to interact with the device.

![start screen][626] 

The interaction will be similar to the guide device. It is possible to click on labels to zoom in and access further information.

![start screen][627] 

#### Game
During the game, the person with the guided device will be asked to organise the drawings of different parts of the ceremonial house/KultHaus. It is a simple click game, with feedback when the choice is right or wrong.

### Changing Content - How to Fill the Excel Data Sheet
As explained in previous [chapters](#data-loading), the content displayed in the app is stored in a server, that you will have to set up and upload the content (jpg, mp4, csv) to. After the first start-up of the app you will have to submit the web address to that server into the form inside the app. The names and assets displayed in each chapter are set by the .csv files you upload here. Hence, if there are no or faulty/empty CSV files in that location the app will not start. You can find sample CSVs under “../ar_interactive_tour_guide_private/ar_interactive_tour_guide_server/CSV”.
 
In the excel sheet, there are three tabs for data, 'themes', 'chapters' and 'items'.
Strictly keep to the convention that all elements remain in their corresponding tabs (themes are in the 'themes' tab, etc.)

##### Column a 'type'
For themes and chapters, this should always be 'folder'. For items, this should be either 'text', 'video', 'image', 'AR', or 'game'
A text item is an item where this column says 'text', same for video, item, etc.

##### Column B, 'parent'
Must be 'root' for all themes. For chapters and items, this must be the name (e.g. third column) of the folder it's in (a theme or a chapter, respectively).

##### Column C, 'name'
A name for each theme, chapter and item. **It is very important that names are unique and that everything has a name**. The name is never displayed in the app, it is used to refer to folders and items internally. Hence I suggest that we keep this systematically as it is right now (chapter1, theme1.2, item1.2.3).

##### Column D, 'colour'
You may set a colour for each theme, chapter or item. This is the colour that will be highlighted in the app. I suggest filling in the colours for themes only, in which case all chapters and items will get their colour from the theme.

##### Column E, 'content'
Fill this in only for items.
For video and image items, these are the URLs to the image or video respectively. 
**For AR items, this must be either 'Giebelwand' or 'Chamber'**.

##### Column F, 'DE'
This is the German title that will be displayed in the app.

##### Column G, 'EN'
English title that will be displayed in the app.

##### Column H, 'Thumbnail'
URL to the thumbnail image to be displayed in the app UI. Fill in only for image and video items. Must be an URL to an image.

##### Column I, J, K, 'Location' 'Date' 'Licence'
Location/Date/Licence to be displayed on images and videos. Leave blank if not relevant for a particular image or video. 

##### Column L, M, 'Text DE' 'Text EN'
DE/EN Text, only for text items. Text to be displayed in the app on a text item. This information is just visible for the guide’s device.


#### How to get the data from the sheet to the app:

In the first tab, click the export button - this will export three files, 'themes.csv', 'chapters.csv', 'items.csv'. Do not change the names of these files. Make sure that you **export the .csv files with UTF-8 encoding**. This can be found in advanced settings while exporting as .csv.
 
Upload the three files to the folder on the FTP server, which is currently:
https://xplore.museum4punkt0.de/xstream/ 
 
In the app, when clicking on Network Settings/Netzwerk on the first screen, this folder URL must be set as the 'URL to the CSV files'. The app will then synchronize the data on every start and everytime you save these network settings.


#### How to add videos to the app:

1. Upload the video or image to the server (the higher the quality the better, but keeping images under 7mb and videos under 60mb.)
2. Add the video to the desired chapter using the spreadsheet, adding a 'Video' field and filing the fields in the Row as explained in the previous points. Use the existing examples in the sheets as reference in case of doubts.
3. Export the csv files and add to the server.


#### Dos and dont´s for the excel:

- Do make sure that the text is UTF-8 encoded when exporting as .csv.
- Do set the name of the exported files as the stated 'themes.csv', 'chapters.csv', and 'items.csv'.
- Do use existing fields as reference for new additions.
- Do fill all the necessary fields of each type.
- Don´t add fields and rows which are not necessary.
- Don´t add links that don´t work, The data ingest will not complete its work and run indefinitely.


## Credits
Contracting entity: Staatliche Museen zu Berlin - Preußischer Kulturbesitz

Authorship: Staatliche Museen zu Berlin - Preußischer Kulturbesitz / NEEEU Spaces GmbH

This manual has been developed by museum4punkt0 and NEEEU Spaces GmbH in the context of the AR Interactive Tour Guide project.

Contact Information for Project Responsibles

museum4punkt0 / Staatliche Museen zu Berlin – Preußischer Kulturbesitz: Cristina Navarro c.navarro@smb.spk-berlin.de / Sandro Schwarz: s.schwarz@smb.spk-berlin.de

NEEEU Spaces GmbH:
Javier Soto Morrás
j@neu.io - hello@neu.io

# Known issues in other versions of iOS"
__OS14__: The first time the app is launched, it needs to be opened in "Audience Mode" to prompt the privacy permission window. If only started as guide mode, the user will not be asked to allow the device to communicate with another device on the local network.


## License
This repository contains the open source parts of the museum4punkt0 AR Interactive Guide Tool project for the Ethnologisches Museum Berlin at the Humboldt Forum. In order to run this project, you need to add the closed-source third-party library
https://assetstore.unity.com/packages/tools/input-management/fingers-touch-gestures-for-unity-41076 at
\ar_interactive_tour_guide_unity\Assets\Fingers
as instructed in the text file in that folder.
All other third party libraries contain their own permissive open source licences.



[MIT License](https://github.com/museum4punkt0/AR-Interactive-Guide-Tool/blob/master/LICENSE)
Copyright © 2020, museum4punkt0 / NEEEU Spaces GmbH

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF ERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

[211]: images/image211.png
[212]: images/image212.png
[411]: images/image411.png
[412]: images/image412.png
[413]: images/image413.png
[414]: images/image414.png
[415]: images/image415.png
[416]: images/image416.png
[417]: images/image417.png
[418]: images/image418.png
[419]: images/image419.png
[420]: images/image420.png
[511]: images/image511.png
[512]: images/image512.png
[611]: images/image611.png
[612]: images/image612.png
[613]: images/image613.png
[614]: images/image614.png
[615]: images/image615.png
[616]: images/image616.png
[617]: images/image617.png
[618]: images/image618.png
[619]: images/image619.png
[620]: images/image620.png
[621]: images/image621.png
[622]: images/image622.png
[623]: images/image623.png
[623_1]: images/image623_1.png
[624]: images/image624.png
[625]: images/image625.png
[626]: images/image626.png
[627]: images/image627.png


