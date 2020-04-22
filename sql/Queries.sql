SELECT wrong_subtitle_id
FROM wrong_subtitles
WHERE subtitle = 'Подзаголовок';

SELECT doc_subtitle_id
FROM wrong_subtitles_doc_subtitles
WHERE wrong_subtitle_id = 1

DELETE FROM irbis_records;
	
SELECT index_MDA, title, subtitle FROM doc_subtitles 
WHERE LOWER (title) LIKE LOWER ('%Гомилетика%') 
AND LOWER (subtitle) LIKE LOWER ('%Гомилетика%');

SELECT subtitle FROM doc_subtitles 
WHERE doc_subtitle_id =
	(SELECT doc_subtitle_id FROM doc_subtitles_wrong_subtitles 
	WHERE wrong_subtitle_id = 
	(SELECT wrong_subtitle_id FROM wrong_subtitles
	WHERE subtitle = 'Катихизические поучения (заповеди о вере и благочестии и т.п.)')); 


SELECT subtitle FROM doc_subtitles AS ds 
INNER JOIN doc_subtitles_wrong_subtitles AS dsws 
ON ds.doc_subtitle_id = junc.doc_subtitle_id 
INNER JOIN wrong_subtitles AS ws 
ON ws.wrong_subtitle_id = junc.wrong_subtitle_id


