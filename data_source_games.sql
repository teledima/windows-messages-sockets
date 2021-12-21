DELETE FROM source_games;
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'source_games';
INSERT INTO source_games(games_name, achievements_name, downloadable_contents_name, categories_name) VALUES ('Cities: Skylines', 'Pioner', 'Cities: Skylines - After Dark', 'Urban planning');
INSERT INTO source_games(games_name, achievements_name, downloadable_contents_name, categories_name) VALUES ('Cities: Skylines', 'Green Energy', 'Cities: Skylines - Snowfall', 'Simulation');
INSERT INTO source_games(games_name, achievements_name, downloadable_contents_name, categories_name) VALUES ('The Witcher', 'Triss'' romance card', 'The Witcher: Enhanced Edition Soundtrack', 'Role game');
INSERT INTO source_games(games_name, achievements_name, downloadable_contents_name, categories_name) VALUES ('The Witcher', null, null, 'Singleplayer');
INSERT INTO source_games(games_name, achievements_name, downloadable_contents_name, categories_name) VALUES ('Forza Horizon 4', 'Optional Safe', 'Forza Horizon 4: Welcome Pack', 'Racing');
