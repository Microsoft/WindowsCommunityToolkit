Welcome to the Notifications section of the toolkit! This contains the Notifications library, including the object model for tile, toast, and badge XML (previously called NotificationsExtensions).

## Where should I add new code?
Any code for generating notifications should be written in the CommunityToolkit.WinUI.Notifications project.

If there's UWP-specific code, use the appropriate `#ifdef`, `WINDOWS_UWP` or `WINRT`.

## What are all the projects for?
All the code is contained on the CommunityToolkit.WinUI.Notifications project.

It outputs `netstandard1.4`, `uap10.0`, `native` for WinRT, and netcoreapp for .Net Core projects. The UWP library is only for C#, while the WinRT library is a Windows Runtime Component for C++.


| C#               | C++      |
| ---------------- | ------------------- |
| NET Standard 1.4 | UWP WinRT Component |
| UWP C# DLL |                     |
| .Net Core DLL |                     |



## Scenarios we want to support

Imagine you add this library to a .NET Standard class library, and you also add it to your UWP app. In this case, your .NET Standard class library will receive the NETStandard dll. Your UWP project will receive the UWP dll.

## How are the test projects organized?

If you look in the UnitTests folder of the repo, you'll notice that there's three projects...
 - UnitTests.Notifications.Shared
 - UnitTests.Notifications.NetCore
 - UnitTests.Notifications.UWP
 - UnitTests.Notifications.WinRT

That's because in our source code, we have some #IF defs for switching between the different types of reflection that C# uses, since it's different between a .NET Standard and WinRT code.

Therefore, there are two different code paths, one path for NETFX_CORE, and another for when that isn't present. The two test projects exercise both code paths.
