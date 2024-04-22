SELECT 
json_arrayagg(JSON_OBJECT(
	'merchantid',	ml.merchantid, 
	'slot',	ml.slot, 
	'item_name',	i.Name, 
	'item_id',	i.id
))
FROM merchantlist ml
	JOIN items i
		ON i.id = ml.item