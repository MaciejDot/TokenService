# TokenService

## Introduction

Example of simplest async setup of token service with asymmetric 16384 key.

## Installation

first install this package globally:
```PowerShell
dotnet tool install --global dotnet-ef
```
Then in package manager

```PowerShell
$env:SQLCONNSTR_TokenService:"Server=(localdb)\\mssqllocaldb;Database=TokenService;Trusted_Connection=True;";
dotnet ef database update
```