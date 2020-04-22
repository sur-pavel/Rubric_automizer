INSERT INTO
    irbis_records (
        title,
        rubric,
        town,
        book_year,
        annotation,
        book_content,
        personal
    )
VALUES
    (
        'Заглавие',
        'Рубрика',
        'Место',
        'Год',
        'Аннотация',
        'Оглавление',
        'Персоналия'
    ) ON CONFLICT (subtitle) DO NOTHING;
	
INSERT INTO  doc_subtitles (title, subtitle)
VALUES ('Заголовок', 'Подзаголовок') ON CONFLICT (subtitle) DO NOTHING;

INSERT INTO wrong_subtitles (subtitle)
VALUES ('Катихизические поучения (заповеди о вере и благочестии и т.п.)') ON CONFLICT (subtitle) DO NOTHING;

CREATE TABLE wrong_subtitles
(
    wrong_subtitle_id SERIAL PRIMARY KEY,
	index_MDA VARCHAR(4),
    title VARCHAR(200),
    subtitle VARCHAR(200)
);

INSERT INTO doc_subtitles_wrong_subtitles (doc_subtitle_id, wrong_subtitle_id)
VALUES (391, 1)