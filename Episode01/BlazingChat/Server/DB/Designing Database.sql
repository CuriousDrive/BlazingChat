-- SQLite
-- drop table User;

CREATE TABLE Users
(
   user_id              INTEGER     PRIMARY KEY AUTOINCREMENT,
   email_address        TEXT        NOT NULL,
   password             TEXT        NOT NULL,
   source               TEXT        NOT NULL,
   first_name           TEXT,
   last_name            TEXT,
   profile_picture_url  TEXT,
   date_of_birth        DATETIME,
   about_me             TEXT,
   notifications        INTEGER,
   dark_theme           INTEGER,
   created_date         DATE
);

-- drop table RefreshToken

CREATE TABLE RefreshTokens
(
   token_id          INTEGER     PRIMARY KEY AUTOINCREMENT,
   user_id           INT         NOT NULL,
   token             TEXT        NOT NULL,
   expiry_date       DATE        NOT NULL,
   FOREIGN KEY(user_id) REFERENCES User(user_id)
);

-- drop table ChatHistory

CREATE TABLE ChatHistory
(
   chat_history_id   INTEGER     PRIMARY KEY AUTOINCREMENT,
   from_user_id      INT     NOT NULL,
   to_user_id        INT     NOT NULL,
   message           TEXT    NOT NULL,
   created_date      DATE    NOT NULL,
   FOREIGN KEY(from_user_id) REFERENCES User(user_id),
   FOREIGN KEY(to_user_id) REFERENCES User(user_id)
);


