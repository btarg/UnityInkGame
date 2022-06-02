INCLUDE ../globals.ink
INCLUDE ../functions.ink

{ player_name == "": -> choose | -> already_chose }

=== choose ===
Please enter your name. #input:player_name
-> already_chose

=== already_chose ===
Your name is <color=yellow>{player_name}</color>, is that correct?
    + [Yes]
        -> named
    + [No]
        -> choose
=== named ==
this is where stuff will go, {player_name}. #speaker:test2

-> END