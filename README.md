# ShellScheduler
Console application for scheduled execution of other commands or applications.

## Usage
You can run ShellScheduler.exe from the command line with the following arguments:
```
ShellScheduler.exe <application path> <execution interval in minutes> <next execution time>
```
Omitting `<next execution time>` will default to now.

Omitting `<execution interval>` will default to executing only once.

## Available Commands
* `set`   - Edit current command, execution interval, and next execution time
* `list`  - Show current command, execution interval, and next execution time
* `once`  - Execute specified command once
* `start` - Start scheduled execution of specified command
* `stop`  - Stop scheduled execution of specified command
* `time`  - Shows current date and time
* `exit`  - Close the shell application

