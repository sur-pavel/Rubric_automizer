using System;using System.Collections.Generic;using System.Linq;namespace RubricAutimatization { public class DocParser { public DocParser() { } } }ELECT
    wrong_subtitle_id
FROM
    wrong_subtitles
WHERE
    subtitle = 'Подзаголовок';
SELECT
    doc_subtitle_id
FROM
    wrong_subtitles_doc_subtitles
WHERE
    wrong_subtitle_id = 1
DELETE FROM
    irbis_records;