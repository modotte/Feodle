namespace App

open Feliz

type Components =
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"