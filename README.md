# QuarmTool
<h3>This program works by reading your Log File, and reading permitted (allowed by the Project Quarm server) data sent by Zeal.</h3>
 
Instructions:
<ul>
<li>
<h2>Project Quarm: Download <a href="https://github.com/EJWellman/QuarmTool/releases/latest/download/QuarmTool.zip">QuarmTool.zip</a>, Unzip it and run QuarmTool.exe</h2>
</li>
<li>The program runs in the system tray. Look there to reopen spells window or settings!</li>
<li><s>Program will check for updates on startup and self update if required, but if you want to check for a new Update, use the menu in the system tray!</s> This feature isn't available yet, but will be in the future!</li>
</ul>
Features:
<br/>
<ul>
 <li>The TOP 5 Random rolls show up in the triggers window. </li> 
 <li>See others on the map in real time. Also support for NParse map location sharing automatically if location sharing is set to everyone. </li> 
<li>Automatically add timers on death of npc so you dont have to!</li> 
<li>Detect EQ directory location instead of user required to enter it.</li> 
<li>Detect Spells cast on others (this is a best guess as I am reading the log file so chloroplast and Regrowth of the growth have the same message)</li>
<li>Filter spells show by class</li> 
<li>Remove Spells from List if "Worn off message occurs"</li> 
<li>Mob Info Window gives details about mobs that you con in game.</li>
<li>Automatically remove dead npc/player from the spell list.</li> 
<li>Auto detect level and class!</li>
<li>DPS is trailing 12 second average.</li>
<li>Show fight Session data for comparisons.</li>
<li>The following spells have counters that are automatically tracked: <br/>Mana Seive<br/> LowerElement (Flux Staff Effect)<br/> Concussion<br/> Flame Lick<br/> Cinder Jolt<br/> Jolt.</li> 
<li>The following Audio/Visual alerts are available: <br/>Enrage<br/> Levitate fading<br/> invis fading<br/> FTE<br/> Charm Break<br/> Failed Feign<br/> Group Invite<br/> Dragon Roar<br/> Root Break<br/> Resists<br/> and <b>CH CHain</b></li>
<li>Timers on map for easy TOD tracking!</li>
<li>All Melee disciplines show in the cooldown section of the timers window.</li>
<li>Pricing information in Mob info window.</li>
<li>Save spells cast on you when you log out so when you log back in they resume counter!</li>
<li>Maps</li>

<li>Timers (Only Minutes are supported)</li>
</ul>
<h5>Ch format is loosely: TAG POSITION CH TARGET</h5>
<ul> 
 <li>
  TAG is optional and can be used to filter out other chains that are going on. For example, if you TAG is CA, it will only show chains that have that TAG in it.
 </li>
 <li>
  POSITION must be 3 in length (Unless Ramp chain, in which case code looks for RAMP[NUMBER]), and follow a format of: 001-999; or AAA-ZZZ
 </li>
 <li>The characters " ch " must be present in the line, capitalization doesnt matter.</li>
 <li>All instances of TAG are stripped out of text. The characters " ch " are stripped out. The first group of 3 characters that are either all the same letter or a number are assigned as the position. </li>
 <li>All instances of Position are stripped out of the text. All non alphanumeric are stripped out of the text. What remains is the target.</li>
</ul>
<p>Chain position MUST BE 3 in length and letters must all be the same, for example: 001; or 013; or aaa; or QQQ. <br/> Ramp chain format is special and RAMP1 through RAMP999 are allowed. The following are included in tests that are supported, This is not all the possible ways, but to show examples of what i have support for!</p></p>

<ul>
<li>Curaja shouts, 'GG 014 CH -- Wreckognize'</li>
<li>Hanbox shouts, 'GG 001 CH -- Beefwich'</li>
<li>Hanbox shouts, 'GG 001 CH --Beefwich'</li>
 <li>Hanbox shouts, 'GG 001 CH --Beefwich 001'</li>
<li>Wartburg says out of character, 'CA 004 CH -- Sam'</li>
 <li>Wartburg says out of character, '004 CH - Sam'</li>
  <li>Hanbox tells the guild, 'GG 001 CH --Beefwich'</li>
  <li>You say out of character, 'CA 002 CH -- Aaryk'</li>
  <li>Windarie auctions, '111 --- CH << Mandair  >> --- 111'</li>
  <li>Mutao auctions, '777 CH <>> Mandair <<> 777'</li>
  <li>Mutao auctions, 'AAA CH <>> Mandair <<> AAA'</li>
  <li>Mutao auctions, 'GGG AAA CH <>> Mandair <<> AAA'</li>
  <li>Mutao auctions, 'BBB CH <>> Mandair <<> BB'</li>
  <li>Mutao auctions, 'AAA CH <>> Mandair <<>'</li>
  <li>Hanbox shouts, 'CA RAMP1 CH --Beefwich'</li>
  <li>Hanbox shouts, 'RAMP2 CH --Beefwich'</li>
  <li>Hanbox shouts, 'CH - name - 001'</li> 
</ul>
<h5>Timers (Only Minutes are supported) -- All below commands work in regular say!</h5>
<ul>
<li>Timer Start Crypt Camp 35</li>
<li>Start Timer Crypt Camp 35</li>
<li>Timer Cancel Crypt Camp</li>
<li>Cancel Timer Crypt Camp</li>
</ul>

<h4>TO DO List</h4>
<ul>     
 <li>Add option to ONLY show spells YOU have cast on yourself and others. </li>   
 <li>Map center on you.</li>  
 <li>Add Window Snapping code so windows can be grouped and all moved together.</li>   
 <li>Remove timers from map when timers removed from list and visa versa.</li>
 <li>Fix buff timers when updating, when update occurs buffs dissapear.</li> 
