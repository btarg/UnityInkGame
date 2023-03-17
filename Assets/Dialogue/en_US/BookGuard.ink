INCLUDE ../globals.ink
INCLUDE ../functions.ink
#speaker:BookNPC

~ getEquippedItem()
{flower_quest_complete == true: -> check_pass | -> no_pass}

=== no_pass ===
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
{hub_book_open == true: -> open | -> autosave_and_open} // only autosave the first time

=== autosave_and_open ===
~ hub_book_open = true
~ fireEvent("AutoSaveEvent")
-> open

=== open ===
{~OPEN SESAME!|let's-a-go!}
~ fireEvent("OpenBookEvent")
-> END