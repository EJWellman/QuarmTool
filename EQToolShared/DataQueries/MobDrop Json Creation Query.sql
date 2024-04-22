SELECT 
json_arrayagg(JSON_OBJECT(
	'item_id', i.id, 
	'loottable_id', lte.loottable_id,
	'drop_chance', lde.chance,
	'item_name', i.Name
))
FROM loottable_entries lte
	LEFT JOIN lootdrop ld
		ON lte.lootdrop_id = ld.id
	LEFT JOIN lootdrop_entries lde
		ON lde.lootdrop_id = ld.id
	JOIN items i
		ON i.id = lde.item_id
--	JOIN npc_types nt
--		ON nt.loottable_id = lte.loottable_id
--	JOIN npc_search ns
--		ON ns.id = nt.id;