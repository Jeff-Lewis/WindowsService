# WindowsService
Windows Service for tracking user activity and actions.

# Installation
To create a service, use WorkPulseInstaller.exe.It will create a new folder "WorkPulseTask" where all neccessary files are. To install the service open Command Prompt as Administrator and from C:\Windows\Microsoft.NET\framework\v4.0.30319 directory (or whatever version you have installed), write the following command: InstallUtil.exe pathToFolderWhereInstalledFolderIs\WorkPulseTask\SecondService.exe. 

# Starting service
Create folder in C:\ and name it : "logger". After successfull installation, go to Services (or service.msc) and find "myService". Click on it and press start. 

# Testing functionality
In folder C:\logger you will find files for tracking user activity. To test microphone usage, try to unmute microphone. Microphone should be muted when not in use.
