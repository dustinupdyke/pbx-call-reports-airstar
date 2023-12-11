# pbx-call-reports

## Installation

Binary will run on Windows or Linux.

1. We will need to install the Microsoft dotnetcore runtime
2.2 or newer from here: https://dotnet.microsoft.com/download 
— note that we only need to install the runtime, not the 
full SDK.

2. Unzip the application to a folder of our choice.

3. If we need to change configuration, it is located in 
./config/appsettings.json — open this file in a text editor.

```
"Api": {
    "Url": "https://pbx.skyswitch.com/ns-api/", # the api endpoint
    "ClientId": "20905.service",                # the accoy clientid 
    "ClientSecret": "e87...37e9e6bf",           # accoy client secret
    "Username": "API@",                         # accoy username
    "Password": "7Q...m1"                       # accoy password
  },
  "Application": {
    "IsDebug": false,                           # this is used for testing only
    "ReportOnCallers": [                        # this array contains all of the reported on callers
      {
        "Name": "AMANDA HALL",
        "Extensions": ["846"]
      }                            
      ...
    ],
    "OutputDirectory": "reports/",              # relative file path for output .xlsx files
    "OutputFileName": "snapshot.xlsx"           # name for the output file
  }
```

4. Running the application is done via the command line (cmd) 
on windows or the terminal in linux.

`dotnet pbx-call-reports`

There is a provided run.bat for Windows and run.sh for linux as well. 

To schedule the application use Windows Task Scheduler or Cron on Linux. 