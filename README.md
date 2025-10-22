# PowerShellAsAService
Easy way to make our PowerShell script run as a service. (GO TO THE WIKI TO GET STARTED!)

Over the recent years there have been some pretty impressive projects, like POSHGUI and Pode/Pode.Web that allows you to run applications and websites built on top of PowerShell.
The latter is the reason that led a colleague and I to sit back and try to find a better way to run PowerShell as a service. This question has been asked a lot, but the answer always points to 
a few specific ways of doing this. NSSM is a popular choice, and one I used up until shifting everything to PS 7/Core, where I ran into some issues with it. There are a few PS scripts
online that help in this way or paid for software, but we had never found a good, straightforward free solution. So we made one! Essentially its just a C# wrapper/pointer. It allows you to create a service and point
to the .exe that has information about your PS script in it. All you need is a copy of VSCode and the 2 files in the PSAAS.zip folder to get started. Total time to complete: 10-15 min.



