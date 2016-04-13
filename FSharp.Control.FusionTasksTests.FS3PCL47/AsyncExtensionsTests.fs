/////////////////////////////////////////////////////////////////////////////////////////////////
//
// FSharp.Control.FusionTasks - F# Async computation <--> .NET Task easy seamless interoperability library.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

module FSharp.Control.FusionTasksTests.AsyncExtensions

open System
open System.IO
open System.Diagnostics
open System.Threading.Tasks

open NUnit.Framework
open FsUnit

[<Test>]
let AsyncBuilderAsAsyncTest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  use ms = new MemoryStream()
  let computation = async {
      do! ms.WriteAsync(data, 0, data.Length)
    }
  do computation |> Async.RunSynchronously  // FSUnit not supported Async/Task based tests, so run synchronously here. 
  ms.ToArray() |> should equal data
  
[<Test>]
let AsyncBuilderAsAsyncTTest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  let computation = async {
      use ms = new MemoryStream()
      do ms.Write(data, 0, data.Length)
      do ms.Position <- 0L
      let! length = ms.ReadAsync(data, 0, data.Length)
      do length |> should equal data.Length
      return ms.ToArray()
    }
  let results = computation |> Async.RunSynchronously  // FSUnit not supported Async/Task based tests, so run synchronously here. 
  results |> should equal data
  
[<Test>]
let AsyncBuilderAsAsyncCTATest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  use ms = new MemoryStream()
  let computation = async {
      do! ms.WriteAsync(data, 0, data.Length).AsyncConfigure(false)
    }
  do computation |> Async.RunSynchronously  // FSUnit not supported Async/Task based tests, so run synchronously here. 
  ms.ToArray() |> should equal data
    
[<Test>]
let AsyncBuilderAsAsyncCTATTest() =
  let r = Random()
  let data = Seq.init 100000 (fun i -> 0uy) |> Seq.toArray
  do r.NextBytes data
  let computation = async {
      use ms = new MemoryStream()
      do ms.Write(data, 0, data.Length)
      do ms.Position <- 0L
      let! length = ms.ReadAsync(data, 0, data.Length).AsyncConfigure(false)
      do length |> should equal data.Length
      return ms.ToArray()
    }
  let results = computation |> Async.RunSynchronously  // FSUnit not supported Async/Task based tests, so run synchronously here. 
  results |> should equal data
