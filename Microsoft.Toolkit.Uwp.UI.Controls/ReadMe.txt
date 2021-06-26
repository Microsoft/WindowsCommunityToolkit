Windows Community Toolkit — UWP Controls

Thanks for installing the “Windows Community Toolkit — UI Controls” package!

This is a meta-package made up of various Windows Community Toolkit packages.
It is for your ease and convenience to use any and all controls available!

You also have the option to only use packages you need which can help optimize the
size of your application once you are ready to ship. Visit https://aka.ms/wct/optimize to learn more.

You can find out more about the Windows Community Toolkit at https://aka.ms/windowstoolkit.
Or even try our controls in our sample app at https://aka.ms/windowstoolkitapp.
Docs are available here: https://aka.ms/windowstoolkitdocs.

The Windows Community Toolkit is made possible by our developer community!
Every contribution made to the Toolkit helps everyone, to learn how to contribute visit https://aka.ms/wct/wiki.

----

This package also depends on the "WinUI" library, so you'll need to set "XamlControlsResources" as your Application resources in "App.xaml":

<Application>
    <Application.Resources>
        <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />
    </Application.Resources>
</Application>

If you have other resources, then we recommend you add those to the "XamlControlsResources.MergedDictionaries".
This works with the platform's resource system to allow overrides of the "XamlControlsResources" resources.

<Application
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:Microsoft.UI.Xaml.Controls">
    <Application.Resources>
        <controls:XamlControlsResources>
            <controls:XamlControlsResources.MergedDictionaries>
                <!-- Other app resources here -->
            </controls:XamlControlsResources.MergedDictionaries>
        </controls:XamlControlsResources>
    </Application.Resources>
</Application>

See http://aka.ms/winui for more information about the "WinUI" library.