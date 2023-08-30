# FolderSync
 C# Console application for one-way folder sync.

## Arguments
The application takes 4 arguments, 2 mandatory (**From**, **To**) and 2 optional (**SyncIntervalSeconds**, **LogFile** ).
- **From** - path of source folder
- **To** - path of target folder
- **SyncIntervalSeconds** - the time interval between two consecutive synchronizations. If the argument is not provided or its value is less than 1,  the synchronization is one-time without repetition.
- **LogFile** - a path to a file where a text log will be saved. Note that if the app is terminated forcefully, part of the logs might be lost.

## Syntax
There are 4 allowed ways (orders) to pass arguments:
- **From** **To**
- **From** **To** **SyncIntervalSeconds**
- **From** **To** **LogFile**
- **From** **To** **SyncIntervalSeconds** **LogFile**

## Author
Created by Petr Sedláček in 2023.