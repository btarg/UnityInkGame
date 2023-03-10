INCLUDE ../globals.ink
INCLUDE ../functions.ink

~ getEquippedItem()
{flower_quest_complete == true: -> complete | -> start}

=== complete ===
#speaker:HubPeakNPC
🎵 purple, purple, purple, purple... 🎵
-> END

=== start ===
{ equipped_item_id == -1: -> no_item | -> has_an_item }
-> END

=== no_item ===
{~ -> hint_wind | -> hint_garden | -> hint_visible}
-> END

=== hint_wind ===
#speaker:HubPeakNPC
i like coming up here to feel the wind.
the wind feels
... #delay:0.33 #continueafter:0.25
<color=purple>purple.</color> #delay:0.075
-> END

=== hint_garden ===
#speaker:HubPeakNPC
my <color=purple>garden</color> will look so beautiful...
all i need is some <color=purple>flowers.</color>
-> END

=== hint_visible ===
#speaker:HubPeakNPC
look, down there!
do you see those <color=purple>pretty plants?</color>
i want to plant some of those myself some day.
-> END


=== has_an_item ===
#speaker:HubPeakNPC
{ equipped_item_id == 2: -> has_purple_flower | -> no_flower }


=== no_flower ===
#speaker:HubPeakNPC
woah, that's a cool <color=yellow>{equipped_item_name}</color> you've got there!
shame it's not <color=purple>purple...</color>
-> END
=== has_purple_flower ===
#speaker:HubPeakNPC
OMGOMGOMGOMGOMGOMGOMGOMGOMG! #delay:0.01
do my eyes decieve me? ...
<color=purple>PURPLE FLOWERS!?!?</color>
{ equipped_item_amount > 1: -> lots | -> not_enough}

=== lots ===
#speaker:HubPeakNPC
GAAAASP!!!!
you have...
<color=yellow>{equipped_item_amount}</color> of them!?
{ equipped_item_amount == 4: -> enough | -> not_enough}

=== enough ===
#speaker:HubPeakNPC
yes... #delay:0.33
YES!! #delay:0.01
thank you, this exactly the amount i needed to start my garden!
here, take this as a token of gratitude.
{givePlayerItem(3, 1)}
~ flower_quest_complete = true
-> END

=== not_enough ===
#speaker:HubPeakNPC
could you perhaps find me <color=yellow>{4 - equipped_item_amount} more of these?</color>
i need more <color=purple>purple</color> in my life.
-> END


