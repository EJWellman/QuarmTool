SELECT 
json_arrayagg(JSON_OBJECT(
'id',                          nt.id,
'name',                   		 REPLACE(nt.name, "#", ""),
'npc_class_id',					 ns.class,
'level',                       nt.level,
'maxlevel',                    nt.maxlevel,
'race',                        r.name,
'AC',                          nt.AC,
'hp',                          nt.hp,
'mana',                        nt.mana,
'loottable_id',                nt.loottable_id,
'merchant_id',                 nt.merchant_id,
'npc_spells_id',               nt.npc_spells_id,
'npc_faction_id',              nt.npc_faction_id,
'primary_faction',             fl.name,
'attack_delay',                nt.attack_delay,
'mindmg',                      nt.mindmg,
'maxdmg',                      nt.maxdmg,
'attack_count',                nt.attack_count,
'special_abilities',           nt.special_abilities,
'runspeed',                    nt.runspeed,
'MR',                          nt.MR,
'CR',                          nt.CR,
'DR',                          nt.DR,
'FR',                          nt.FR,
'PR',                          nt.PR,
'see_invis',                   nt.see_invis,
'see_invis_undead',            nt.see_invis_undead,
'see_sneak',                   nt.see_sneak,
'see_improved_hide',           nt.see_improved_hide,
'mitigates_slow',              case when nt.slow_mitigation <> 0
                               then 1
                               ELSE 0
                               END,
'slow_mitigation',             nt.slow_mitigation,
'unique_spawn_by_name',        nt.unique_spawn_by_name,
'isquest',                     nt.isquest,
'combat_hp_regen',             nt.combat_hp_regen,
'combat_mana_regen',           nt.combat_mana_regen,
'greed',                       nt.greed,
'zone_name',                   ns.long_name,
'zone_code',                   ns.zone,
'zone_name_guess',				 z.long_name,
'zone_code_guess',				 z.short_name
))
FROM npc_types nt
JOIN npc_search ns
	ON ns.id = nt.id
JOIN races r
	ON r.id = ns.race
LEFT JOIN npc_faction nf
	ON nf.id = nt.npc_faction_id
LEFT JOIN faction_list fl
	ON fl.id = nf.primaryfaction
JOIN zone z
	ON z.zoneidnumber = FLOOR(CAST((nt.id / 1000) AS DOUBLE));