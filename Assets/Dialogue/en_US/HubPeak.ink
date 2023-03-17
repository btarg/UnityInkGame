#speaker:HubPeakNPC
INCLUDE ../globals.ink
INCLUDE ../functions.ink

~ getEquippedItem()
{tutorial_complete == true: -> entry | -> unknown}

=== entry ===
{flower_quest_complete == true: -> complete | -> start}

=== unknown ===
you are a ((stranger/danger))
have you talked to <color=Orange>Toast</color> yet?
he will be your ((guide/tutorial))

=== complete ===
purple, purple, purple, purple

=== start ===
{ equipped_item_id == -1: -> no_item | -> has_an_item }

=== no_item ===
{~ -> hint_wind | -> hint_garden | -> hint_visible}

=== hint_wind ===
i like coming up here to feel the ((wind/breeze)).
the wind feels
... #delay:0.33 #continueafter:0.25
<color=purple>purple.</color> #delay:0.075
-> END

=== hint_garden ===
my <color=purple>garden</color> will be so ((beautiful/pretty/homely))...
i will need <color=purple>flowers</color> to complete my ((goal/dream))
-> END

=== hint_visible ===
((look/observe)), down there!
do you see those <color=purple>pretty ((plants/organisms))?</color>
i want to ((plant/obtain)) some of those myself some day
-> END

=== has_an_item ===
{ equipped_item_id == 2: -> has_purple_flower | -> no_flower }

=== no_flower ===
woah, that's a ((cool/neat/nice)) <color=yellow>{equipped_item_name}</color> you've got there
((shame/unfortunate)) it's not <color=purple>purple...</color>
-> END
=== has_purple_flower ===
OMGOMGOMGOMGOMGOMGOMGOMGOMG!
do my eyes decieve me? ...
<color=purple>PURPLE FLOWERS!?!?</color>
{ equipped_item_amount > 1: -> lots | -> not_enough}

=== lots ===
GAAAASP!!!!
you have...
<color=yellow>{equipped_item_amount}</color> of them!?
{ equipped_item_amount == 4: -> enough | -> not_enough}

=== enough ===
yes... #delay:0.33
YES!! #delay:0.01
thank you, this exactly the amount i needed to start my garden! #delay:0.025
here, take this as a token of ((gratitude/respect/friendship)).
{takePlayerItem(2, 4)}
{givePlayerItem(3, 1)}
~ flower_quest_complete = true
-> END

=== not_enough ===
could you perhaps find me <color=yellow>{4 - equipped_item_amount} more of these?</color>
i need more <color=purple>purple</color> in my life.
-> END


