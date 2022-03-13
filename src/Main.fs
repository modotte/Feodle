module Main

open System

open Browser
open Fable.Core.JsInterop
open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish

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
    CurrentGuess: string
    Tries: int
    Answer: string
    State: GameState
}

type Message =
    | GuessChanged of string
    | AddedGuess
    | GameStateUpdated of GameState
    | GameReset

let randomChoiceOf (choices: string array) =
    let index = Random().Next(choices |> Array.length)
    choices[index]

let init = { 
    Guesses = [||]
    CurrentGuess = ""
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
    | GuessChanged answer -> { model with CurrentGuess = answer }, Cmd.none
    | AddedGuess -> 
        { model with
            Guesses = [|{
                UserGuess = model.CurrentGuess
                ColoredGuess = asColored model.Answer model.CurrentGuess
            }|] |> Array.append model.Guesses
            Tries = model.Tries + 1
            CurrentGuess = ""
        }, Cmd.none
    | GameStateUpdated state -> { model with State = state }, Cmd.none
    | GameReset -> { fst init with Answer = randomChoiceOf words }, Cmd.none

let isWordInList answer = words |> Array.contains answer
let handleGuess (key: Types.KeyboardEvent) model dispatch =
    if key.code = "Enter" then
        if model.CurrentGuess |> isWordInList then
            if model.Tries = MAX_TRIES then
                dispatch AddedGuess
                dispatch (GameStateUpdated Lost)
            else
                if model.CurrentGuess = model.Answer then
                    dispatch AddedGuess
                    dispatch (GameStateUpdated Won)
                else
                    dispatch AddedGuess

module View =
    let makeGithubForkBadge =
        Html.a [
            prop.href "https://github.com/modotte/Feodle"
            prop.children [
                Html.img [
                    prop.classes [ "attachment-full"; "size-full" ]
                    prop.alt "Fork me on Github"
                    prop.src "https://github.blog/wp-content/uploads/2008/12/forkme_left_green_007200.png?resize=149%2C149"
                ]
            ]
        ]

    let makeGuessesList model =
        Bulma.box [
            Bulma.columns [
                columns.isCentered
                prop.children [
                    Html.ul [
                        Bulma.field.div (
                            model.Guesses |> Array.map (fun entry -> 
                                Html.li [
                                    Html.span [
                                        prop.children [
                                            Html.h2 entry.UserGuess

                                            entry.ColoredGuess
                                            |> Array.map (fun c ->
                                                match c with
                                                | Green -> "🟩"
                                                | Yellow -> "🟨"
                                                | Black -> "⬛️"
                                            ) 
                                            |> String.concat ""
                                            |> Html.h2
                                        ]
                                    ]
                                ]
                            )
                        )
                    ]
                ]
            ]
        ]

    let makeGuessEntry model dispatch =
        Bulma.field.div [
            field.isGrouped
            field.isGroupedCentered

            prop.children [

                match model.State with
                | InProgress -> 
                    Bulma.input.text [
                        prop.valueOrDefault model.CurrentGuess
                        prop.autoFocus true
                        prop.onKeyUp (fun key -> handleGuess key model dispatch)
                        prop.onTextChange (GuessChanged >> dispatch)
                        prop.maxLength MAX_WORD_LENGTH
                    ]
                | _ ->
                    Bulma.button.button [
                        color.isSuccess
                        prop.text "Reset and play again!"
                        prop.onClick (fun _ -> dispatch GameReset)
                    ]
            ]
        ]

    [<ReactComponent>]
    let mainView () = 
        let model, dispatch = React.useElmish(init, update, [||])
        Bulma.container [

            prop.children [
                makeGithubForkBadge

                Bulma.title "Feodle - A barebone, breadboad minimal Wordle by modotte"
                makeGuessesList model

                Html.h1 [
                    prop.hidden (model.State = InProgress)
                    prop.text $"Answer: {model.Answer}"
                ]

                makeGuessEntry model dispatch
            ]
        ]

ReactDOM.render(
    View.mainView,
    document.getElementById "feliz-app"
)