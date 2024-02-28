This is a collection of edited mods from mod authors to suite my personal tastes.
see guide "How to manually create the dll for new or old versions of Bannerlord"
https://www.nexusmods.com/mountandblade2bannerlord/articles/40

Copied the .dll mod using dnSpy.
--> using dnSpy > open/include all of orignal game files (Taleworld.dll)'s then open the mods .dll
--> edit then compile to make changes to the mod dll.

for example,
use dnspy, open all Taleworld dll's and RBM dll's
Open RBMAI, find 'PostureLogic' edit C# class - calculateHealthDamage return 0;
compile & save.
(note: not sure why 'RBMConfig.xxx' is not working, so had to remove or edit each of them with a value/bool.)


game version 1.29

## list of Mods:

Battle Stats v1.2.7 by jzeno9 
https://www.nexusmods.com/mountandblade2bannerlord/mods/2572
-> a mod by jzeno9 which records battle stats such as Kills, total Battles, Casualities and more.
add a 'death' counter
- separate Casualities into Wounded and Dead.

Realistic Battle Mod v3.8.0 by Marnah93 and Philozoraptor
https://www.nexusmods.com/mountandblade2bannerlord/mods/791

The damage taken from a posture break makes duels without a shield very hard and unfair. 
- remove hp loss from a posture break 'calculateHealthDamage' = 0;