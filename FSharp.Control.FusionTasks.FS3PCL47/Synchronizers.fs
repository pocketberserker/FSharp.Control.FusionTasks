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

namespace Microsoft.FSharp.Control

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

///////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Pseudo lock primitive on F#'s async workflow/.NET Task.
/// </summary>
[<Sealed>]
[<NoEquality>]
[<NoComparison>]
[<AutoSerializable(false)>]
type AsyncLock () =

  let _queue = new Queue<unit -> unit>()
  let mutable _enter = false

  let locker continuation =
    let result =
      Monitor.Enter _queue
      try
        match _enter with
        | true ->
          _queue.Enqueue(continuation)
          false
        | false ->
          _enter <- true
          true
      finally
        Monitor.Exit _queue
    match result with
    | true -> continuation()
    | false -> ()

  let unlocker () =
    let result =
      Monitor.Enter _queue
      try
        match _queue.Count with
        | 0 ->
          _enter <- false
          None
        | _ ->
          Some (_queue.Dequeue())
      finally
        Monitor.Exit _queue
    match result with
    | Some continuation -> continuation()
    | None -> ()

  let disposable = {
    new IDisposable with
      member __.Dispose() = unlocker()
  }

  member internal __.asyncLock(token: CancellationToken) =
    let acs = new AsyncCompletionSource<IDisposable>()
    token.Register (fun _ -> acs.SetCanceled(token)) |> ignore
    locker (fun _ -> acs.SetResult disposable)
    acs.Async

  member internal __.asyncLock() =
    let acs = new AsyncCompletionSource<IDisposable>()
    locker (fun _ -> acs.SetResult disposable)
    acs.Async

  member internal __.lockAsync(token: CancellationToken) =
    let tcs = new TaskCompletionSource<IDisposable>()
    // TODO: Cannot derived token through tcs's SetCanceled.
    token.Register (fun _ -> tcs.SetCanceled()) |> ignore
    locker (fun _ -> tcs.SetResult disposable)
    tcs.Task

  member internal __.lockAsync() =
    let tcs = new TaskCompletionSource<IDisposable>()
    locker (fun _ -> tcs.SetResult disposable)
    tcs.Task

///////////////////////////////////////////////////////////////////////////////////

namespace System

open System.Threading.Tasks
open Microsoft.FSharp.Control

/// Hold asynchronos factory fuction.
type internal AsyncLazyFactory<'T> =
  | AsyncFactory of (unit -> Async<'T>)
  | TaskFactory of (unit -> Task<'T>)

/// <summary>
/// Asynchronos lazy instance generator.
/// </summary>
/// <typeparam name="'T">Computation result type</typeparam> 
[<Sealed>]
[<NoEquality>]
[<NoComparison>]
[<AutoSerializable(false)>]
type AsyncLazy<'T> =

  val internal _lock : AsyncLock
  val internal _asyncBody : AsyncLazyFactory<'T>
  val mutable internal _value : 'T option

  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="asyncBody">Lazy instance factory.</param>
  /// </summary>
  new (asyncBody: unit -> Async<'T>) = {
    _lock = new AsyncLock()
    _asyncBody = AsyncFactory asyncBody
    _value = None
  }
  
  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="asyncBody">Lazy instance factory.</param>
  /// </summary>
  new (asyncBody: unit -> Task<'T>) = {
    _lock = new AsyncLock()
    _asyncBody = TaskFactory asyncBody
    _value = None
  }

  member internal this.asyncGetValue() = async {
    use! al = this._lock.asyncLock()
    match this._value with
    | Some value -> return value
    | None ->
    let! value =
      match this._asyncBody with
      | AsyncFactory asyncBody -> asyncBody()
      | TaskFactory taskBody -> Infrastructures.asAsyncT(taskBody(), None)
    this._value <- Some value
    return value
  }

  // TODO:
//  member internal this.getValueAsync() =
//    let lockTask = this._lock.lockAsync()
//    lockTask.ContinueWith()
