module Main

open System

open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

let [<Literal>] MAX_TRIES = 6
let [<Literal>] MAX_WORD_LENGTH = 5
let words = [|
    "cargo"
    "mango"
    "fruit"
    "cycle"
    "black"
    "white"
    "later"
    "color"
    "pedal"
    "radar"
    "blues"
    "annal"
    "union"
    "alloy"
    "banal"
|]

type Color = Black | Yellow | Green

type GameState = Lost | InProgress | Won
type Entry = {
    UserGuess: string
    ColoredGuess: Color array
}
type Model = {
    Guesses: Entry array
    Guess: string
    Tries: int
    Answer: string
    State: GameState
}

type Message =
    | GuessChanged of string
    | AddedGuess
    | GameStateUpdated of GameState

let randomChoiceOf (choices: string array) =
    let index = Random().Next(choices |> Array.length)
    choices[index]

let init = { 
    Guesses = [||]
    Guess = ""
    Tries = 1
    Answer = randomChoiceOf words
    State = InProgress }, Cmd.none

let asColored (answer: string) (guess: string) =
    // TODO: Handle duplicate letters in answer and guess
    guess
    |> Seq.toArray
    |> Array.mapi (fun i x -> 
        if answer.IndexOf(x) = -1 then
            Black
        else
            if x = answer[i] then
                Green
            else
                Yellow
    )

let update message model =
    match message with
    | GuessChanged answer -> { model with Guess = answer }, Cmd.none
    | AddedGuess -> 
        { model with
            Guesses = [|{
                UserGuess = model.Guess
                ColoredGuess = asColored model.Answer model.Guess
            }|] |> Array.append model.Guesses
            Tries = model.Tries + 1 }, Cmd.none
    | GameStateUpdated state -> { model with State = state }, Cmd.none

let isWordInList answer = words |> Array.contains answer

module View =
    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Html.div [
            Html.h1 [
                prop.hidden (model.State = InProgress)
                prop.text $"Answer: {model.Answer}"
            ]
            Html.input [
                prop.autoFocus true
                prop.onKeyUp (fun key -> 
                    if key.code = "Enter" then
                        if model.Guess |> isWordInList then
                            if model.Tries = MAX_TRIES then
                                dispatch AddedGuess
                                dispatch (GameStateUpdated Lost)
                            else
                                if model.Guess = model.Answer then
                                    dispatch AddedGuess
                                    dispatch (GameStateUpdated Won)
                                else
                                    dispatch AddedGuess
                )
                prop.onTextChange (GuessChanged >> dispatch)
                prop.maxLength MAX_WORD_LENGTH
                prop.disabled (
                    match model.State with
                    | InProgress -> false
                    | _ -> true
                )
            ]

            Html.ul [
                Html.div (
                    model.Guesses |> Array.map (fun x -> 
                        Html.li [
                            Html.span [
                                prop.children [
                                    Html.h2 x.UserGuess 
                                    let colored = 
                                        x.ColoredGuess
                                        |> Array.map (fun c ->
                                            match c with
                                            | Green -> "🟩"
                                            | Yellow -> "🟨"
                                            | Black -> "⬛️"
                                        ) 
                                        |> String.concat ""
                                    Html.h2 colored
                                ]
                            ]
                        ]
                    )
                )
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)