</ul>

<h3>FAQS</h3>
<h4>Why does chrome warn me?</h4> 
<img alt="image" src="https://user-images.githubusercontent.com/3393733/223326270-a079946d-57dc-41dd-a58e-f46a0c776b54.png">
<ul>
<li>TL;DR: Ignore it and download!
But really, it's just that it's a 'new' thing, and it's untrusted for that reason.</li> 
</ul>
<h4>Why does windows pop up a blue screen?</h4> 
<img height="300" alt="image" src="https://user-images.githubusercontent.com/3393733/223328194-0946d278-09dc-4504-bed8-172d63fa98e0.png">
<ul>
<li>Press More info and "Run anyway"</li> 
</ul>
<h4>Why does the program not start?</h4> 
<img height="300" alt="image" src="https://user-images.githubusercontent.com/3393733/223326377-7cab3be5-bee5-4029-b513-0e8b2ff0bb78.png">
<ul>
<li>In the above image, you are running the exe from INSIDE the zip. You must first EXTRACT the exe, then you can run it!</li> 
</ul>
<h4>Why does my settings window say Configuration missing?</h4> 
<img alt="image" src="https://user-images.githubusercontent.com/3393733/222051822-fc4b750d-2efa-4eb9-bc00-589d3cc5b781.png">
<ul>
<li>QuarmTool was unable to automatically detect your PQ/Takp install folder. You must specify it yourself!</li>
<li>QuarmTool detected that eq logging is turned off. You must click enable logging. This will turn on EQ's logging which is where QuarmTool gets information from.</li>
</ul>

<h4>Do I have to set my class and level?</h4> 
<ul>
<li>If you cast spells QuarmTool will automatically detect your class and level once you start casting spells.</li>
<li>You should still enter your class and level. It helps ensure calculations on spell durations are (more) accurate.</li>
</ul>

<h4>I only care about spells cast on me, not everyone else!</h4> 
<ul>
<li>Great, goto settings and make sure the box is checked; 'Only show spells that effect you'.</li> 
</ul>

<h4>I only want to see cleric buffs; there are too many buffs to see!</h4> 
<ul>
<li>Great, goto settings and make sure that cleric is the only class selected in the "Other Spells" section.</li> 
</ul>

<h4>I have everything working, but i dont see my location on the map, why?</h4> 
<ul>
<li>You need to type /loc into chat so that your location is feed to the log file.</li> 
<li>Normally, players create a hotkey that is bound to their movement keys. Then add a /loc so that each time you move, the macro for /loc is called.</li> 
</ul>

<h4>How do i get the latest update?</h4> 
<ul>
<li>Come back here, and check for a new release. Auto-updates are a feature I'm looking into.</li> 
</ul>

<h4>Why do the spell effect still show for dead npcs?</h4> 
<ul>
<li>This program reads your log file. So, if you were too far away to see the 'slain' message, then there is no way for QuarmTool to know the npc is dead.</li> 
</ul>

<h4>Why do the see spell effects for others even though I checked the box 'Only show spells that effect you?'</h4> 
<ul>
<li>QuarmTool will ALWAYS show detrimental spell effects on everyone. The reason is that QuarmTool CANNOT tell the difference between an NPC and a PC. This means that in order to show spell effects on NPC, like slow/tash, etc, I must show ALL detrimental spell effects on all NPS/PC alike.</li> 
</ul>

<h4>How do I remove an NPC/Player from the spell list?</h4> 
<img alt="image" src="https://user-images.githubusercontent.com/3393733/222474771-41cc3276-9a9e-4a30-b868-5fb4d0b87de4.png">
<ul>
<li>This can happen for many reasons. The most common being that they left the group and you want to remove them from the list.</li>
<li>Click the Trashcan next to the next and that NPC/Player will be removed from the list. This is a one-time action and if you cast on them again, they will reappear.</li> 
</ul>

<h4>Where and how are sieves tracked?</h4>  
<img alt="image" src="https://user-images.githubusercontent.com/3393733/230679391-92754265-ec66-4643-ad30-5b6e4cdd164b.png">
<ul>
<li>Every time a message for the enchanter Mana sieve spell is encountered, the counter is incremented.</li>
<li>So, if you are in range of the sieve messages, you will have an accurate count in the Trigger list.</li> 
</ul>

<h4>What is this DPS session tracking?</h4>  
<img alt="image" src="https://user-images.githubusercontent.com/3393733/230679632-732b2d60-e471-4508-bcfe-720f37ae2c35.png">
<ul>
<li>The first row is the saved fight data which shows all-time-best DPS, Biggest hit and Total Damage.</li>
<li>Current Session is best DPS, Biggest hit and Total Damage, except the timeframe starts since you have logged on.</li>
<li>Last Session is the old session data. You can use this to compare weapon swapping and differnt setups.</li>
</ul>

<h4>Why cant I see others DPS? I only see my own!</h4>  
<img alt="image" src="https://user-images.githubusercontent.com/3393733/232231377-1259ae63-f644-4e49-a246-8f1101f71190.png">
<ul>
<li>Turn on others hits in your eqsettings!</li>
</ul>

<h4>Map timers, how do they work?</h4>  
<ul>
<li>Right click in the map and a menu will appear, add the time, and press add.</li>
<li>You can move the timer around by dragging it.</li>
<li>You can delete the timer by right clicking over it and  clicking the delete option.</li>
<li>Timer color starts at Green and changes to Red the closer it gets to zero time remaining. Timers will last for 4 minutes after the time has expired then automatically remove themselves.</li>
</ul>
