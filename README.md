# HeartRateToWeb

## Credits

This project has been made originally by [@loic2665](https://github.com/loic2665).
Here the original link to the [repository](https://github.com/loic2665/HeartRateToWeb).
The CSharp project has been cleaned, you should note that the python one didn't got any updates.

## Introduction

This program let you use the heart sensor of your Samsung Watch to display it on software that can read file like OBS. (tutorial below + watch compatibility list below). 

You will find there a python HTTP server and two CSharp HTTP server with one more user friendly because of the UI.

There is a module for [VRCOSC](https://github.com/VolcanicArts/VRCOSC) if you are interested too.

## How does it work ?

When you launch the file, the web server will start on your localhost and your LAN IP.

Usually for the best reaction time, you will use the LAN IP on your watch, and the localhost address on your software.

To maximise the compatibility between devices, I started the project on a web app. This is fast and a convinient way to put things in place.

## Start the program

#### Windows

On Windows you can use two easy ways.

##### CSharp

*You should note that you need the [.NET Core 3.1](https://dotnet.microsoft.com/fr-fr/download/dotnet/3.1) to run this app.*

Download the last release and launch one of the program `HeartRateToWeb.exe` or `TestHeartRateToWeb.exe`.

For get the current BPM count, just fetch `http://<local-ip>:6547` and you will get the data.

##### Python

Download the program in the release section, to be sure, put the program in is own folder
and double click on it, you will see your IP and Port to enter in the wearable app.

Then you can reduce it. (not close it !)

That's it.

If you have python3+ installed you can launch the program in a Powershell
by typing : 

```
PS > C:\Users\<username>\Downloads\heartbeat>python main.py [port]
``` 
#### Linux

You can start with : 
```
user@localhost~/Downloads/heartbeat$ python3 main.py [port]
```

#### MacOS

Sorry, I have no clue. Something similar to Linux ?

## Bugs ? How do I report one ?

For now, none. If you found one, please use the issue section.

How ? 

 - You must indicate your Platform (Windows / Linux / MacOS) and your version.
 - What you were expecting from the program ?
 - What you actually get from the program ?
 - Computers specs (CPU / RAM / Network Config)


## Compatibility


| Watch Model                        | Compatibility          | Notes                              |
|------------------------------------|------------------------|------------------------------------|
| Samsung Galaxy Watch               | Yes                    |                                    |
| Samsung Galaxy Watch Active        | Yes                    | Confirmed by others               |
| Samsung Galaxy Watch Active 2      | Yes                    | Confirmed by others               |
| Samsung Gear Sport                 | Yes                    | Confirmed by @ikemarx             |
| Samsung Gear S3                    | Yes      |                                    |
| Samsung Gear S2                    | Must be compatible     |                                    |
| Samsung Gear S                     | Unknown                |                                    |
| Samsung Galaxy Fit(e)              | No                     | No app store available            |
| Watches before these models        | Likely too old         |                                    |

Watches with the Tizen Platform version 2.3.2 and above are supposed to be compatible.

#### What about the Huawei Watches, Garmin, etc... ?

Well it's complicated, because I don't own these watches. I have no clue if you can create your own app like Samsung.

## Todo

 - Add a auto stop feature when the watch is not connected anymore.

## Releases

You can find release in the release section. There are none for now.
I will wait the watch app to be released first.

## Build an exe from python

For windows users you can build the script with this command

```
pyinstaller --onefile main.py --name heartbeatreceiver-build
```

Then go to the `dist` folder, and run the exe file. If you want to use the `/obs` endpoint, please make sure that the `obs.html` file are in the 
same folder, and your `js` and `css` folders too if you want to have more custom style.

## Tutorials

### Youtube video

https://youtu.be/-LpL4qPggFM

### OBS

There is multiple way to use the heartbeat on OBS, you can choose to read directly from the file, from the heartbeat endpoint 
or the obs endpoint.

#### From file

On OBS, you can add a text source, be sure to select "read from a file", and select the `hr.txt` file in the same directory of your
executable. OBS will take care of refreshing the content automaticly.

#### From / endpoint

Only on Windows as far as I know, you can add a Browser source on your OBS, and enter the following
address : `http://localhost:6547/` this will output the heartrate at this time.

This is the BPM count at the last record, it's not updated automatically if you add an OBS source, this is useful if you try to create a module for something else.

#### From the /obs endpoint

This is why this project is made, OBS. If you have some knowledge in HTML / CSS and know where to copy/paste code, then
you will be able to make your own template.

The `/obs` endpoint output a HTML page with support of CSS and JS. 
jQuery will be used to refresh the heartrate in real time.

Add a browser source with `http://localost:6547/obs` and OBS will start to show the webpage you created.

#### Using VRCOSC

Download the `HeartRateGearVRCOSCModule.dll` and copy it to `C:\Users\<username>\AppData\Roaming\VRCOSC\packages\local`.

## Demos 

https://streamable.com/azuhqn

https://streamable.com/pwtlhf
