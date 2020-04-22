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
    word VARCHAR(200),
	UNIQUE (word)
);

DROP TABLE IF EXISTS wrong_subtitles
CASCADE;

CREATE TABLE wrong_subtitles
(
    wrong_subtitle_id SERIAL PRIMARY KEY,
    subtitle VARCHAR(200),
	UNIQUE (subtitle)
);

DROP TABLE IF EXISTS doc_subtitles_wrong_subtitles
CASCADE;

CREATE TABLE doc_subtitles_wrong_subtitles
(
    doc_subtitle_id int NOT NULL,
    wrong_subtitle_id int NOT NULL,
    PRIMARY KEY (doc_subtitle_id, wrong_subtitle_id),
    FOREIGN KEY (doc_subtitle_id) REFERENCES doc_subtitles,
    FOREIGN KEY (wrong_subtitle_id) REFERENCES wrong_subtitles
);

