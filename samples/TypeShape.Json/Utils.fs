﻿[<AutoOpen>]
module internal Vardusia.Utils

open System
open System.Text
open System.Globalization

let inline append sb t =
    let _ = (^StringBuilder : (member Append : ^t -> ^StringBuilder) (sb, t))
    ()

let inline appendEscaped (sb : StringBuilder) (str : string) =
    let n = str.Length - 1
    append sb '"'
    for i = 0 to n do
        match str.[i] with
        | '\n' -> append sb "\\n"
        | '\r' -> append sb "\\r"
        | '\"' -> append sb "\\\""
        | s -> append sb s
    append sb '"'

let inline getOrInit (ref : byref< ^t>) =
    match ref with
    | null -> ref <- new ^t() ; ref
    | _ -> (^t : (member Clear : unit -> unit) ref) ; ref

let inline format fmt (input : ^t) =
    (^t : (member ToString : IFormatProvider -> string) (input, fmt))

let inline parse fmt (input : string) =
    (^t : (static member Parse : string * IFormatProvider -> ^t) (input, fmt))

let inline tryParseNumber fmt (result : byref<_>) (input : string) =
    (^t : (static member TryParse : string * NumberStyles * IFormatProvider * byref< ^t> -> bool) 
                                (input, NumberStyles.Any, fmt, &result))

module Array =
    let inline mapFast (f : ^a -> ^b) (xs : ^a[]) =
        let n = xs.Length
        let ys = Array.zeroCreate< ^b> n
        for i = 0 to n - 1 do ys.[i] <- f xs.[i]
        ys

    let inline tryPickFast (f : ^a -> ^b option) (xs : ^a[]) =
        let n = xs.Length
        let mutable result = None
        let mutable i = 0
        while i < n && (match result with None -> true | Some _ -> false) do
            result <- f xs.[i]
            i <- i + 1
        result

module Seq =
    let inline mapFast (f : ^a -> ^b) (ts : seq<'a>) =
        let ra = ResizeArray()
        for t in ts do ra.Add (f t)
        ra.ToArray()