CREATE TABLE irbis_records
(
    record_id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    rubric VARCHAR(1000) NOT NULL,
    town VARCHAR(50) NOT NULL,
    book_year VARCHAR (10) NOT NULL,
    annotation TEXT,
    book_content TEXT,
    personal VARCHAR(100)
);

CREATE TABLE doc_subtitles
(
    doc_subtitle_id SERIAL PRIMARY KEY,
	index_MDA VARCHAR(4),
    title VARCHAR(200),
    subtitle VARCHAR(200)
);


CREATE TABLE wrong_subtitles
(
    wrong_subtitle_id SERIAL PRIMARY KEY,
	index_MDA VARCHAR(4),
    title VARCHAR(200),
    subtitle VARCHAR(200)
);

CREATE TABLE doc_subtitles_wrong_subtitles
(
    doc_subtitle_id int NOT NULL,
    wrong_subtitle_id int NOT NULL,
    PRIMARY KEY (doc_subtitle_id, wrong_subtitle_id),
    FOREIGN KEY (doc_subtitle_id) REFERENCES doc_subtitles,
    FOREIGN KEY (wrong_subtitle_id) REFERENCES wrong_subtitles
);