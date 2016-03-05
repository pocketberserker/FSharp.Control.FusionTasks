/////////////////////////////////////////////////////////////////////////////////////////////////
//
// FSharp.Control.FusionTasks - Async computation elements fusioning Tasks in F#/C#
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

namespace System.Threading.Tasks

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FSharp.Control

[<Extension>]
[<Sealed>]
[<AbstractClass>]
type TaskExtensions =

  [<Extension>]
  static member AsAsync (task: Task) = task |> Async.AwaitTask

  [<Extension>]
  static member AsAsync (task: Task<'T>) = task |> Async.AwaitTask

  [<Extension>]
  static member AsTask (async: Async<unit>) = async |> Async.AsTask

  [<Extension>]
  static member AsTask (async: Async<unit>, token: CancellationToken) = (async, token) |> Async.AsTask

  [<Extension>]
  static member AsTask (async: Async<'T>) = async |> Async.AsTask

  [<Extension>]
  static member AsTask (async: Async<'T>, token: CancellationToken) = (async, token) |> Async.AsTask

  [<Extension>]
  static member GetAwaiter (async: Async<unit>) =
    let task = async |> Async.AsTask
    task.GetAwaiter()

  [<Extension>]
  static member GetAwaiter (async: Async<'T>) =
    let task = async |> Async.AsTask
    task.GetAwaiter()
