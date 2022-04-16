# RoboForge

RoboForge is a multiplayer, PvE, battle arena style game; where multiple people fight off waves of enemies and unlock new weapons/upgrades along the way.
Main focus of the game is to bring people together and reward team play

# Project description
This project was migrated from Plastic SCM due to it being unreliable, once all the problems with Plastic get resolved, it is possible that the project will be moved back to it

## Details

Project is built ontop of TopDown engine asset and uses Photon Fusion for networking, In the future, Playfab may also be added to the stack to store player data and manage accounts

## Git LFS

Since unity projects can be very large, I have to use Git LFS (large file storage) to store assets

## Plugins used

- TopDown Engine - a backbone of the project, used almost everywhere to implement complex systems such as Weapon, AI, Character controll ext.
- Odin inspector - mostly used for making custom editors/inspectors and to some extent for serialization
- Riderflow - productivity asset for scene building and management
- Github for unity - Github integration for unity, includes git LFS and adds file locking  
- Rainbow folders - colored folders in project menu, yeah that's it
- Multiple assets for art and music
- Several Unity plugins, the list is too long to put here, check package manifest or package manager in unity

