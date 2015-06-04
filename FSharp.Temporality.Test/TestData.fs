﻿module TestData

open FsCheck
open Temporality

let getPeriod (d1,d2) = 
    let minDate = min d1 d2
    let maxDate = max d1 d2
    { StartDate = minDate
      EndDate = maxDate }

type RandomPeriod = 
    static member Gen() = 
        let randomPeriod = 
            Arb.generate<DateTime>
            |> Gen.two
            |> Gen.map (getPeriod)
        
        let emptyPeriod = 
            Arb.generate<DateTime>
            |> Gen.map(fun d -> getPeriod(d,d))

        Gen.frequency [ (3, randomPeriod); (1, emptyPeriod) ]
        
    static member Values() = 
        RandomPeriod.Gen()
        |> Arb.fromGen

let toTemporaries l = 
    let rec internalToTemporaries s l = 
        seq { 
            match l with
            | (value, duration) :: tail -> 
                yield { Period = Period.from s duration
                        Value = value }
                yield! internalToTemporaries (s + duration) tail
            | [] -> yield! []
        }
    internalToTemporaries DateTime.MinValue l

let toTemporal l = toTemporaries l |> Temporal.toTemporal

type RandomTemporal = 
    static member Gen() =
        let emptyTemporal = 
            [ gen { return [] |> Temporal.toTemporal } ]
            |> Gen.oneof
        
        let overlapHelloTemporal = 
            Gen.map(fun p -> { Period = p; Value = "Hello" }) (RandomPeriod.Gen())
            |> Gen.listOf
            |> Gen.map(fun temporaries -> temporaries |> Temporal.toTemporal)
        let noOverlapTemporal = 
            [ 1, gen { return "Hello" }
              2, gen { return "World" } ]
            |> Gen.frequency 
            |> Gen.map (fun value -> (value, TimeSpan.FromDays(1.)))
            |> Gen.listOf
            |> Gen.map (toTemporal)

        [ emptyTemporal
          overlapHelloTemporal
          noOverlapTemporal ]
        |> Gen.oneof
    static member Values() = RandomTemporal.Gen() |> Arb.fromGen