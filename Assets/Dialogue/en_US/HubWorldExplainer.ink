#speaker:HubWorldExplainerNPC
INCLUDE ../globals.ink
INCLUDE ../functions.ink

{ player_name == "": -> start | -> already_spoken }
{ tutorial_complete == true: -> already_spoken | -> start }

=== already_spoken ===
i have acquainted you with the <color=yellow>((hub world/spaghetti)), yes?</color>

+ [I'm familiar]
    -> finished_speaking
+ [Hub World?]
    -> named
+ [Change name]
    -> change_name

=== start ===
you feel ((new/inexperienced/confused))
i have not seen you here before
-> choose

=== choose ===
what is your name? #input:player_name
-> check_name

=== check_name ===
your ((name/callsign)) is <color=yellow>{player_name}</color>?
    + [Yes]
        -> named
    + [No]
        -> choose


=== change_name ===
you wish for us to call you something else, <color=yellow>{player_name}</color>?
    + [Yes]
        -> choose
    + [No]
        -> finished_speaking

=== named ==
{ tutorial_complete == true: -> finished_speaking | -> explain }

=== explain ===
we live, <color=yellow>{player_name}</color>, we live!
the <color=yellow>dream</color> is our home now
many branching paths: the <color=yellow>hub</color> is like ((spaghetti/noodle)), you may feel disoriented
the <color=yellow>pause menu</color> will carry you home when you tire of exploration
you may also use it to <color=yellow>relive a ((memory/save/moment))</color>
~ tutorial_complete = true
we are ((finished/complete)) now, {player_name}. venture!
-> finished_speaking

=== finished_speaking ===
return here when you become lost and afraid

-> END