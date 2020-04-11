-- SQLite
select * from contacts;

CREATE TABLE COMPANY(
   ID INT PRIMARY KEY     NOT NULL,
   NAME           TEXT    NOT NULL,
   AGE            INT     NOT NULL,
   ADDRESS        CHAR(50),
   SALARY         REAL
);

CREATE TABLE User
(
   user_id           INTEGER     PRIMARY KEY AUTOINCREMENT,
   email_address     TEXT    NOT NULL,
   password          TEXT    NOT NULL,
   source            TEXT    NOT NULL,
   date_of_birth     DATE,
   about_me          TEXT,
   notfications      BOOLEAN,
   dark_theme        BOOLEAN,
   created_date      DATE
);

CREATE TABLE RefreshToken
(
   token_id          INTEGER     PRIMARY KEY AUTOINCREMENT,
   user_id           INT     NOT NULL,
   token             TEXT    NOT NULL,
   expiry_date       DATE    NOT NULL,
   FOREIGN KEY(user_id) REFERENCES User(user_id)
);

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

drop table COMPANY;




