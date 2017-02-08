 # Welcome to DRS Race Manager Project.
 The goal of this project is to have a fully functioning Race Manager System for LFS in a local installation.
 Key Functions of the program
 Enable local logging of selected Race Data into a local MySQL Database
 - Nickname (not username)
 - Best Lap Time for all Cars And Tracks. (84 Track configs multiplied by 20 cars 1680 fields pr. driver)
 - Finish statistics. (Number of 1, 2, 3, 4, 5, 6, 7 and 8 places) +pts
 - Number of KM driven.

Insim Display on Game Screen Properties.
 - InSim GUI
 - Driven distance
 - Points
 - Nickname
 - Additional functionality.
 - Create a c# Display clientprogram, that pulls data from the SQL base and displays on screens.
 - Create a Web plugin, that pulls data from SQL base and displays on web page.
 - Create a Facebook app that shows data from SQL base
 - Create a IOS app for Drammen Racing Senter that connects to SQL
 - Create a Android app for Drammen Racing Senter that connects to SQL

To-do-list:
 - Use seperate tables for fastest times on different tracks (instead of one big file)
 - Have a few hours of work on the !ap admin command
 - include wind, kick, ban (days), servermessages, motd and such in !ap command
 - Improve LAP.LapTime string format to 00:00:00
 - Improve MySQL database functions
 - !info
 - Show a table of users included with points to their name after a race is finished
