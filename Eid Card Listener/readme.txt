Modified Version of Belgian Eid Viewer

Job : 

 Eid Card listener, catch eid data from eid reader and save them to JSON.

Usage : 

 1/ Run EidCard Listener\EidSamples\bin\Release\EidCardListenerPKCS11.exe

 2/ Type the saving path of the output JSON Eid Data

 3/ Put the Eid Card to the card reader

 4/ Ctrl+C for Exit
 

Original readme:

sample requirements

In order to build the SDK C# samples, the following software needs to be installed:

1) development environment:
Microsoft Visual Studio 2012 express project files are present in the sdk.
You can download Microsoft Visual Studio 2012 express from:
https://www.microsoft.com/en-us/download/details.aspx?id=34673

2) C# pkcs11 wrapper:
you'll need to build pkcs11net
We included the sources of pkcs11net in this SDK under sdk\wrappers\C# pkcs11wrappers\pkcs11net
(we also included already build Net.Pkcs11.dll's in the "sdk\wrappers\C# pkcs11wrappers\pkcs11net\build" folder, they should match with the current state of the sources)

3) Adjust the link to net.pkcs11 in the 'references' folder in the example Visual studio projects if needed.
(this link should point towards the Net.Pkcs11.dll of your project's architecture)


