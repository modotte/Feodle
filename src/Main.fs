module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type Components =
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"

[<ReactComponent>]
let main = Html.h1 "Hello world"

ReactDOM.render(
    main,
    document.getElementById "feliz-app"
)