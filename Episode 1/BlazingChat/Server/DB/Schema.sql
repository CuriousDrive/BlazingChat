CREATE TABLE User
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

INSERT INTO User (user_id,email_address,password,source,first_name,last_name,profile_picture_url,
date_of_birth,about_me,notifications,dark_theme,created_date)
VALUES
(1,'julius.caesar@gmail.com','julius.caesar','APPL','Julius','Caesar',NULL,NULL,NULL,1,1,NULL),
(2,'daniel.tonini@gmail.com','daniel.tonini','APPL','Daniel','Tonini',NULL,NULL,NULL,0,0,NULL),
(3,'gary.thomas@gmail.com','gary.thomas','APPL','Gary','Thomas',NULL,NULL,NULL,0,0,NULL),
(4,'martin.sommer@gmail.com','martin.somme','APPL','Martin','Sommer',NULL,NULL,NULL,0,0,NULL),
(5,'howard.snyder@gmail.com','howard.snyder','APPL','Howard','Snyder',NULL,NULL,NULL,0,0,NULL),
(6,'margaret.smith@gmail.com','margaret.smith','APPL','Margaret','Smith',NULL,NULL,NULL,0,0,NULL),
(7,'carine.schmitt@gmail.com','carine.schmitt','APPL','Carine','schmitt',NULL,NULL,NULL,0,0,NULL),
(8,'mary.saveley@gmail.com','mary.saveley','APPL','Mary','Saveley',NULL,NULL,NULL,0,0,NULL),
(9,'annette.roulet@gmail.com','annette.roulet','APPL','Annette','Roulet',NULL,NULL,NULL,0,0,NULL),
(10,'diego.roel@gmail.com','diego.roel','APPL','Diego','Roel',NULL,NULL,NULL,0,0,NULL);
