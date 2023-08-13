# Console-Music-Player
Console Music Player created by Patryk 'UltiPro' Wójtowicz using .NET C#.

Console application with volume and tracks controls. There are implemented transitions through folders and disks (in root of disk). Refreshing tracks, files and folders. Error handling in the absence of folders or files while the program is running. Automatic playback of songs in the order of the list (in the case of the end of the list, starting it again). Animations and remembering the last path and player volume. Application is dedicated for Windows OS (tested only on 10) and CMD (not Powershell). Project developed in .NET 6.

# Dependencies and Usage

Dependencies:

<ul>
  <li>WMPLib 1.0.0</li>
</ul>

Running the app:

> cd "/Console Music Player"

> dotnet run

Publishing the app:

> cd "/Console Music Player"

> dotnet publish

> cd "/bin/Debug/net6.0/publish"

Compressed ready to use program:

> Console Music Player.zip

# Preview

![Welcome Screen Preview](/screenshots/WelcomeScreen.png)

![Main Screen 1 Preview](/screenshots/MainScreen1.png)

![Main Screen 2 Preview](/screenshots/MainScreen2.png)

![Main Screen 3 Preview](/screenshots/MainScreen3.png)

![Main Screen 4 Preview](/screenshots/MainScreen4.png)

![Main Screen Animation Preview](/screenshots/MainScreenAnimation.gif)
