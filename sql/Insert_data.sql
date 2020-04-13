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
INSERT INTO
    doc_subtitles (title, subtitle)
VALUES
    ('Заголовок', 'Подзаголовок') ON CONFLICT (subtitle) DO NOTHING;
INSERT INTO
    wrong_subtitles (title, subtitle)
VALUES
    ('Заголовок', 'Подзаголовок') ON CONFLICT (subtitle) DO NOTHING;
INSERT INTO
    wrong_subtitles_doc_subtitles (doc_subtitle_id, wrong_subtitle_id)
VALUES
    (1, 1)