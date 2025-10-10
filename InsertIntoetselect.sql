USE games_pendu;

ALTER TABLE letters MODIFY iscorrect BOOLEAN;


INSERT INTO letters (VALUE)
VALUES
("A"),
("B"),
("C"),
("D"),
("E"),
("F"),
("G"),
("H"),
("I"),
("J"),
("K"),
("L"),
("M"),
("N"),
("O"),
("P"),
("Q"),
("R"),
("S"),
("T"),
("U"),
("V"),
("W"),
("X"),
("Y"),
("Z");

INSERT INTO themes (`name`)
VALUES
('Fruits et légumes'),
('Animal');


INSERT INTO words (VALUE,`Themes_id`)
VALUES
('Pomme',1),
('Banane',1),
('Kiwi',1),
('Lion',2),
('Chien',2),
('Ours',2);


