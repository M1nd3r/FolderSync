# FolderSync
 C# Console application for one-way folder sync.

## Arguments
The application takes 4 arguments, 2 mandatory (**From**, **To**) and 2 optional (**SyncIntervalSeconds**, **LogFile** ).
- **From** - path of source folder
- **To** - path of target folder
- **SyncIntervalSeconds** - time interval between two consecutive synchronizations. Value less then 1 will result in a one-time synchronization without repetition.
- **LogFile** - a path to a file where text log will be saved. If log file already exists, new log will be appended. If the file does not exist, app will try to create it. Note that if app is terminated forcefully, parts of log from session might be lost.

## Syntax
There are 4 allowed ways (orders) to pass arguments:
- **From** **To**
- **From** **To** **SyncIntervalSeconds**
- **From** **To** **LogFile**
- **From** **To** **SyncIntervalSeconds** **LogFile**

## Author
Created by Petr Sedláček in 2023.