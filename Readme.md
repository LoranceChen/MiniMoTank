# MiniMOTank 
MiniMOTank represent *mini multiple online tank game*.  
the responsity is clientside build with Unity3D Engine.The server side build with Scala Actor distribute framework.  

## Dependency
- Unirx and RSG Promise for reactive programming style
- Newtonsoft for json prase

## Some Bug Notice
- `SchedulerOn(Scheduler.MainThread)` not works. Replaced by `SchedulerOn(Scheduler.MainThreadEndOfFrame)
`
