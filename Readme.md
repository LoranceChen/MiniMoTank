# MiniMOTank 
MiniMOTank represent *mini multiple online tank game*.  
the responsity is clientside build with Unity3D Engine.The server side build with Scala Actor distribute framework.  

## Dependency
- Unirx and RSG Promise for reactive programming style
- Newtonsoft for json prase

## Done
- [16-11-29] fix `SchedulerOn(Scheduler.MainThread)` not works.

## Notice
- create `MainThreadDispather` GameObject by handle avoid dynamic create on another thread which will throw exception:create GameObject not in main thread.
