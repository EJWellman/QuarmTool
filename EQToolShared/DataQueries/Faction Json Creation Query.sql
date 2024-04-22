SELECT 
json_arrayagg(JSON_OBJECT(
	'npc_id', nt.id,
	'zone_name', ns.long_name,
	'zone_code', ns.zone,
	'faction_id', nfe.faction_id,
	'faction_name', fl.name,
	'faction_hit', nfe.value,
	'sort_order', nfe.sort_order
))
FROM npc_types nt
JOIN npc_search ns
	ON ns.id = nt.id
JOIN npc_faction_entries nfe
	ON nfe.npc_faction_id = nt.npc_faction_id
		AND nfe.value <> 0
JOIN faction_list fl
	ON fl.id = nfe.faction_id
ORDER BY nt.id, nfe.sort_order