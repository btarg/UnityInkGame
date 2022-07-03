INCLUDE ../functions.ink
INCLUDE ../globals.ink

~ getEquippedItem()
{spoke_to_npc == true: -> already_spoken | -> start}

=== already_spoken ===
That item was super neat!
-> END

=== already_given_item ===
I hope you like my item!
You should try <color=yellow>Equipping</color> it.
-> END

=== start ===
{ equipped_item_id == -1: -> no_item | -> has_an_item }
-> END

=== no_item ===
{ has_given_item == true: -> already_given_item | -> give_item }
-> END

=== give_item ===
You would look really cool with an <color=yellow>item</color>!
{givePlayerItem(1, 2)}
Enjoy!
~ has_given_item = true
-> END

=== has_an_item ===
Woah, that's a cool <color=yellow>{equipped_item_name}</color> you've got there!
~ spoke_to_npc = true
{ equipped_item_amount > 1: -> lots | -> END}

=== lots ===
Wow, you have <color=yellow>{equipped_item_amount}</color> of them!?
-> END