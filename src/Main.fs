// Copyright (c) 2022 Modotte
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
    let index = Random().Next(choices.Length)
    choices[index]

let init = { 
    Guesses = [||]
    CurrentGuess = ""
    Tries = 1
    Answer = randomChoiceOf Words.words
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

let withGuessChanged answer model = { model with CurrentGuess = answer }, Cmd.none
let withAddedGuess model =
    { model with
        Guesses = [|{
            UserGuess = model.CurrentGuess
            ColoredGuess = asColored model.Answer model.CurrentGuess
        }|] |> Array.append model.Guesses
        Tries = model.Tries + 1
        CurrentGuess = ""
    }, Cmd.none
let withGameStateUpdated state model = { model with State = state }, Cmd.none
let withGameReset model = { fst init with Answer = randomChoiceOf Words.words }, Cmd.none

let update message model =
    match message with
    | GuessChanged answer -> withGuessChanged answer model
    | AddedGuess -> withAddedGuess model
    | GameStateUpdated state -> withGameStateUpdated state model
    | GameReset -> withGameReset model

let isWordInList answer = Words.words |> Array.contains answer
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

    let makeColoredBoxes colors =
        colors
        |> Array.map (fun c ->
            match c with
            | Green -> "🟩"
            | Yellow -> "🟨"
            | Black -> "⬛️"
        ) 
        |> String.concat ""
        |> Html.h2

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
                                            makeColoredBoxes entry.ColoredGuess
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
                        prop.maxLength Words.MAX_WORD_LENGTH
                        prop.placeholder "Enter 5 letter word guess"
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

                Bulma.title "Feodle - A barebone Wordle implementation for practicing"
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