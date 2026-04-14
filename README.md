# ConsoleApp2  


A simple .NET Console Application built with the new **.slnx** solution format.

## Overview

This is a basic C# console app. It demonstrates a modern .NET project using the clean XML-based `.slnx` solution file (introduced in .NET 9 / Visual Studio 2022 17.14+).

## Project Structure
├── ConsoleApp2.slnx          # New XML solution file
├── ConsoleApp2/              # Main project folder
│   ├── ConsoleApp2.csproj
│   ├── Program.cs
│   └── ...
├── .github/
│   └── workflows/
│       └── dotnet.yml        # GitHub Actions CI pipeline
└── README.md                                                            

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later (required for full `.slnx` support)

## How to Run

### From the command line

```bash
# Restore, build and run
dotnet run --project ConsoleApp2/ConsoleApp2.csproj

# Or build via the solution file
dotnet build ConsoleApp2.slnx    
dotnet run --project ConsoleApp2/ConsoleApp2.csproj --configuration Release
<img width="1911" height="1078" alt="workflow" src="https://github.com/user-attachments/assets/d46fa636-7790-4eb5-8233-bba1ffc019bd" />

