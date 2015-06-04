﻿module PeriodProperties

open FsCheck
open FsCheck.Xunit

open Temporality

[<Arbitrary(typeof<TestData.RandomPeriod>)>]
module IntersectProperties =

    [<Property>]
    let ``Always ∩ Always = Always`` () =
        Period.Always |> Period.intersect Period.Always = Period.Always

    [<Property>]
    let ``Always ∩ p = p`` p = 
        Period.Always |> Period.intersect p = p

    [<Property>]
    let ``Never ∩ p = Never`` p = 
        Period.Never |> Period.intersect p = Period.Never
    
    [<Property>]
    let ``Never ∩ Never = Never`` () = 
        Period.Never |> Period.intersect Period.Never = Period.Never

    [<Property>]
    let ``p ∩ p = p`` p = 
        p |> Period.intersect p = p

    [<Property>]
    let ``p1 ∩ p2 ∪ p1 = p1`` p1 p2 =
        match Period.intersect p1 p2 with
        | i when i = Period.Never -> Period.union p1 i = Period.Never
        | i -> Period.union i p1 = p1

[<Arbitrary(typeof<TestData.RandomPeriod>)>]
module UnionProperties = 
    
    [<Property>]
    let ``Never ∪ Never = Never``() =
        Period.Never |> Period.union Period.Never = Period.Never

    [<Property>]
    let ``Always ∪ Always = Always``()=
        Period.Always |> Period.union Period.Always = Period.Always

    [<Property>]
    let ``Always ∪ p = Always`` p = 
        Period.Always |> Period.union p = Period.Always

    [<Property>]
    let ``Never ∪ p = Never`` p = 
        Period.Never |> Period.union p = Period.Never

    [<Property>]
    let ``p ∪ p = p`` p = 
        p |> Period.union p = p

    [<Property>]
    let ``p1 ∪ p2 ∩ p1 = p1`` p1 p2 = 
        match Period.union p1 p2 with
        | u when u = Period.Never -> Period.intersect p1 p2 = Period.Never
        | u -> Period.intersect u p1 = p1