INCLUDE ../functions.ink

-> start
=== start ===
Return to the <color=yellow>Hub World?</color>
    + [Yes]
        -> hub
    + [No]
        -> cancel

=== cancel ===
#continueafter:0
-> END
=== hub ===
~ loadScene("HubWorld")
-> END