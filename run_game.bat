csc /t:library /out:HaliteHelper.dll World.cs hlt\Vector.cs hlt\Collision.cs hlt\Constants.cs hlt\DebugLog.cs hlt\DockMove.cs hlt\Entity.cs hlt\GameMap.cs hlt\Metadata.cs hlt\MetadataParser.cs hlt\Move.cs hlt\Navigation.cs hlt\Networking.cs hlt\Planet.cs hlt\Player.cs hlt\Position.cs hlt\Ship.cs hlt\ThrustMove.cs hlt\UndockMove.cs hlt\Util.cs

csc /reference:HaliteHelper.dll -out:MyBot.exe MyBot.cs 

csc /reference:HaliteHelper.dll -out:OpponentBot.exe MyBot.cs 

.\halite -d "240 160" "MyBot.exe" "OpponentBot.exe Enemy"

pause>nul