echo off
cls

echo **** Creating Directories ****
mkdir C:\\"Program Files"\WiiLAB\WiiLAB
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions\GraphingFunctions
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions\BouncingBallFunctions
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\EG111-H
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\DemoPrograms
mkdir C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\DemoPrograms\DemoPresentation
mkdir C:\\"Program Files"\WiiLAB\WiiLABTest
mkdir C:\\"Documents and Settings"\\"All Users"\\"Start Menu"\Programs\WiiLAB

echo **** Copying nessecary files to your computer. ****
copy .\WiiLAB\* C:\\"Program Files"\WiiLAB\WiiLAB /v /y
copy .\WiiLAB_Matlab\WiimoteFunctions\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions /v /y
copy .\WiiLAB_Matlab\WiimoteFunctions\GraphingFunctions\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions\GraphingFunctions /v /y
copy .\WiiLAB_Matlab\WiimoteFunctions\BouncingBallFunctions\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\WiimoteFunctions\BouncingBallFunctions /v /y
copy .\WiiLAB_Matlab\EG111-H\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\EG111-H /v /y
copy .\WiiLAB_Matlab\DemoPrograms\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\DemoPrograms /v /y
copy .\WiiLAB_Matlab\DemoPrograms\DemoPresentation\* C:\\"Program Files"\WiiLAB\WiiLAB_Matlab\DemoPrograms\DemoPresentation /v /y
copy .\WiiLABTest\* C:\\"Program Files"\WiiLAB\WiiLABTest /v /y
copy .\WiiLABTest\Shortcut\* C:\\"Documents and Settings"\\"All Users"\\"Start Menu"\Programs\WiiLAB /v /y

echo **** Unregister WiiLab.dll ****
C:\windows\Microsoft.NET\Framework\v2.0.50727\regasm C:\\"Program Files"\WiiLAB\WiiLAB\WiiLAB.dll /unregister
echo **** Register WiiLab.dll   ****
C:\windows\Microsoft.NET\Framework\v2.0.50727\regasm C:\\"Program Files"\WiiLAB\WiiLAB\WiiLAB.dll /tlb:WiiLAB.tlb /codebase



echo ****WiiLAB Successfully installed! Press enter to view the ReadMe File.****


pause

echo off
cls

C:\\"Program Files"\WiiLAB\WiiLAB\readme.txt

exit




