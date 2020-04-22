DROP TABLE IF EXISTS doc_subtitles
CASCADE;

CREATE TABLE doc_subtitles
(
    doc_subtitle_id SERIAL PRIMARY KEY,
	index_MDA VARCHAR(4),
    title VARCHAR(200),
    subtitle VARCHAR(200)
);

DROP TABLE IF EXISTS spell_dictionary
CASCADE;

CREATE TABLE spell_dictionary
(
    word_id SERIAL PRIMARY KEY,	
    word VARCHAR(200)
);