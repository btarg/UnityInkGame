INCLUDE ../globals.ink
INCLUDE ../functions.ink

~ getEquippedItem()
{flower_quest_complete == true: -> check_pass | -> no_pass}

=== no_pass ===
#speaker:BookNPC
you need a <color=yellow>pass</color> to enter #continueafter:0.5
<color=yellow>THE BOOK.</color>
(i find it fun to say it dramatically like that)
-> END

=== check_pass ===
{equipped_item_id == 3: -> let_through | -> nope}

=== nope ===
i need to see your <color=yellow>pass</color> if you want to get through...
-> END

=== let_through ===
okay, you may enter <color=yellow>THE BOOK</color>
~ fireEvent("OpenBookEvent")

-> END