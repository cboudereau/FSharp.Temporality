﻿module FoldProperties

open FsCheck.Xunit
open Outatime

[<Arbitrary(typeof<TestData.RandomStringTemporal>)>]
module FoldTemporal = 

    [<Property>]
    let ``identity fold should be equal to source`` (temporaries:Temporary<string> list) = 
        let expected = temporaries |> Outatime.build |> Outatime.toList

        let actual = 
            temporaries
            |> Outatime.build
            |> Outatime.fold (fun state p v -> seq { yield! state; yield p := v }) Seq.empty
            |> Seq.toList

        expected = actual