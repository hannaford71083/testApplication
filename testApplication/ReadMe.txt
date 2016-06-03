
**************************    READ ME    **************************

----- Future Direction of Test Harness ----

This will look at 2 main aims:

1) Need to test the thing is working for more than 1 group (2-3 groups), a minimum requirement
  Can a user go through the entire lifecycle of the game:
    a)  Login 
    b)  Join Game
    c)  Start Game
    d)  go through game
    e)  finish game
    f)  ??? - exit or go back to Chat Room

2) High Volume load testing:
  Prequisits for this are:
    a) Ability to add users incrementally
    b) Measure perfomance (round trip time OR memory usage)
    c) Are 2 physical machines required to test server (as CPU overhead of running test harness is significant enough to affect server performance)
    d) end goal, can a user volume cut off point be predicted on the siganlR server resulting in users after a certain point being denied access or queued


Is there a method to log data on AppHarbour?


What is the quickest Route to demonstrating usuage of Running Game?

Testing process:

*DONE*  a) Look into more appropriate round trip time and to capture this data (to display in graph)
*DONE*  b) Implement above
        c) Put test harness on another machine to test 
*DONE*  d) Get graph that hopefully shows time taken increases with volume of simultaneous games ran
*DONE*  e) estimate number from this