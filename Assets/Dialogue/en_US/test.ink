INCLUDE ../functions.ink
INCLUDE ../globals.ink

{getEquippedItem()}
{spoke_to_npc == true: -> already_spoken | -> start}

=== already_spoken ===
That item was super neat!
-> END

=== start ===
{ equipped_item_id == -1: -> no_item | -> has_an_item }
-> END

=== no_item ===
You would look really cool with an <color=yellow>item</color>!
It's a shame you don't have one...
-> END

=== has_an_item ===
Woah, that's a cool <color=yellow>{equipped_item_name}</color> you've got there!
~ spoke_to_npc = true
{ equipped_item_amount > 1: -> lots | -> END}

=== lots ===
Wow, you have <color=yellow>{equipped_item_amount}</color> of them!?
-> END