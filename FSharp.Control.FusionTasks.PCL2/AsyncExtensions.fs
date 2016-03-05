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

namespace FSharp.Control

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Runtime.CompilerServices

[<AutoOpen>]
module AsyncExtensions =

  let private asTask(async: Async<'T>, token: CancellationToken option) =
    let tcs = TaskCompletionSource<'T>()
    Async.StartWithContinuations(
      async,
      tcs.SetResult,
      tcs.SetException,
      (fun ce -> tcs.SetException(ce)),
      (match token with
        | Some t -> t
        | None -> Async.DefaultCancellationToken))
    tcs.Task

  let private asAsync(task: Task) =
    let tcs = TaskCompletionSource<unit>()
    task.ContinueWith(
        new Func<Task, unit>(fun _ -> tcs.SetResult(())),
        TaskContinuationOptions.OnlyOnRanToCompletion).
      ContinueWith(
        new Action<Task>(fun t -> tcs.SetException(t.Exception)),
        TaskContinuationOptions.OnlyOnFaulted).
      ContinueWith(
        new Action<Task>(fun _ -> tcs.SetCanceled()),
        TaskContinuationOptions.OnlyOnCanceled)
      |> ignore
    tcs.Task |> Async.AwaitTask

  type Async with
    static member AsTask(task: Async<'T>, ?token: CancellationToken) = asTask(task, token)
    static member AsTask(task: Async<unit>, ?token: CancellationToken) = asTask(task, token) :> Task
    static member AwaitTask(task: Task) = asAsync(task)

  type AsyncBuilder with
    member __.Source(computation: Task) =
      computation |> asAsync
    member this.Bind(computation: Task<'T>, binder: 'T -> Async<'U>) =
      this.Bind(computation |> Async.AwaitTask, binder)
