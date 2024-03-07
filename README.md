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
RBMAI.Posture
 - increased maxPosture 100f to 300f, regenPerTick 0.01f to 0.05f.
 - remove hp loss from a posture break 'calculateHealthDamage' = 0;
RBMConfig.Utilities
 - lowerArmorQuality() - defaultProbabilty from 0.05f to 0.5f
 (note, refernce rbmconfig like: RBM.Config.RBM.Config.[function])
RBMCombat.DamageRework
 - Javelin damage to shields - 25f to 20f
 - Arrow damage to shields 1.5f to 2f
 - bolt damage to shields 1.5 to 2.25f

Bodyguards by carbon198
 BodyguardsMCM
 - raised min/max slider from 0-25% to 0-100%, no more additional troops to create bodyguards.

Bannerlord2 > Modules > Native > ModuleData 
* The issue where ranged units not shooting even when units infront are crouched is due to the collision box size being almost the same for crouching and standing.

* The collision box can be edited in a file named monster.xml


monsters.xml
 - reduced collision(capsule size?) when crouched from 0.6 to 0.0
 
Edit:
		<Capsules>
			<body_capsule
				radius="0.37"
				pos1="0.0, 0.0, 1.55"
				pos2="0.0, 0, 0.8" />
			<crouched_body_capsule
				radius="0.37"
				pos1="0.0, 0.0, 1.55"
				pos2="0.0, 0, 0.0" />
		</Capsules>



