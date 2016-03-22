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

#if PCL7
open Microsoft.Runtime.CompilerServices
#else
#if PCL47
open Microsoft.Runtime.CompilerServices
#else
open System.Runtime.CompilerServices
#endif
#endif

/// <summary>
/// Seamless conversion extensions in standard .NET Task based infrastructure.
/// </summary>
[<Extension>]
[<Sealed>]
[<AbstractClass>]
type TaskExtensions =

  ///////////////////////////////////////////////////////////////////////////////////
  // .NET (C#) side Task --> Async conversion extensions.

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <param name="task">.NET Task</param>
  /// <returns>F# Async (FSharpAsync&lt;Unit&gt;)</returns>
  [<Extension>]
  static member AsAsync (task: Task) =
    AsyncExtensions.asAsync (task, None)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <param name="task">.NET Task</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>F# Async (FSharpAsync&lt;Unit&gt;)</returns>
  [<Extension>]
  static member AsAsync (task: Task, token: CancellationToken) =
    AsyncExtensions.asAsync (task, Some token)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="task">.NET Task&lt;'T&gt;</param>
  /// <returns>F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</returns>
  [<Extension>]
  static member AsAsync (task: Task<'T>) =
    AsyncExtensions.asAsyncT (task, None)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="task">.NET Task&lt;'T&gt;</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</returns>
  [<Extension>]
  static member AsAsync (task: Task<'T>, token: CancellationToken) =
    AsyncExtensions.asAsyncT (task, Some token)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <param name="cta">.NET ConfiguredTaskAwaitable</param>
  /// <returns>F# Async (FSharpAsync&lt;Unit&gt;)</returns>
  [<Extension>]
  static member AsAsync (cta: ConfiguredTaskAwaitable) =
    AsyncExtensions.asAsyncCTA(cta)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="cta">.NET ConfiguredTaskAwaitable&lt;'T&gt;</param>
  /// <returns>F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</returns>
  [<Extension>]
  static member AsAsync (cta: ConfiguredTaskAwaitable<'T>) =
    AsyncExtensions.asAsyncCTAT(cta)

  /// <summary>
  /// Seamless conversion from .NET Task to F# Async.
  /// </summary>
  /// <param name="ya">.NET YieldAwaitable</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</returns>
  [<Extension>]
  static member AsAsync (ya: YieldAwaitable) =
    AsyncExtensions.asAsyncYA(ya)

  ///////////////////////////////////////////////////////////////////////////////////
  // .NET (C#) side Async --> Task conversion extensions.

  /// <summary>
  /// Seamless conversion from F# Async to .NET Task.
  /// </summary>
  /// <param name="async">F# Async (FSharpAsync&lt;Unit&gt;)</param>
  /// <returns>.NET Task</returns>
  [<Extension>]
  static member AsTask (async: Async<unit>) =
    AsyncExtensions.asTask (async, None) :> Task

  /// <summary>
  /// Seamless conversion from F# Async to .NET Task.
  /// </summary>
  /// <param name="async">F# Async (FSharpAsync&lt;Unit&gt;)</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>.NET Task</returns>
  [<Extension>]
  static member AsTask (async: Async<unit>, token: CancellationToken) =
    AsyncExtensions.asTask (async, Some token) :> Task

  /// <summary>
  /// Seamless conversion from F# Async to .NET Task.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="async">F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</param>
  /// <returns>.NET Task&lt;'T&gt;</returns>
  [<Extension>]
  static member AsTask (async: Async<'T>) =
    AsyncExtensions.asTask (async, None)

  /// <summary>
  /// Seamless conversion from F# Async to .NET Task.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="async">F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</param>
  /// <param name="token">Cancellation token</param>
  /// <returns>.NET Task&lt;'T&gt;</returns>
  [<Extension>]
  static member AsTask (async: Async<'T>, token: CancellationToken) =
    AsyncExtensions.asTask (async, Some token)

  ///////////////////////////////////////////////////////////////////////////////////
  // .NET (C#) side Async awaitabler extensions.

  /// <summary>
  /// Seamless awaiter support for F# Async.
  /// </summary>
  /// <param name="async">F# Async (FSharpAsync&lt;Unit&gt;)</param>
  /// <returns>.NET TaskAwaiter</returns>
  [<Extension>]
  static member GetAwaiter (async: Async<unit>) =
    let task = AsyncExtensions.asTask (async, None) :> Task
    task.GetAwaiter()

  /// <summary>
  /// Seamless awaiter support for F# Async.
  /// </summary>
  /// <typeparam name="'T">Computation result type</typeparam> 
  /// <param name="async">F# Async&lt;'T&gt; (FSharpAsync&lt;'T&gt;)</param>
  /// <returns>.NET TaskAwaiter&lt;'T&gt;</returns>
  [<Extension>]
  static member GetAwaiter (async: Async<'T>) =
    let task = AsyncExtensions.asTask (async, None)
    task.GetAwaiter()
