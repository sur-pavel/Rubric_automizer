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


