"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" SideScroller.sln /p:Configuration=Release /m
del game.zip
"C:\Program Files\7-Zip\7z.exe" a -tzip "game.zip" "SideScroller/bin/Release/"