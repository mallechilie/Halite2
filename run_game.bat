csc /warn:0 /nologo -out:Run\MyBot.exe hlt\*.cs *.cs
csc /warn:0 /nologo -out:Run\OpponentBot.exe hlt\*.cs *.cs
.\halite -d "240 160" "Run\MyBot.exe" "Run\OpponentBot.exe Enemy"
pause