
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



Presentation Possible Questions/Stuff to Present :

- Outline What Game can do, what I have been working on (test harness)
  - Problems considering Threadsafety (considering the tradeoff with performance, although this was not measured)


- Live demo 
  - Get current App to work on AppHarbour
  * Check into Git, give it a whirl :)
  - Work on Mobile
  *Scale Canvas
  - Will be smooth (use of velocity)
  *Implement ability to do this (simple as possible, is it possible to use a library, think about possible future use)

- Demonstrate the application can handle peak load
  - outline this in presentation (ability to do further )
  - Basic Game load testing and Peak load precautions
  * Work out what is the max load that can be handled with test harness and hardcode this into SignalR chat
  * We can potentially add the PerformanceCounter (see what deal is with Azure scaling) http://stackoverflow.com/questions/278071/how-to-get-the-cpu-usage-in-c and https://msdn.microsoft.com/en-us/library/system.diagnostics.performancecounter(v=vs.110).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-2
  - Testing Chat room
  * Should be a lot easier to do - task for later

- What is required before can be put live?
  - Need to Develop a way of accessing chat rooms (on different servers) using DB will need to work on this with backend
  - Further load testing? Would like advice on this
  - Needs to be code reviewed
  * refactored, removing a lot of the Dictionaries, or have reference to the GroupList (due to keeping track of signalR Group's th )

  TODO NOW:
  * Work out what is the max load that can be handled with test harness and hardcode this into SignalR chat
    1) What is the peak time measured, work out appropriate measure, i.e. will this suffer with connection density increase

*IN PROG* a) Look into more appropriate round trip time and to capture this data (to display in graph)
        b) Implement above
        c) Put test harness on another machine to test 
        d) Get graph that hopefully shows time taken increases with volume of simultaneous games ran
        e) estimate number from this

    * Can I measure CPU performance ( don't do this for now, best to talk with backend folks about this )
    2) Bulid in mechanism to limit users joining game
    3) What is peak loading look like on a feature, see Jacobs Google analytics (maybe he can log me in on his account)