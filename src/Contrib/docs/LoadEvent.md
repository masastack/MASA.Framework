# LoadEvent

## Getting "Event" relationship chain failed

When Event, EventHandler, and the main project are not in the same assembly, there will be a failure to obtain the "Event" relationship chain when publishing Events through EventBus. When we use AddEventBus without a special specified assembly, the assembly under the current domain is used by default. Due to the delayed loading feature of dotnet, the acquisition of the event relationship chain is incomplete. There are the following two solutions :

1. When using AddEventBus, specify the complete set of application assemblies used by the current project by specifying Assemblies

2. Before using AddEventBus, by calling Event directly
Any method or class of the assembly where the EventHandler is located, make sure that the application assembly where it is located has been loaded into the current assembly ( AppDomain.CurrentDomain.GetAssemblies() )
