# FusionTasks
![FusionTasks](https://raw.githubusercontent.com/kekyo/FSharp.Control.FusionTasks/master/Images/FSharp.Control.FusionTasks.128.png)

## Status
* NuGet Package (F# 2.0): [![NuGet FusionTasks (F# 2.0)](https://img.shields.io/nuget/v/FSharp.Control.FusionTasks.FS20.svg?style=flat)](https://www.nuget.org/packages/FSharp.Control.FusionTasks.FS20)
* NuGet Package (F# 3.0): [![NuGet FusionTasks (F# 3.0)](https://img.shields.io/nuget/v/FSharp.Control.FusionTasks.FS30.svg?style=flat)](https://www.nuget.org/packages/FSharp.Control.FusionTasks.FS30)
* NuGet Package (F# 3.1): [![NuGet FusionTasks (F# 3.1)](https://img.shields.io/nuget/v/FSharp.Control.FusionTasks.FS31.svg?style=flat)](https://www.nuget.org/packages/FSharp.Control.FusionTasks.FS31)
* NuGet Package (F# 4.0): [![NuGet FusionTasks (F# 4.0)](https://img.shields.io/nuget/v/FSharp.Control.FusionTasks.FS40.svg?style=flat)](https://www.nuget.org/packages/FSharp.Control.FusionTasks.FS40)
* Continuous integration: [![AppVeyor FusionTasks](https://img.shields.io/appveyor/ci/kekyo/fsharp-control-fusiontasks.svg?style=flat)](https://ci.appveyor.com/project/kekyo/fsharp-control-fusiontasks)

## What is this?
* F# Async computation <--> .NET Task easy seamless interoperability library.
* Sample codes (F# side):

``` fsharp
let AsyncBuilderAsAsyncTest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  use ms = new MemoryStream()
  let computation = async {
	  // FusionTasks directory interpreted System.Threading.Tasks.Task class in F# computation block.
	  do! ms.WriteAsync(data, 0, data.Length)
	}
  do computation |> Async.RunSynchronously
  ms.ToArray() |> should equal data
  
let AsyncBuilderAsAsyncTTest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  let computation = async {
	  use ms = new MemoryStream()
	  do ms.Write(data, 0, data.Length)
	  do ms.Position <- 0L
	  // FusionTasks directory interpreted System.Threading.Tasks.Task<T> class in F# computation block.
	  let! length = ms.ReadAsync(data, 0, data.Length)
	  do length |> should equal data.Length
	  return ms.ToArray()
	}
  let results = computation |> Async.RunSynchronously
  results |> should equal data
```

* Sample codes (C# side):

``` csharp
using System.Threading.Tasks;
using Microsoft.FSharp.Control;

public async Task AsyncAwaitableTest()
{
  // FusionTasks simple usage F#'s Async<unit> direct awaitable.
  await FSharpAsync.Sleep(500);
  Console.WriteLine("F# async computation done.");
}

public static async Task AsyncTAwaitableTest(FSharpAsync<int> asy)
{
  // FusionTasks simple usage F#'s Async<int> direct awaitable.
  var result = await asy;
  Console.WriteLine("F# async computation done: Result=" + result);
}
```

## Features
* Easy interoperability .NET Task <--> F#'s Async.
* F# async computation block now support direct .NET Task handle with let! or do!.
* .NET (C# async-await) now support direct F#'s Async.
* SyncronizationContext capture operation support (F#: Configure method / .NET (C#) AsAsyncContext method)

## Benefits
* Easy interoperability, combination and relation standard .NET OSS packages using Task and F#'s Async.
* F# 2.0, 3.0, 3.1 and 4.0 with .NET 4.0/4.5 include PCL Profile 47/78/259.

## Environments
* .NET Framework 4.0/4.5
* .NET Framework Portable class library (Profile 47/78/259)
* F# 2.0, 3.0, 3.1, 4.0 (NuGet package separated, choose)

## How to use
* Search NuGet package and install "FSharp.Control.FusionTasks.FS??". You must select F# version.
* F# use, autoopen'd namespace "FSharp.Control". "System.Threading.Tasks" is optional.
* C# use, using namespace "System.Threading.Tasks". "Microsoft.FSharp.Control" is optional.

## TODO
* Improvements PCL Profiles.
* Improvements more easier/effective interfaces.

## License
* Copyright (c) 2016 Kouji Matsui
* Under Apache v2

## History
* 0.5.6:
  * Add PCL Profile 78.
  * Fixed minor PCL moniker fragments.
* 0.5.5:
  * Fixed version number.
  * Fixed icon image url.
* 0.5.4:
  * Auto open FSharp.Control.
  * Manage AppVeyor CI.
* 0.5.3: Implement awaiter classes.
* 0.5.2: Add dependency assemblies.
* 0.5.1: NuGet package support.
