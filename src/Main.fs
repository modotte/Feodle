module Main

open Feliz
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

type Components =
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"

ReactDOM.render(
    Components.HelloWorld(),
    document.getElementById "feliz-app"
)