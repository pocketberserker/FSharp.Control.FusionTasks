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

[<AutoOpen>]
module AsyncExtensions =

  let private asTask(async: Async<'T>, token: CancellationToken option) =
    let tcs = TaskCompletionSource<'T>()
    Async.StartWithContinuations(
      async,
      tcs.SetResult,
      tcs.SetException,
      (fun ce -> tcs.SetException(ce)), // Derived from original OperationCancelledException
      (match token with
        | Some t -> t
        | None -> Async.DefaultCancellationToken))
    tcs.Task

  let private asAsync(task: Task) =
    Async.FromContinuations(
      fun (completed, caught, _) ->
        task.ContinueWith(
            new Func<Task, unit>(fun _ -> completed(())),
            TaskContinuationOptions.OnlyOnRanToCompletion).
          ContinueWith(
            new Action<Task>(fun task -> caught(task.Exception)),
            TaskContinuationOptions.OnlyOnFaulted).
          ContinueWith(
            new Action<Task>(fun task -> caught(task.Exception)), // Derived from original OperationCancelledException
            TaskContinuationOptions.OnlyOnCanceled)
        |> ignore)

  let private asAsyncT(task: Task<'T>) =
    Async.FromContinuations(
      fun (completed, caught, _) ->
        task.ContinueWith(
            new Func<Task<'T>, unit>(fun task -> completed(task.Result)),
            TaskContinuationOptions.OnlyOnRanToCompletion).
          ContinueWith(
            new Action<Task>(fun task -> caught(task.Exception)),
            TaskContinuationOptions.OnlyOnFaulted).
          ContinueWith(
            new Action<Task>(fun task -> caught(task.Exception)), // Derived from original OperationCancelledException
            TaskContinuationOptions.OnlyOnCanceled)
        |> ignore)

  type Async with

    /// <summary>
    /// Seamless conversion from F# Async to .NET Task.
    /// </summary>
    /// <param name="async">F# Async</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>.NET Task</returns>
    static member AsTask(task: Async<unit>, ?token: CancellationToken) = asTask(task, token) :> Task

    /// <summary>
    /// Seamless conversion from F# Async to .NET Task.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="async">F# Async</param>
    /// <param name="token">Cancellation token (optional)</param>
    /// <returns>.NET Task</returns>
    static member AsTask(task: Async<'T>, ?token: CancellationToken) = asTask(task, token)

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <param name="task">.NET Task</param>
    /// <returns>F# Async</returns>
    static member AsAsync(task: Task) = asAsync(task)

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async.
    /// </summary>
    /// <param name="task">.NET Task</param>
    /// <returns>F# Async</returns>
    static member AsAsync(task: Task<'T>) = asAsyncT(task)

  type AsyncBuilder with

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <param name="expr">.NET Task (expression result)</param>
    /// <returns>F# Async</returns>
    member __.Source(expr: Task) = expr |> asAsync

    /// <summary>
    /// Seamless conversion from .NET Task to F# Async in Async workflow.
    /// </summary>
    /// <typeparam name="'T">Computation result type</typeparam> 
    /// <param name="expr">.NET Task (expression result)</param>
    /// <returns>F# Async</returns>
    member __.Source(expr: Task<'T>) = expr |> asAsyncT
