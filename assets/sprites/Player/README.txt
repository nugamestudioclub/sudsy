Animation Setup
Stars around frames indicate a loop
Ex:
*1 2 3 4* is a loop going between 1 2 3 and 4

IDLE Loop: *1 2 3 4*
IDLE -> Slide: 5, 6, 7, *8, 9, 10, 11*
IDLE -> Jump: 5, 6, 7, 12, *13, 14, 15, 16*
Jump -> IDLE: 12, 7, 6, 5, *1 2 3 4*   // Occurs when hitting the ground
Slide -> IDLE: 7, 6, 5, *1 2 3 4*


Frames 1-4 : IDLE/MOVING
Frames 5-7 : Between frames IDLE <-> SLIDING and IDLE <-> JUMP
Frames 8-11 : SLIDING
Frame 12: Plays after frame 7 for the jump animation
Frame 13-16: JUMP